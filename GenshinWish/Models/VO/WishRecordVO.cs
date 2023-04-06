using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.VO
{
    public class WishRecordVO
    {
        public string GoodsName { get; set; }

        public string GoodsType { get; set; }

        public string GoodsSubType { get; set; }

        public string RareType { get; set; }

        public string WishType { get; set; }

        public int Cost { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
