using Quran.DAL.Contracts;
using Quran.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Quran.DAL.Implementations
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private PetaPoco.Database dataContext;
        
        protected Repository(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
        }

        protected virtual IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected virtual PetaPoco.Database DataContext
        {
            get { return dataContext ?? (dataContext = DatabaseFactory.Get()); }
        }

        public virtual void Add(T entity)
        {
            // Insert it
            DataContext.Insert(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = GetAll().AsQueryable().Where(where);
            //IEnumerable<T> objects = dbset.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                DataContext.Delete<T>(obj);
        }
        public virtual T GetById(long id)
        {
            return Single(id);
        }
        public virtual T GetById(string id)
        {
            return Single(id);
        }
        
        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            //return dbset.Where(where).ToList();
            return GetAll().AsQueryable().Where(where).ToList();
        }
        public virtual T Get(Expression<Func<T, bool>> where)
        {
            //return dbset.Where(where).FirstOrDefault<T>();
            return GetAll().AsQueryable().Where(where).FirstOrDefault<T>();
        }

        /*private static string[] ExpressionProperties(Expression<Func<T, object>> func)
        {
            var properties = func.Body.Type.GetProperties();
            
            return typeof(T).GetProperties()
                .Where(p => properties.Any(x => p.Name == x.Name))
                .Select(p =>
                {
                    var attr = (ColumnAttribute)p.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
                    return (attr != null ? attr.Name : p.Name);
                }).ToArray();
        }*/

        public virtual T Single(object primaryKey)
        {
            return DataContext.Single<T>(primaryKey);
        }

        public virtual IEnumerable<T> GetAll()
        {
            var pd = PetaPoco.Database.PocoData.ForType(typeof(T));

            var sql = "SELECT * FROM " + pd.TableInfo.TableName;

            return DataContext.Query<T>(sql);
        }
        public virtual IEnumerable<T> Query(string where = "", string orderBy = "", int limit = 0, string columns = "*", params object[] args)
        {
            string sql = BuildSql(where, orderBy, limit, columns);
            return Query(sql, args);
        }
        public virtual IEnumerable<T> Query(string sql, params object[] args)
        {
            return DataContext.Query<T>(sql, args);
        }
        public PetaPoco.Page<T> PagedQuery(long pageNumber, long itemsPerPage, string sql, params object[] args)
        {
            return DataContext.Page<T>(pageNumber, itemsPerPage, sql, args) as PetaPoco.Page<T>;
        }
        public virtual PetaPoco.Page<T> PagedQuery(long pageNumber, long itemsPerPage, PetaPoco.Sql sql)
        {
            return DataContext.Page<T>(pageNumber, itemsPerPage, sql) as PetaPoco.Page<T>;
        }
        public virtual int Insert(T poco)
        {
            int result = 0;
            object insertResult = DataContext.Insert(poco);
            if (insertResult != null)
            {
                result = Convert.ToInt32(insertResult);
            }
            return result;
        }
        public virtual int Insert(string tableName, string primaryKeyName, bool autoIncrement, T poco)
        {
            int result = 0;
            object insertResult = DataContext.Insert(tableName, primaryKeyName, autoIncrement, poco);
            if (insertResult != null)
            {
                result = Convert.ToInt32(insertResult);
            }
            return result;
        }
        public virtual int Insert(string tableName, string primaryKeyName, T poco)
        {
            int result = 0;
            object insertResult = DataContext.Insert(tableName, primaryKeyName, poco);
            if (insertResult != null)
            {
                result = Convert.ToInt32(insertResult);
            }
            return result;
        }
        
        public virtual int Update(T poco)
        {
            return DataContext.Update(poco);
        }
        public virtual int Update(T poco, object primaryKeyValue)
        {
            return DataContext.Update(poco, primaryKeyValue);
        }
        public virtual int Update(string tableName, string primaryKeyName, T poco)
        {
            return DataContext.Update(tableName, primaryKeyName, poco);
        }

        public virtual int Delete(T entity)
        {
            return DataContext.Delete<T>(entity);
        }

        public virtual int DeleteByKey(object primaryKey)
        {
            return DataContext.Delete<T>(primaryKey);
        }

        public void Save(T entity)
        {
            DataContext.Save(entity);
        }

        public static string BuildSql(string where = "", string orderBy = "", int limit = 0, string columns = "*")
        {
            var pd = PetaPoco.Database.PocoData.ForType(typeof(T));
            return  BuildSql(pd.TableInfo.TableName, where, orderBy, limit, columns);
        }

        public static string BuildSql(string tableName, string where = "", string orderBy = "", int limit = 0, string columns = "*")
        {
            string sql = limit > 0 ? "SELECT TOP " + limit + " {0} FROM {1} " : "SELECT {0} FROM {1} ";
            if (!string.IsNullOrEmpty(where))
                sql += where.Trim().StartsWith("where", StringComparison.CurrentCultureIgnoreCase) ? where : "WHERE " + where;
            if (!String.IsNullOrEmpty(orderBy))
                sql += orderBy.Trim().StartsWith("order by", StringComparison.CurrentCultureIgnoreCase) ? orderBy : " ORDER BY " + orderBy;
            return string.Format(sql, columns, tableName);
        }

    }
}
