
function registerKeyboard(keyboard) {
    window.incipitKeyboard = keyboard;
}

$(window).resize(function () {
    if (!window.incipitKeyboard) return;
    window.incipitKeyboard.invokeMethod('OnWindowResize', $("#searchPanel").width());
});