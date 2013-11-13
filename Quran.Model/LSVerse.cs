using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quran.Model
{
    using System;
    using System.Collections.Generic;

    [PetaPoco.TableName("LSVerse")]
    [PetaPoco.PrimaryKey("VerseId,TransId", autoIncrement = false)]
    public partial class LSVerse
    {
        public int VerseId { get; set; }
        public int TransId { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }
}
