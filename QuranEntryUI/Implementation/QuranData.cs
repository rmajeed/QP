using Quran.DAL.Implementations;
using Quran.Model;
using Quran.Repository;
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

namespace QuranEntryUI.Implementation
{
    public class QuranData : QuranBase
    {
        private QuranData() {}

        public QuranData(QuranInfo info)
            : base(info)
        {
        }

        protected override void UpdateSajdaAndRuku(IChapter chapterData, int ayahId, ref string ayahText)
        {
            //check sajda
            if ((ayahText.Length - ayahText.Replace("۩", "").Length) > 0)
            {
                ayahText = ayahText.Replace("۩", "");
                chapterData.AddSajda(ayahId);
            }

            //Raku check
            if ((ayahText.Length - ayahText.Replace("۞", "").Length) > 0)
            {
                ayahText = ayahText.Replace("۞", "");
                chapterData.AddRuku(ayahId);
            }
        }
    }

    public class LSQuranData : QuranBase
    {
        private LSQuranData() { }

        public LSQuranData(QuranInfo info)
            : base(info)
        {
        }
    }

    public abstract class QuranBase : IQuran
    {
        DatabaseFactory dbFactory = new DatabaseFactory();
        PetaPoco.Database DBContext_ = null;

        public QuranInfo quranInfo;
        public IDictionary<byte, IChapter> chapterDict;

        protected QuranBase() {}

        public QuranBase(QuranInfo info)
        {
            quranInfo = info;
            chapterDict = new Dictionary<byte, IChapter>();
            UpdateChaptersFromDB();
        }

        private PetaPoco.Database DBContext
        {
            get
            {
                if (DBContext_ == null)
                {
                    DBContext_ = dbFactory.Get();
                }

                return DBContext_;
            }
        }

