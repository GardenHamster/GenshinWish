using GenshinWish.Exceptions;
using GenshinWish.Service.WishService;
using Microsoft.AspNetCore.Mvc;

namespace GenshinWish.Controllers
{
    public abstract class BaseWishController<T> : BaseController where T : BaseWishService, new()
    {
        protected T baseWishService;
        protected static readonly object SyncLock = new object();

        public BaseWishController(T baseWishService)
        {
            this.baseWishService = baseWishService;
        }

        [NonAction]
        protected void CheckImgWidth(int imgWidth)
        {
            if (imgWidth < 0 || imgWidth > 1920) throw new ParamException("图片宽度只能设定在0~1920之间");
        }



    }
}
