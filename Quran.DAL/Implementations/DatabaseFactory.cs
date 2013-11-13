using Quran.DAL.Contracts;
using Quran.Model;

namespace Quran.DAL.Implementations
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private PetaPoco.Database dataContext;
        
        public PetaPoco.Database Get()
        {
            return dataContext ?? (dataContext = new PetaPoco.Database("QuranContext"));
        }
        protected override void DisposeCore()
        {
            if (dataContext != null)
                dataContext.Dispose();
        }
    }
}
