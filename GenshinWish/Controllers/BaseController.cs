using GenshinWish.Common;
using GenshinWish.Exceptions;
using GenshinWish.Helper;
using GenshinWish.Models.Api;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.VO;
using GenshinWish.Type;
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
        [NonAction]
        protected string GetAuthCode()
        {
            return HttpContext.Request.Headers["authorzation"];
        }

        [NonAction]
        protected void CheckNullParam(params string[] paramArr)
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
        /// <param name="upItem"></param>
        /// <param name="wishResult"></param>
        /// <param name="authorize"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [NonAction]
        protected ApiWishResult CreateWishResult(UpItemBO upItem, WishResultBO wishResult, AuthorizeDto authorize, bool toBase64, int imgWidth)
        {
            ApiWishResult apiResult = new ApiWishResult();
            apiResult.WishCount = wishResult.WishRecords.Count();
            apiResult.ApiDailyCallSurplus = authorize.ApiCallSurplus;
            apiResult.Character180Surplus = wishResult.MemberInfo.Char180Surplus;
            apiResult.Character90Surplus = wishResult.MemberInfo.Char180Surplus % 90;
            apiResult.Weapon80Surplus = wishResult.MemberInfo.Wpn80Surplus;
            apiResult.WeaponAssignValue = wishResult.MemberInfo.AssignValue;
            apiResult.Standard90Surplus = wishResult.MemberInfo.Std90Surplus;
            apiResult.FullCharacter90Surplus = wishResult.MemberInfo.FullChar90Surplus;
            apiResult.FullWeapon80Surplus = wishResult.MemberInfo.FullWpn80Surplus;
            apiResult.Star5Goods = wishResult.WishRecords.Where(m => m.GoodsItem.RareType == RareType.五星).ToArray().ToGoodsVO();
            apiResult.Star4Goods = wishResult.WishRecords.Where(m => m.GoodsItem.RareType == RareType.四星).ToArray().ToGoodsVO();
            apiResult.Star3Goods = wishResult.WishRecords.Where(m => m.GoodsItem.RareType == RareType.三星).ToArray().ToGoodsVO();
            apiResult.Star5Up = upItem.Star5UpItems.ToGoodsVO();
            apiResult.Star4Up = upItem.Star4UpItems.ToGoodsVO();

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
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [NonAction]
        public ApiGenerateResult CreateGenerateResult(GenerateDataDto generateData, WishRecordBO[] SortRecords, AuthorizeDto authorizeDto, bool toBase64, int imgWidth)
        {
            ApiGenerateResult apiResult = new ApiGenerateResult();
            apiResult.WishCount = SortRecords.Count();
            apiResult.ApiDailyCallSurplus = authorizeDto.ApiCallSurplus;
            apiResult.Star5Goods = SortRecords.Where(m => m.GoodsItem.RareType == RareType.五星).ToArray().ToGoodsVO();
            apiResult.Star4Goods = SortRecords.Where(m => m.GoodsItem.RareType == RareType.四星).ToArray().ToGoodsVO();
            apiResult.Star3Goods = SortRecords.Where(m => m.GoodsItem.RareType == RareType.三星).ToArray().ToGoodsVO();
            using Bitmap wishImage = CreateWishImg(SortRecords, generateData.UseSkin, generateData.Uid);

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
        /// 绘制祈愿结果图片,返回Bitmap
        /// </summary>
        /// <param name="sortWishRecords"></param>
        /// <param name="withSkin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        [NonAction]
        protected Bitmap CreateWishImg(WishRecordBO[] sortWishRecords, bool withSkin, string uid)
        {
            if (sortWishRecords.Count() == 1)
            {
                return DrawHelper.createWishImg(sortWishRecords.First(), withSkin, uid);
            }
            else
            {
                return DrawHelper.createWishImg(sortWishRecords, withSkin, uid);
            }
        }

    }
}
