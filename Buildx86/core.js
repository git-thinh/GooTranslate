//https://able.bio/drenther/track-page-visibility-in-react-using-render-props--78o9yw5

var _IS_KEY_BACKSPACE_OR_DELETE = false;
var _IS_KEY_ENTER = false;
//=====================================================================
if (document && document.body) document.body.style.display = 'none';

console.log('core.js .....');


function ___getMeanText() {
    var el = document.querySelector('.text-wrap.tlid-copy-target');
    if (el) return el.textContent;
    return '';
}

function ___inputKeydown(e) {
    var key = e.keyCode || e.charCode;
    if (key == 8 || key == 46) {
        ___inputProcess('DELETE_CHAR')
        return;
    }
}

function ___inputKeypress(e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code == 13) {
        ___inputProcess('ENTER')
        return;
    }
    ___inputProcess('DATA')
}

function ___inputProcess(key) {
    setTimeout(function () {
        var _input = document.getElementById('source').value;
        console.log(key, _input);
    }, 100);
}

function ___onDomReady() {
    console.info('DOM loaded');
    //-----------------------------------------------------------------
    var uri_core_css = '[URL_CORE_CSS]';
    console.info('URL_CORE_CSS = ', uri_core_css);
    var head = document.getElementsByTagName('head')[0];
    var link = document.createElement('link');
    link.rel = 'stylesheet';
    link.type = 'text/css';
    link.href = uri_core_css;
    link.media = 'all';
    head.appendChild(link);
    //-----------------------------------------------------------------
    var header = document.getElementById('gb');
    if (header) header.style.display = 'none';
    //-----------------------------------------------------------------
    //document.body.addEventListener('DOMSubtreeModified', function () {
    //    var time = 'DOM Changed at ' + new Date();
    //    console.log(time);
    //}, false);

    //var elSuggest = document.getElementById('gt-src-is')
    //if (elSuggest) elSuggest.addEventListener('DOMSubtreeModified', function (event) {
    //    var source = document.getElementById('source');
    //    if (source) {
    //        var _input = source.value;
    //        console.log(_input);
    //    }
    //}, false);
    //-----------------------------------------------------------------
    var elSource = document.getElementById('source')
    if (elSource) {
        elSource.setAttribute('onkeypress', '___inputKeypress(event, this.value.trim())');
        elSource.setAttribute('onkeydown', '___inputKeydown(event)');
    }
    //-----------------------------------------------------------------
    setTimeout(function () {
        if (document && document.body) document.body.style.display = 'block';
    }, 100);
}

if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", ___onDomReady); else ___onDomReady();