using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.DTO
{
    public class ReceiveRecordDto
    {
        public int GoodsId { get; set; }

        public string GoodsName { get; set; }

        public GoodsType GoodsType { get; set; }

        public GoodsSubType GoodsSubType { get; set; }

        public RareType RareType { get; set; }

        public PoolType PoolType { get; set; }

        public int Cost { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
