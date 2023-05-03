using GenshinWish.Attribute;
using GenshinWish.Cache;
using GenshinWish.Exceptions;
using GenshinWish.Helper;
using GenshinWish.Models.Api;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Models.VO;
using GenshinWish.Service;
using GenshinWish.Type;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinWish.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RecordController : BaseController
    {
        protected AuthorizeService authorizeService;
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected MemberGoodsService memberGoodsService;
        protected WishRecordService wishRecordService;
        protected ReceiveRecordService receiveRecordService;

        public RecordController(AuthorizeService authorizeService, MemberService memberService, GoodsService goodsService,
            MemberGoodsService memberGoodsService, WishRecordService wishRecordService, ReceiveRecordService receiveRecordService)
        {
            this.authorizeService = authorizeService;
            this.memberService = memberService;
            this.goodsService = goodsService;
            this.memberGoodsService = memberGoodsService;
            this.wishRecordService = wishRecordService;
            this.receiveRecordService = receiveRecordService;
        }

        /// <summary>
        /// 获取当前所有祈愿池的up内容
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetPoolInfo([FromForm] AuthorizeDto authorizeDto)
        {
            try
            {
                AuthorizePO authorizePO = authorizeDto.Authorize;
                Dictionary<int, UpItemBO> wpnDic = DefaultPool.WeaponPools.Merge(goodsService.LoadWeaponPool(authorizePO.Id));
                Dictionary<int, UpItemBO> charDic = DefaultPool.CharacterPools.Merge(goodsService.LoadCharacterPool(authorizePO.Id));
                Dictionary<int, UpItemBO> stdDic = new Dictionary<int, UpItemBO>() { { 0, DefaultPool.StandardPool } };

                return ApiResult.Success(new
                {
                    weapon = wpnDic.Select(m => new
                    {
                        poolIndex = m.Key,
                        Star5UpList = m.Value.Star5UpItems.ToGoodsVO(),
                        Star4UpList = m.Value.Star4UpItems.ToGoodsVO()
                    }),
                    character = charDic.Select(m => new
                    {
                        poolIndex = m.Key,
                        Star5UpList = m.Value.Star5UpItems.ToGoodsVO(),
                        Star4UpList = m.Value.Star4UpItems.ToGoodsVO()
                    }),
                    standard = stdDic.Select(m => new
                    {
                        poolIndex = m.Key,
                        Star5UpList = m.Value.Star5UpItems.ToGoodsVO(),
                        Star4UpList = m.Value.Star4UpItems.ToGoodsVO()
                    }),
                });
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 获取成员祈愿详情
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetWishDetail([FromForm] AuthorizeDto authorizeDto, string memberCode)
        {
            try
            {
                CheckNullParam(memberCode);
                AuthorizePO authorizePO = authorizeDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                WishDetailDto wishDetail = receiveRecordService.GetWishDetail(memberInfo.Id);
                GoodsItemBO assignItem = goodsService.getAssignItem(memberInfo.AssignId);
                return ApiResult.Success(new
                {
                    memberInfo.Char180Surplus,
                    Character90Surplus = memberInfo.Char180Surplus % 90,
                    Character10Surplus = memberInfo.Char20Surplus % 10,
                    Weapon10Surplus = memberInfo.Wpn20Surplus % 10,
                    Weapon80Surplus = memberInfo.Wpn80Surplus,
                    Standard90Surplus=memberInfo.Std90Surplus,
                    Standard10Surplus = memberInfo.Std10Surplus,
                    AssignItem = assignItem?.GoodsName ?? string.Empty,
                    AssignValue = memberInfo.AssignValue,
                    CharacterWishTimes = wishDetail.CharWishTimes,
                    WeaponWishTimes = wishDetail.WpnWishTimes,
                    StandardWishTimes = wishDetail.StdWishTimes,
                    TotalWishTimes = wishDetail.TotalWishTimes,
                    Star4Count = wishDetail.Star4Count,
                    Star5Count = wishDetail.Star5Count,
                    CharacterStar4Rate = wishDetail.CharStar4Rate,
                    WeaponStar4Rate = wishDetail.WpnStar4Rate,
                    StandardStar4Rate = wishDetail.StdStar4Rate,
                    CharacterStar5Rate = wishDetail.CharStar5Rate,
                    WeaponStar5Rate = wishDetail.WpnStar5Rate,
                    StandardStar5Rate = wishDetail.StdStar5Rate,
                });
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 获取群内欧气排行
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetLuckRanking([FromForm] AuthorizeDto authorizeDto)
        {
            try
            {
                int top = 20;
                int days = 3;
                AuthorizePO authorizePO = authorizeDto.Authorize;
                LuckRankingVO luckRankingVO = receiveRecordService.GetLuckRanking(authorizePO.Id, days, top);
                return ApiResult.Success(luckRankingVO);
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 获取成员出货记录
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetWishRecord([FromForm] AuthorizeDto authorizeDto, string memberCode)
        {
            try
            {
                CheckNullParam(memberCode);
                AuthorizePO authorizePO = authorizeDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                var star5Records = receiveRecordService.GetRecords(memberInfo.Id, RareType.五星, 20);
                var star4Records = receiveRecordService.GetRecords(memberInfo.Id, RareType.四星, 20);
                return ApiResult.Success(new
                {
                    star5 = star5Records.ToWishRecordVO(),
                    star4 = star4Records.ToWishRecordVO(),
                });
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 返回数据库中初始数据集
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public InitDataDto GetInitData([FromForm] AuthorizeDto authorizeDto)
        {
            try
            {
                InitDataDto initDataDto = new InitDataDto();
                initDataDto.Goods = goodsService.GetGoodsList();
                initDataDto.GoodsPools = goodsService.GetPublicPool();
                return initDataDto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return new();
            }
        }




    }
}
