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

namespace GenshinWish.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StandardController : BaseWishController<StandardService>
    {
        protected AuthorizeService authorizeService;
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected WishRecordService wishRecordService;
        protected ReceiveRecordService receiveRecordService;
        protected MemberGoodsService memberGoodsService;

        public StandardController(StandardService standardService, AuthorizeService authorizeService, MemberService memberService, GoodsService goodsService,
             WishRecordService wishRecordService, ReceiveRecordService receiveRecordService, MemberGoodsService memberGoodsService) : base(standardService)
        {
            this.authorizeService = authorizeService;
            this.memberService = memberService;
            this.goodsService = goodsService;
            this.wishRecordService = wishRecordService;
            this.receiveRecordService = receiveRecordService;
            this.memberGoodsService = memberGoodsService;
        }

        /// <summary>
        /// 单抽常祈愿池
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
                int poolIndex = 0;
                int wishCount = 1;
                CheckNullParam(memberCode);
                CheckImgWidth(imgWidth);

                WishResultBO wishResult = null;
                UpItemBO upItem = DefaultPool.StandardPool;
                AuthorizePO authorizePO = authorizeDto.Authorize;

                lock (SyncLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    List<MemberGoodsBO> memberGoods = memberGoodsService.GetMemberGoods(memberInfo.Id);
                    wishResult = baseWishService.GetWishResult(authorizePO, memberInfo, upItem, memberGoods, wishCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    wishRecordService.AddRecord(memberInfo.Id, PoolType.常驻, poolIndex, wishCount);//添加祈愿记录
                    receiveRecordService.AddRecords(wishResult, PoolType.常驻, memberInfo.Id);//添加成员出货记录
                    memberGoodsService.AddMemberGoods(wishResult, memberGoods, memberInfo.Id);//更新背包物品数量
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

        /// <summary>
        /// 十连常驻祈愿池
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
                int poolIndex = 0;
                int wishCount = 10;
                CheckNullParam(memberCode);
                CheckImgWidth(imgWidth);

                WishResultBO wishResult = null;
                UpItemBO upItem = DefaultPool.StandardPool;
                AuthorizePO authorizePO = authorizeDto.Authorize;

                lock (SyncLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    List<MemberGoodsBO> memberGoods = memberGoodsService.GetMemberGoods(memberInfo.Id);
                    wishResult = baseWishService.GetWishResult(authorizePO, memberInfo, upItem, memberGoods, wishCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    wishRecordService.AddRecord(memberInfo.Id, PoolType.常驻, poolIndex, wishCount);//添加祈愿记录
                    receiveRecordService.AddRecords(wishResult, PoolType.常驻, memberInfo.Id);//添加成员出货记录
                    memberGoodsService.AddMemberGoods(wishResult, memberGoods, memberInfo.Id);//更新背包物品数量
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
