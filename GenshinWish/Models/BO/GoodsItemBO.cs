using GenshinWish.Models.PO;
using GenshinWish.Type;

namespace GenshinWish.Models.BO
{
    /// <summary>
    /// 补给项目
    /// </summary>
    public record GoodsItemBO
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


        public GoodsItemBO() { }

        public GoodsItemBO(GoodsPO goods)
        {
            this.GoodsID = goods.Id;
            this.GoodsName = goods.GoodsName;
            this.RareType = goods.RareType;
            this.GoodsType = goods.GoodsType;
            this.GoodsSubType = goods.GoodsSubType;
        }

    }
}
