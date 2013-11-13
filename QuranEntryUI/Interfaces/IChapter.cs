using Quran.DAL.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranEntryUI.Interfaces
{
    public interface IChapter
    {
        byte ID { get; }
        string Name { get; set; }
        int TotalAyahs { get; }
        bool RevelationPlace { get; set; }
        bool IsValid { get; set; }
        bool HasValidVerses { get; }
        bool AddVerse(int id, string text, string description);
        bool AddSajda(int ayahId);
        bool AddRuku(int ayahId);
        bool CommitChanges();
    }
}
