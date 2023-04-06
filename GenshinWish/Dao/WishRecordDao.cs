using GenshinWish.Models.PO;
using GenshinWish.Type;
using System;

namespace GenshinWish.Dao
{
    public class WishRecordDao : DbContext<WishRecordPO>
    {
        public int getWishTimes(int memberId, DateTime startTime, DateTime endTime)
        {
            return Db.Queryable<WishRecordPO>().Where(o => o.CreateDate >= startTime && o.CreateDate <= endTime && o.MemberId == memberId).Count();
        }

        public int getWishTimes(int memberId)
        {
            return Db.Queryable<WishRecordPO>().Where(o => o.MemberId == memberId).Sum(o => o.WishCount);
        }

        public int getWishTimes(int memberId, WishType wishType)
        {
            return Db.Queryable<WishRecordPO>().Where(o => o.MemberId == memberId && o.WishType == wishType).Sum(o => o.WishCount);
        }

        public int resetRecord(int memberId)
        {
            return Db.Deleteable<WishRecordPO>().Where(o => o.MemberId == memberId).ExecuteCommand();
        }


    }
}
