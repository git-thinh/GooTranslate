var ___SCREENS_COMMON_MIXIN = {},
    ___SCREEN_EVENT_SUBMIT_ID = {},
    ___PROPS_DATA_SHARED = '',
    ___COMS_MIXIN = {
        data: {
            _eleID: null
        },
        ready: function () {
            var _id = this.$el.id;
            if (_id == null || _id.length == 0) {
                _id = '___vue-com-' + this._uid;
                if (this.$el && this.$el.setAttribute) {
                    this.$el.setAttribute('id', _id);
                }
            }
            this._eleID = _id;
            ___COMS_ID.push(_id);
        },
        props: ___DATA_SHARED,
        computed: {
        },
        methods: {
            f_base_show: function () {
                var el = document.getElementById(this.el_id);
                if (el) el.style.opacity = 1;
                f_log(this.el_id, 'SHOW');
            },
            f_base_hide: function () {
                var el = document.getElementById(this.el_id);
                if (el) el.style.opacity = 0;
                f_log(this.el_id, 'HIDE');
            },
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
___DATA_SHARED.forEach(function (v) { ___PROPS_DATA_SHARED += ' :' + v + '="' + v + '" '; });
___SCREENS_COMMON_MIXIN = {
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
/*=============================================================
/ COMPONENTS
/=============================================================*/
Vue.component('com-nav-bottom', {
    mixins: [___COMS_MIXIN, ___SCREENS_COMMON_MIXIN, ___CORE_INTERFACE_MIXIN],
    template: '<div :id="el_id"></div>',
    data: function () {
        return {
            code: 'com-nav-bottom',
            el_id: 'com-nav-bottom-' + this._uid + '-' + (new Date().getTime()),
            uc_toolbar_name: 'uc_toolbar_name' + this.el_id
        };
    },
    beforeDestroy: function () {
        var _self = this;
        f_log(_self.code + ':: beforeDestroy');
        if (w2ui[_self.uc_toolbar_name]) w2ui[_self.uc_toolbar_name].destroy();
    },
    destroyed: function () {
        var _self = this;
        f_log(_self.code + ':: destroyed');
    },
    beforeCompile: function () {
        var _self = this;
        f_log(_self.code + ':: beforeCompile');
        this.f_base_hide();
    },
    compiled: function () {
        var _self = this;
        f_log(_self.code + ':: compiled');
    },
    ready: function () {
        var _self = this;
        f_log(_self.code + ':: ready');

        _self.f_toolbar_init();

        this.f_base_show();
        //_LOADING.f_hide();
        this.$on(___DATA_BROADCAST.Dictionary, function (data) {
            console.log('??????????? com-nav-bottom = ', data.length);
        });
    },
    methods: {
        f_toolbar_init: function () {
            var _self = this;

            if (!w2ui[_self.uc_toolbar_name]) {
                $(_self.$el).w2toolbar({
                    name: _self.uc_toolbar_name,
                    style: 'background:#fff;',
                    onClick: function (tbEvent) {
                        switch (tbEvent.target) {
                            case 'id_toolbar_suggestion':
                                _self.suggestion_Show();
                                break;
                            default:
                                var id = '#tb_' + this.name + '_item_' + w2utils.escapeId(event.target);
                                $(id).w2overlay('<div style="margin:8px;">Pressed ' + (new Date().getTime()) + '</div>');
                                break;
                        }
                    },
                    items: [
                        {
                            type: 'menu-radio', id: 'item2', icon: 'fa fa-bars',
                            text: function (item) {
                                var text = item.selected;
                                var el = this.get('item2:' + item.selected);
                                return '' + el.text + '';
                            },
                            selected: 'id3',
                            items: [
                                { id: 'id1', text: 'Item 1', icon: 'fa fa-camera' },
                                { id: 'id2', text: 'Item 2', icon: 'fa fa-picture-o' },
                                { id: 'id3', text: 'Item 3', icon: 'fa fa-glass', count: 12 }
                            ]
                        },
                        { type: 'button', id: 'id_toolbar_home', text: '', icon: 'icon-basic-home' },
                        { type: 'button', id: 'id_toolbar_search', text: '', icon: 'icon-basic-magnifier' },
                        { type: 'button', id: 'id_toolbar_suggestion', text: '', icon: 'fa fa-list-ol' },
                        { type: 'spacer' },
                        { type: 'button', id: 'id_toolbar_task', text: '', icon: 'icon-basic-mixer2', count: 7, },
                        { type: 'button', id: 'id_toolbar_mail', text: '', icon: 'fa fa-envelope-o', count: 7, },
                        { type: 'button', id: 'id_toolbar_chat', text: '', icon: 'icon-basic-message-multiple', count: 9, },
                        {
                            type: 'drop', id: 'item4', text: '', icon: 'fa fa-bell-o', tooltip: 'Notification Messages', count: 5,
                            html: '<div style="padding: 10px; line-height: 1.5">You can put any HTML in the drop down.<br>Include tables, images, etc.</div>'
                        },
                        {
                            type: 'menu', id: 'id_toolbar_user_menu', text: '', icon: 'fa fa-user-o', items: [
                                { type: 'button', id: 'id_toolbar_user', text: 'Account Infomation', icon: 'icon-basic-info' },
                                { type: 'button', id: 'id_toolbar_help', text: 'Help', icon: 'icon-basic-question' },
                                { type: 'button', id: 'id_toolbar_settings', text: 'Settings', icon: 'icon-basic-gear' },
                                { text: '--' },
                                { text: 'Item 1', icon: 'fa fa-camera', count: 5 },
                                { text: 'Item 2', icon: 'fa fa-picture-o', disabled: true },
                                { text: 'Item 3', icon: 'fa fa-glass', count: 12 },
                                { text: '--' },
                                { type: 'button', id: 'id_toolbar_logout', text: 'Logout', icon: 'fa fa-sign-out' },
                                { type: 'button', id: 'id_toolbar_close', text: 'Exit Application', icon: 'icon-arrows-circle-remove' },
                            ]
                        },

                        ////{
                        ////    type: 'menu', id: 'item1', text: 'Menu', icon: 'fa fa-table', count: 17, items: [
                        ////      { text: 'Item 1', icon: 'fa fa-camera', count: 5 },
                        ////      { text: 'Item 2', icon: 'fa fa-picture-o', disabled: true },
                        ////      { text: 'Item 3', icon: 'fa fa-glass', count: 12 }
                        ////    ]
                        ////},
                        ////{ type: 'break' },
                        ////{
                        ////    type: 'menu-radio', id: 'item2', icon: 'fa fa-star',
                        ////    text: function (item) {
                        ////        var text = item.selected;
                        ////        var el = this.get('item2:' + item.selected);
                        ////        return 'Radio: ' + el.text;
                        ////    },
                        ////    selected: 'id3',
                        ////    items: [
                        ////        { id: 'id1', text: 'Item 1', icon: 'fa fa-camera' },
                        ////        { id: 'id2', text: 'Item 2', icon: 'fa fa-picture-o' },
                        ////        { id: 'id3', text: 'Item 3', icon: 'fa fa-glass', count: 12 }
                        ////    ]
                        ////},
                        ////{ type: 'break' },
                        ////{
                        ////    type: 'menu-check', id: 'item3', text: 'Check', icon: 'fa fa-heart',
                        ////    selected: ['id3', 'id4'],
                        ////    onRefresh: function (event) {
                        ////        event.item.count = event.item.selected.length;
                        ////    },
                        ////    items: [
                        ////        { id: 'id1', text: 'Item 1', icon: 'fa fa-camera' },
                        ////        { id: 'id2', text: 'Item 2', icon: 'fa fa-picture-o' },
                        ////        { id: 'id3', text: 'Item 3', icon: 'fa fa-glass', count: 12 },
                        ////        { text: '--' },
                        ////        { id: 'id4', text: 'Item 4', icon: 'fa fa-glass' }
                        ////    ]
                        ////},
                        ////{ type: 'break' },
                        ////{
                        ////    type: 'drop', id: 'item4', text: 'Dropdown', icon: 'fa fa-plus',
                        ////    html: '<div style="padding: 10px; line-height: 1.5">You can put any HTML in the drop down.<br>Include tables, images, etc.</div>'
                        ////},
                        ////{ type: 'break', id: 'break3' },
                        ////{
                        ////    type: 'html', id: 'item5',
                        ////    html: function (item) {
                        ////        var html =
                        ////          '<div style="padding: 3px 10px;">' +
                        ////          ' CUSTOM:' +
                        ////          '    <input size="10" onchange="var el = w2ui.toolbar.set(\'item5\', { value: this.value });" ' +
                        ////          '         style="padding: 3px; border-radius: 2px; border: 1px solid silver" value="' + (item.value || '') + '"/>' +
                        ////          '</div>';
                        ////        return html;
                        ////    }
                        ////},
                        ////{ type: 'spacer' },
                        ////{ type: 'button', id: 'item6', text: 'Item 6', icon: 'fa fa-flag' }

                    ]
                });
            }
        }
    }
});

Vue.component('com-dictionary', {
    mixins: [___COMS_MIXIN, ___SCREENS_COMMON_MIXIN, ___CORE_INTERFACE_MIXIN],
    template: '<div :id="el_id"><ul><li v-for="(index, it) in items" @click="itemClick(it)"><label>{{it.text}}</label><p>{{it.mean}}</p><ol><li><em>More</em><li></ol></li></ul></div>',
    data: function () {
        var _self = this;

        //console.log('Dictionary = ', ___DATA.objCore.Translate.Dictionary);
        //console.log('objCore = ', _self.objCore);
        //console.log('screenInfo = ', _self.screenInfo);

        var _items = [];
        for (var key in ___DATA.objCore.Translate.Dictionary) {
            _items.push({ id: key.split(' ').join('_'), text: key, mean: ___DATA.objCore.Translate.Dictionary[key] });
        }
        console.log('_items = ', _items);

        return {
            code: 'com-dictionary',
            el_id: 'com-' + this._uid + '-' + (new Date().getTime()),
            items: _items
        };
    },
    beforeDestroy: function () {
        var _self = this;
        f_log(_self.code + ':: beforeDestroy');
        _self.freeResource();
    },
    destroyed: function () {
        var _self = this;
        f_log(_self.code + ':: destroyed');
    },
    beforeCompile: function () {
        var _self = this;
        f_log(_self.code + ':: beforeCompile');
        this.f_base_hide();
    },
    compiled: function () {
        var _self = this;
        f_log(_self.code + ':: compiled');
    },
    ready: function () {
        var _self = this;
        f_log(_self.code + ':: ready');
        _self.setup();
    },
    methods: {
        setup: function () {
        },
        freeResource: function () {
        },
        itemClick: function (item) {
            var _self = this;
            console.log('SELECTED = ', JSON.stringify(item));
            _self.translate_Execute(item.text);
        }
    }
});








