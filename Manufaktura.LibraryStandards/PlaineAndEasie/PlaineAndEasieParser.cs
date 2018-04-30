using System;
using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieParser<TOutput> where TOutput : class, new()
    {
        private Lazy<PlaineAndEasieParsingStrategy<TOutput>[]> strategies;

        protected PlaineAndEasieParser()
        {
            strategies = new Lazy<PlaineAndEasieParsingStrategy<TOutput>[]>(() => GetType().Assembly.GetTypes()
                    .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(PlaineAndEasieParsingStrategy<TOutput>)))
                    .Select(t => Activator.CreateInstance(t))
                    .Cast<PlaineAndEasieParsingStrategy<TOutput>>()
                    .ToArray());
        }

        public TOutput Parse(string plaineAndEasie)
        {
            var output = new TOutput();
            for (var i = 0; i < plaineAndEasie.Length; i++)
            {
                var strategy = strategies.Value.FirstOrDefault(s => s.IsRelevant(plaineAndEasie.Substring(i)));
                if (strategy == null)
                {
                    continue;
                }

                i += strategy.Parse(plaineAndEasie.Substring(i), output);
            }
            return output;
        }
    }
}