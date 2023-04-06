using GenshinWish.Type;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.PO
{
    [SugarTable("goods")]
    public class GoodsPO : BasePO
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "物品名称")]
        public string GoodsName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "物品类型")]
        public GoodsType GoodsType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "物品子类型")]
        public GoodsSubType GoodsSubType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "稀有类型")]
        public RareType RareType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", ColumnDescription = "是否常驻")]
        public bool IsPerm { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "tinyint", ColumnDescription = "是否被禁用，缺少相关素材时请将此值设为1")]
        public bool IsDisable { get; set; }

        public GoodsPO() { }

        public GoodsPO(string goodsName, GoodsType goodsType, GoodsSubType goodsSubType, RareType rareType, bool isPerm)
        {
            GoodsName = goodsName;
            GoodsType = goodsType;
            GoodsSubType = goodsSubType;
            RareType = rareType;
            IsPerm = isPerm;
        }


    }
}
