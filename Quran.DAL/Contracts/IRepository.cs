using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Quran.DAL.Contracts
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        int Update(T entity);
        int Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(long Id);
        T GetById(string Id);
        T Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
    }
}
