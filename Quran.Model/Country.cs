//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Quran.Model
{
    using System;
    using System.Collections.Generic;

    [PetaPoco.TableName("Country")]
    [PetaPoco.PrimaryKey("Id")]
    public partial class Country
    {
        public Country()
        {
            this.Cities = new HashSet<City>();
            this.Districts = new HashSet<District>();
            this.States = new HashSet<State>();
        }
    
        public byte Id { get; set; }
        public string Name { get; set; }
        public string ISOCode2 { get; set; }
        public string ISOCode3 { get; set; }
        public string DialCode { get; set; }
        public string Description { get; set; }

        [PetaPoco.Ignore]
        public virtual ICollection<City> Cities { get; set; }
        [PetaPoco.Ignore]
        public virtual ICollection<District> Districts { get; set; }
        [PetaPoco.Ignore]
        public virtual ICollection<State> States { get; set; }
    }
}
