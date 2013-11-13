using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quran.Model
{
    using System;
    using System.Collections.Generic;

    [PetaPoco.TableName("Verse")]
    [PetaPoco.PrimaryKey("Id")]
    public partial class Verse
    {
        public int Id { get; set; }
        public short Ayah { get; set; }
        public byte Chapter { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public byte ScriptId { get; set; }
    }
}
