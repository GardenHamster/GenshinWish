using GenshinWish.Models.PO;

namespace GenshinWish.Models.BO
{
    public record PoolItemBO : GoodsItemBO
    {
        public int PoolIndex { get; set; }

        public PoolItemBO() { }

        public PoolItemBO(GoodsPoolPO pool, GoodsPO goods) : base(goods)
        {
            this.PoolIndex = pool.PoolIndex;
        }

    }
}
