if (document && document.body) document.body.style.display = 'none';

console.log('core.js .....');


function __getMeanText() {
    var el = document.querySelector('.text-wrap.tlid-copy-target');
    if (el) return el.textContent;
    return '';
}

function onDomReady() {
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
    setTimeout(function () {
        if (document && document.body) document.body.style.display = 'block';
    }, 100);
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", onDomReady);
} else {
    onDomReady();
}