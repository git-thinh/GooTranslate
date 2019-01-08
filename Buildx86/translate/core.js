/*============================================================
/ DATA: CONTRACTOR - CREATE EVENT DATA CHANGE
/============================================================*/
___DATA_BROADCAST = { Dictionary: 'dic' };
for (var v in ___DATA_BROADCAST) ___registerDataBroadcast(___DATA_BROADCAST[v]);
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
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
            f_log('AUTO_SAVE: ', ___DATA[___DATA_BROADCAST.Dictionary]);
        }
    },
    //========================================================================
    // DICTIONARY
    dictionary_Add: function (text, mean) {
        var _self = this;
        if (text == null || mean == null || text.length == 0 || mean.length == 0) return false;
        text = text.toLowerCase().trim();
        if (___DATA[___DATA_BROADCAST.Dictionary][text] == null) {
            ___DATA[___DATA_BROADCAST.Dictionary][text] = mean;
            _self._hasChangeData = true;
            f_log('DICTIONARY_ADD: ', text);
            ___sendBroadcastData(___DATA_BROADCAST.Dictionary);
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
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////

var _SELECT_OBJ = { x: 0, y: 0, text: '', id: '' };
////////////////////////////////////////////////////////////
var _CLIENT_ID = 1;
var _GET_ID = function () { var date = new Date(); var id = _CLIENT_ID + ("0" + (date.getMonth() + 1)).slice(-2) + ("0" + date.getDate()).slice(-2) + ("0" + date.getHours()).slice(-2) + ("0" + date.getMinutes()).slice(-2) + ("0" + date.getSeconds()).slice(-2) + (date.getMilliseconds() + Math.floor(Math.random() * 100)).toString().substring(0, 3); return parseInt(id); };
var APP_INFO = { Width: $(window).width() };
////////////////////////////////////////////////////////////

function f_english_Translate() {
    if (_SELECT_OBJ != null) {
        var otran = JSON.parse(JSON.stringify(_SELECT_OBJ));
        f_log('TRANSLATE ', otran);
        var text = otran.text.toLowerCase().trim();

        //___API.speechWord(text, 1);
        //___API.playMp3FromUrl('https://s3.amazonaws.com/audio.oxforddictionaries.com/en/mp3/hello_gb_1.mp3', 1);
        //___API.playMp3FromUrl('http://audio.oxforddictionaries.com/en/mp3/ranker_gb_1_8.mp3', 1);

        //var audio = document.createElement('embed');
        ////audio.style.display = 'none';
        //audio.id = 'iframeAudio';
        //audio.width = '100px';
        //audio.height = '100px';
        //audio.autostart = 'true';
        //audio.autoplay = 'true';
        //audio.setAttribute('src', 'https://s3.amazonaws.com/audio.oxforddictionaries.com/en/mp3/hello_gb_1.mp3');
        ////audio.play();
        //document.body.appendChild(audio);

        // <iframe src="audio/source.mp3" allow="autoplay" style="display:none" id="iframeAudio"></iframe> 
        // <audio autoplay loop  id="playAudio"><source src="audio/source.mp3"></audio>
        // 

        ////if (_.some(_words, function (w) { return w == text; }) == false) _words.push(text);

        ////f_post('//api/translate/v1', otran.text, function (_res) {
        ////    f_log('OK', _res);
        ////    if (_res && _res.length > 0) {
        ////        otran.mean_vi = _res;
        ////        f_english_TranslateShowResult(otran);
        ////    } else {

        ////    }
        ////}, function (_err) {
        ////    f_log('ERR', _err);
        ////})
    }
}

function f_event_processCenter(event) {
    var type = event.type,
        el = event.target,
        tagName = el.tagName,
        id = el.id,
        text = el.innerText,
        textSelect = '';

    if (id == null || id.trim().length == 0) {
        var id = _GET_ID();
        el.setAttribute('id', id);
    }

    textSelect = window.getSelection().toString();
    switch (type) {
        case 'mousedown':
            //if (console.clear) console.clear();

            var elbox = document.getElementById('___box_tran');
            if (elbox) elbox.style.display = 'none';

            _SELECT_OBJ = { id: id, cached: false, x: event.x, y: event.y };
            if (el.className == '___translated') _SELECT_OBJ.cached = true;

            break;
        case 'mouseup':
            if (_SELECT_OBJ != null) {
                _SELECT_OBJ.x = event.x;
                _SELECT_OBJ.y = event.y;
                if (textSelect && textSelect.trim().length > 0) _SELECT_OBJ.text = textSelect;
            }
            break;
        case 'click':
            if (_SELECT_OBJ != null) {
                if (textSelect && textSelect.trim().length > 0) _SELECT_OBJ.text = textSelect;
            }
            break;
        case 'dblclick':
            if (_SELECT_OBJ != null) {
                if (textSelect && textSelect.trim().length > 0) _SELECT_OBJ.text = textSelect;
            }
            break;
    }

    //f_log(tagName + '.' + type + ': ' + JSON.stringify(_SELECT_OBJ));

    if (_SELECT_OBJ != null) {
        if (_SELECT_OBJ.cached == true) {
            f_displayTranslateCache(_SELECT_OBJ);
            _SELECT_OBJ = null;
        } else {
            if (_SELECT_OBJ.text && _SELECT_OBJ.text.length > 0) {
                f_english_Translate();
                _SELECT_OBJ = null;
            }
        }
    }

    //f_log(tagName + '.' + type + ': ' + id + ' \r\nSELECT= ' + textSelect + ' \r\nTEXT= ', text);
    //event.preventDefault();
    //event.stopPropagation();
}

if (window.addEventListener) {
    window.addEventListener("mouseup", f_event_processCenter, true);
    window.addEventListener("mousedown", f_event_processCenter, true);
    window.addEventListener("click", f_event_processCenter, true);
    window.addEventListener("dblclick", f_event_processCenter, true);
}
