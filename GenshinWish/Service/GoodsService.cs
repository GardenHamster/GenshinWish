using GenshinWish.Cache;
using GenshinWish.Dao;
using GenshinWish.Models.BO;
using GenshinWish.Models.PO;
using GenshinWish.Type;
using System.Collections.Generic;
using System.Linq;

namespace GenshinWish.Service
{
    public class GoodsService : BaseService
    {
        private GoodsDao goodsDao;
        private GoodsPoolDao goodsPoolDao;

        public GoodsService(GoodsDao goodsDao, GoodsPoolDao goodsPoolDao)
        {
            this.goodsDao = goodsDao;
            this.goodsPoolDao = goodsPoolDao;
        }

        /// <summary>
        /// 根据Id获取YSGoodsItem
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GoodsItemBO GetGoodsItemById(int id)
        {
            GoodsPO goods = goodsDao.GetById(id);
            if (goods == null) return null;
            return new GoodsItemBO(goods);
        }

        /// <summary>
        /// 根据物品id获取GoodsPO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GoodsPO GetGoodsById(int id)
        {
            return goodsDao.GetById(id);
        }

        /// <summary>
        /// 根据物品名称获取GoodsPO
        /// </summary>
        /// <param name="goodsName"></param>
        /// <returns></returns>
        public GoodsPO GetGoodsByName(string goodsName)
        {
            return goodsDao.getByGoodsName(goodsName);
        }

        /// <summary>
        /// 获取所有Goods
        /// </summary>
        /// <returns></returns>
        public List<GoodsPO> GetGoodsList()
        {
            return goodsDao.getGoodsList();
        }

