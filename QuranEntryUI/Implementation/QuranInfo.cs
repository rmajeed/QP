using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranEntryUI.Implementation
{
    public struct QuranInfo
    {
        public byte ScriptId;
        public int Translator;
        public int Language;

        public QuranInfo(byte script, int translator, int language)
        {
            ScriptId = script;
            Translator = translator;
            Language = language;
        }
    }
}
