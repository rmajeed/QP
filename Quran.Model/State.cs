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

    [PetaPoco.TableName("State")]
    [PetaPoco.PrimaryKey("Id")]
    public partial class State
    {
        public State()
        {
            this.Cities = new HashSet<City>();
            this.Districts = new HashSet<District>();
        }
    
        public byte Id { get; set; }
        public byte CountryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<City> Cities { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<District> Districts { get; set; }
    }
}
