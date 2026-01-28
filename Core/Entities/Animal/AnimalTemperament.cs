using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Animal
{
    public class AnimalTemperament
    {
        [Range(1, 5)]
        public int AnimalsFriendlyLevel { get; set; }
        [Range(1, 5)]       
        public int ChildrenFriendlyLevel { get; set; }
        [Range(1, 5)]
        public int HouseTrainedLevel { get; set; }
    }
}
