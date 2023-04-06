using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using SqlSugar.IOC;
using System.Linq;
using GenshinWish.Service;
using GenshinWish.Util;
using GenshinWish.Type;
using GenshinWish.Models.DTO;
using GenshinWish.Attribute;
using GenshinWish.Models.PO;
using GenshinWish.Exceptions;
using GenshinWish.Common;
using GenshinWish.Models.Api;
using GenshinWish.Models.BO;

namespace GenshinWish.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SettingController : BaseController
    {
        protected AuthorizeService authorizeService;
        protected GoodsService goodsService;
        protected MemberService memberService;
        protected MemberGoodsService memberGoodsService;
        protected WishRecordService wishRecordService;

        public SettingController(AuthorizeService authorizeService, GoodsService goodsService, MemberService memberService, MemberGoodsService memberGoodsService, WishRecordService wishRecordService)
        {
            this.authorizeService = authorizeService;
            this.goodsService = goodsService;
            this.memberService = memberService;
            this.memberGoodsService = memberGoodsService;
            this.wishRecordService = wishRecordService;
        }

        /// <summary>
        /// 定轨武器
        /// </summary>
        /// <param name="authDto"></param>
        /// <param name="memberCode"></param>
        /// <param name="goodsName"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult SetMemberAssign([FromForm] AuthorizeDto authDto, string memberCode, string goodsName, string memberName = "")
        {
            try
            {
                int poolIndex = 0;
                checkNullParam(memberCode, goodsName);
                AuthorizePO authorizePO = authDto.Authorize;
                GoodsPO goodsInfo = goodsService.GetGoodsByName(goodsName.Trim());
                if (goodsInfo == null) return ApiResult.GoodsNotFound;

                Dictionary<int, UpItemBO> upItemDic = goodsService.LoadArmItem(authorizePO.Id);
                UpItemBO ysUpItem = upItemDic.ContainsKey(poolIndex) ? upItemDic[poolIndex] : null;
                if (ysUpItem == null) ysUpItem = DataCache.DefaultArmItem.ContainsKey(poolIndex) ? DataCache.DefaultArmItem[poolIndex] : null;
                if (ysUpItem == null) return ApiResult.PoolNotConfigured;

                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (ysUpItem.Star5UpList.Where(o => o.GoodsID == goodsInfo.Id).Any() == false) return ApiResult.AssignNotFound;
                memberService.AssignWeapon(memberInfo, goodsInfo.Id);
                return ApiResult.Success();
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
        /// 获取成员定轨信息
        /// </summary>
        /// <param name="authDto"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetMemberAssign([FromForm] AuthorizeDto authDto, string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                AuthorizePO authorizePO = authDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null || memberInfo.AssignId == 0) return ApiResult.Success("未找到定轨信息");
                GoodsPO goodsInfo = goodsService.GetGoodsById(memberInfo.AssignId);
                if (goodsInfo == null) return ApiResult.Success("未找到定轨信息");
                return ApiResult.Success(new
                {
                    goodsInfo.GoodsName,
                    GoodsType = Enum.GetName(typeof(GoodsType), goodsInfo.GoodsType),
                    GoodsSubType = Enum.GetName(typeof(GoodsSubType), goodsInfo.GoodsSubType),
                    RareType = Enum.GetName(typeof(RareType), goodsInfo.RareType),
                    AssignValue = memberInfo.AssignValue
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
        /// 设定角色池
        /// </summary>
        /// <param name="authDto"></param>
        /// <param name="poolDto"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult SetCharacterPool([FromForm] AuthorizeDto authDto, [FromBody] CharacterPoolDto poolDto)
        {
            try
            {
                if (poolDto.PoolIndex < 0 || poolDto.PoolIndex > 10) throw new ParamException("参数错误");
                if (poolDto.UpItems == null || poolDto.UpItems.Count == 0 || poolDto.UpItems.Count > 4) throw new ParamException("参数错误");

                AuthorizePO authorizePO = authDto.Authorize;
                List<GoodsPO> goodsList = new List<GoodsPO>();
                foreach (string item in poolDto.UpItems)
                {
                    string goodsName = item.Trim();
                    GoodsPO goodsInfo = goodsService.GetGoodsByName(goodsName);
                    if (goodsInfo == null) return new ApiResult(ResultCode.GoodsNotFound, $"找不到名为{goodsName}的角色");
                    goodsList.Add(goodsInfo);
                }

                List<GoodsPO> star5Goods = goodsList.Where(m => m.GoodsType == GoodsType.角色 && m.RareType == RareType.五星).ToList();
                List<GoodsPO> star4Goods = goodsList.Where(m => m.GoodsType == GoodsType.角色 && m.RareType == RareType.四星).ToList();
                if (star5Goods.Count < 1) throw new ParamException("必须指定一个五星角色");
                if (star5Goods.Count > 1) throw new ParamException("只能指定一个五星角色");
                if (star4Goods.Count < 3) throw new ParamException("必须指定三个四星角色");
                if (star4Goods.Count > 3) throw new ParamException("只能指定三个四星角色");

                goodsService.ClearPool(authorizePO.Id, WishType.角色, poolDto.PoolIndex);
                goodsService.InsertGoodsPool(star5Goods, authorizePO.Id, WishType.角色, poolDto.PoolIndex);
                goodsService.InsertGoodsPool(star4Goods, authorizePO.Id, WishType.角色, poolDto.PoolIndex);

                return ApiResult.Success();
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
        /// 设定角色池
        /// </summary>
        /// <param name="authDto"></param>
        /// <param name="poolDto"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult SetWeaponPool([FromForm] AuthorizeDto authDto, [FromBody] WeaponPoolDTO poolDto)
        {
            try
            {
                if (poolDto.UpItems == null || poolDto.UpItems.Count == 0 || poolDto.UpItems.Count > 7) throw new ParamException("参数错误");

                AuthorizePO authorizePO = authDto.Authorize;
                List<GoodsPO> goodsList = new List<GoodsPO>();
                foreach (string item in poolDto.UpItems)
                {
                    string goodsName = item.Trim();
                    GoodsPO goodsInfo = goodsService.GetGoodsByName(goodsName);
                    if (goodsInfo == null) return new ApiResult(ResultCode.GoodsNotFound, $"找不到名为{goodsName}的武器");
                    goodsList.Add(goodsInfo);
                }

                List<GoodsPO> star5Goods = goodsList.Where(m => m.GoodsType == GoodsType.武器 && m.RareType == RareType.五星).ToList();
                List<GoodsPO> star4Goods = goodsList.Where(m => m.GoodsType == GoodsType.武器 && m.RareType == RareType.四星).ToList();
                if (star5Goods.Count < 2) throw new ParamException("必须指定两个五星武器");
                if (star5Goods.Count > 2) throw new ParamException("只能指定两个五星武器");
                if (star4Goods.Count < 5) throw new ParamException("必须指定五个四星武器");
                if (star4Goods.Count > 5) throw new ParamException("只能指定五个四星武器");

                goodsService.ClearPool(authorizePO.Id, WishType.武器, 0);
                goodsService.InsertGoodsPool(star5Goods, authorizePO.Id, WishType.武器, 0);
                goodsService.InsertGoodsPool(star4Goods, authorizePO.Id, WishType.武器, 0);

                return ApiResult.Success();
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
        /// 清除一个授权码配置的所有角色池
        /// </summary>
        /// <param name="authDto"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult ResetCharacterPool([FromForm] AuthorizeDto authDto)
        {
            try
            {
                AuthorizePO authorizePO = authDto.Authorize;
                goodsService.ClearPool(authorizePO.Id, WishType.角色);
                return ApiResult.Success();
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
        /// 清除一个授权码配置的所有武器池
        /// </summary>
        /// <param name="authDto"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult ResetWeaponPool([FromForm] AuthorizeDto authDto)
        {
            try
            {
                AuthorizePO authorizePO = authDto.Authorize;
                goodsService.ClearPool(authorizePO.Id, WishType.武器);
                return ApiResult.Success();
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
        /// 重置一个成员的祈愿记录
        /// </summary>
        /// <param name="authDto"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult ResetWishRecord([FromForm] AuthorizeDto authDto, string memberCode)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                AuthorizePO authorizePO = authDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                memberGoodsService.ResetGoods(memberInfo.Id);
                wishRecordService.ResetRecord(memberInfo.Id);
                memberService.ResetSurplus(memberInfo);
                DbScoped.SugarScope.CommitTran();
                return ApiResult.Success();
            }
            catch (BaseException ex)
            {
                DbScoped.SugarScope.RollbackTran();
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
        /// 修改一个授权码服装出现的概率
        /// </summary>
        /// <param name="authDto"></param>
        /// <param name="rate">0~100</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult SetSkinRate([FromForm] AuthorizeDto authDto, int rate)
        {
            try
            {
                if (rate < 0 || rate > 100) throw new ParamException("参数错误");
                AuthorizePO authorizePO = authDto.Authorize;
                authorizeService.UpdateSkinRate(authorizePO.Id, rate);
                return ApiResult.Success();
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



    }
}
