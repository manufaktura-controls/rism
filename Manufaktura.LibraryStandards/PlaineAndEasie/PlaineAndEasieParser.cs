using System;
using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public abstract class PlaineAndEasieParser
    {
        protected internal abstract void AddClef(char clefType, int lineNumber, bool isMensural);

        protected internal abstract void AddKey(int numberOfFifths);

        protected internal abstract void AddTimeSignature(string symbol, int numerator, int denominator);

        protected internal abstract void AddNote(char step, int alter);
        protected internal abstract void AddRest();

        protected internal abstract void AddWholeMeasureRests(int numberOfMeasures);

        protected internal abstract void AddBarline(PlaineAndEasieBarlineTypes barlineType);

        protected internal int CurrentRhythmicLogValue { get; set; } = 2;
        protected internal int CurrentNumberOfDots { get; set; }
        protected internal bool IsBeamingEnabled { get; set; }
        protected internal int CurrentOctave { get; set; } = 4;

        protected internal virtual void OnBeamingEnded()
        {
        }
    }

    public abstract class PlaineAndEasieParser<TOutput> : PlaineAndEasieParser
    {
        private static Lazy<PlaineAndEasieParsingStrategy[]> strategies;

        protected abstract TOutput CreateOutputObject();

        protected readonly TOutput output;

        protected PlaineAndEasieParser()
        {
            strategies = new Lazy<PlaineAndEasieParsingStrategy[]>(() => typeof(PlaineAndEasieParsingStrategy).Assembly.GetTypes()
                    .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(PlaineAndEasieParsingStrategy)))
                    .Select(t => Activator.CreateInstance(t))
                    .Cast<PlaineAndEasieParsingStrategy>()
                    .ToArray());
            output = CreateOutputObject();
        }

        public TOutput Parse(string plaineAndEasie)
        {
            for (var i = 0; i < plaineAndEasie.Length;)
            {
                var strategy = strategies.Value.FirstOrDefault(s => s.IsRelevant(plaineAndEasie.Substring(i)));
                if (strategy == null)
                {
                    i++;
                    continue;
                }

                i += strategy.ControlSignLength;
                i += strategy.Parse(this, plaineAndEasie.Substring(i));
            }
            return output;
        }
    }
}