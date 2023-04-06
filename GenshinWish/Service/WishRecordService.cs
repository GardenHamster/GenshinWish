using GenshinWish.Dao;
using GenshinWish.Models.PO;
using GenshinWish.Type;
using System;

namespace GenshinWish.Service
{
    public class WishRecordService : BaseService
    {
        private WishRecordDao wishRecordDao;

        public WishRecordService(WishRecordDao wishRecordDao)
        {
            this.wishRecordDao = wishRecordDao;
        }

        public WishRecordPO AddRecord(int memberId, WishType wishType, int poolIndex, int count)
        {
            WishRecordPO wishRecord = new WishRecordPO();
            wishRecord.MemberId = memberId;
            wishRecord.WishType = wishType;
            wishRecord.WishIndex = poolIndex;
            wishRecord.WishCount = count;
            wishRecord.CreateDate = DateTime.Now;
            return wishRecordDao.Insert(wishRecord);
        }

        public int ResetRecord(int memberId)
        {
            return wishRecordDao.resetRecord(memberId);
        }

    }
}
