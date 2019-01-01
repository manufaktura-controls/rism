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
namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieRhythmicValueChangeParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => int.TryParse(s[0].ToString(), out _) && !s.StartsWith("7.");   //7. is neumatic notation

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var dots = 0;
            for (var i = 1; i < s.Length; i++)
            {
                if (s[i] == '.') dots++;
                else break;
            }
            parser.CurrentNumberOfDots = dots;
            var rismRhythmicValue = int.Parse(s[0].ToString());
            parser.CurrentRhythmicLogValue = DetermineRhythmicLogValue(rismRhythmicValue);

            return 1 + dots;
        }

        private static int DetermineRhythmicLogValue(int rismRhythmicValue)
        {
            switch (rismRhythmicValue)
            {
                case 0: return -2;
                case 1: return 0;
                case 2: return 1;
                case 3: return 5;
                case 4: return 2;
                case 5: return 6;
                case 6: return 4;
                case 7: return 7;
                case 8: return 3;
                case 9: return -1;
                default: return 2;
            }
        }
    }
}