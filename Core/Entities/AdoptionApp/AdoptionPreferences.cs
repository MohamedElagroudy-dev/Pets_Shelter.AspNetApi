using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.AdoptionApp
{
    public class AdoptionPreferences
    {
        // Desired animal types
        public bool Dog { get; set; }
        public bool Cat { get; set; }
        public bool Bird { get; set; }
        public bool Lizard { get; set; }
        public bool Rabbit { get; set; }
        public bool Other { get; set; }

        // Conditions
        public bool HouseTrained { get; set; }
        public bool Declawed { get; set; }
        public bool Young { get; set; }
        public bool MultiplePets { get; set; }
        public bool SpecialConsiderations { get; set; }
    }

}
