using System.Collections.Generic;

namespace CountingKs.Models
{
    public class MeasureModel
    {
        public ICollection<LinkModel> Links { get; set; }

        public string Description { get; set; }
        public double Calories { get; set; }
    }
}