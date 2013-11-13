using Quran.DAL.Implementations;
using Quran.Model;
using Quran.Repository;
using QuranEntryUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranEntryUI.Implementation
{
    public class LSChapterData : ChapterBase
    {
        private LSChapter lsChapter;
        private IList<LSVerse> verses;
        private IDictionary<string, Verse> verseDict = new Dictionary<string, Verse>();

        private LSChapterData() { }

        public LSChapterData(QuranInfo info, ChapterLite chapter, DatabaseFactory dbFactory)
            : base (info, chapter, dbFactory)
        {
            verses = new List<LSVerse>();
            lsChapter = new LSChapter();
            lsChapter.Id = chapter.Id;
            lsChapter.TransId = info.Translator;
            LoadDBVerses();
        }

        private void LoadDBVerses()
        {
            IVerseRepository verseRepo = new VerseRepository(dbFactory);
            foreach (var verse in verseRepo.GetAll())
            {
                verseDict.Add(GetVerseCompositeId(verse.Ayah, verse.Chapter), verse);
            }
        }

        private string GetVerseCompositeId(int id, byte chapter)
        {
            return string.Format("{0}:{1}", id, chapter);
        }

        public override string Name
        {
            get { return lsChapter.Name; }
            set { lsChapter.Name = value; }
        }

        public override bool HasValidVerses
        {
            get { return (chapter.TotalAyahs == verses.Count); }
        }

        public override bool AddVerse(int id, string text, string description)
        {
            if (id > 0 &&
                quranInfo.Translator > 0 &&
                !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
            {
                LSVerse verse = new LSVerse();
                verse.VerseId = verseDict[GetVerseCompositeId(id, this.ID)].Id;
                verse.Text = text;
                verse.Description = description;
                verse.TransId = quranInfo.Translator;
                verses.Add(verse);
                return true;
            }
            return false;
        }

        protected override bool CommitChangesInternal()
        {
            bool result = false;

            //update chapter master data first
            if (result = UpdateChapter())
            {
                //now update detail i.e. verses
                result = UpdateVerses();
            }
            return result;
        }

        private bool UpdateVerses()
        {
            bool result = false;
            Debug.WriteLine("Inserting language specific chapter verses data.");
            ILSVerseRepository verseRepo = new LSVerseRepository(dbFactory);
            if ((result = (verseRepo.Insert(verses) == verses.Count)) == false)
            {
                Debug.WriteLine("###### Error: language specific  chapter verses data insertion failed ######");
            }
            return result;
        }

        private bool UpdateChapter()
        {
            bool result = true;
            Debug.WriteLine("Inserting langauage specific chapter master data.");
            LSChapterRepository chapterRepo = new LSChapterRepository(dbFactory);
            if (chapterRepo.Insert(lsChapter) <= 0)
            {
                Debug.WriteLine("###### Error: Language specific chapter master data insertion failed ######");
                result = false;
            }
            return result;
        }
    }
}
