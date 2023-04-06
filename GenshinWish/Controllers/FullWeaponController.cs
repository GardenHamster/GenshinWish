using GenshinWish.Attribute;
using GenshinWish.Common;
using GenshinWish.Exceptions;
using GenshinWish.Models.Api;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Service;
using GenshinWish.Service.WishService;
using GenshinWish.Type;
using GenshinWish.Util;
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
        public FullWeaponController(FullWeaponService fullWeaponService, AuthorizeService authorizeService, MemberService memberService,
            GoodsService goodsService, WishRecordService wishRecordService, MemberGoodsService memberGoodsService)
            : base(fullWeaponService, authorizeService, memberService, goodsService, wishRecordService, memberGoodsService)
        {
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
            try
            {
                int wishCount = 1;
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                WishResultBO wishResult = null;
                UpItemBO ysUpItem = DataCache.FullArmItem;
                AuthorizePO authorizePO = authorizeDto.Authorize;

                lock (SyncLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    List<MemberGoodsDto> memberGoods = memberGoodsService.GetMemberGoods(memberInfo.Id);
                    wishResult = baseWishService.GetWishResult(authorizePO, memberInfo, ysUpItem, memberGoods, wishCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    DbScoped.SugarScope.CommitTran();
                }

                ApiWishResult apiResult = CreateWishResult(ysUpItem, wishResult, authorizeDto, toBase64, imgWidth);
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
            try
            {
                int wishCount = 10;
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                WishResultBO wishResult = null;
                UpItemBO ysUpItem = DataCache.FullArmItem;
                AuthorizePO authorizePO = authorizeDto.Authorize;

                lock (SyncLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    List<MemberGoodsDto> memberGoods = memberGoodsService.GetMemberGoods(memberInfo.Id);
                    wishResult = baseWishService.GetWishResult(authorizePO, memberInfo, ysUpItem, memberGoods, wishCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    DbScoped.SugarScope.CommitTran();
                }

                ApiWishResult apiResult = CreateWishResult(ysUpItem, wishResult, authorizeDto, toBase64, imgWidth);
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
