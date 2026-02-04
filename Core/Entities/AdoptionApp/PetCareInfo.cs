using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.AdoptionApp
{
    public class PetCareInfo
    {
        public string ResponsiblePerson { get; set; } = null!;
        public string AdoptionReason { get; set; } = null!;
        public string AloneTimeDetails { get; set; } = null!;
        public string LivingEnvironment { get; set; } = null!;
    }

}
