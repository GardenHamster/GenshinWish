using GenshinWish.Attribute;
using GenshinWish.Cache;
using GenshinWish.Exceptions;
using GenshinWish.Helper;
using GenshinWish.Models.Api;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Service;
using GenshinWish.Service.WishService;
using GenshinWish.Type;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinWish.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FullWeaponController : BaseWishController<FullWeaponService>
    {
        protected AuthorizeService authorizeService;
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected WishRecordService wishRecordService;
        protected ReceiveRecordService receiveRecordService;
        protected MemberGoodsService memberGoodsService;

        public FullWeaponController(FullWeaponService fullWeaponService, AuthorizeService authorizeService, MemberService memberService, GoodsService goodsService,
             WishRecordService wishRecordService, ReceiveRecordService receiveRecordService, MemberGoodsService memberGoodsService) : base(fullWeaponService)
        {
            this.authorizeService = authorizeService;
            this.memberService = memberService;
            this.goodsService = goodsService;
            this.wishRecordService = wishRecordService;
            this.receiveRecordService = receiveRecordService;
            this.memberGoodsService = memberGoodsService;
        }

        /// <summary>
        /// 单抽全武器祈愿池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode">成员编号(可以传入QQ号)</param>
        /// <param name="memberName"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { ApiLimit.Yes })]
        public ApiResult Once([FromForm] AuthorizeDto authorizeDto, string memberCode, string memberName = "", bool toBase64 = false, int imgWidth = 0)
        {
            return Wish(authorizeDto, memberCode, memberName, toBase64, imgWidth, 10);
        }


        /// <summary>
        /// 十连全武器祈愿池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode">成员编号(可以传入QQ号)</param>
        /// <param name="memberName"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { ApiLimit.Yes })]
        public ApiResult Ten([FromForm] AuthorizeDto authorizeDto, string memberCode, string memberName = "", bool toBase64 = false, int imgWidth = 0)
        {
            return Wish(authorizeDto, memberCode, memberName, toBase64, imgWidth, 10);
        }


        /// <summary>
        /// 百连全武器祈愿池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode">成员编号(可以传入QQ号)</param>
        /// <param name="memberName"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { ApiLimit.Yes })]
        public ApiResult Hundred([FromForm] AuthorizeDto authorizeDto, string memberCode, string memberName = "", bool toBase64 = false, int imgWidth = 0)
        {
            return Wish(authorizeDto, memberCode, memberName, toBase64, imgWidth, 100);
        }


        /// <summary>
        /// 祈愿函数
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode"></param>
        /// <param name="memberName"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <param name="wishCount"></param>
        /// <returns></returns>
        private ApiResult Wish(AuthorizeDto authorizeDto, string memberCode, string memberName, bool toBase64, int imgWidth, int wishCount)
        {
            try
            {
                CheckNullParam(memberCode);
                CheckImgWidth(imgWidth);

                WishResultBO wishResult = null;
                UpItemBO upItem = DefaultPool.FullWeaponPool;
                AuthorizePO authorizePO = authorizeDto.Authorize;

                lock (SyncLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    List<MemberGoodsBO> memberGoods = memberGoodsService.GetMemberGoods(memberInfo.Id);
                    wishResult = baseWishService.GetWishResult(authorizePO, memberInfo, upItem, memberGoods, wishCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    DbScoped.SugarScope.CommitTran();
                }

                ApiWishResult apiResult = CreateWishResult(upItem, wishResult, authorizeDto, toBase64, imgWidth);
                return ApiResult.Success(apiResult);
            }
            catch (BaseException ex)
            {
                DbScoped.SugarScope.RollbackTran();
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"authorzation：{GetAuthCode()}，memberCode：{memberCode}，toBase64：{toBase64}，imgWidth：{imgWidth}");
                DbScoped.SugarScope.RollbackTran();
                return ApiResult.ServerError;
            }
        }



    }
}
