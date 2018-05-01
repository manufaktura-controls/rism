using Manufaktura.Controls.Extensions;
using Manufaktura.Controls.Model;
using Manufaktura.LibraryStandards.PlaineAndEasie;
using Manufaktura.Music.Model;
using System.Collections.Generic;

namespace Manufaktura.RismCatalogue.Services.PlaineAndEasie
{
    public class PlaineAndEasie2ScoreParser : PlaineAndEasieParser<Score>
    {
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

        protected override void AddKey(int numberOfFifths)
        {
            output.FirstStaff.Add(new Key(numberOfFifths));
        }

        protected override void AddNote(char step, int alter)
        {
            var note = new Note(new Pitch(step.ToString(), alter, CurrentOctave), new RhythmicDuration(CurrentRhythmicLogValue));
            output.FirstStaff.Add(note);
            if (IsBeamingEnabled) notesToRebeam.Add(note);
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
            
        }

        protected override Score CreateOutputObject()
        {
            var score = new Score();
            score.Staves.Add(new Staff());
            return score;
        }

        protected override void OnBeamingEnded()
        {
            base.OnBeamingEnded();
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