
function registerKeyboard(keyboard) {
    window.incipitKeyboard = keyboard;

    $(".btn-key").click(function (event) {
        var pitch = $(event.currentTarget).attr("midiPitch");
        window.noteViewer.invokeMethod('AddNote', pitch);
    });
}

function registerNoteViewer(noteViewer) {
    window.noteViewer = noteViewer;
}

$(window).resize(function () {
    if (!window.incipitKeyboard) return;
    window.incipitKeyboard.invokeMethod('OnWindowResize', $("#searchPanel").width());
});