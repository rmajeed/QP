using Quran.DAL.Implementations;
using Quran.Model;
using Quran.Repository;
using QuranEntryUI.Implementation;
using QuranEntryUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace QuranEntryUI
{
    public class QuranProcessor
    {
        #region Public Members

        public static IQuran ProcessXml(string xmlFile)
        {
            IQuran iQuran = null;

            XDocument doc = XDocument.Load(xmlFile);
            var quran = doc.XPathSelectElement("//Quran");

            if (quran != null)
            {
                QuranInfo quranInfo = new QuranInfo();
                quranInfo.Language = Convert.ToInt32(quran.Attribute("language").Value);
                quranInfo.ScriptId = (byte)(quran.Attribute("script").Value.Equals("Usmani") ? 1 : 2);
                quranInfo.Translator = Convert.ToInt32(quran.Attribute("translator").Value);

                Debug.WriteLine(string.Format("Processing Quran data: Language: {0} Script: {1} Translator: {2}.", quranInfo.Language, quran.Attribute("script").Value, quranInfo.Translator));

                var chapters = from elem1 in quran.Descendants("Chapter")
                               where elem1 != null
                               select elem1;

                if (quranInfo.Language == 1)
                {
                    iQuran = new QuranData(quranInfo);
                }
                else
                {
                    iQuran = new LSQuranData(quranInfo);
                }

                iQuran.ProcessChapters(chapters.ToList());
            }

            return iQuran;
        }
        #endregion
    }
}
