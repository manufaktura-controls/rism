var PlaybackManager = function () {
    var self = this;
    this.isPlaying = ko.observable(false);
    this.currentMelodyId = ko.observable();
    this.stopToken = 0;

    this.play = function (melodyId) {
        var svg = $("#melody-" + melodyId + " svg");

        self.currentMelodyId(melodyId);

        self.isPlaying(true);
        self.stopToken++;
        var currentStopToken = self.stopToken;
        var overalTime = 0;

        function colorBlack(elements) {
            for (var i in elements) {
                var e = elements[i];
                if (e.tagName === "line" || e.tagName === "path") e.style.stroke = "#000";
                else e.style.fill = "#000";
            }
        }
        function colorRed(elements) {
            for (var i in elements) {
                var e = elements[i];
                if (e.tagName === "line" || e.tagName === "path") e.style.stroke = "#c34853";
                else e.style.fill = "#c34853";
            }
        }

        function getNoteForIdAndRepetition(noteCollection, id, repetition) {
            if (noteCollection == null) return null;
            for (var n in noteCollection) {
                if (noteCollection[n].id === id && noteCollection[n].repetition === repetition) return noteCollection[n];
            }
            return null;
        }

        var notes = [];

        svg.children().each(function (i, e) {
            var unparsedPlaybackStartAttribute = $(e).attr("data-playback-start");
            if (unparsedPlaybackStartAttribute == null) return;

            var delayTimes = unparsedPlaybackStartAttribute.split(" ");
            if (delayTimes == null) return;

            var pitchUnparsed = $(e).attr("data-midi-pitch");
            var pitch = pitchUnparsed ? parseInt(pitchUnparsed) : null;
            var durationUnparsed = $(e).attr("data-playback-duration");
            if (durationUnparsed == null) return;
            var duration = parseInt(durationUnparsed);
            var elementId = $(e).attr("id");

            for (var repetitionNumber in delayTimes) {
                var delayTime = parseInt(delayTimes[repetitionNumber]);

                var existingNoteInfo = getNoteForIdAndRepetition(notes, elementId, repetitionNumber);
                if (existingNoteInfo != null) {
                    existingNoteInfo.elements.push(e);
                }
                else {
                    var note = { delayTime: delayTime, pitch: pitch, duration: duration, elements: [], id: elementId, repetition: repetitionNumber };
                    note.elements.push(e);
                    notes.push(note);
                    overalTime = delayTime + duration;
                }
            }
        });

        for (var i in notes) {
            var noteInfo = notes[i];

            setTimeout(function (note) {
                return function () {
                    if (self.stopToken !== currentStopToken) {
                        colorBlack(note.elements);
                        return;
                    }

                    if (note.pitch != null) {
                        //console.info('Playing repetition ' + note.pitch + ' with ' + note.elements.length + ' elements.');
                        MIDI.noteOn(0, note.pitch, 127, 0);
                        MIDI.noteOff(0, note.pitch, note.duration * 0.001);
                    }
                    colorRed(note.elements);
                };
            }(noteInfo), noteInfo.delayTime);

            setTimeout(function (note) {
                return function () {
                    colorBlack(note.elements);
                };
            }(noteInfo), noteInfo.delayTime + noteInfo.duration);
        }

        setTimeout(function () {
            if (self.stopToken !== currentStopToken) return;
            self.isPlaying(false);
        }, overalTime);
    }

    this.stop = function () {
        self.stopToken++;
        self.isPlaying(false);
    }
}




