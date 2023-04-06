using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.BO
{
    public class RegionBO<T>
    {
        public int StartRegion { get; set; }

        public int EndRegion { get; set; }

        public T Item { get; set; }

        public RegionBO(T item, int startRegion, int endRegion)
        {
            Item = item;
            StartRegion = startRegion;
            EndRegion = endRegion;
        }
    }

}
