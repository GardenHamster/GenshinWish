using GenshinWish.Models.PO;
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
