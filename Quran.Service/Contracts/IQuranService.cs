using Quran.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quran.Service.Contracts
{
    public interface IQuranService
    {
        //default is Arabic text
        List<VerseView> GetVerses(byte chapter, int number);

        List<VerseView> GetChapter(byte number);

        List<ChapterView> GetChapter(byte number, string language = "ar");
    }
}
