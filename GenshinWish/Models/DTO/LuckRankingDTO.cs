using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.DTO
{
    public class LuckRankingDto
    {
        public int AuthId { get; set; }

        public string MemberCode { get; set; }

        public string MemberName { get; set; }

        public RareType RareType { get; set; }

        public int RareCount { get; set; }

        public int TotalWishTimes { get; set; }

        public double RareRate { get; set; }

    }
}
