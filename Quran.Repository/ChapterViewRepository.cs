using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quran.DAL.Contracts;
using Quran.Model;

namespace Quran.Repository
{
    public class ChapterViewRepository : BaseRepository<ChapterView>, IChapterViewRepository
    {
        public ChapterViewRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<ChapterView> GetChapter(byte id, string language = "ar")
        {
            //sanity check
            language = language.ToLower();
            language = language.Replace("arabic", "ar");
            language = language.Replace("english", "en");
            //execute the store proceedure
            var chapter = this.DataContext.Fetch<ChapterView, VerseLite, ChapterView>(new ChapterVerseRelator().MapIt,
                ";EXEC uspFetchChapter @0, @1, @2", id, language, language.Equals("ar", StringComparison.InvariantCultureIgnoreCase) ? "" : "pickthal");
            //return the result
            return chapter;
        }
    }

    public interface IChapterViewRepository : IBaseRepository<ChapterView>
    {
        List<ChapterView> GetChapter(byte id, string language = "ar");
    }

    class ChapterVerseRelator
    {
        /*
        * In order to support OneToMany relationship mapping, we need to be able to 
        * delay returning an LHS object until we've processed its many RHS objects
        * 
        * To support this, PetaPoco allows a relator callback to return null - indicating
        * that the object isn't yet fully populated.  
        * 
        * In order to flush the final object, PetaPoco will call the relator function 
        * one final time with all parameters set to null.  It only does this if the callback
        * returned null at least once during the processing of the result set (this saves
        * simple lamba mapping functions from having to deal with nulls).
        * 
        */

        public ChapterView current;

        public ChapterView MapIt(ChapterView a, VerseLite p)
        {
            // Terminating call.  Since we can return null from this function
            // we need to be ready for PetaPoco to callback later with null
            // parameters
            if (a == null)
                return current;

            // Is this the same chapter as the current one we're processing
            if (current != null && current.Id == a.Id && string.Equals(current.Name, a.Name))
            {
                // Yes, just add this verse to the current chapter's collection of verses
                current.Verses.Add(p);
                // Return null to indicate we're not done with this chapter yet
                return null;
            }

            // This is a different chapter to the current one, or this is the 
            // first time through and we don't have a chapter yet
            // Save the current chapter
            var prev = current;
            // Setup the new current chapter
            current = a;
            current.Verses = new List<VerseLite>();
            current.Verses.Add(p);
            // Return the now populated previous chapter (or null if first time through)
            return prev;
        }
    }
}
