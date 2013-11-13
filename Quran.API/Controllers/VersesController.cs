using Quran.Common.Contracts;
using Quran.Common.Filters;
using Quran.Model;
using Quran.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace Quran.API.Controllers
{
    public class VersesController : ApiController
    {
        ILogService loggerService;
        IQuranService _quranservice;

        public VersesController(ILogService loggerService, IQuranService quranService)
        {
            this.loggerService = loggerService;
            this._quranservice = quranService;
        }

        /// <summary>
        /// To fetch Verse by number
        /// </summary>
        /// <returns></returns>
        //[AcceptVerbs(Get, Head)]
        [EnableCors]
        [System.Web.Http.HttpGet]
        public IQueryable<VerseView> FindVerse(byte chapter, int number)
        {
            loggerService.Logger().Info("Calling with null parameter as : chapter : " + chapter + " : number : " + number);
            return _quranservice.GetVerses(chapter, number).AsQueryable<VerseView>();
        }

        /// <summary>
        /// To fetch all verses in a chapter
        /// </summary>
        /// <returns></returns>
        [EnableCors]
        [System.Web.Http.HttpGet]
        public IQueryable<VerseView> FindChapter(byte number)
        {
            loggerService.Logger().Info("Calling with null parameter as : chapter : " + number);
            return _quranservice.GetChapter(number).AsQueryable<VerseView>();
        }

        /// <summary>
        /// To fetch the chapter data
        /// </summary>
        /// <returns></returns>
        [EnableCors]
        [System.Web.Http.HttpGet]
        public IQueryable<ChapterView> FindChapter(byte number, string language)
        {
            loggerService.Logger().Info("Calling with null parameter as : chapter : " + number);
            return _quranservice.GetChapter(number, language).AsQueryable<ChapterView>();
        }
    }
}
