using GenshinWish.Models.DTO;
using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.VO
{
    public record WishRecordVO
    {
        public string GoodsName { get; set; }

        public string GoodsType { get; set; }

        public string GoodsSubType { get; set; }

        public string RareType { get; set; }

        public string PoolType { get; set; }

        public int Cost { get; set; }

        public DateTime CreateDate { get; set; }

        public WishRecordVO(ReceiveRecordDto record)
        {
            this.GoodsName = record.GoodsName;
            this.GoodsType = Enum.GetName(typeof(GoodsType), record.GoodsType);
            this.GoodsSubType = Enum.GetName(typeof(GoodsSubType), record.GoodsSubType);
            this.RareType = Enum.GetName(typeof(RareType), record.RareType);
            this.PoolType = Enum.GetName(typeof(PoolType), record.PoolType);
            this.Cost = record.Cost;
            this.CreateDate = record.CreateDate;
        }



    }
}
