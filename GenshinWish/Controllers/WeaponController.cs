using GenshinWish.Attribute;
using GenshinWish.Cache;
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
    public class WeaponController : BaseWishController<WeaponService>
    {
        protected AuthorizeService authorizeService;
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected WishRecordService wishRecordService;
        protected ReceiveRecordService receiveRecordService;
        protected MemberGoodsService memberGoodsService;

        public WeaponController(WeaponService weaponService, AuthorizeService authorizeService, MemberService memberService, GoodsService goodsService,
             WishRecordService wishRecordService, ReceiveRecordService receiveRecordService, MemberGoodsService memberGoodsService) : base(weaponService)
        {
            this.authorizeService = authorizeService;
            this.memberService = memberService;
            this.goodsService = goodsService;
            this.wishRecordService = wishRecordService;
            this.receiveRecordService = receiveRecordService;
            this.memberGoodsService = memberGoodsService;
        }

        /// <summary>
        /// 单抽武器祈愿池
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
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                WishResultBO wishResult = null;
                AuthorizePO authorizePO = authorizeDto.Authorize;
                Dictionary<int, UpItemBO> upItemDic = goodsService.LoadWeaponPool(authorizePO.Id);
                UpItemBO upItem = upItemDic.ContainsKey(poolIndex) ? upItemDic[poolIndex] : null;
                if (upItem == null) upItem = DefaultPool.WeaponPools.ContainsKey(poolIndex) ? DefaultPool.WeaponPools[poolIndex] : null;
                if (upItem == null) return ApiResult.PoolNotConfigured;

                lock (SyncLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    List<MemberGoodsDto> memberGoods = memberGoodsService.GetMemberGoods(memberInfo.Id);
                    wishResult = baseWishService.GetWishResult(authorizePO, memberInfo, upItem, memberGoods, wishCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    wishRecordService.AddRecord(memberInfo.Id, WishType.武器, poolIndex, wishCount);//添加祈愿记录
                    receiveRecordService.AddRecords(wishResult, WishType.武器, memberInfo.Id);//添加成员出货记录
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
        /// 十连武器祈愿池
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
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                WishResultBO wishResult = null;
                AuthorizePO authorizePO = authorizeDto.Authorize;
                Dictionary<int, UpItemBO> upItemDic = goodsService.LoadWeaponPool(authorizePO.Id);
                UpItemBO upItem = upItemDic.ContainsKey(poolIndex) ? upItemDic[poolIndex] : null;
                if (upItem == null) upItem = DefaultPool.WeaponPools.ContainsKey(poolIndex) ? DefaultPool.WeaponPools[poolIndex] : null;
                if (upItem == null) return ApiResult.PoolNotConfigured;

                lock (SyncLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    List<MemberGoodsDto> memberGoods = memberGoodsService.GetMemberGoods(memberInfo.Id);
                    wishResult = baseWishService.GetWishResult(authorizePO, memberInfo, upItem, memberGoods, wishCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    wishRecordService.AddRecord(memberInfo.Id, WishType.武器, poolIndex, wishCount);//添加祈愿记录
                    receiveRecordService.AddRecords(wishResult, WishType.武器, memberInfo.Id);//添加成员出货记录
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

        public ApiResult Hundred([FromForm] AuthorizeDto authorizeDto, string memberCode, string memberName = "", bool toBase64 = false, int imgWidth = 0)
        {
            try
            {
                int poolIndex = 0;
                int wishCount = 100;
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                WishResultBO wishResult = null;
                AuthorizePO authorizePO = authorizeDto.Authorize;
                Dictionary<int, UpItemBO> upItemDic = goodsService.LoadWeaponPool(authorizePO.Id);
                UpItemBO upItem = upItemDic.ContainsKey(poolIndex) ? upItemDic[poolIndex] : null;
                if (upItem == null) upItem = DefaultPool.WeaponPools.ContainsKey(poolIndex) ? DefaultPool.WeaponPools[poolIndex] : null;
                if (upItem == null) return ApiResult.PoolNotConfigured;

                lock (SyncLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    List<MemberGoodsDto> memberGoods = memberGoodsService.GetMemberGoods(memberInfo.Id);
                    wishResult = baseWishService.GetWishResult(authorizePO, memberInfo, upItem, memberGoods, wishCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    wishRecordService.AddRecord(memberInfo.Id, WishType.武器, poolIndex, wishCount);//添加祈愿记录
                    receiveRecordService.AddRecords(wishResult, WishType.武器, memberInfo.Id);//添加成员出货记录
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
