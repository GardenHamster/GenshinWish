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
        public decimal CharStar4Rate { get; set; }
        public decimal WpnStar4Rate { get; set; }
        public decimal StdStar4Rate { get; set; }
        public decimal CharStar5Rate { get; set; }
        public decimal WpnStar5Rate { get; set; }
        public decimal StdStar5Rate { get; set; }
        public decimal CharWishTimes { get; set; }
        public decimal WpnWishTimes { get; set; }
        public decimal StdWishTimes { get; set; }
        public int TotalWishTimes { get; set; }
    }
}
