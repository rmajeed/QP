using Quran.DAL.Implementations;
using Quran.Model;
using QuranEntryUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranEntryUI.Implementation
{
    public abstract class ChapterBase : IChapter
    {
        protected ChapterLite chapter;
        private bool isValid;
        protected QuranInfo quranInfo;
        protected DatabaseFactory dbFactory;

        protected ChapterBase() { }

        public ChapterBase(QuranInfo info, ChapterLite chapter, DatabaseFactory dbFactory)
        {
            this.chapter = chapter;
            this.quranInfo = info;
            this.dbFactory = dbFactory;
            isValid = (chapter != null && chapter.Id > 0 && !string.IsNullOrEmpty(chapter.Name) && chapter.TotalAyahs > 0);
        }

        public byte ID
        {
            get
            {
                return chapter.Id;
            }
        }

        public int TotalAyahs 
        {
            get { return chapter.TotalAyahs; }
        }

        public virtual string Name { get; set; }

        public bool RevelationPlace
        {
            get
            {
                return chapter.RevPlace;
            }
            set
            {
                chapter.RevPlace = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
            }
        }

        public virtual bool HasValidVerses
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool AddVerse(int id, string text, string description)
        {
            throw new NotImplementedException();
        }

        public virtual bool AddSajda(int ayahId)
        {
            throw new NotImplementedException();
        }

        public virtual bool AddRuku(int ayahId)
        {
            throw new NotImplementedException();
        }

        public bool CommitChanges()
        {
            bool result = false;

            if (this.IsValid && this.HasValidVerses)
            {
                Debug.WriteLine(string.Format("Processing chapter '{0}' data ===> Id:{1}, Ayahs:{2}, Place:{3}).", this.Name, this.ID, this.TotalAyahs, this.RevelationPlace));
                try
                {
                    if (result = CommitChangesInternal())
                    {
                        Debug.WriteLine(string.Format("Successfully updated chapter '{0}' data.", this.Name));
                    }
                    else
                    {
                        Debug.WriteLine("###### Error: failed to commit chapter data changes ######");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("###### Error: exception fired while commiting chapter data changes ######");
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Debug.WriteLine(string.Format("Failed to process chapter '{0}' invalid data.", this.Name));
            }
            return result;
        }

        protected abstract bool CommitChangesInternal();
    }
}
