using GenshinWish.Type;
using SqlSugar;

namespace GenshinWish.Models.PO
{
    [SugarTable("goods_pool")]
    public class GoodsPoolPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "授权码ID，0表示admin配置的默认蛋池，非0时表示该授权码的自定义蛋池")]
        public int AuthId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "蛋池类型")]
        public WishType WishType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "物品ID")]
        public int GoodsId { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnDescription = "蛋池编号，用于标识多卡池")]
        public int PoolIndex { get; set; }

    }
}
