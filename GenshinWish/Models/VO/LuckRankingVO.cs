using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.VO
{
    public record LuckRankingVO
    {
        public List<RareRankingVO> Star5Ranking { get; set; }

        public List<RareRankingVO> Star4Ranking { get; set; }

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public long CacheTime { get; set; }

        public int Top { get; set; }

        public int CountDay { get; set; }
    }
}
