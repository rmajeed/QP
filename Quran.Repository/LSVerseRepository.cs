using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quran.DAL.Contracts;
using Quran.Model;

namespace Quran.Repository
{
    public class LSVerseRepository : BaseRepository<LSVerse>, ILSVerseRepository
    {
        public LSVerseRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public int Insert(IList<LSVerse> verses)
        {
            int insertCount = 0;
            if (verses != null)
            {
                foreach (LSVerse verse in verses)
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

    public interface ILSVerseRepository : IBaseRepository<LSVerse>
    {
        int Insert(IList<LSVerse> verses);
    }
}