        /// <summary>
        /// 加载蛋池数据到内存
        /// </summary>
        public void LoadGoodsPool()
        {
            DefaultPool.Star3FullItems = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.武器, RareType.三星));//三星常驻武器
            DefaultPool.Star4WeaponItems = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.武器, RareType.四星));//四星常驻武器
            DefaultPool.Star5WeaponItems = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.武器, RareType.五星));//五星常驻武器
            DefaultPool.Star4CharacterItems = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.角色, RareType.四星));//四星常驻角色
            DefaultPool.Star5CharacterItems = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.角色, RareType.五星));//五星常驻角色
            DefaultPool.Star5FullItems = ConcatList(DefaultPool.Star5CharacterItems, DefaultPool.Star5WeaponItems);
            DefaultPool.Star4FullItems = ConcatList(DefaultPool.Star4CharacterItems, DefaultPool.Star4WeaponItems);

            //加载默认常驻池
            DefaultPool.StandardPool = LoadPermItem();

            //加载默认角色池
            DefaultPool.CharacterPools = LoadCharacterPool(0);

            //加载默认武器池
            DefaultPool.WeaponPools = LoadWeaponPool(0);

            //加载全角色池
            DefaultPool.FullCharacterPool = LoadFullCharItem();

            //加载全武器池
            DefaultPool.FullWeaponPool = LoadFullWpnItem();
        }

        public UpItemBO LoadPermItem()
        {
            UpItemBO upItem = new UpItemBO();
            upItem.Star5UpItems = DefaultPool.Star5FullItems;
            upItem.Star4UpItems = DefaultPool.Star4FullItems;
            upItem.Star5FixItems = new List<GoodsItemBO>();
            upItem.Star4FixItems = new List<GoodsItemBO>();
            upItem.Star5FullItems = DefaultPool.Star5FullItems;
            upItem.Star4FullItems = DefaultPool.Star4FullItems;
            upItem.Star3FullItems = DefaultPool.Star3FullItems;
            upItem.PoolIndex = 0;
            return upItem;
        }

        public Dictionary<int, UpItemBO> LoadCharacterPool(int authId)
        {
            List<GoodsItemBO> itemList = goodsDao.getByWishType(authId, WishType.角色);
            Dictionary<int, UpItemBO> upItemDic = new Dictionary<int, UpItemBO>();
            List<int> poolIndexList = itemList.Select(m => m.PoolIndex).Distinct().ToList();
            foreach (int poolIndex in poolIndexList)
            {
                List<GoodsItemBO> star5UpItems = itemList.Where(m => m.RareType == RareType.五星 && m.PoolIndex == poolIndex).ToList();
                List<GoodsItemBO> star4UpItems = itemList.Where(m => m.RareType == RareType.四星 && m.PoolIndex == poolIndex).ToList();
                List<GoodsItemBO> star5FixItems = GetFixItems(DefaultPool.Star5CharacterItems, star5UpItems);
                List<GoodsItemBO> star4FixItems = GetFixItems(DefaultPool.Star4FullItems, star4UpItems);
                List<GoodsItemBO> star5FullItems = ConcatList(DefaultPool.Star5CharacterItems, star5UpItems);
                List<GoodsItemBO> star4FullItems = ConcatList(DefaultPool.Star4CharacterItems, DefaultPool.Star4WeaponItems, star4UpItems);
                UpItemBO upItem = new UpItemBO();
                upItem.Star5UpItems = star5UpItems;
                upItem.Star4UpItems = star4UpItems;
                upItem.Star5FixItems = star5FixItems;
                upItem.Star4FixItems = star4FixItems;
                upItem.Star5FullItems = star5FullItems;
                upItem.Star4FullItems = star4FullItems;
                upItem.Star3FullItems = DefaultPool.Star3FullItems;
                upItem.PoolIndex = poolIndex;
                upItemDic[poolIndex] = upItem;
            }
            return upItemDic;
        }

        public Dictionary<int, UpItemBO> LoadWeaponPool(int authId)
        {
            List<GoodsItemBO> itemList = goodsDao.getByWishType(authId, WishType.武器);
            Dictionary<int, UpItemBO> upItemDic = new Dictionary<int, UpItemBO>();
            List<int> poolIndexList = itemList.Select(m => m.PoolIndex).Distinct().ToList();
            foreach (int poolIndex in poolIndexList)
            {
                List<GoodsItemBO> star5UpItems = itemList.Where(m => m.RareType == RareType.五星 && m.PoolIndex == poolIndex).ToList();
                List<GoodsItemBO> star4UpItems = itemList.Where(m => m.RareType == RareType.四星 && m.PoolIndex == poolIndex).ToList();
                List<GoodsItemBO> star5FixItems = GetFixItems(DefaultPool.Star5WeaponItems, star5UpItems);
                List<GoodsItemBO> star4FixItems = GetFixItems(DefaultPool.Star4WeaponItems, star4UpItems);
                List<GoodsItemBO> star5FullItems = ConcatList(DefaultPool.Star5WeaponItems, star5UpItems);
                List<GoodsItemBO> star4FullItems = ConcatList(DefaultPool.Star4WeaponItems, star4UpItems);
                UpItemBO upItem = new UpItemBO();
                upItem.Star5UpItems = star5UpItems;
                upItem.Star4UpItems = star4UpItems;
                upItem.Star5FixItems = star5FixItems;
                upItem.Star4FixItems = star4FixItems;
                upItem.Star5FullItems = star5FullItems;
                upItem.Star4FullItems = star4FullItems;
                upItem.Star3FullItems = DefaultPool.Star3FullItems;
                upItem.PoolIndex = poolIndex;
                upItemDic[poolIndex] = upItem;
            }
            return upItemDic;
        }

        public UpItemBO LoadFullCharItem()
        {
            List<GoodsItemBO> itemList = goodsDao.getByGoodsType(GoodsType.角色);
            List<GoodsItemBO> star5UpItems = itemList.Where(m => m.RareType == RareType.五星).ToList();
            List<GoodsItemBO> star4UpItems = itemList.Where(m => m.RareType == RareType.四星).ToList();
            List<GoodsItemBO> star5FixItems = new List<GoodsItemBO>();
            List<GoodsItemBO> star4FixItems = new List<GoodsItemBO>();
            List<GoodsItemBO> star5FullItems = star5UpItems;
            List<GoodsItemBO> star4FullItems = ConcatList(DefaultPool.Star4WeaponItems, star4UpItems);
            UpItemBO upItem = new UpItemBO();
            upItem.Star5UpItems = star5UpItems;
            upItem.Star4UpItems = star4UpItems;
            upItem.Star5FixItems = star5FixItems;
            upItem.Star4FixItems = star4FixItems;
            upItem.Star5FullItems = star5FullItems;
            upItem.Star4FullItems = star4FullItems;
            upItem.Star3FullItems = DefaultPool.Star3FullItems;
            upItem.PoolIndex = 0;
            return upItem;
        }

        public UpItemBO LoadFullWpnItem()
        {
            List<GoodsItemBO> itemList = goodsDao.getByGoodsType(GoodsType.武器);
            List<GoodsItemBO> star5UpItems = itemList.Where(m => m.RareType == RareType.五星).ToList();
            List<GoodsItemBO> star4UpItems = itemList.Where(m => m.RareType == RareType.四星).ToList();
            List<GoodsItemBO> star5FixItems = new List<GoodsItemBO>();
            List<GoodsItemBO> star4FixItems = new List<GoodsItemBO>();
            List<GoodsItemBO> star5FullItems = star5UpItems;
            List<GoodsItemBO> star4FullItems = star4UpItems;
            UpItemBO upItem = new UpItemBO();
            upItem.Star5UpItems = star5UpItems;
            upItem.Star4UpItems = star4UpItems;
            upItem.Star5FixItems = star5FixItems;
            upItem.Star4FixItems = star4FixItems;
            upItem.Star5FullItems = star5FullItems;
            upItem.Star4FullItems = star4FullItems;
            upItem.Star3FullItems = DefaultPool.Star3FullItems;
            upItem.PoolIndex = 0;
            return upItem;
        }

        /// <summary>
        /// 返回非up列表
        /// </summary>
        /// <returns></returns>
        private List<GoodsItemBO> GetFixItems(List<GoodsItemBO> fullList, List<GoodsItemBO> upList)
        {
            List<GoodsItemBO> list = new List<GoodsItemBO>();
            foreach (GoodsItemBO goodsItem in fullList)
            {
                if (upList.Where(m => m.GoodsName == goodsItem.GoodsName).Any()) continue;
                list.Add(goodsItem);
            }
            return list;
        }

        /// <summary>
        /// 将GoodsPO转化为YSGoodsItem
        /// </summary>
        /// <param name="poList"></param>
        /// <returns></returns>
        private List<GoodsItemBO> ChangeToGoodsItem(List<GoodsPO> poList)
        {
            List<GoodsItemBO> goodsItemList = new List<GoodsItemBO>();
            foreach (GoodsPO goodsPO in poList)
            {
                goodsItemList.Add(new GoodsItemBO(goodsPO));
            }
            return goodsItemList;
        }

        /// <summary>
        /// 根据id返回定轨物品，如果未定轨时，返回null
        /// </summary>
        /// <param name="assignId"></param>
        /// <returns></returns>
        public GoodsItemBO getAssignItem(int assignId)
        {
            if (assignId == 0) return null;
            GoodsPO goods = goodsDao.GetById(assignId);
            if (goods == null) return null;
            return new GoodsItemBO(goods);
        }

        /// <summary>
        /// 根据id返回定轨物品，如果当前UP池中不包含该武器，返回null
        /// </summary>
        /// <param name="ySUpItem"></param>
        /// <param name="assignId"></param>
        /// <returns></returns>
        public GoodsItemBO getAssignItem(UpItemBO ySUpItem, int assignId)
        {
            if (assignId == 0) return null;
            GoodsPO goods = goodsDao.GetById(assignId);
            if (goods is null) return null;
            if (ySUpItem.Star5UpItems.Where(o => o.GoodsID == assignId).Any() == false) return null;
            return new GoodsItemBO(goods);
        }

        /// <summary>
        /// 连接所有集合,返回无重复部分
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        private List<GoodsItemBO> ConcatList(params List<GoodsItemBO>[] lists)
        {
            List<GoodsItemBO> returnList = new List<GoodsItemBO>();
            foreach (var list in lists)
            {
                foreach (var item in list)
                {
                    if (returnList.Where(m => m.GoodsName == item.GoodsName).Any()) continue;
                    returnList.Add(item);
                }
            }
            return returnList;
        }

        /// <summary>
        /// 清理蛋池
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="wishType"></param>
        /// <returns></returns>
        public int ClearPool(int authId, WishType wishType)
        {
            return goodsPoolDao.clearPool(authId, wishType);
        }

        /// <summary>
        /// 清理蛋池
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="wishType"></param>
        /// <param name="poolIndex"></param>
        /// <returns></returns>
        public int ClearPool(int authId, WishType wishType, int poolIndex)
        {
            return goodsPoolDao.clearPool(authId, wishType, poolIndex);
        }

        /// <summary>
        /// 获取公共蛋池信息
        /// </summary>
        /// <returns></returns>
        public List<GoodsPoolPO> GetPublicPool()
        {
            return goodsPoolDao.getGoodsPool(0);
        }

        /// <summary>
        /// 设置蛋池
        /// </summary>
        /// <param name="goods"></param>
        /// <param name="authId"></param>
        /// <param name="wishType"></param>
        /// <param name="poolIndex"></param>
        /// <returns></returns>
        public void InsertGoodsPool(List<GoodsPO> goods, int authId, WishType wishType, int poolIndex)
        {
            foreach (var good in goods)
            {
                GoodsPoolPO goodsPool = new GoodsPoolPO();
                goodsPool.AuthId = authId;
                goodsPool.PoolIndex = poolIndex;
                goodsPool.GoodsId = good.Id;
                goodsPool.WishType = wishType;
                goodsPoolDao.Insert(goodsPool);
            }
        }





    }
}
