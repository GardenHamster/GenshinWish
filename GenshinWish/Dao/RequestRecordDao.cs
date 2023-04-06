using GenshinWish.Models.PO;
using System;

namespace GenshinWish.Dao
{
    public class RequestRecordDao : DbContext<RequestRecordPO>
    {
        public int getRequestTimes(int authId, DateTime startTime, DateTime endTime)
        {
            return Db.Queryable<RequestRecordPO>().Where(o => o.CreateDate >= startTime && o.CreateDate <= endTime && o.AuthId == authId).Count();
        }

    }
}
