using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.DTO
{
    public class WishDetailDto
    {
        public int Star3Count { get; set; }
        public int Star4Count { get; set; }
        public int Star5Count { get; set; }
        public int CharStar4Rate { get; set; }
        public int WpnStar4Rate { get; set; }
        public int StdStar4Rate { get; set; }
        public int CharStar5Rate { get; set; }
        public int WpnStar5Rate { get; set; }
        public int StdStar5Rate { get; set; }
        public int CharWishTimes { get; set; }
        public int WpnWishTimes { get; set; }
        public int StdWishTimes { get; set; }
        public int TotalWishTimes { get; set; }
    }
}
