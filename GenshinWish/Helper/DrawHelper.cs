using GenshinWish.Common;
using GenshinWish.Models.BO;
using GenshinWish.Type;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GenshinWish.Helper
{
    public static class DrawHelper
    {
        private static readonly SKTypeface Typeface = GetTypeface();

        private static SKTypeface GetTypeface()
        {
            try
            {
                string fontPath = FilePath.getFontPath();
                if (File.Exists(fontPath) == false) return null;
                return SKTypeface.FromFile(fontPath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }

        private static void DrawBackground(SKCanvas canvas, SKImageInfo imageInfo)
        {
            string backgroundPath = FilePath.getBackgroundPath();
            using FileStream fileStream = File.OpenRead(backgroundPath);
            using SKBitmap backgroundBitmap = SKBitmap.Decode(fileStream);
            var source = new SKRect(0, 0, imageInfo.Width, imageInfo.Height);
            var dest = new SKRect(0, 0, imageInfo.Width, imageInfo.Height);
            canvas.DrawBitmap(backgroundBitmap, source, dest);
        }

        /// <summary>
        /// 根据祈愿记录生成结果图
        /// </summary>
        /// <param name="records"></param>
        /// <param name="useSkin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static SKImage CreateWishImg(WishRecordBO[] records, bool useSkin, string uid)
        {
            int canvasWidth = 1920;
            int canvasHeight = 1080;
            int indexX = 230 + 151 * 9;
            int indexY = 228;

            var imgInfo = new SKImageInfo(canvasWidth, canvasHeight);
            using SKSurface surface = SKSurface.Create(imgInfo);
            using SKCanvas canvas = surface.Canvas;

            DrawBackground(canvas, imgInfo);
            for (int i = records.Length - 1; i >= 0; i--)
            {
                WishRecordBO wishRecord = records[i];
                GoodsItemBO goodsItem = wishRecord.GoodsItem;
                drawFrame(canvas, goodsItem, indexX, indexY);//画框
                drawShading(canvas, goodsItem, indexX, indexY);//画底纹
                drawRoleOrEquip(canvas, goodsItem, indexX, indexY, useSkin);//画装备或角色
                drawLight(canvas, goodsItem, indexX, indexY);//画光框
                drawIcon(canvas, wishRecord, indexX, indexY);//画装备图标
                drawProspect(canvas, goodsItem, indexX, indexY);//画框前景色(带星)
                drawTrans(canvas, wishRecord, indexX, indexY);//画转化和转化素材
                drawStar(canvas, wishRecord, indexX, indexY);//画星星
                drawNewIcon(canvas, wishRecord, indexX, indexY);//画New图标
                drawCloseIcon(canvas);//画关闭图标
                indexX -= 148;
            }

            drawBubbles(canvas);//画泡泡
            drawWaterMark(canvas);//画水印
            drawUID(canvas, uid);//画UID
            return bitmap;
        }

        /// <summary>
        /// 根据祈愿记录生成结果图
        /// </summary>
        /// <param name="wishRecord"></param>
        /// <param name="withSkin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static Bitmap createWishImg(WishRecordBO wishRecord, bool withSkin, string uid)
        {
            string backImgUrl = FilePath.getBackgroundPath();
            Bitmap bitmap = new Bitmap(backImgUrl);
            using SKCanvas bgGraphics = SKCanvas.FromImage(bitmap);
            GoodsItemBO goodsItem = wishRecord.GoodsItem;
            if (goodsItem.GoodsType == GoodsType.角色)
            {
                drawRole(bgGraphics, goodsItem, withSkin);//画角色大图
                drawRoleIcon(bgGraphics, goodsItem);//画角色元素图标
                drawRoleName(bgGraphics, goodsItem);//画角色名
                drawRoleStar(bgGraphics, goodsItem);//画星星
                drawRoleTrans(bgGraphics, wishRecord);//画转化和转化素材
            }
            if (goodsItem.GoodsType == GoodsType.武器)
            {
                drawEquipBg(bgGraphics, goodsItem);//画装备背景
                drawEquip(bgGraphics, goodsItem);//画装备大图
                drawEquipIcon(bgGraphics, goodsItem);//画装备图标
                drawEquipName(bgGraphics, goodsItem);//画装备名
                drawEquipStar(bgGraphics, goodsItem);//画星星
                drawEquipTrans(bgGraphics, goodsItem);//画星辉
            }
            drawBubbles(bgGraphics);//画泡泡
            drawWaterMark(bgGraphics);//画水印
            drawUID(bgGraphics, uid);//画UID
            return bitmap;
        }

        /*-------------------------------------------------------单抽---------------------------------------------------------------------*/
        private static void drawRole(SKCanvas canvas, GoodsItemBO goodsItem, bool useSkin)
        {
            int indexX = -339;
            int indexY = -133;
            string imagePath = FilePath.getYSBigRoleImgPath(goodsItem, useSkin);
            using FileStream fileStream = File.OpenRead(imagePath);
            using SKBitmap originBitmap = SKBitmap.Decode(fileStream);
            using SKBitmap resizeBitmap = originBitmap.Resize(new SKImageInfo(2688, 1344), SKFilterQuality.Low);
            SKRect source = new SKRect(0, 0, originBitmap.Width, originBitmap.Height);
            SKRect dest = new SKRect(indexX, indexY, indexX + resizeBitmap.Width, indexY + resizeBitmap.Height);
            canvas.DrawBitmap(originBitmap, source, dest);
        }

        private static void drawRoleIcon(SKCanvas canvas, GoodsItemBO goodsItem)
        {
            int indexX = 73;
            int indexY = 547;
            string imagePath = FilePath.getYSBigElementIconPath(goodsItem);
            using FileStream fileStream = File.OpenRead(imagePath);
            using SKBitmap bitmap = SKBitmap.Decode(fileStream);
            SKRect source = new SKRect(0, 0, bitmap.Width, bitmap.Height);
            SKRect dest = new SKRect(indexX, indexY, indexX + bitmap.Width, indexY + bitmap.Height);
            canvas.DrawBitmap(bitmap, source, dest);
        }

        private static void drawRoleName(SKCanvas canvas, GoodsItemBO goodsItem)
        {




            using GraphicsPath path = new GraphicsPath();
            Font nameFont = new Font(FontName, 45, FontStyle.Bold);
            StringFormat format = StringFormat.GenericTypographic;
            RectangleF rect = new RectangleF(194, 576, 600, 200);
            float size = canvas.DpiY * nameFont.SizeInPoints / 72;
            path.AddString(goodsItem.GoodsName, nameFont.FontFamily, (int)nameFont.Style, size, rect, format);
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            canvas.DrawPath(new Pen(Color.Black, 2), path);
            canvas.FillPath(Brushes.White, path);
        }

        private static void drawRoleStar(SKCanvas canvas, GoodsItemBO goodsItem)
        {
            int starWidth = 34;
            int starHeight = 34;
            int indexX = 200;
            int indexY = 663;
            int starCount = goodsItem.RareType.getStarCount();
            using Image imgStar = new Bitmap(FilePath.getYSStarPath());
            for (int i = 0; i < starCount; i++)
            {
                canvas.DrawBitmap(imgStar, indexX, indexY, starWidth, starHeight);
                indexX += starWidth + 5;
            }
        }

        private static void drawRoleTrans(SKCanvas canvas, WishRecordBO wishRecord)
        {
            GoodsItemBO goodsItem = wishRecord.GoodsItem;
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount > 1 && wishRecord.OwnedCount <= 7)
            {
                using Image imgStarDust = new Bitmap(FilePath.getYSStarDustIconPath(goodsItem));
                canvas.DrawBitmap(imgStarDust, 837, 917, imgStarDust.Width, imgStarDust.Height);//画星尘
                using Image imgStarLight = new Bitmap(FilePath.getYSStarLightIconPath(goodsItem.RareType == RareType.五星 ? 10 : 2));
                canvas.DrawBitmap(imgStarLight, 950, 917, imgStarLight.Width, imgStarLight.Height);//画星辉
                using Font tranFont = new Font(FontName, 19, FontStyle.Regular);
                using SolidBrush brushWatermark = new SolidBrush(Color.White);
                canvas.DrawString("重复角色，已转化", tranFont, brushWatermark, 842, 870);
            }
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount > 7)
            {
                using Image imgStarLight = new Bitmap(FilePath.getYSStarLightIconPath(goodsItem.RareType == RareType.五星 ? 25 : 5));
                canvas.DrawBitmap(imgStarLight, 900, 917, imgStarLight.Width, imgStarLight.Height);//画星辉
                using Font tranFont = new Font(FontName, 19, FontStyle.Regular);
                using SolidBrush brushWatermark = new SolidBrush(Color.White);
                canvas.DrawString("重复角色，已转化", tranFont, brushWatermark, 842, 870);
            }
        }

        private static void drawEquipBg(SKCanvas canvas, GoodsItemBO goodsItem)
        {
            using Image imgBg = new Bitmap(FilePath.getYSEquipBgPath(goodsItem));
            canvas.DrawBitmap(imgBg, 449, -17, 1114, 1114);
        }

        private static void drawEquip(SKCanvas canvas, GoodsItemBO goodsItem)
        {
            using Image imgEquip = new Bitmap(FilePath.getYSEquipImgPath(goodsItem));
            canvas.DrawBitmap(imgEquip, 699, -75, 614, 1230);
        }

        private static void drawEquipIcon(SKCanvas canvas, GoodsItemBO goodsItem)
        {
            using Image imgIcon = new Bitmap(FilePath.getYSBlackEquipIconPath(goodsItem));
            canvas.DrawBitmap(imgIcon, 47, 512, imgIcon.Width, imgIcon.Height);
        }

        private static void drawEquipName(SKCanvas canvas, GoodsItemBO goodsItem)
        {
            using GraphicsPath path = new GraphicsPath();
            Font nameFont = new Font(FontName, 45, FontStyle.Bold);
            StringFormat format = StringFormat.GenericTypographic;
            RectangleF rect = new RectangleF(190, 555, 600, 200);
            float size = canvas.DpiY * nameFont.SizeInPoints / 72;
            path.AddString(goodsItem.GoodsName, nameFont.FontFamily, (int)nameFont.Style, size, rect, format);
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            canvas.DrawPath(new Pen(Color.Black, 2), path);
            canvas.FillPath(Brushes.White, path);
        }

        private static void drawEquipStar(SKCanvas canvas, GoodsItemBO goodsItem)
        {
            int starWidth = 34;
            int starHeight = 34;
            int indexX = 190;
            int indexY = 643;
            int starCount = goodsItem.RareType.getStarCount();
            using Image imgStar = new Bitmap(FilePath.getYSStarPath());
            for (int i = 0; i < starCount; i++)
            {
                canvas.DrawBitmap(imgStar, indexX, indexY, starWidth, starHeight);
                indexX += starWidth + 5;
            }
        }

        private static void drawEquipTrans(SKCanvas canvas, GoodsItemBO goodsItem)
        {
            using Image imgToken = new Bitmap(FilePath.getYSTokenPath(goodsItem));
            canvas.DrawBitmap(imgToken, 1394, 485, imgToken.Width, imgToken.Height);
        }

        private static void drawBubbles(SKCanvas canvas)
        {
            int randomBigCount = new Random().Next(5, 11);
            List<Image> bigImageList = new List<Image>();
            List<string> bigPathList = FilePath.getBubblesBigPathList();
            foreach (var item in bigPathList) bigImageList.Add(new Bitmap(item));
            for (int i = 0; i < randomBigCount; i++)
            {
                int randomWidth = RandomHelper.getRandomBetween(50, 200);
                int randomXIndex = RandomHelper.getRandomBetween(20, 1900);
                int randomYIndex = RandomHelper.getRandomBetween(20, 1060);
                Image randomImage = bigImageList[RandomHelper.getRandomBetween(0, bigImageList.Count - 1)];
                canvas.DrawBitmap(randomImage, randomXIndex, randomYIndex, randomWidth, randomWidth);
            }

            int randomSmallCount = new Random().Next(50, 101);
            List<Image> smallImageList = new List<Image>();
            List<string> smallPathList = FilePath.getBubblesSmallPathList();
            foreach (var item in smallPathList) smallImageList.Add(new Bitmap(item));
            for (int i = 0; i < randomSmallCount; i++)
            {
                int randomWidth = RandomHelper.getRandomBetween(5, 15);
                int randomXIndex = RandomHelper.getRandomBetween(20, 1900);
                int randomYIndex = RandomHelper.getRandomBetween(20, 1060);
                Image randomImage = smallImageList[RandomHelper.getRandomBetween(0, smallImageList.Count - 1)];
                canvas.DrawBitmap(randomImage, randomXIndex, randomYIndex, randomWidth, randomWidth);
            }

            foreach (var item in bigImageList) item.Dispose();
            foreach (var item in smallImageList) item.Dispose();
        }

        /*-------------------------------------------------------十连---------------------------------------------------------------------*/

        private static void drawFrame(SKCanvas canvas, GoodsItemBO goodsItem, int indexX, int indexY)
        {
            string imagePath = FilePath.getFrameImgPath();
            using FileStream fileStream = File.OpenRead(imagePath);
            using SKBitmap bitmap = SKBitmap.Decode(fileStream);
            SKRect source = new SKRect(0, 0, bitmap.Width, bitmap.Height);
            SKRect dest = new SKRect(indexX, indexY, indexX + bitmap.Width, indexY + bitmap.Height);
            canvas.DrawBitmap(bitmap, source, dest);
        }

        private static void drawProspect(SKCanvas canvas, GoodsItemBO goodsItem, int indexX, int indexY)
        {
            string imagePath = FilePath.getYSProspectImgPath();
            using FileStream fileStream = File.OpenRead(imagePath);
            using SKBitmap bitmap = SKBitmap.Decode(fileStream);
            SKRect source = new SKRect(0, 0, bitmap.Width, bitmap.Height);
            SKRect dest = new SKRect(indexX+9, indexY+10, indexX + bitmap.Width, indexY + bitmap.Height);
            canvas.DrawBitmap(bitmap, source, dest);
        }

        private static void drawRoleOrEquip(SKCanvas canvas, GoodsItemBO goodsItem, int indexX, int indexY, bool withSkin)
        {
            if (goodsItem.GoodsType == GoodsType.角色)
            {
                using Image imgRole = new Bitmap(FilePath.getYSSmallRoleImgPath(goodsItem, withSkin));
                using Image imgResize = new Bitmap(imgRole, imgRole.Width, imgRole.Height);
                canvas.DrawBitmap(imgResize, indexX - 1, indexY + 4, new Rectangle(-3, 0, imgResize.Width, imgResize.Height), GraphicsUnit.Pixel);
            }
            if (goodsItem.GoodsType == GoodsType.武器)
            {
                using Image imgEquip = new Bitmap(FilePath.getYSEquipImgPath(goodsItem));
                using Image imgResize = new Bitmap(imgEquip, 305, 610);
                canvas.DrawBitmap(imgResize, indexX + 5, indexY, new Rectangle(80, 0, 140, 550), GraphicsUnit.Pixel);
            }
        }

        private static void drawIcon(SKCanvas canvas, WishRecordBO wishRecord, int indexX, int indexY)
        {
            GoodsItemBO goodsItem = wishRecord.GoodsItem;
            if (goodsItem.GoodsType == GoodsType.武器)
            {
                using Image imgIcon = new Bitmap(FilePath.getYSWhiteEquipIconPath(goodsItem));
                canvas.DrawBitmap(imgIcon, indexX + 30, indexY + 430, 100, 100);//画武器图标
            }
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount <= 1)
            {
                using Image imgIcon = new Bitmap(FilePath.getYSSmallElementIconPath(goodsItem));
                canvas.DrawBitmap(imgIcon, indexX + 40, indexY + 440, 72, 72);//画元素图标
            }
        }

        private static void drawTrans(SKCanvas canvas, WishRecordBO wishRecord, int indexX, int indexY)
        {
            GoodsItemBO goodsItem = wishRecord.GoodsItem;
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount > 1 && wishRecord.OwnedCount <= 7)
            {
                using Image imgStarLight = new Bitmap(FilePath.getYSStarLightIconPath(goodsItem.RareType == RareType.五星 ? 10 : 2));
                canvas.DrawBitmap(imgStarLight, indexX + 25, indexY + 323, imgStarLight.Width, imgStarLight.Height);//画星辉
                using Image imgStarDust = new Bitmap(FilePath.getYSStarDustIconPath(goodsItem));
                canvas.DrawBitmap(imgStarDust, indexX + 25, indexY + 443, imgStarDust.Width, imgStarDust.Height);//画星尘
                using Font tranFont = new Font(FontName, 19, FontStyle.Regular);
                using SolidBrush brushWatermark = new SolidBrush(Color.White);
                canvas.DrawString("转化", tranFont, brushWatermark, indexX + 48, indexY + 632);
            }
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount > 7)
            {
                using Image imgStarLight = new Bitmap(FilePath.getYSStarLightIconPath(goodsItem.RareType == RareType.五星 ? 25 : 5));
                canvas.DrawBitmap(imgStarLight, indexX + 25, indexY + 443, imgStarLight.Width, imgStarLight.Height);//画星辉
                using Font tranFont = new Font(FontName, 19, FontStyle.Regular);
                using SolidBrush brushWatermark = new SolidBrush(Color.White);
                canvas.DrawString("转化", tranFont, brushWatermark, indexX + 48, indexY + 632);
            }
        }


        private static void drawLight(SKCanvas canvas, GoodsItemBO goodsItem, int indexX, int indexY)
        {
            int shiftXIndex = 0;
            int shiftYIndex = -5;
            if (goodsItem.RareType == RareType.五星) shiftXIndex = -103;
            if (goodsItem.RareType == RareType.四星) shiftXIndex = -98;
            if (goodsItem.RareType == RareType.三星) shiftXIndex = 2;
            using Image imgLight = new Bitmap(FilePath.getYSLightPath(goodsItem));
            canvas.DrawBitmap(imgLight, indexX + shiftXIndex, shiftYIndex, imgLight.Width, imgLight.Height);
        }

        private static void drawStar(SKCanvas canvas, WishRecordBO wishRecord, int indexX, int indexY)
        {
            if (wishRecord.OwnedCount > 1 && wishRecord.GoodsItem.GoodsType == GoodsType.角色) return;

            int starWidth = 21;
            int starHeight = 21;
            GoodsItemBO goodsItem = wishRecord.GoodsItem;
            int starCount = goodsItem.RareType.getStarCount();

            int indexXAdd = (155 - starCount * starWidth) / 2;
            using Image imgStar = new Bitmap(FilePath.getYSStarPath());
            for (int i = 0; i < starCount; i++)
            {
                canvas.DrawBitmap(imgStar, indexX + indexXAdd, indexY + 535, starWidth, starHeight);
                indexXAdd += starWidth;
            }
        }

        private static void drawShading(SKCanvas canvas, GoodsItemBO goodsItem, int indexX, int indexY)
        {
            if (goodsItem.RareType != RareType.五星) return;
            using Image imgShading = new Bitmap(FilePath.getYSShadingPath());
            canvas.DrawBitmap(imgShading, indexX + 3, indexY + 45, imgShading.Width, imgShading.Height);
        }

        private static void drawCloseIcon(SKCanvas canvas)
        {
            using Image imgClose = new Bitmap(FilePath.getYSCloseIconPath());
            canvas.DrawBitmap(imgClose, 1920 - 105, 20, imgClose.Width, imgClose.Height);
        }

        private static void drawNewIcon(SKCanvas canvas, WishRecordBO wishRecord, int indexX, int indexY)
        {
            if (wishRecord.OwnedCount > 1) return;
            using Image imgNew = new Bitmap(FilePath.getYSNewIconPath());
            canvas.DrawBitmap(imgNew, indexX + 100, indexY + 20, (float)(imgNew.Width * 0.95), (float)(imgNew.Height * 0.95));
        }

        private static void drawWaterMark(SKCanvas canvas)
        {
            using Font watermarkFont = new Font(FontName, 10, FontStyle.Regular);
            using SolidBrush brushWatermark = new SolidBrush(Color.FromArgb(150, 178, 193));
            canvas.DrawString("本图片由GardenHamster/GenshinWish模拟生成，并不代表游戏内的实际效果", watermarkFont, brushWatermark, 10, 1050);
        }

        private static void drawUID(SKCanvas canvas, string uid)
        {
            if (string.IsNullOrWhiteSpace(uid) == false) return;
            using Font uidFont = new Font(FontName, 15, FontStyle.Regular);
            using SolidBrush brushWatermark = new SolidBrush(Color.White);
            canvas.DrawString($"UID：{uid}", uidFont, brushWatermark, 1650, 1042);
        }


        private static int getStarCount(this RareType rareType)
        {
            if (rareType == RareType.五星) return 5;
            if (rareType == RareType.四星) return 4;
            if (rareType == RareType.三星) return 3;
            return 0;
        }


    }
}
