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
    window.viewModel.startNewSearch(searchQuery);
}

function playNote(note) {
    setTimeout(function () {
        MIDI.noteOn(0, note, 127, 0);
    }, 0);

    setTimeout(function () {
        MIDI.noteOff(0, note, 0);
    }, 500);
}

function playQuery(note) {
    window.viewModel.player.play("query");
}

$(window).resize(function () {
    if (!window.incipitKeyboard) return;
    window.incipitKeyboard.invokeMethod('OnWindowResize', $("#searchPanel").width());
});