
using System;
namespace Quran.DAL.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}
