var _SCREENS_COMMON_VUE_MIXIN = {},
    ___SCREEN_EVENT_SUBMIT_ID = {},
    _HOMEUI_VUE_COMS_DATA_SHARED = '',
    _HOMEUI_VUE_MIXIN_COMS = {
        data: {
            _eleID: null
        },
        mounted: function () {
            var _id = this.$el.id;
            if (_id == null || _id.length == 0) {
                _id = '___vue-com-' + this._uid;
                if (this.$el && this.$el.setAttribute) {
                    this.$el.setAttribute('id', _id);
                }
            }
            this._eleID = _id;
        },
        props: ['objScreens', 'screenInfo'],
        computed: {
        },
        methods: {
            screenEmit: function (screenId) {
                var _self = this;
                //console.log('[1]', _self.screenInfo);
                if (screenId == null && _self.screenInfo) screenId = _self.screenInfo.Id;
                if (screenId == null) return;
                //console.log('[2]',screenId, ___SCREEN_EVENT_SUBMIT_ID[screenId], ___SCREEN_EVENT_SUBMIT_ID);
                if (___SCREEN_EVENT_SUBMIT_ID[screenId] == null) return;

                var screenInfoSubmit = _self.getScreenInfo(screenId);
                ___SCREEN_EVENT_SUBMIT_ID[screenId].forEach(function (id) {
                    var el = document.getElementById(id);
                    if (el && el.__vue__) el.__vue__.$emit(screenId, screenInfoSubmit);
                });
            },
            addScreenEmit: function (screenId, funcCallback) {
                var _self = this;
                //console.log(screenId, _self._eleID);
                if (___SCREEN_EVENT_SUBMIT_ID[screenId] == null)
                    ___SCREEN_EVENT_SUBMIT_ID[screenId] = [_self._eleID];
                else
                    ___SCREEN_EVENT_SUBMIT_ID[screenId].push(_self._eleID);
                this.$on(screenId, funcCallback);
            },
            getScreenInfo: function (screenId) {
                var _self = this;
                if (_self.objScreens != null && _self.objScreens[screenId] != null) {
                    return _self.objScreens[screenId];
                }
                return null;
            },
            getScreenHomeUI: function () {
                return vueHome;
            },
            getWidgetArea: function () {
                return vueHome.getWidgetArea();
            },
            screenDialogClose: function () {
                var dialog = this.$root;
                if (dialog && typeof dialog['screenDialogClose'] == 'function') dialog.screenDialogClose();
            },
            screenDialogCloseNoCallback: function () {
                var dialog = this.$root;
                if (dialog && typeof dialog['screenDialogClose'] == 'function') dialog.screenDialogCloseNoCallback();
            },
            getDataTextSynchronous: function (url) {
                //if (url.indexOf('/') != 0) url = '/' + url;
                //if (url.indexOf('http') != 0) url = 'http://' + ___NODEJS_HOST + url;
                //f_log_kit(url);
                var r = new XMLHttpRequest();
                r.open('GET', url, false);
                r.send(null);
                if (r.status === 200) return r.responseText;
                return '';
            }
        }
    };
_HOMEUI_VUE_MIXIN_COMS.props.forEach(function (v) { _HOMEUI_VUE_COMS_DATA_SHARED += ' :' + v + '="' + v + '" '; });
/*================================================================================================
/ SCREENS COMMONS: Screen Error, Screen Alert, Screen Confirm ...
/================================================================================================*/
_SCREENS_COMMON_VUE_MIXIN = {
    methods: {
        screenOpen: function (options) {
            var parentId = '';
            if (this.$el == null) {
                parentId = _SCREENS_ID.HOME;
                options._screenParentIsHomeUI = true;
            }
            else {
                parentId = this.$el.id;
            }

            //console.log('screenOpen: parentId = ', parentId);

            options._screenParentElemID = parentId;
            f_hui_screenOpen(options);
        },
        screenBlankOpen: function () {
            var _self = this;
            _self.screenOpen({
                Id: _SCREENS_ID.BLANK,
                Components: 'blank-screen',
                className: 'blank-screen',
                overlayShow: false,
                Footer: { buttonOk: false, buttonCancel: false }
            });
        },
        screenErrorOpen: function (screenId, message, optionsFooter, components) {
            if (message == null || message.length == 0) return;

            var _self = this;
            if (optionsFooter == null) optionsFooter = {};
            if (optionsFooter.buttonOk == null) optionsFooter.buttonOk = false;
            if (optionsFooter.buttonCancel == null) optionsFooter.buttonCancel = false;

            _self.screenOpen({
                Id: screenId,
                Components: 'error-screen',
                className: 'screen-error',
                overlayShow: false,
                Header: {
                    Message: message
                },
                Footer: optionsFooter
            });
        },
        screenAlertOpen: function (screenId, message, optionsFooter, components) {
            var _self = this;
            if (optionsFooter == null) optionsFooter = { buttonOk: true, buttonCancel: false };
            _self.screenOpen({
                Id: screenId,
                className: 'screen-alert',
                Header: {
                    headerIcon: 'icon-b_warning_large',
                    Message: message
                },
                Footer: optionsFooter
            });
        },
        screenWarningOpen: function (screenId, message, optionsFooter, components) {
            var _self = this;
            if (optionsFooter == null) optionsFooter = { buttonOk: true, buttonCancel: false };
            _self.screenOpen({
                Id: screenId,
                className: 'screen-alert',
                Header: {
                    headerIcon: 'icon-b_warning_large',
                    Message: message
                },
                Footer: optionsFooter
            });
        },
        screenConfirmOpen: function (screenId, message, optionsFooter, components) {
            var _self = this;
            _self.screenOpen({
                Id: screenId,
                className: 'screen-confirm',
                Header: {
                    headerIcon: 'icon-b_warning_large',
                    classNameMessage: 'f-size20',
                    Message: message
                },
                Footer: optionsFooter
            });
        },
        screenToastOpen: function (screenId, message, optionsFooter, components) {
            var _self = this;
            if (optionsFooter == null) optionsFooter = { buttonOk: false, buttonCancel: false };
            _self.screenOpen({
                Id: screenId,
                timeoutDisplay: 5000,
                className: 'screen-toast toast-top screen-toast-widget-remove',
                overlayShow: false,
                Header: {
                    headerIcon: 'icon-b_warning_large',
                    Message: message
                },
                Footer: optionsFooter
            });
        }
    }
};
/*================================================================================================
/ COMPONENTS
/================================================================================================*/
Vue.component('com-nav-bottom', {
    mixins: [_HOMEUI_VUE_MIXIN_COMS, _SCREENS_COMMON_VUE_MIXIN], 
    template: '<ul><li v-for="(it, index) in items" v-html="[\'item-\' + it]"></li></ul>',
    data: function () {
        var _self = this; 
        var cf = { items: [1, 2, 3, 4, 5] };
        return cf;
    },
    computed: {
    },
    mounted: function () {
    },
    methods: {
    },
    watch: {
    }
});












