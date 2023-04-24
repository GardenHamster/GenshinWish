using GenshinWish.Cache;
using GenshinWish.Common;
using GenshinWish.Dao;
using GenshinWish.Helper;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Models.VO;
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
        /// 从InitData.json中同步物品信息
        /// </summary>
        /// <param name="initData"></param>
        public void SyncGoods(InitDataDto initData)
        {
            if (initData is null) return;
            var dbGoodsList = GetGoodsList();
            var syGoodsList = initData.Goods;
            foreach (var item in syGoodsList)
            {
                if (dbGoodsList.Where(o => o.Id == item.Id).Any())
                {
                    goodsDao.Update(item);
                }
                else
                {
                    goodsDao.Insert(item);
                }
            }
        }

        /// <summary>
        /// 加载蛋池数据到内存
        /// </summary>
        public void LoadGoodsPool()
        {
            DefaultPool.Star3FullItems = goodsDao.getStandardGoods(GoodsType.武器, RareType.三星).ToGoodsItem();//三星常驻武器
            DefaultPool.Star4WeaponItems = goodsDao.getStandardGoods(GoodsType.武器, RareType.四星).ToGoodsItem();//四星常驻武器
            DefaultPool.Star5WeaponItems = goodsDao.getStandardGoods(GoodsType.武器, RareType.五星).ToGoodsItem();//五星常驻武器
            DefaultPool.Star4CharacterItems = goodsDao.getStandardGoods(GoodsType.角色, RareType.四星).ToGoodsItem();//四星常驻角色
            DefaultPool.Star5CharacterItems = goodsDao.getStandardGoods(GoodsType.角色, RareType.五星).ToGoodsItem();//五星常驻角色
            DefaultPool.Star5FullItems = ConcatList(DefaultPool.Star5CharacterItems, DefaultPool.Star5WeaponItems);
            DefaultPool.Star4FullItems = ConcatList(DefaultPool.Star4CharacterItems, DefaultPool.Star4WeaponItems);
            DefaultPool.StandardPool = LoadStandardPool();//加载常驻池
            DefaultPool.CharacterPools = LoadCharacterPool(0);//加载默认角色池
            DefaultPool.WeaponPools = LoadWeaponPool(0);//加载默认武器池
            DefaultPool.FullCharacterPool = LoadFullCharItem();//加载全角色池
            DefaultPool.FullWeaponPool = LoadFullWpnItem();//加载全武器池
        }

        public UpItemBO LoadStandardPool()
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
            List<PoolItemBO> itemList = goodsPoolDao.getPoolItems(authId, PoolType.角色);
            Dictionary<int, UpItemBO> upItemDic = new Dictionary<int, UpItemBO>();
            List<int> poolIndexList = itemList.Select(m => m.PoolIndex).Distinct().ToList();
            foreach (int poolIndex in poolIndexList)
            {
                List<GoodsItemBO> star5UpItems = itemList.Where(m => m.RareType == RareType.五星 && m.PoolIndex == poolIndex).Cast<GoodsItemBO>().ToList();
                List<GoodsItemBO> star4UpItems = itemList.Where(m => m.RareType == RareType.四星 && m.PoolIndex == poolIndex).Cast<GoodsItemBO>().ToList();
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
            List<PoolItemBO> itemList = goodsPoolDao.getPoolItems(authId, PoolType.武器);
            Dictionary<int, UpItemBO> upItemDic = new Dictionary<int, UpItemBO>();
            List<int> poolIndexList = itemList.Select(m => m.PoolIndex).Distinct().ToList();
            foreach (int poolIndex in poolIndexList)
            {
                List<GoodsItemBO> star5UpItems = itemList.Where(m => m.RareType == RareType.五星 && m.PoolIndex == poolIndex).Cast<GoodsItemBO>().ToList();
                List<GoodsItemBO> star4UpItems = itemList.Where(m => m.RareType == RareType.四星 && m.PoolIndex == poolIndex).Cast<GoodsItemBO>().ToList();
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
            return fullList.Where(o => upList.Contains(o) == false).ToList();
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
        /// <param name="upItem"></param>
        /// <param name="assignId"></param>
        /// <returns></returns>
        public GoodsItemBO getAssignItem(UpItemBO upItem, int assignId)
        {
            if (assignId == 0) return null;
            GoodsPO goods = goodsDao.GetById(assignId);
            if (goods is null) return null;
            if (upItem.Star5UpItems.Where(o => o.GoodsID == assignId).Any() == false) return null;
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
        /// <param name="poolType"></param>
        /// <returns></returns>
        public int ClearPool(int authId, PoolType poolType)
        {
            return goodsPoolDao.clearPool(authId, poolType);
        }

        /// <summary>
        /// 清理蛋池
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="poolType"></param>
        /// <param name="poolIndex"></param>
        /// <returns></returns>
        public int ClearPool(int authId, PoolType poolType, int poolIndex)
        {
            return goodsPoolDao.clearPool(authId, poolType, poolIndex);
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
        /// <param name="poolType"></param>
        /// <param name="poolIndex"></param>
        /// <returns></returns>
        public void InsertGoodsPool(List<GoodsPO> goods, int authId, PoolType poolType, int poolIndex)
        {
            foreach (var good in goods)
            {
                GoodsPoolPO goodsPool = new GoodsPoolPO();
                goodsPool.AuthId = authId;
                goodsPool.PoolIndex = poolIndex;
                goodsPool.GoodsId = good.Id;
                goodsPool.PoolType = poolType;
                goodsPoolDao.Insert(goodsPool);
            }
        }


    }
}
