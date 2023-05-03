using GenshinWish.Attribute;
using GenshinWish.Cache;
using GenshinWish.Common;
using GenshinWish.Exceptions;
using GenshinWish.Helper;
using GenshinWish.Models.Api;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
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
    public class GenerateController : BaseController
    {
        protected GoodsService goodsService;
        protected GenerateService generateService;

        public GenerateController(GenerateService generateService, GoodsService goodsService)
        {
            this.goodsService = goodsService;
            this.generateService = generateService;
        }

        /// <summary>
        /// 生成自定义单抽结果图
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="generateData"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult Once([FromForm] AuthorizeDto authorizeDto, [FromBody] GenerateDataDto generateData, bool toBase64 = false, int imgWidth = 0)
        {
            try
            {
                if (generateData == null || generateData.GoodsData.Count == 0) throw new ParamException("参数错误");
                GoodsDataDto goodsData = generateData.GoodsData.First();
                string goodsName = goodsData.GoodsName.Trim();
                GoodsPO dbGoods = goodsService.GetGoodsByName(goodsName);
                if (dbGoods == null) return new ApiResult(ResultCode.GoodsNotFound, $"找不到名为{goodsName}的物品");
                GoodsItemBO goodsItem = new GoodsItemBO(dbGoods);
                WishRecordBO wishRecord = new WishRecordBO(goodsItem, goodsData.OwnedCount, 1);
                WishRecordBO[] sortRecords = new WishRecordBO[] { wishRecord };
                ApiGenerateResult generateResult = CreateGenerateResult(generateData, sortRecords, authorizeDto, toBase64, imgWidth);
                return ApiResult.Success(generateResult);
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
        /// 生成自定义十连结果图
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="generateData"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult Ten([FromForm] AuthorizeDto authorizeDto, [FromBody] GenerateDataDto generateData, bool toBase64 = false, int imgWidth = 0)
        {
            try
            {
                if (generateData == null || generateData.GoodsData.Count == 0 || generateData.GoodsData.Count > 10) throw new ParamException("参数错误");

                List<WishRecordBO> wishRecords = new List<WishRecordBO>();
                for (int i = 0; i < generateData.GoodsData.Count; i++)
                {
                    GoodsDataDto goodsData = generateData.GoodsData[i];
                    string goodsName = goodsData.GoodsName.Trim();
                    GoodsPO dbGoods = goodsService.GetGoodsByName(goodsName);
                    if (dbGoods == null) return new ApiResult(ResultCode.GoodsNotFound, $"找不到名为{goodsName}的物品");
                    GoodsItemBO goodsItem = new GoodsItemBO(dbGoods);
                    WishRecordBO wishRecord = new WishRecordBO(goodsItem, goodsData.OwnedCount, 1);
                    wishRecords.Add(wishRecord);
                }

                List<WishRecordBO> star5Records = wishRecords.Where(o => o.GoodsItem.RareType == RareType.五星).ToList();
                List<WishRecordBO> star4Records = wishRecords.Where(o => o.GoodsItem.RareType == RareType.四星).ToList();
                List<WishRecordBO> star3Records = wishRecords.Where(o => o.GoodsItem.RareType == RareType.三星).ToList();
                if (wishRecords.Count > 10) throw new ParamException("最多包含十个物品");
                if (star5Records.Count < 1 && star4Records.Count < 1) throw new ParamException("必须包含一个或多个五星或者四星物品");

                while (wishRecords.Count < 10)
                {
                    GoodsItemBO randomItem = DefaultPool.Star3FullItems.Random();
                    wishRecords.Add(new WishRecordBO(randomItem, 2, 0));
                }

                WishRecordBO[] sortRecords = generateService.SortRecords(wishRecords.ToArray()).Take(10).ToArray();
                ApiGenerateResult generateResult = CreateGenerateResult(generateData, sortRecords, authorizeDto, toBase64, imgWidth);
                return ApiResult.Success(generateResult);
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
