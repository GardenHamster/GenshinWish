using GenshinWish.Attribute;
using GenshinWish.Common;
using GenshinWish.Exceptions;
using GenshinWish.Models.Api;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Models.VO;
using GenshinWish.Service;
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
    public class WishDetailController : BaseController
    {
        protected AuthorizeService authorizeService;
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected MemberGoodsService memberGoodsService;
        protected WishRecordService wishRecordService;
        protected ReceiveRecordService receiveRecordService;

        public WishDetailController(AuthorizeService authorizeService, MemberService memberService, GoodsService goodsService,
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
                Dictionary<int, UpItemBO> wpnUpDic = DataCache.DefaultArmItem.Merge(goodsService.LoadArmItem(authorizePO.Id));
                Dictionary<int, UpItemBO> charUpDic = DataCache.DefaultRoleItem.Merge(goodsService.LoadRoleItem(authorizePO.Id));
                Dictionary<int, UpItemBO> stdUpDic = new Dictionary<int, UpItemBO>() { { 0, DataCache.DefaultPermItem } };

                return ApiResult.Success(new
                {
                    arm = wpnUpDic.Select(m => new
                    {
                        poolIndex = m.Key,
                        poolInfo = new
                        {
                            Star5UpList = ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = ChangeToGoodsVO(m.Value.Star4UpList)
                        }
                    }),
                    role = charUpDic.Select(m => new
                    {
                        poolIndex = m.Key,
                        poolInfo = new
                        {
                            Star5UpList = ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = ChangeToGoodsVO(m.Value.Star4UpList)
                        }
                    }),
                    perm = stdUpDic.Select(m => new
                    {
                        poolIndex = m.Key,
                        poolInfo = new
                        {
                            Star5UpList = ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = ChangeToGoodsVO(m.Value.Star4UpList)
                        }
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
        public ApiResult GetMemberWishDetail([FromForm] AuthorizeDto authorizeDto, string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                AuthorizePO authorizePO = authorizeDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                WishDetailDto wishDetail = receiveRecordService.GetWishDetail(memberInfo.Id);
                GoodsItemBO assignItem = goodsService.getAssignItem(memberInfo.AssignId);
                return ApiResult.Success(new
                {
                    memberInfo.Char180Surplus,
                    Char90Surplus = memberInfo.Char180Surplus % 90,
                    Char10Surplus = memberInfo.Char20Surplus % 10,
                    Wpn10Surplus = memberInfo.Wpn20Surplus % 10,
                    memberInfo.Wpn80Surplus,
                    memberInfo.Std90Surplus,
                    memberInfo.Std10Surplus,
                    AssignItem = assignItem?.GoodsName ?? string.Empty,
                    memberInfo.AssignValue,
                    wishDetail.CharWishTimes,
                    wishDetail.WpnWishTimes,
                    wishDetail.StdWishTimes,
                    wishDetail.TotalWishTimes,
                    wishDetail.Star4Count,
                    wishDetail.Star5Count,
                    wishDetail.CharStar4Rate,
                    wishDetail.WpnStar4Rate,
                    wishDetail.StdStar4Rate,
                    wishDetail.CharStar5Rate,
                    wishDetail.WpnStar5Rate,
                    wishDetail.StdStar5Rate,
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
                int days = 7;
                AuthorizePO authorizePO = authorizeDto.Authorize;
                LuckRankingVO luckRankingVO = receiveRecordService.getLuckRanking(authorizePO.Id, days, top);
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
        public ApiResult GetMemberWishRecords([FromForm] AuthorizeDto authorizeDto, string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                AuthorizePO authorizePO = authorizeDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                var star5Records = receiveRecordService.getRecords(memberInfo.Id, RareType.五星, 20);
                var star4Records = receiveRecordService.getRecords(memberInfo.Id, RareType.四星, 20);
                return ApiResult.Success(new
                {
                    star5 = ChangeToWishRecordVO(star5Records),
                    star4 = ChangeToWishRecordVO(star4Records),
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
        public InitDataDto GetInitDatas([FromForm] AuthorizeDto authorizeDto)
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
