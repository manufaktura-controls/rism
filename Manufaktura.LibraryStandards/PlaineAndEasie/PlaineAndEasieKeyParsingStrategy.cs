﻿/*BSD 3-Clause License

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
    public class PlaineAndEasieKeyParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private readonly char[] Flats = new char[] { 'B', 'E', 'A', 'D', 'G', 'C', 'F' };

        private readonly char[] Sharps = new char[] { 'F', 'C', 'G', 'D', 'A', 'E', 'B' };

        public override int ControlSignLength => 1;

        public override bool IsRelevant(string s) => s[0] == '$';
        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length <= 1)
            {
                parser.AddKey(0);
                return s?.Length ?? 0;
            }

            var modifier = s.StartsWith("b") ? -1 : 1;
            var fifths = 0;
            for (var i = 1; i < 7 && i < s.Length; i++)
            {
                if (s[i] != (modifier == 1 ? Sharps : Flats)[i - 1]) break;
                fifths++;
            }

            parser.AddKey(fifths * modifier);

            return fifths + 1;
        }
    }
}