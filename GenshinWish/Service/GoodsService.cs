using GenshinWish.Common;
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
            DataCache.ArmStar3PermList = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.武器, RareType.三星));//三星常驻武器
            DataCache.ArmStar4PermList = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.武器, RareType.四星));//四星常驻武器
            DataCache.ArmStar5PermList = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.武器, RareType.五星));//五星常驻武器
            DataCache.RoleStar4PermList = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.角色, RareType.四星));//四星常驻角色
            DataCache.RoleStar5PermList = ChangeToGoodsItem(goodsDao.getStandardGoods(GoodsType.角色, RareType.五星));//五星常驻角色
            DataCache.Star5PermList = ConcatList(DataCache.RoleStar5PermList, DataCache.ArmStar5PermList);
            DataCache.Star4PermList = ConcatList(DataCache.RoleStar4PermList, DataCache.ArmStar4PermList);

            //加载默认常驻池
            DataCache.DefaultPermItem = LoadPermItem();

            //加载默认角色池
            DataCache.DefaultRoleItem = LoadRoleItem(0);

            //加载默认武器池
            DataCache.DefaultArmItem = LoadArmItem(0);

            //加载全角色池
            DataCache.FullRoleItem = LoadFullRoleItem();

            //加载全武器池
            DataCache.FullArmItem = LoadFullArmItem();
        }

        public UpItemBO LoadPermItem()
        {
            UpItemBO upItem = new UpItemBO();
            upItem.Star5UpList = DataCache.Star5PermList;
            upItem.Star4UpList = DataCache.Star4PermList;
            upItem.Star5NonUpList = new List<GoodsItemBO>();
            upItem.Star4NonUpList = new List<GoodsItemBO>();
            upItem.Star5AllList = DataCache.Star5PermList;
            upItem.Star4AllList = DataCache.Star4PermList;
            upItem.Star3AllList = DataCache.ArmStar3PermList;
            return upItem;
        }

        public Dictionary<int, UpItemBO> LoadRoleItem(int authId)
        {
            Dictionary<int, UpItemBO> upItemDic = new Dictionary<int, UpItemBO>();
            List<GoodsItemBO> roleItemList = goodsDao.getByWishType(authId, WishType.角色);
            List<int> poolIndexList = roleItemList.Select(m => m.PoolIndex).Distinct().ToList();
            foreach (int poolIndex in poolIndexList)
            {
                List<GoodsItemBO> roleStar5UpList = roleItemList.Where(m => m.RareType == RareType.五星 && m.PoolIndex == poolIndex).ToList();
                List<GoodsItemBO> roleStar4UpList = roleItemList.Where(m => m.RareType == RareType.四星 && m.PoolIndex == poolIndex).ToList();
                List<GoodsItemBO> roleStar5NonUpList = GetNonUpList(DataCache.RoleStar5PermList, roleStar5UpList);
                List<GoodsItemBO> roleStar4NonUpList = GetNonUpList(DataCache.Star4PermList, roleStar4UpList);
                List<GoodsItemBO> roleStar5AllList = ConcatList(DataCache.RoleStar5PermList, roleStar5UpList);
                List<GoodsItemBO> roleStar4AllList = ConcatList(DataCache.RoleStar4PermList, DataCache.ArmStar4PermList, roleStar4UpList);
                UpItemBO roleUpItem = new UpItemBO();
                roleUpItem.Star5UpList = roleStar5UpList;
                roleUpItem.Star4UpList = roleStar4UpList;
                roleUpItem.Star5NonUpList = roleStar5NonUpList;
                roleUpItem.Star4NonUpList = roleStar4NonUpList;
                roleUpItem.Star5AllList = roleStar5AllList;
                roleUpItem.Star4AllList = roleStar4AllList;
                roleUpItem.Star3AllList = DataCache.ArmStar3PermList;
                upItemDic[poolIndex] = roleUpItem;
            }
            return upItemDic;
        }

        public Dictionary<int, UpItemBO> LoadArmItem(int authId)
        {
            Dictionary<int, UpItemBO> upItemDic = new Dictionary<int, UpItemBO>();
            List<GoodsItemBO> armItemList = goodsDao.getByWishType(authId, WishType.武器);
            List<int> poolIndexList = armItemList.Select(m => m.PoolIndex).Distinct().ToList();
            foreach (int poolIndex in poolIndexList)
            {
                List<GoodsItemBO> armStar5UpList = armItemList.Where(m => m.RareType == RareType.五星 && m.PoolIndex == poolIndex).ToList();
                List<GoodsItemBO> armStar4UpList = armItemList.Where(m => m.RareType == RareType.四星 && m.PoolIndex == poolIndex).ToList();
                List<GoodsItemBO> armStar5NonUpList = GetNonUpList(DataCache.ArmStar5PermList, armStar5UpList);
                List<GoodsItemBO> armStar4NonUpList = GetNonUpList(DataCache.ArmStar4PermList, armStar4UpList);
                List<GoodsItemBO> armStar5AllList = ConcatList(DataCache.ArmStar5PermList, armStar5UpList);
                List<GoodsItemBO> armStar4AllList = ConcatList(DataCache.ArmStar4PermList, armStar4UpList);
                UpItemBO armUpItem = new UpItemBO();
                armUpItem.Star5UpList = armStar5UpList;
                armUpItem.Star4UpList = armStar4UpList;
                armUpItem.Star5NonUpList = armStar5NonUpList;
                armUpItem.Star4NonUpList = armStar4NonUpList;
                armUpItem.Star5AllList = armStar5AllList;
                armUpItem.Star4AllList = armStar4AllList;
                armUpItem.Star3AllList = DataCache.ArmStar3PermList;
                upItemDic[poolIndex] = armUpItem;
            }
            return upItemDic;
        }

        public UpItemBO LoadFullRoleItem()
        {
            List<GoodsItemBO> roleItemList = goodsDao.getByGoodsType(GoodsType.角色);
            List<GoodsItemBO> roleStar5UpList = roleItemList.Where(m => m.RareType == RareType.五星).ToList();
            List<GoodsItemBO> roleStar4UpList = roleItemList.Where(m => m.RareType == RareType.四星).ToList();
            List<GoodsItemBO> roleStar5NonUpList = new List<GoodsItemBO>();
            List<GoodsItemBO> roleStar4NonUpList = new List<GoodsItemBO>();
            List<GoodsItemBO> roleStar5AllList = roleStar5UpList;
            List<GoodsItemBO> roleStar4AllList = ConcatList(DataCache.ArmStar4PermList, roleStar4UpList);
            UpItemBO roleUpItem = new UpItemBO();
            roleUpItem.Star5UpList = roleStar5UpList;
            roleUpItem.Star4UpList = roleStar4UpList;
            roleUpItem.Star5NonUpList = roleStar5NonUpList;
            roleUpItem.Star4NonUpList = roleStar4NonUpList;
            roleUpItem.Star5AllList = roleStar5AllList;
            roleUpItem.Star4AllList = roleStar4AllList;
            roleUpItem.Star3AllList = DataCache.ArmStar3PermList;
            return roleUpItem;
        }

        public UpItemBO LoadFullArmItem()
        {
            List<GoodsItemBO> armItemList = goodsDao.getByGoodsType(GoodsType.武器);
            List<GoodsItemBO> armStar5UpList = armItemList.Where(m => m.RareType == RareType.五星).ToList();
            List<GoodsItemBO> armStar4UpList = armItemList.Where(m => m.RareType == RareType.四星).ToList();
            List<GoodsItemBO> armStar5NonUpList = new List<GoodsItemBO>();
            List<GoodsItemBO> armStar4NonUpList = new List<GoodsItemBO>();
            List<GoodsItemBO> armStar5AllList = armStar5UpList;
            List<GoodsItemBO> armStar4AllList = armStar4UpList;
            UpItemBO armUpItem = new UpItemBO();
            armUpItem.Star5UpList = armStar5UpList;
            armUpItem.Star4UpList = armStar4UpList;
            armUpItem.Star5NonUpList = armStar5NonUpList;
            armUpItem.Star4NonUpList = armStar4NonUpList;
            armUpItem.Star5AllList = armStar5AllList;
            armUpItem.Star4AllList = armStar4AllList;
            armUpItem.Star3AllList = DataCache.ArmStar3PermList;
            return armUpItem;
        }

        /// <summary>
        /// 返回非up列表
        /// </summary>
        /// <returns></returns>
        private List<GoodsItemBO> GetNonUpList(List<GoodsItemBO> AllList, List<GoodsItemBO> UpList)
        {
            List<GoodsItemBO> NonUpList = new List<GoodsItemBO>();
            foreach (GoodsItemBO goodsItem in AllList)
            {
                if (UpList.Where(m => m.GoodsName == goodsItem.GoodsName).Any()) continue;
                NonUpList.Add(goodsItem);
            }
            return NonUpList;
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
            if (ySUpItem.Star5UpList.Where(o => o.GoodsID == assignId).Any() == false) return null;
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
