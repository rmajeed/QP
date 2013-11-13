using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quran.DAL.Contracts;
using Quran.DAL.Implementations;
using Quran.Model;

namespace Quran.Repository
{
    public abstract class BaseRepository<T> : Repository<T>, IBaseRepository<T> where T : class
    {
        public BaseRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public interface IBaseRepository<T> : IRepository<T> where T : class
    {
    }
}
