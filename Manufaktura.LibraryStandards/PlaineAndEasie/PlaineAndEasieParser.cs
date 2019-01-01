using System;
using System.Linq;
using System.Reflection;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public abstract class PlaineAndEasieParser
    {
        protected internal int CurrentNumberOfDots { get; set; }

        protected internal int CurrentOctave { get; set; } = 4;

        protected internal int CurrentRhythmicLogValue { get; set; } = 2;

        protected internal int GroupSize { get; set; }
        protected internal bool IsBeamingEnabled { get; set; }

        protected internal bool IsGroupingEnabled { get; set; }
        protected internal char LastAddedStep { get; set; }

        protected internal int PendingAlter { get; set; }

        protected internal bool PendingNatural { get; set; }

        protected internal abstract void AddBarline(PlaineAndEasieBarlineTypes barlineType);

        protected internal abstract void AddClef(char clefType, int lineNumber, bool isMensural);

        protected internal abstract void AddFermata();

        protected internal abstract void AddKey(int numberOfFifths);

        protected internal abstract void AddNote(char step, int alter, bool hasNatural, bool hasTrill, bool hasSlur);

        protected internal abstract void AddRest();

        protected internal abstract void AddTimeSignature(string symbol, int numerator, int denominator);

        protected internal abstract void AddWholeMeasureRests(int numberOfMeasures);

        protected internal abstract void MakeTuple();

        protected internal abstract void RebeamGroup();
    }

    public abstract class PlaineAndEasieParser<TOutput> : PlaineAndEasieParser
    {
        protected readonly TOutput output;
        private static Lazy<PlaineAndEasieParsingStrategy[]> strategies;

        protected PlaineAndEasieParser()
        {
            strategies = new Lazy<PlaineAndEasieParsingStrategy[]>(() => typeof(PlaineAndEasieParsingStrategy).GetTypeInfo().Assembly.DefinedTypes
                    .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(PlaineAndEasieParsingStrategy)))
                    .Select(t => Activator.CreateInstance(t.AsType()))
                    .Cast<PlaineAndEasieParsingStrategy>()
                    .ToArray());
            output = CreateOutputObject();
        }

        public TOutput Parse(string clef, string key, string timeSignature, string musicalNotation)
        {
            if (!string.IsNullOrWhiteSpace(clef)) new PlaineAndEasieClefParsingStrategy().Parse(this, clef);
            if (!string.IsNullOrWhiteSpace(key))
                new PlaineAndEasieKeyParsingStrategy().Parse(this, key);
            else
                AddKey(0);

            if (!string.IsNullOrWhiteSpace(timeSignature)) new PlaineAndEasieTimeSignatureParsingStrategy().Parse(this, timeSignature);

            for (var i = 0; i < musicalNotation.Length;)
            {
                var strategy = strategies.Value.FirstOrDefault(s => s.IsRelevant(musicalNotation.Substring(i)));
                if (strategy == null)
                {
                    i++;
                    continue;
                }

                i += strategy.ControlSignLength;
                i += strategy.Parse(this, musicalNotation.Substring(i));
            }
            return output;
        }

        protected abstract TOutput CreateOutputObject();
    }
}