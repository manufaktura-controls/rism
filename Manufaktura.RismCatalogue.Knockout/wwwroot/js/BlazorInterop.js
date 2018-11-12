function registerKeyboard(keyboard) {
    window.incipitKeyboard = keyboard;

    $(".btn-key").each(function (i, element) {
        if ($(element).attr("initialized")) return;

        $(element).click(function (event) {
            var pitch = $(event.currentTarget).attr("midiPitch");
            window.noteViewer.invokeMethod('AddNote', pitch);
        });
        $(element).attr("initialized", true);
    });
}

function registerNoteViewer(noteViewer) {
    if (window.noteViewer) return;
    window.noteViewer = noteViewer;
}

function startNewSearch(searchQuery) {

}

$(window).resize(function () {
    if (!window.incipitKeyboard) return;
    window.incipitKeyboard.invokeMethod('OnWindowResize', $("#searchPanel").width());
});