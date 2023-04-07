using GenshinWish.Dao;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using System.Collections.Generic;
using System.Linq;

namespace GenshinWish.Service
{
    public class MemberGoodsService : BaseService
    {
        private MemberGoodsDao memberGoodsDao;

        public MemberGoodsService(MemberGoodsDao memberGoodsDao)
        {
            this.memberGoodsDao = memberGoodsDao;
        }

        /// <summary>
        /// 获取群员已获得物品及数量
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<MemberGoodsBO> GetMemberGoods(int memberId)
        {
            return memberGoodsDao.GetGoods(memberId);
        }

        public void AddMemberGoods(WishResultBO wishResult, List<MemberGoodsBO> memberGoods, int memberId)
        {
            var countList = wishResult.WishRecords.GroupBy(o => o.GoodsItem.GoodsID).Select(o => new { GroodsId = o.Key, Count = o.Count() });
            foreach (var item in countList)
            {
                if (memberGoods.Where(o => o.GoodsId == item.GroodsId).Any())
                {
                    memberGoodsDao.AddCount(memberId, item.GroodsId, item.Count);
                }
                else
                {
                    memberGoodsDao.Insert(new MemberGoodsPO(memberId, item.GroodsId, item.Count));
                }
            }
        }

        public int ResetGoods(int memberId)
        {
            return memberGoodsDao.ResetGoods(memberId);
        }

    }
}
