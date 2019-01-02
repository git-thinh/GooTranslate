
console.log('core.js .....');

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
    if (header) header.remove();
    //-----------------------------------------------------------------
}

if (document.readyState === "loading") {  // Loading hasn't finished yet
    document.addEventListener("DOMContentLoaded", onDomReady);
} else {  // `DOMContentLoaded` has already fired
    onDomReady();
}