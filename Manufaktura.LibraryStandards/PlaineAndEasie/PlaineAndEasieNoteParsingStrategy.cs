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
using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieNoteParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private static readonly char[] Steps = new[] { 'C', 'D', 'E', 'F', 'G', 'A', 'B' };
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => Steps.Any(c => c == s[0]);

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var hasTrill = false;
            var hasSlur = false;

            var i = 0;
            var step = s[i++];
            for (; i < s.Length; i++)
            {
                if (s[i] == 't') hasTrill = true;
                else if (s[i] == '+') hasSlur = true;
                else break;
            }

            if (parser.LastAddedStep != default(char) && parser.LastAddedStep != step)
            {
                parser.PendingAlter = 0;
                parser.PendingNatural = false;
            }

            parser.AddNote(step, parser.PendingAlter, parser.PendingNatural, hasTrill, hasSlur);
            parser.LastAddedStep = step;
            if (parser.IsGroupingEnabled) parser.GroupSize++;

            return i;
        }
    }
}