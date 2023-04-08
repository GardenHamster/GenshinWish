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

        public int ReceiveCount { get; set; }

        public int WishTimes { get; set; }

        public double ReceiveRate { get; set; }

    }
}
