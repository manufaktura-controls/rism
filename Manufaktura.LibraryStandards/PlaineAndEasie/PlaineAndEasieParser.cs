/*BSD 3-Clause License

Copyright (c) 2019, Manufaktura programów Jacek Salamon
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/
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