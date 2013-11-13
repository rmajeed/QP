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
    public class ChapterData : ChapterBase
    {
        public IList<Verse> verses;
        public IList<int> rukus;
        public IList<int> sajdas;

        private ChapterData() { }

        public ChapterData(QuranInfo info, ChapterLite chapter, DatabaseFactory dbFactory)
            : base(info, chapter, dbFactory)
        {
            verses = new List<Verse>();
            rukus = new List<int>();
            sajdas = new List<int>();
        }

        public override string Name
        {
            get { return chapter.Name; }
            set { chapter.Name = value; }
        }

        public override bool HasValidVerses
        {
            get { return (chapter.TotalAyahs == verses.Count); }
        }

        public override bool AddVerse(int id, string text, string description)
        {
            if (id > 0 &&
                    quranInfo.ScriptId > 0 &&
                    !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
            {
                Verse verse = new Verse();
                verse.Ayah = (short)id;
                verse.Chapter = chapter.Id;
                verse.Text = text;
                verse.ScriptId = quranInfo.ScriptId;
                verse.Description = description;
                verses.Add(verse);
                return true;
            }
            return false;
        }

        public override bool AddSajda(int ayahId)
        {
            if (sajdas != null &&
                !sajdas.Contains(ayahId))
            {
                sajdas.Add(ayahId);
                return true;
            }
            return false;
        }

        public override bool AddRuku(int ayahId)
        {
            if (rukus != null &&
                !rukus.Contains(ayahId))
            {
                rukus.Add(ayahId);
                return true;
            }
            return false;
        }

        protected override bool CommitChangesInternal()
        {
            bool result = false;

            IVerseRepository verseRepo = new VerseRepository(dbFactory);
            //update verses first
            if (result = UpdateVerses(verseRepo))
            {
                //update sajdas
                if (result = CheckAndUpdateSajdas(dbFactory.Get(), verseRepo))
                {
                    //update rukus
                    result = CheckAndUpdateRukus(dbFactory.Get(), verseRepo);
                }
            }
            return result;
        }

        private bool UpdateVerses(IVerseRepository verseRepo)
        {
            bool result = false;
            Debug.WriteLine("Inserting chapter verses data.");
            if (result = (verseRepo.Insert(verses) == verses.Count))
            {
                Debug.WriteLine("Updating chapter master data.");
                ChapterLiteRepository chapterRepo = new ChapterLiteRepository(dbFactory);
                if (chapterRepo.Update(chapter) <= 0)
                {
                    Debug.WriteLine("###### Error: chapter data update failed ######");
                    result = false;
                }
            }
            else
            {
                Debug.WriteLine("###### Error: chapter verses data insertion failed ######");
            }
            return result;
        }

        private bool CheckAndUpdateSajdas(PetaPoco.Database DBContext, IVerseRepository verseRepo)
        {
            if (DBContext != null && sajdas.Count > 0)
            {
                Debug.WriteLine(string.Format("Inserting sajda data: {0}", sajdas.Count));
                foreach (int sajda in sajdas)
                {
                    Verse vFound = verseRepo.Get(verse => verse.Chapter == this.ID && verse.Ayah == sajda);
                    if (vFound != null)
                    {
                        if (DBContext.Execute("insert into Sajda(AyahId) values (@0)", vFound.Id) <= 0)
                        {
                            Debug.WriteLine("###### Error: chapter sajda data insertion failed ######");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("###### Error: chapter sajda data insertion failed. Couldn't find the related verse data from DB. ######");
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckAndUpdateRukus(PetaPoco.Database DBContext, IVerseRepository verseRepo)
        {
            if (DBContext != null && rukus.Count > 0)
            {
                Debug.WriteLine(string.Format("Inserting ruku data: {0}", rukus.Count));
                foreach (int ruku in rukus)
                {
                    Verse vFound = verseRepo.Get(verse => verse.Chapter == this.ID && verse.Ayah == ruku);
                    if (vFound != null)
                    {
                        if (DBContext.Execute("insert into Ruku(ScriptId, AyahId) values (@0, @1)", vFound.ScriptId, vFound.Id) <= 0)
                        {
                            Debug.WriteLine("###### Error: chapter ruku data insertion failed ######");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("###### Error: chapter ruku data insertion failed. Couldn't find the related verse data from DB. ######");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
