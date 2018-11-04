using Manufaktura.Controls.Extensions;
using Manufaktura.Controls.Model;
using Manufaktura.LibraryStandards.PlaineAndEasie;
using Manufaktura.Music.Model;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Services.PlaineAndEasie
{
    public class PlaineAndEasie2ScoreParser : PlaineAndEasieParser<Score>
    {
        private List<Note> notesToMakeTuple = new List<Note>();
        private List<Note> notesToRebeam = new List<Note>();
        protected override void AddBarline(PlaineAndEasieBarlineTypes barlineType)
        {
            var barlineStyle = barlineType != PlaineAndEasieBarlineTypes.Single ? BarlineStyle.LightHeavy : BarlineStyle.Regular;
            var barline = new Barline(barlineStyle);

            if (barlineType == PlaineAndEasieBarlineTypes.RepeatForward) barline.RepeatSign = RepeatSignType.Forward;
            else if (barlineType == PlaineAndEasieBarlineTypes.RepeatBackward) barline.RepeatSign = RepeatSignType.Backward;
            else if (barlineType == PlaineAndEasieBarlineTypes.RepeatBoth) barline.RepeatSign = RepeatSignType.Backward;

            output.FirstStaff.Add(barline);
            if (barlineType == PlaineAndEasieBarlineTypes.RepeatBoth) output.FirstStaff.Add(new Barline { RepeatSign = RepeatSignType.Forward });
        }

        protected override void AddClef(char clefType, int lineNumber, bool isMensural)
        {
            var scoreClefType = ParseClefType(clefType);
            var octaveShift = clefType == 'g' ? -1 : 0;
            output.FirstStaff.Add(new Clef(scoreClefType, lineNumber, octaveShift));
        }

        protected override void AddFermata()
        {
            notesToMakeTuple.Clear();

            var note = output.FirstStaff.Elements.OfType<Note>().LastOrDefault();
            if (note != null) note.HasFermataSign = true;
        }

        protected override void AddKey(int numberOfFifths)
        {
            output.FirstStaff.Add(new Key(numberOfFifths));
        }

        protected override void AddNote(char step, int alter, bool hasNatural, bool hasTrill, bool hasSlur)
        {
            var currentKey = output.FirstStaff.Elements.OfType<Key>().LastOrDefault();
            var keyAlter = currentKey?.StepToAlter(step.ToString()) ?? 0;

            var lastNote = output.FirstStaff.Elements.OfType<Note>().LastOrDefault();

            var note = new Note(new Pitch(step.ToString(), alter == 0 ? keyAlter : alter, CurrentOctave),
                new RhythmicDuration(CurrentRhythmicLogValue, CurrentNumberOfDots))
            {
                HasNatural = hasNatural,
                TrillMark = hasTrill ? NoteTrillMark.Above : NoteTrillMark.None,
            };
            if (lastNote != null && lastNote.Slurs.Any(s => s.Type == NoteSlurType.Start))
                note.Slurs.Add(new Slur(NoteSlurType.Stop));
            if (hasSlur)
                note.Slurs.Add(new Slur(NoteSlurType.Start));

            output.FirstStaff.Add(note);
            if (IsBeamingEnabled) notesToRebeam.Add(note);
            if (IsGroupingEnabled) notesToMakeTuple.Add(note);
        }

        protected override void AddRest()
        {
            output.FirstStaff.Add(new Rest(new RhythmicDuration(CurrentRhythmicLogValue)));
        }

        protected override void AddTimeSignature(string symbol, int numerator, int denominator)
        {
            if (symbol == "c") output.FirstStaff.Add(TimeSignature.CommonTime);
            else if (symbol == "c/") output.FirstStaff.Add(TimeSignature.CutTime);
            else output.FirstStaff.Add(new TimeSignature(TimeSignatureType.Numbers, numerator, denominator));
        }

        protected override void AddWholeMeasureRests(int numberOfMeasures)
        {
            output.FirstStaff.Add(new Rest(RhythmicDuration.Whole) { MultiMeasure = numberOfMeasures });
        }

        protected override Score CreateOutputObject()
        {
            var score = new Score();
            score.Staves.Add(new Staff());
            return score;
        }

        protected override void MakeTuple()
        {
            if (notesToMakeTuple.Count < 2) return;

            notesToMakeTuple.First().Tuplet = TupletType.Start;
            notesToMakeTuple.Last().Tuplet = TupletType.Stop;

            notesToMakeTuple.Clear();
        }

        protected override void RebeamGroup()
        {
            if (!notesToRebeam.Any()) return;

            var upDirectionRatio = notesToRebeam.Count(n => n.StemDirection == VerticalDirection.Up) / notesToRebeam.Count;
            notesToRebeam.ApplyStemDirection(upDirectionRatio > 0.5 ? VerticalDirection.Up : VerticalDirection.Down);
            notesToRebeam.Rebeam(Controls.Formatting.RebeamMode.Simple);
            notesToRebeam.Clear();
        }

        private static ClefType ParseClefType(char peClefType)
        {
            switch (peClefType)
            {
                case 'C': return ClefType.CClef;
                case 'F': return ClefType.FClef;
                case 'G': return ClefType.GClef;
                case 'g': return ClefType.GClef;
                default: return ClefType.GClef;
            }
        }
    }
}