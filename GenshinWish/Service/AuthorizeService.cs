using GenshinWish.Dao;
using GenshinWish.Models.PO;

namespace GenshinWish.Service
{
    public class AuthorizeService : BaseService
    {
        private AuthorizeDao authorizeDao;

        public AuthorizeService(AuthorizeDao authorizeDao)
        {
            this.authorizeDao = authorizeDao;
        }

        public AuthorizePO GetAuthorize(string code)
        {
            return authorizeDao.GetAuthorize(code);
        }

        /// <summary>
        /// 修改皮肤概率
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="rare"></param>
        /// <returns></returns>
        public int UpdateSkinRate(int authId, int rare)
        {
            AuthorizePO authorize = authorizeDao.GetById(authId);
            authorize.SkinRate = rare;
            return authorizeDao.Update(authorize);
        }

    }
}
