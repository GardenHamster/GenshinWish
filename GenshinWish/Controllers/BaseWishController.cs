using GenshinWish.Exceptions;
using GenshinWish.Service;
using GenshinWish.Service.WishService;

namespace GenshinWish.Controllers
{
    public abstract class BaseWishController<T> : BaseController where T : BaseWishService, new()
    {
        protected T baseWishService;
        protected AuthorizeService authorizeService;
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected WishRecordService wishRecordService;
        protected MemberGoodsService memberGoodsService;
        protected static readonly object SyncLock = new object();

        public BaseWishController(T t, AuthorizeService authorizeService, MemberService memberService,
            GoodsService goodsService, WishRecordService wishRecordService, MemberGoodsService memberGoodsService)
        {
            baseWishService = t;
            this.authorizeService = authorizeService;
            this.memberService = memberService;
            this.goodsService = goodsService;
            this.wishRecordService = wishRecordService;
            this.memberGoodsService = memberGoodsService;
        }

        protected void CheckImgWidth(int imgWidth)
        {
            if (imgWidth < 0 || imgWidth > 1920) throw new ParamException("图片宽度只能设定在0~1920之间");
        }



    }
}
