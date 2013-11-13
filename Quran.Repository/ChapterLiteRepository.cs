using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quran.DAL.Contracts;
using Quran.Model;

namespace Quran.Repository
{
    public class ChapterLiteRepository : BaseRepository<ChapterLite>, IChapterLiteRepository
    {
        public ChapterLiteRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IEnumerable<ChapterLite> GetChapters()
        {
            return this.DataContext.Query<ChapterLite>("select * from dbo.Chapter");
            //return this.DataContext.Query<ChapterLite, VerseView>(
              //  "SELECT * FROM dbo.Chapter LEFT JOIN dbo.VerseView on dbo.Chapter.Id == dbo.VerseView.Chapter");
        }
    }

    public interface IChapterLiteRepository : IBaseRepository<ChapterLite>
    {
        IEnumerable<ChapterLite> GetChapters();
    }
}
