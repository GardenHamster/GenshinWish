using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.DTO
{
    public class MemberGoodsDto
    {
        public int GoodsId { get; set; }

        public string GoodsName { get; set; }

        public int Count { get; set; }

        public GoodsType GoodsType { get; set; }

        public RareType RareType { get; set; }

    }
}
