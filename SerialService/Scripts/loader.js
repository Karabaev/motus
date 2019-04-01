function ShowLoader() {
    var loader = document.querySelector('.loader')
    if (!loader.showModal) {
        dialogPolyfill.registerDialog(loader);
    }
    loader.showModal();
}
function CloseLoader() {
    var loader = document.querySelector('.loader')
    if (loader.showModal) {
        loader.close();
    }
}