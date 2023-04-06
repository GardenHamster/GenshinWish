using GenshinWish.Models.PO;
using GenshinWish.Type;

namespace GenshinWish.Models.BO
{
    /// <summary>
    /// 补给项目
    /// </summary>
    public class GoodsItemBO
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int GoodsID { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 稀有类型
        /// </summary>
        public RareType RareType { get; set; }

        /// <summary>
        /// 物品类型(角色/武器)
        /// </summary>
        public GoodsType GoodsType { get; set; }

        /// <summary>
        /// 物品子类型(详细类型)
        /// </summary>
        public GoodsSubType GoodsSubType { get; set; }

        /// <summary>
        /// 蛋池编号
        /// </summary>
        public int PoolIndex { get; set; }

        public GoodsItemBO() { }

        public GoodsItemBO(GoodsPO goods)
        {
            GoodsID = goods.Id;
            GoodsName = goods.GoodsName;
            RareType = goods.RareType;
            GoodsType = goods.GoodsType;
            GoodsSubType = goods.GoodsSubType;
        }

    }
}
