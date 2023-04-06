using GenshinWish.Common;
using GenshinWish.Exceptions;
using GenshinWish.Models.Api;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.VO;
using GenshinWish.Type;
using GenshinWish.Util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GenshinWish.Controllers
{
    public class BaseController : ControllerBase
    {
        protected string GetAuthCode()
        {
            return HttpContext.Request.Headers["authorzation"];
        }

        protected void checkNullParam(params string[] paramArr)
        {
            if (paramArr == null) return;
            foreach (var item in paramArr)
            {
                if (string.IsNullOrWhiteSpace(item)) throw new ParamException("参数错误");
            }
        }

        /// <summary>
        /// 创建结果集
        /// </summary>
        /// <param name="ySUpItem"></param>
        /// <param name="wishResult"></param>
        /// <param name="authorize"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        protected ApiWishResult CreateWishResult(UpItemBO ySUpItem, WishResultBO wishResult, AuthorizeDto authorize, bool toBase64, int imgWidth)
        {
            ApiWishResult apiResult = new ApiWishResult();
            apiResult.WishCount = wishResult.WishRecords.Count();
            apiResult.ApiDailyCallSurplus = authorize.ApiCallSurplus;
            apiResult.Role180Surplus = wishResult.MemberInfo.Char180Surplus;
            apiResult.Role90Surplus = wishResult.MemberInfo.Char180Surplus % 90;
            apiResult.Arm80Surplus = wishResult.MemberInfo.Wpn80Surplus;
            apiResult.ArmAssignValue = wishResult.MemberInfo.AssignValue;
            apiResult.Perm90Surplus = wishResult.MemberInfo.Std90Surplus;
            apiResult.FullRole90Surplus = wishResult.MemberInfo.FullChar90Surplus;
            apiResult.FullArm80Surplus = wishResult.MemberInfo.FullWpn80Surplus;
            apiResult.Star5Goods = ChangeToGoodsVO(wishResult.WishRecords.Where(m => m.GoodsItem.RareType == RareType.五星).ToArray());
            apiResult.Star4Goods = ChangeToGoodsVO(wishResult.WishRecords.Where(m => m.GoodsItem.RareType == RareType.四星).ToArray());
            apiResult.Star3Goods = ChangeToGoodsVO(wishResult.WishRecords.Where(m => m.GoodsItem.RareType == RareType.三星).ToArray());
            apiResult.Star5Up = ChangeToGoodsVO(ySUpItem.Star5UpList);
            apiResult.Star4Up = ChangeToGoodsVO(ySUpItem.Star4UpList);

            bool withSkin = authorize.Authorize.SkinRate > 0 && RandomHelper.getRandomBetween(1, 100) <= authorize.Authorize.SkinRate;
            using Bitmap wishImage = CreateWishImg(wishResult.SortWishRecords, withSkin, wishResult.MemberInfo.MemberCode);

            if (toBase64)
            {
                apiResult.ImgBase64 = ImageHelper.ToBase64(wishImage);
            }
            else
            {
                FileInfo fileInfo = ImageHelper.saveImageToJpg(wishImage, FilePath.getImgSavePath(), imgWidth);
                apiResult.ImgPath = Path.Combine(fileInfo.Directory.Parent.Name, fileInfo.Directory.Name, fileInfo.Name);
                apiResult.ImgHttpUrl = ApiConfig.ImgHttpUrl.Replace("{imgPath}", $"{fileInfo.Directory.Parent.Name}/{fileInfo.Directory.Name}/{fileInfo.Name}");
                apiResult.ImgSize = fileInfo.Length;
            }

            return apiResult;
        }

        /// <summary>
        /// 创建结果集
        /// </summary>
        /// <param name="generateData"></param>
        /// <param name="SortRecords"></param>
        /// <param name="authorizeDto"></param>
        /// <returns></returns>
        public ApiGenerateResult CreateGenerateResult(GenerateDataDto generateData, WishRecordBO[] SortRecords, AuthorizeDto authorizeDto)
        {
            ApiGenerateResult apiResult = new ApiGenerateResult();
            apiResult.WishCount = SortRecords.Count();
            apiResult.ApiDailyCallSurplus = authorizeDto.ApiCallSurplus;
            apiResult.Star5Goods = ChangeToGoodsVO(SortRecords.Where(m => m.GoodsItem.RareType == RareType.五星).ToArray());
            apiResult.Star4Goods = ChangeToGoodsVO(SortRecords.Where(m => m.GoodsItem.RareType == RareType.四星).ToArray());
            apiResult.Star3Goods = ChangeToGoodsVO(SortRecords.Where(m => m.GoodsItem.RareType == RareType.三星).ToArray());
            using Bitmap wishImage = CreateWishImg(SortRecords, generateData.UseSkin, generateData.Uid);

            if (generateData.ToBase64)
            {
                apiResult.ImgBase64 = ImageHelper.ToBase64(wishImage);
            }
            else
            {
                FileInfo fileInfo = ImageHelper.saveImageToJpg(wishImage, FilePath.getImgSavePath(), generateData.ImgWidth);
                apiResult.ImgPath = Path.Combine(fileInfo.Directory.Parent.Name, fileInfo.Directory.Name, fileInfo.Name);
                apiResult.ImgHttpUrl = ApiConfig.ImgHttpUrl.Replace("{imgPath}", $"{fileInfo.Directory.Parent.Name}/{fileInfo.Directory.Name}/{fileInfo.Name}");
                apiResult.ImgSize = fileInfo.Length;
            }

            return apiResult;
        }

        /// <summary>
        /// 绘制祈愿结果图片,返回Bitmap
        /// </summary>
        /// <param name="sortWishRecords"></param>
        /// <param name="withSkin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        protected Bitmap CreateWishImg(WishRecordBO[] sortWishRecords, bool withSkin, string uid)
        {
            if (sortWishRecords.Count() == 1) return DrawHelper.createWishImg(sortWishRecords.First(), withSkin, uid);
            return DrawHelper.createWishImg(sortWishRecords, withSkin, uid);
        }

        /// <summary>
        /// 转换为WishRecordVO
        /// </summary>
        /// <param name="recordList"></param>
        /// <returns></returns>
        protected List<WishRecordVO> ChangeToWishRecordVO(List<ReceiveRecordDto> recordList)
        {
            return recordList.Select(m => new WishRecordVO()
            {
                GoodsName = m.GoodsName,
                GoodsType = Enum.GetName(typeof(GoodsType), m.GoodsType),
                GoodsSubType = Enum.GetName(typeof(GoodsSubType), m.GoodsSubType),
                RareType = Enum.GetName(typeof(RareType), m.RareType),
                WishType = Enum.GetName(typeof(WishType), m.WishType),
                Cost = m.Cost,
                CreateDate = m.CreateDate
            }).ToList();
        }

        /// <summary>
        /// 转换为GoodsVO
        /// </summary>
        /// <param name="wishRecords"></param>
        /// <returns></returns>
        protected List<GoodsVO> ChangeToGoodsVO(WishRecordBO[] wishRecords)
        {
            return wishRecords.Select(m => new GoodsVO()
            {
                Cost = m.Cost,
                GoodsName = m.GoodsItem.GoodsName,
                GoodsType = Enum.GetName(typeof(GoodsType), m.GoodsItem.GoodsType),
                GoodsSubType = Enum.GetName(typeof(GoodsSubType), m.GoodsItem.GoodsSubType),
                RareType = Enum.GetName(typeof(RareType), m.GoodsItem.RareType),
            }).ToList();
        }

        /// <summary>
        /// 转换为GoodsVO
        /// </summary>
        /// <param name="goodsItems"></param>
        /// <returns></returns>
        protected List<GoodsVO> ChangeToGoodsVO(List<GoodsItemBO> goodsItems)
        {
            return goodsItems.Select(m => new GoodsVO()
            {
                GoodsName = m.GoodsName,
                GoodsType = Enum.GetName(typeof(GoodsType), m.GoodsType),
                GoodsSubType = Enum.GetName(typeof(GoodsSubType), m.GoodsSubType),
                RareType = Enum.GetName(typeof(RareType), m.RareType)
            }).ToList();
        }

    }
}
