using Quran.DAL.Contracts;
using Quran.Model;

namespace Quran.DAL.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseFactory databaseFactory;
        private PetaPoco.Database dataContext;
        private readonly PetaPoco.Transaction _petaTransaction;

        public UnitOfWork(IDatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
            _petaTransaction = new PetaPoco.Transaction(DataContext);

        }

        protected PetaPoco.Database DataContext
        {
            get { return dataContext ?? (dataContext = databaseFactory.Get()); }
        }

        public void Dispose()
        {
            DataContext.Dispose();
        }

        public void Commit()
        {
            _petaTransaction.Complete();
        }
    }
}
