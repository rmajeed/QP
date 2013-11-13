using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuranEntryUI.Interfaces
{
    public interface IQuran
    {
        bool ProcessChapters(IList<XElement> chapters);
        bool CommitChanges();
    }
}
