using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quran.DAL.Contracts;
using Quran.Model;

namespace Quran.Repository
{
    public class LanguageRepository : BaseRepository<Language>
    {
        public LanguageRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
}
