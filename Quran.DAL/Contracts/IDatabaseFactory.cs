using Quran.Model;
using System;

namespace Quran.DAL.Contracts
{
    public interface IDatabaseFactory : IDisposable
    {
        PetaPoco.Database Get();
    }
}
