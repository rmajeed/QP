using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quran.DAL.Contracts;
using Quran.Model;

namespace Quran.Repository
{
    public class VerseRepository : BaseRepository<Verse>, IVerseRepository
    {
        public VerseRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        /*public IEnumerable<VerseView> GetVerses(short Chapter = 1, int StartVerse = 0, int EndVerse = 0, string lang = "ar", string translation = "", string script = "usmani")
        {
            return this.DataContext.Query<VerseView>(";EXEC uspFetchVerses @0, @1, @2, @3, @4, @5", Chapter, lang.ToLower(), StartVerse, EndVerse, translation.ToLower(), script.ToLower());
        }*/

        public int Insert(IList<Verse> verses)
        {
            int insertCount = 0;
            if (verses != null)
            {
                foreach (Verse verse in verses)
                {
                    if (this.Insert(verse) > 0)
                    {
                        insertCount++;
                    }
                }
            }
            return insertCount;
        }
    }

    public interface IVerseRepository : IBaseRepository<Verse>
    {
        //IEnumerable<VerseView> GetVerses(short chapter = 1, int start = 0, int end = 0, string lang = "ar", string translation = "", string script = "Usmani");
        int Insert(IList<Verse> verses);
    }
}
