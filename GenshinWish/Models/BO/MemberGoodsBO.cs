using GenshinWish.Models.PO;
using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.BO
{
    public class MemberGoodsBO
    {
        public int GoodsId { get; set; }

        public string GoodsName { get; set; }

        public GoodsType GoodsType { get; set; }

        public RareType RareType { get; set; }

        public int Count { get; set; }

    }
}
