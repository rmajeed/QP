using log4net;

namespace Quran.Common.Contracts
{
    public interface ILogService
    {
        ILog Logger();
    }
}
