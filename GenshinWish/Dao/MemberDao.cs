using GenshinWish.Models.PO;

namespace GenshinWish.Dao
{
    public class MemberDao : DbContext<MemberPO>
    {
        public MemberPO getMember(int authId, string memberCode)
        {
            return Db.Queryable<MemberPO>().Where(o => o.MemberCode == memberCode && o.AuthId == authId).First();
        }

    }



}
