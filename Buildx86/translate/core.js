___DATA.objCore.Translate = { Dictionary: {} };
___SCREENS = {
    NAV_BOTTOM: { Id: 'navi-bottom-1001', className: 'screen-hook-nav-bottom', Components: 'com-nav-bottom', overlayVisiable: false, Footer: { buttonOk: false, buttonCancel: false } },
    DICTIONARY: { Id: 'screen-dictionary-1001', className: 'screen-dictionary', Components: 'com-dictionary', overlayVisiable: false, Footer: { buttonOk: false, buttonCancel: false } },
};
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
function CoreInterface() {
    this._timeOutAutoSave = 15000;
    this._hasChangeData = false;
    this._elSuggestion = document.getElementById('gt-src-is');
    this._elInputTranslate = document.getElementById('source');
}
CoreInterface.prototype = {
    setup: function () {
        var _self = this;
        //-----------------------------------------------------------------
        _self.autoSave_RunInterval();
        //-----------------------------------------------------------------
        var header = document.getElementById('gb');
        if (header) header.style.display = 'none';
        //-----------------------------------------------------------------
        //document.body.addEventListener('DOMSubtreeModified', function () {
        //    var time = 'DOM Changed at ' + new Date();
        //    console.log(time);
        //}, false);

        if (this._elSuggestion) this._elSuggestion.addEventListener('DOMSubtreeModified', this.suggestion_eventChange, false);
        //-----------------------------------------------------------------
        var elSource = document.getElementById('source');
        if (elSource) {
            elSource.setAttribute('onkeypress', '___CORE.___inputKeypress(event, this.value.trim())');
            elSource.setAttribute('onkeydown', '___CORE.___inputKeydown(event)');
        }
        //-----------------------------------------------------------------
        setTimeout(function () {
            console.log('STATE_READY');
        }, 100);
        setTimeout(function () {
            ___screenOpen(___SCREENS.NAV_BOTTOM);
        }, 300);
    },
    //========================================================================
    // TRANSLATE
    translate_Execute: function (text) {
        var _self = this;
        _self._elInputTranslate.value = text;
    },
    //========================================================================
    // AUTO_SAVE
    autoSave_RunInterval: function () {
        var _self = this;
        setInterval(function () { _self.autoSave_Update(); }, _self._timeOutAutoSave);
    },
    autoSave_Update: function () {
        var _self = this;
        if (_self._hasChangeData) {
            //Boolean f_api_writeFile(String file, String data)
            //___API.f_api_writeFile();
            _self._hasChangeData = false;
            f_log('AUTO_SAVE: ', ___DATA.objCore.Translate.Dictionary);
        }
    },
    //========================================================================
    // DICTIONARY
    dictionary_Add: function (text, mean) {
        var _self = this;
        if (text == null || mean == null || text.length == 0 || mean.length == 0) return false;
        text = text.toLowerCase().trim();
        if (___DATA.objCore.Translate.Dictionary[text] == null) {
            ___DATA.objCore.Translate.Dictionary[text] = mean;
            _self._hasChangeData = true;
            f_log('DICTIONARY_ADD: ', text);
            return true;
        }
        return false;
    },
    //========================================================================
    ___getMeanText: function () {
        var el = document.querySelector('.text-wrap.tlid-copy-target');
        if (el) return el.textContent;
        return '';
    },
    ___inputKeydown: function (e) {
        var key = e.keyCode || e.charCode;
        if (key == 8 || key == 46) {
            this.___inputProcess('DELETE_CHAR');
            return;
        }
    },
    ___inputKeypress: function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) {
            this.___inputProcess('ENTER');
            return;
        }
        this.___inputProcess('DATA');
    },
    ___inputProcess: function (key) {
        setTimeout(function () {
            //var _input = document.getElementById('source').value;
            //console.log(key, _input);
            //___CORE.suggestion_getData();
        }, 100);
    },
    //========================================================================
    suggestion_eventChange: function (e) {
        ___CORE.suggestion_getData();
    },
    suggestion_getData: function () {
        var _self = this;
        var ell = this._elSuggestion.querySelectorAll('.gt-is-itm .gt-is-sg');
        var elr = this._elSuggestion.querySelectorAll('.gt-is-itm .gt-is-tr');
        for (var i = 0; i < ell.length; i++) {
            var text = ell[i].textContent;
            var mean = elr[i].textContent;
            //console.log(text, mean);
            _self.dictionary_Add(text, mean);
        }
    },
    suggestion_Show: function () {
        //this._elSuggestion.style.display = 'inline-block';
        ___screenOpen(___SCREENS.DICTIONARY);
    }
};
___CORE = new CoreInterface();
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
___CORE_INTERFACE_MIXIN = {
    methods: {
        translate_Execute: function (text) {
            ___CORE.translate_Execute(text);
        },
        suggestion_Show: function () {
            ___CORE.suggestion_Show();
        }
    }
};