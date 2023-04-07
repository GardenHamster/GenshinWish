using GenshinWish.Models.BO;
using GenshinWish.Models.PO;
using GenshinWish.Type;
using System.Collections.Generic;
using System.Text;

namespace GenshinWish.Dao
{
    public class GoodsDao : DbContext<GoodsPO>
    {
        public List<GoodsPO> getGoodsList()
        {
            return Db.Queryable<GoodsPO>().OrderBy(o => o.Id).ToList();
        }

        public GoodsPO getByGoodsName(string goodsName)
        {
            return Db.Queryable<GoodsPO>().Where(o => o.GoodsName == goodsName && o.IsDisable == false).First();
        }

        public List<GoodsItemBO> getByGoodsType(GoodsType goodsType)
        {
            return Db.Queryable<GoodsPO>()
            .Where(g => g.GoodsType == goodsType && g.IsDisable == false)
            .Select(g => new GoodsItemBO(g)).ToList();
        }

        public List<GoodsPO> getStandardGoods(GoodsType goodsType, RareType rareType)
        {
            return Db.Queryable<GoodsPO>().Where(o => o.GoodsType == goodsType && o.RareType == rareType && o.IsPerm == true && o.IsDisable == false).ToList();
        }

    }
}
