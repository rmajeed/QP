using Quran.DAL.Contracts;
using Quran.Model;
using Quran.Repository;
using Quran.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Quran.Service.Implementations
{
    public class QuranService : IQuranService
    {
        IUnitOfWork unitOfWork;
        IVerseViewRepository _verseRepository;
        IChapterViewRepository chapterRepo;

        public QuranService(
            IUnitOfWork unitOfWork,
            IBaseRepository<ChapterView> chapterRepository)
        {
            this.unitOfWork = unitOfWork;
            //this._verseRepository = verseRepository as IVerseViewRepository;
            chapterRepo = chapterRepository as ChapterViewRepository;
        }

        //default is Arabic text
        public List<VerseView> GetVerses(byte chapter, int number)
        {
            var verses = _verseRepository.GetVerses(chapter, number, number);
            return verses.ToList();
        }

        public List<VerseView> GetChapter(byte number)
        {
            var verses = _verseRepository.GetVerses(number);
            return verses.ToList();
        }

        public List<ChapterView> GetChapter(byte number, string language = "ar")
        {
            return chapterRepo.GetChapter(number, language);
        }
    }
}
