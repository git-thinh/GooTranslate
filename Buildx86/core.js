console.log('core.js .....');
//https://able.bio/drenther/track-page-visibility-in-react-using-render-props--78o9yw5
var ___LIBS = { Scheme: '', Host: '', fileName: '', appendFiles: [] };
___LIBS = JSON_LIBS___;
console.info('JSON_LIBS = ', JSON.stringify(___LIBS));
//=====================================================================
//=====================================================================
if (document && document.body) document.body.style.display = 'none';

function ___getMeanText() {
    var el = document.querySelector('.text-wrap.tlid-copy-target');
    if (el) return el.textContent;
    return '';
}

function ___inputKeydown(e) {
    var key = e.keyCode || e.charCode;
    if (key == 8 || key == 46) {
        ___inputProcess('DELETE_CHAR');
        return;
    }
}

function ___inputKeypress(e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code == 13) {
        ___inputProcess('ENTER');
        return;
    }
    ___inputProcess('DATA');
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
    var head = document.getElementsByTagName('head')[0], elemLib;
    for (var i = 0; i < ___LIBS.appendFiles.length; i++) {
        var url = ___LIBS.Scheme + '://' + ___LIBS.Host + '/' + ___LIBS.appendFiles[i];
        console.log(' +++++ URL [' + i + '] ', url);
        if (___LIBS.appendFiles[i].indexOf('.css') != -1) {
            elemLib = document.createElement('link');
            elemLib.rel = 'stylesheet';
            elemLib.type = 'text/css';
            elemLib.href = url;
            elemLib.media = 'all';
        } else {
            if (___LIBS.appendFiles[i] == 'jquery.min.js') {
                if (window.jQuery != null) {
                    console.log(' ----- URL [' + i + '] ', url);
                    continue;
                }
            }
            elemLib = document.createElement('script');
            elemLib.src = url;
        }
        head.appendChild(elemLib);
    }
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
    var elSource = document.getElementById('source');
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