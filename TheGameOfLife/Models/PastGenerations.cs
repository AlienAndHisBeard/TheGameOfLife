using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evolution = TheGameOfLife.Models.Structs.Structs.Evolution;

namespace TheGameOfLife.Models
{
    public class PastGenerations
    {
        private List<Evolution> GenerationList { get; set; }

        public PastGenerations() 
        {
            GenerationList = new List<Evolution>();
        }

        public Evolution GetLast() { return GenerationList.Last(); }
        public bool Remove(Evolution evolution) { return GenerationList.Remove(evolution); }
        public void Add(Evolution evolution) { GenerationList.Add(evolution); }

        public bool IsEmpty() { return GenerationList.Count() <= 0; }
    }
}
