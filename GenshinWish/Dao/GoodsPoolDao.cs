using GenshinWish.Models.BO;
using GenshinWish.Models.PO;
using GenshinWish.Models.VO;
using GenshinWish.Type;
using System.Collections.Generic;

namespace GenshinWish.Dao
{
    public class GoodsPoolDao : DbContext<GoodsPoolPO>
    {
        public List<GoodsPoolPO> getGoodsPool(int authId)
        {
            return Db.Queryable<GoodsPoolPO>().Where(o => o.AuthId == authId).OrderBy(o => o.PoolType).OrderBy(o => o.PoolIndex).OrderBy(o => o.Id).ToList();
        }

        public List<PoolItemBO> getPoolItems(int authId, PoolType poolType)
        {
            return Db.Queryable<GoodsPoolPO>()
            .InnerJoin<GoodsPO>((p, g) => p.GoodsId == g.Id)
            .Where((p, g) => p.AuthId == authId && p.PoolType == poolType && g.IsDisable == false)
            .Select((p, g) => new PoolItemBO
            {
                GoodsID = g.Id,
                GoodsName = g.GoodsName,
                RareType = g.RareType,
                GoodsType = g.GoodsType,
                GoodsSubType = g.GoodsSubType,
                PoolIndex = p.PoolIndex
            }).ToList();
        }

        public List<PoolItemBO> getPoolItems(int authId, PoolType poolType, int poolIndex)
        {
            return Db.Queryable<GoodsPoolPO>()
            .InnerJoin<GoodsPO>((p, g) => p.GoodsId == g.Id)
            .Where((p, g) => p.AuthId == authId && p.PoolType == poolType && p.PoolIndex == poolIndex && g.IsDisable == false)
            .Select((p, g) => new PoolItemBO
            {
                GoodsID = g.Id,
                GoodsName = g.GoodsName,
                RareType = g.RareType,
                GoodsType = g.GoodsType,
                GoodsSubType = g.GoodsSubType,
                PoolIndex = p.PoolIndex
            }).ToList();
        }

        public int clearPool(int authId, PoolType poolType, int poolIndex)
        {
            return Db.Deleteable<GoodsPoolPO>().Where(o => o.AuthId == authId && o.PoolType == poolType && o.PoolIndex == poolIndex).ExecuteCommand();
        }

        public int clearPool(int authId, PoolType poolType)
        {
            return Db.Deleteable<GoodsPoolPO>().Where(o => o.AuthId == authId && o.PoolType == poolType).ExecuteCommand();
        }

    }
}