        private void UpdateChaptersFromDB()
        {
            try
            {
                ChapterLiteRepository cr = new ChapterLiteRepository(dbFactory);
                foreach (var chapter in cr.GetChapters())
                {
                    chapterDict.Add(chapter.Id, CreateChapterData(chapter));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }

        public bool ProcessChapters(IList<XElement> chapters)
        {
            bool result = false;

            if (chapters != null)
            {
                try
                {
                    result = true;
                    foreach (var chapter in chapters)
                    {
                        if (this.AddChapter(chapter) == false)
                        {
                            result = false;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            if (!result)
            {
                Debug.WriteLine("########### Quran data processing aborted ################");
            }
            else
            {
                Debug.WriteLine("Quran data processed successfully.");
            }

            return result;
        }

        private bool AddChapter(XElement chapter)
        {
            bool result = false;

            if (chapter != null)
            {
                //get the requied info from xml element
                string name = chapter.Attribute("name").Value;
                byte id = Convert.ToByte(chapter.Attribute("id").Value);
                int totalAyahs = Convert.ToInt32(chapter.Attribute("total_ayahs").Value);
                bool revPlace = chapter.Attribute("revelation_place").Value.Equals("Makki") ? false : true;
                string text = chapter.Value;

                //create language specific chapter data object
                IChapter cd = this.chapterDict[id];

                if (cd != null)
                {
                    //update the chapter revelation place info
                    cd.RevelationPlace = revPlace;

                    //trim whitespaces and special unicode whitespace
                    //cd.Name = cd.Name.Trim().Trim(Convert.ToChar(32)).Trim();
                    cd.Name = name.Trim().Trim(Convert.ToChar(32)).Trim();

                    //process the surah text by splitting it into verses
                    if (ExtractChapterDataFromText(cd, text) == false)
                    {
                        Debug.WriteLine(string.Format("Failed to update chapter '{0}' data.", name));
                    }

                    result = true;
                }
            }
            return result;
        }

        protected virtual bool Validate(IChapter chapter, string text)
        {
            bool result = true;
            if (!chapter.IsValid)
            {
                Debug.WriteLine("Can't process empty or invalid chapter object.");
                result = false;
            }

            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                Debug.WriteLine(string.Format("Can't process empty text for Surah '{0} - {1}'.", chapter.ID, chapter.Name));
                result = false;
            }

            return result;
        }

        protected virtual string AdjustEncoding(string text)
        {
            //string conversion to UTF8 encoding
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream(byteArray);

            // convert stream to string
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        protected virtual void UpdateSajdaAndRuku(IChapter chapterData, int ayahId, ref string ayahText) { }

        protected virtual bool ExtractChapterDataFromText(IChapter chapterData, string text)
        {
            bool result = false;

            if (Validate(chapterData, text))
            {
                Debug.WriteLine(string.Format("Processing Surah '{0} - {1}' ayahs '{2}' which is '{3}'.", chapterData.ID, chapterData.Name, chapterData.TotalAyahs, (chapterData.RevelationPlace) ? "Madani" : "Makki"));

                //string conversion to UTF8 encoding
                string surahText = AdjustEncoding(text);

                if (!string.IsNullOrEmpty(surahText))
                {
                    string[] ayahs = Regex.Split(surahText, @"\p{Z}*\p{P}+\p{C}*\p{N}+\p{C}*\p{P}+\p{Z}*");

                    //don't use the last item which is end of paragraph
                    int ayahCount = (ayahs[ayahs.Length - 1].Length == 0 || ayahs[ayahs.Length - 1].Length == 1) ? ayahs.Length - 1 : ayahs.Length;

                    if (chapterData.TotalAyahs == ayahCount)
                    {
                        for (int i = 0; i < ayahCount; i++)
                        {
                            //trim whitespaces and special unicode whitespace
                            string ayah = ayahs[i].Trim().Trim(Convert.ToChar(32)).Trim();

                            //check and update sajda & Raku
                            UpdateSajdaAndRuku(chapterData, i + 1, ref ayah);

                            //Add the extracted ayah text to chapter
                            if (chapterData.AddVerse((short)(i + 1), //verse Id
                                    ayah, // ayah text
                                    string.Format("Auto added at '{0}'.", DateTime.Now.ToString()) //comments as description
                                    ) == false)
                            {
                                Debug.WriteLine(string.Format("Failed to insert verse '{0}' text '{1}'.", (i + 1), ayah));
                                break;
                            }
                        }

                        if (!chapterData.HasValidVerses)
                        {
                            Debug.WriteLine(string.Format("Surah '{0}' ayahs count '{1}' is not valid.", chapterData.Name, ayahCount));
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Surah '{0}' ayahs count '{1}' is not valid.", chapterData.Name, ayahCount));
                    }
                }
            }

            return (chapterData.IsValid = result);
        }

        private IChapter CreateChapterData(ChapterLite dbChapter)
        {
            IChapter chapterData = null;
            //arabic quran data is being added
            if (quranInfo.Translator == 0 || quranInfo.Language == 1)
            {
                chapterData = new ChapterData(quranInfo, dbChapter, dbFactory);
            }
            else
            {
                chapterData = new LSChapterData(quranInfo, dbChapter, dbFactory);
            }

            return chapterData;
        }

        public bool CommitChanges()
        {
            if (chapterDict.Count > 0)
            {
                PetaPoco.Database DBContext = dbFactory.Get();
                if (DBContext != null)
                {
                    var sortedDict = (from entry in chapterDict orderby entry.Key ascending select entry)
                                        .ToDictionary(pair => pair.Key, pair => pair.Value);

                    PetaPoco.Transaction trans = DBContext.GetTransaction();

                    try
                    {
                        foreach (byte key in sortedDict.Keys)
                        {
                            if (!sortedDict[key].CommitChanges())
                            {
                                trans.Dispose();
                                return false;
                            }
                        }
                        trans.Complete();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Dispose();
                        Debug.WriteLine("###### Error: exception fired while inserting chapter verses ######");
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Debug.WriteLine("###### Error: can't add quran info invalid Database ######");
                }
            }

            return false;
        }
    }
}
