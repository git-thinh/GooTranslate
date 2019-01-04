function CoreInterface() {
    this._elSuggestion = document.getElementById('gt-src-is');
}
CoreInterface.prototype = {
    setup: function () {
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
            ___CORE.suggestion_getData();
        }, 100);
    },
    //========================================================================
    suggestion_eventChange: function (e) {
        //___CORE.suggestion_getData();
    },
    suggestion_getData: function () {
        var ell = this._elSuggestion.querySelectorAll('.gt-is-itm .gt-is-sg');
        var elr = this._elSuggestion.querySelectorAll('.gt-is-itm .gt-is-tr');
        for (var i = 0; i < ell.length; i++) {
            var text = ell[i].textContent;
            var mean = elr[i].textContent;
            console.log(text, mean);
        }
    },
    suggestion_Show: function () {
        this._elSuggestion.style.display = 'inline-block';
    }
};
___CORE = new CoreInterface();
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
___CORE_INTERFACE_MIXIN = {
    methods: {
        suggestion_Show: function () {
            ___CORE.suggestion_Show();
        } 
    }
};