using GenshinWish.Models.PO;
using GenshinWish.Type;
using System.Collections.Generic;

namespace GenshinWish.Dao
{
    public class GoodsPoolDao : DbContext<GoodsPoolPO>
    {
        public List<GoodsPoolPO> getGoodsPool(int authId)
        {
            return Db.Queryable<GoodsPoolPO>().Where(o => o.AuthId == authId).OrderBy(o => o.WishType).OrderBy(o => o.PoolIndex).OrderBy(o => o.Id).ToList();
        }

        public int clearPool(int authId, WishType wishType, int poolIndex)
        {
            return Db.Deleteable<GoodsPoolPO>().Where(o => o.AuthId == authId && o.WishType == wishType && o.PoolIndex == poolIndex).ExecuteCommand();
        }

        public int clearPool(int authId, WishType wishType)
        {
            return Db.Deleteable<GoodsPoolPO>().Where(o => o.AuthId == authId && o.WishType == wishType).ExecuteCommand();
        }

    }
}
