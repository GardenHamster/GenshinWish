using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.VO
{
    public class RareRankingVO
    {
        public string MemberCode { get; set; }

        public string MemberName { get; set; }

        public int Count { get; set; }

        public int TotalWishTimes { get; set; }

        public double Rate { get; set; }
    }
}
