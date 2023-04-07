using GenshinWish.Models.BO;
using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.VO
{
    public record GoodsVO
    {
        public string GoodsName { get; set; }

        public string GoodsType { get; set; }

        public string GoodsSubType { get; set; }

        public string RareType { get; set; }

        public GoodsVO(WishRecordBO record)
        {
            this.GoodsName = record.GoodsItem.GoodsName;
            this.GoodsType = Enum.GetName(typeof(GoodsType), record.GoodsItem.GoodsType);
            this.GoodsSubType = Enum.GetName(typeof(GoodsSubType), record.GoodsItem.GoodsSubType);
            this.RareType = Enum.GetName(typeof(RareType), record.GoodsItem.RareType);
        }

        public GoodsVO(GoodsItemBO item)
        {
            GoodsName = item.GoodsName;
            GoodsType = Enum.GetName(typeof(GoodsType), item.GoodsType);
            GoodsSubType = Enum.GetName(typeof(GoodsSubType), item.GoodsSubType);
            RareType = Enum.GetName(typeof(RareType), item.RareType);
        }

    }
}
