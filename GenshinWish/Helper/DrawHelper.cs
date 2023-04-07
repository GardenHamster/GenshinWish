using GenshinWish.Common;
using GenshinWish.Models.BO;
using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GenshinWish.Helper
{
    public static class DrawHelper
    {
        /// <summary>
        /// 全局字体
        /// </summary>
        private static readonly string FontName = "HYWenHei-85W";

        /// <summary>
        /// 根据祈愿记录生成结果图
        /// </summary>
        /// <param name="records"></param>
        /// <param name="useSkin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static Bitmap createWishImg(WishRecordBO[] records, bool useSkin, string uid)
        {
            int startIndexX = 230 + 151 * 9;
            int startIndexY = 228;
            int indexX = startIndexX;
            int indexY = startIndexY;
            Bitmap bitmap = new Bitmap(FilePath.getBackgroundPath());
            using Graphics bgGraphics = Graphics.FromImage(bitmap);
            for (int i = records.Length - 1; i >= 0; i--)
            {
                WishRecordBO wishRecord = records[i];
                GoodsItemBO goodsItem = wishRecord.GoodsItem;
                drawFrame(bgGraphics, goodsItem, indexX, indexY);//画框
                drawShading(bgGraphics, goodsItem, indexX, indexY);//画底纹
                drawRoleOrEquip(bgGraphics, goodsItem, indexX, indexY, useSkin);//画装备或角色
                drawLight(bgGraphics, goodsItem, indexX, indexY);//画光框
                drawIcon(bgGraphics, wishRecord, indexX, indexY);//画装备图标
                drawProspect(bgGraphics, goodsItem, indexX, indexY);//画框前景色(带星)
                drawTrans(bgGraphics, wishRecord, indexX, indexY);//画转化和转化素材
                drawStar(bgGraphics, wishRecord, indexX, indexY);//画星星
                drawNewIcon(bgGraphics, wishRecord, indexX, indexY);//画New图标
                drawCloseIcon(bgGraphics);//画关闭图标
                indexX -= 148;
            }
            drawBubbles(bgGraphics);//画泡泡
            drawWaterMark(bgGraphics);//画水印
            if (string.IsNullOrWhiteSpace(uid) == false) drawUID(bgGraphics, uid);//画UID
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
            using Graphics bgGraphics = Graphics.FromImage(bitmap);
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
            if (string.IsNullOrWhiteSpace(uid) == false) drawUID(bgGraphics, uid);//画UID
            return bitmap;
        }

        /*-------------------------------------------------------单抽---------------------------------------------------------------------*/
        private static void drawRole(Graphics bgGraphics, GoodsItemBO goodsItem, bool withSkin)
        {
            using Image imgRole = new Bitmap(FilePath.getYSBigRoleImgPath(goodsItem, withSkin));
            using Image imgResize = new Bitmap(imgRole, 2688, 1344);
            bgGraphics.DrawImage(imgResize, -339, -133, new Rectangle(0, 0, imgResize.Width, imgResize.Height), GraphicsUnit.Pixel);
        }

        private static void drawRoleIcon(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            using Image imgIcon = new Bitmap(FilePath.getYSBigElementIconPath(goodsItem));
            bgGraphics.DrawImage(imgIcon, 73, 547, imgIcon.Width, imgIcon.Height);
        }

        private static void drawRoleName(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            using GraphicsPath path = new GraphicsPath();
            Font nameFont = new Font(FontName, 45, FontStyle.Bold);
            StringFormat format = StringFormat.GenericTypographic;
            RectangleF rect = new RectangleF(194, 576, 600, 200);
            float size = bgGraphics.DpiY * nameFont.SizeInPoints / 72;
            path.AddString(goodsItem.GoodsName, nameFont.FontFamily, (int)nameFont.Style, size, rect, format);
            bgGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            bgGraphics.DrawPath(new Pen(Color.Black, 2), path);
            bgGraphics.FillPath(Brushes.White, path);
        }

        private static void drawRoleStar(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            int starWidth = 34;
            int starHeight = 34;
            int indexX = 200;
            int indexY = 663;
            int starCount = goodsItem.RareType.getStarCount();
            using Image imgStar = new Bitmap(FilePath.getYSStarPath());
            for (int i = 0; i < starCount; i++)
            {
                bgGraphics.DrawImage(imgStar, indexX, indexY, starWidth, starHeight);
                indexX += starWidth + 5;
            }
        }

        private static void drawRoleTrans(Graphics bgGraphics, WishRecordBO wishRecord)
        {
            GoodsItemBO goodsItem = wishRecord.GoodsItem;
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount > 1 && wishRecord.OwnedCount <= 7)
            {
                using Image imgStarDust = new Bitmap(FilePath.getYSStarDustIconPath(goodsItem));
                bgGraphics.DrawImage(imgStarDust, 837, 917, imgStarDust.Width, imgStarDust.Height);//画星尘
                using Image imgStarLight = new Bitmap(FilePath.getYSStarLightIconPath(goodsItem.RareType == RareType.五星 ? 10 : 2));
                bgGraphics.DrawImage(imgStarLight, 950, 917, imgStarLight.Width, imgStarLight.Height);//画星辉
                using Font tranFont = new Font(FontName, 19, FontStyle.Regular);
                using SolidBrush brushWatermark = new SolidBrush(Color.White);
                bgGraphics.DrawString("重复角色，已转化", tranFont, brushWatermark, 842, 870);
            }
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount > 7)
            {
                using Image imgStarLight = new Bitmap(FilePath.getYSStarLightIconPath(goodsItem.RareType == RareType.五星 ? 25 : 5));
                bgGraphics.DrawImage(imgStarLight, 900, 917, imgStarLight.Width, imgStarLight.Height);//画星辉
                using Font tranFont = new Font(FontName, 19, FontStyle.Regular);
                using SolidBrush brushWatermark = new SolidBrush(Color.White);
                bgGraphics.DrawString("重复角色，已转化", tranFont, brushWatermark, 842, 870);
            }
        }

        private static void drawEquipBg(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            using Image imgBg = new Bitmap(FilePath.getYSEquipBgPath(goodsItem));
            bgGraphics.DrawImage(imgBg, 449, -17, 1114, 1114);
        }

        private static void drawEquip(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            using Image imgEquip = new Bitmap(FilePath.getYSEquipImgPath(goodsItem));
            bgGraphics.DrawImage(imgEquip, 699, -75, 614, 1230);
        }

        private static void drawEquipIcon(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            using Image imgIcon = new Bitmap(FilePath.getYSBlackEquipIconPath(goodsItem));
            bgGraphics.DrawImage(imgIcon, 47, 512, imgIcon.Width, imgIcon.Height);
        }

        private static void drawEquipName(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            using GraphicsPath path = new GraphicsPath();
            Font nameFont = new Font(FontName, 45, FontStyle.Bold);
            StringFormat format = StringFormat.GenericTypographic;
            RectangleF rect = new RectangleF(190, 555, 600, 200);
            float size = bgGraphics.DpiY * nameFont.SizeInPoints / 72;
            path.AddString(goodsItem.GoodsName, nameFont.FontFamily, (int)nameFont.Style, size, rect, format);
            bgGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            bgGraphics.DrawPath(new Pen(Color.Black, 2), path);
            bgGraphics.FillPath(Brushes.White, path);
        }

        private static void drawEquipStar(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            int starWidth = 34;
            int starHeight = 34;
            int indexX = 190;
            int indexY = 643;
            int starCount = goodsItem.RareType.getStarCount();
            using Image imgStar = new Bitmap(FilePath.getYSStarPath());
            for (int i = 0; i < starCount; i++)
            {
                bgGraphics.DrawImage(imgStar, indexX, indexY, starWidth, starHeight);
                indexX += starWidth + 5;
            }
        }

        private static void drawEquipTrans(Graphics bgGraphics, GoodsItemBO goodsItem)
        {
            using Image imgToken = new Bitmap(FilePath.getYSTokenPath(goodsItem));
            bgGraphics.DrawImage(imgToken, 1394, 485, imgToken.Width, imgToken.Height);
        }

        private static void drawBubbles(Graphics bgGraphics)
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
                bgGraphics.DrawImage(randomImage, randomXIndex, randomYIndex, randomWidth, randomWidth);
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
                bgGraphics.DrawImage(randomImage, randomXIndex, randomYIndex, randomWidth, randomWidth);
            }

            foreach (var item in bigImageList) item.Dispose();
            foreach (var item in smallImageList) item.Dispose();
        }

        /*-------------------------------------------------------十连---------------------------------------------------------------------*/

        private static void drawFrame(Graphics bgGraphics, GoodsItemBO goodsItem, int indexX, int indexY)
        {
            using Image imgFrame = new Bitmap(FilePath.getFrameImgPath());
            bgGraphics.DrawImage(imgFrame, indexX, indexY, imgFrame.Width, imgFrame.Height);
        }

        private static void drawProspect(Graphics bgGraphics, GoodsItemBO goodsItem, int indexX, int indexY)
        {
            using Image imgProspect = new Bitmap(FilePath.getYSProspectImgPath());
            bgGraphics.DrawImage(imgProspect, indexX + 9, indexY + 10, imgProspect.Width, imgProspect.Height);
        }

        private static void drawRoleOrEquip(Graphics bgGraphics, GoodsItemBO goodsItem, int indexX, int indexY, bool withSkin)
        {
            if (goodsItem.GoodsType == GoodsType.角色)
            {
                using Image imgRole = new Bitmap(FilePath.getYSSmallRoleImgPath(goodsItem, withSkin));
                using Image imgResize = new Bitmap(imgRole, imgRole.Width, imgRole.Height);
                bgGraphics.DrawImage(imgResize, indexX - 1, indexY + 4, new Rectangle(-3, 0, imgResize.Width, imgResize.Height), GraphicsUnit.Pixel);
            }
            if (goodsItem.GoodsType == GoodsType.武器)
            {
                using Image imgEquip = new Bitmap(FilePath.getYSEquipImgPath(goodsItem));
                using Image imgResize = new Bitmap(imgEquip, 305, 610);
                bgGraphics.DrawImage(imgResize, indexX + 5, indexY, new Rectangle(80, 0, 140, 550), GraphicsUnit.Pixel);
            }
        }

        private static void drawIcon(Graphics bgGraphics, WishRecordBO wishRecord, int indexX, int indexY)
        {
            GoodsItemBO goodsItem = wishRecord.GoodsItem;
            if (goodsItem.GoodsType == GoodsType.武器)
            {
                using Image imgIcon = new Bitmap(FilePath.getYSWhiteEquipIconPath(goodsItem));
                bgGraphics.DrawImage(imgIcon, indexX + 30, indexY + 430, 100, 100);//画武器图标
            }
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount == 0)
            {
                using Image imgIcon = new Bitmap(FilePath.getYSSmallElementIconPath(goodsItem));
                bgGraphics.DrawImage(imgIcon, indexX + 40, indexY + 440, 72, 72);//画元素图标
            }
        }

        private static void drawTrans(Graphics bgGraphics, WishRecordBO wishRecord, int indexX, int indexY)
        {
            GoodsItemBO goodsItem = wishRecord.GoodsItem;
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount > 1 && wishRecord.OwnedCount <= 7)
            {
                using Image imgStarLight = new Bitmap(FilePath.getYSStarLightIconPath(goodsItem.RareType == RareType.五星 ? 10 : 2));
                bgGraphics.DrawImage(imgStarLight, indexX + 25, indexY + 323, imgStarLight.Width, imgStarLight.Height);//画星辉
                using Image imgStarDust = new Bitmap(FilePath.getYSStarDustIconPath(goodsItem));
                bgGraphics.DrawImage(imgStarDust, indexX + 25, indexY + 443, imgStarDust.Width, imgStarDust.Height);//画星尘
                using Font tranFont = new Font(FontName, 19, FontStyle.Regular);
                using SolidBrush brushWatermark = new SolidBrush(Color.White);
                bgGraphics.DrawString("转化", tranFont, brushWatermark, indexX + 48, indexY + 632);
            }
            if (goodsItem.GoodsType == GoodsType.角色 && wishRecord.OwnedCount > 7)
            {
                using Image imgStarLight = new Bitmap(FilePath.getYSStarLightIconPath(goodsItem.RareType == RareType.五星 ? 25 : 5));
                bgGraphics.DrawImage(imgStarLight, indexX + 25, indexY + 443, imgStarLight.Width, imgStarLight.Height);//画星辉
                using Font tranFont = new Font(FontName, 19, FontStyle.Regular);
                using SolidBrush brushWatermark = new SolidBrush(Color.White);
                bgGraphics.DrawString("转化", tranFont, brushWatermark, indexX + 48, indexY + 632);
            }
        }


        private static void drawLight(Graphics bgGraphics, GoodsItemBO goodsItem, int indexX, int indexY)
        {
            int shiftXIndex = 0;
            int shiftYIndex = -5;
            if (goodsItem.RareType == RareType.五星) shiftXIndex = -103;
            if (goodsItem.RareType == RareType.四星) shiftXIndex = -98;
            if (goodsItem.RareType == RareType.三星) shiftXIndex = 2;
            using Image imgLight = new Bitmap(FilePath.getYSLightPath(goodsItem));
            bgGraphics.DrawImage(imgLight, indexX + shiftXIndex, shiftYIndex, imgLight.Width, imgLight.Height);
        }

        private static void drawStar(Graphics bgGraphics, WishRecordBO wishRecord, int indexX, int indexY)
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
                bgGraphics.DrawImage(imgStar, indexX + indexXAdd, indexY + 535, starWidth, starHeight);
                indexXAdd += starWidth;
            }
        }

        private static void drawShading(Graphics bgGraphics, GoodsItemBO goodsItem, int indexX, int indexY)
        {
            if (goodsItem.RareType != RareType.五星) return;
            using Image imgShading = new Bitmap(FilePath.getYSShadingPath());
            bgGraphics.DrawImage(imgShading, indexX + 3, indexY + 45, imgShading.Width, imgShading.Height);
        }

        private static void drawCloseIcon(Graphics bgGraphics)
        {
            using Image imgClose = new Bitmap(FilePath.getYSCloseIconPath());
            bgGraphics.DrawImage(imgClose, 1920 - 105, 20, imgClose.Width, imgClose.Height);
        }

        private static void drawNewIcon(Graphics bgGraphics, WishRecordBO wishRecord, int indexX, int indexY)
        {
            if (wishRecord.OwnedCount > 1) return;
            using Image imgNew = new Bitmap(FilePath.getYSNewIconPath());
            bgGraphics.DrawImage(imgNew, indexX + 100, indexY + 20, (float)(imgNew.Width * 0.95), (float)(imgNew.Height * 0.95));
        }

        private static void drawWaterMark(Graphics bgGraphics)
        {
            using Font watermarkFont = new Font(FontName, 10, FontStyle.Regular);
            using SolidBrush brushWatermark = new SolidBrush(Color.FromArgb(150, 178, 193));
            bgGraphics.DrawString("本图片由GardenHamster/GenshinWish模拟生成，并不代表游戏内的实际效果", watermarkFont, brushWatermark, 10, 1050);
        }

        private static void drawUID(Graphics bgGraphics, string uid)
        {
            using Font uidFont = new Font(FontName, 15, FontStyle.Regular);
            using SolidBrush brushWatermark = new SolidBrush(Color.White);
            bgGraphics.DrawString($"UID：{uid}", uidFont, brushWatermark, 1650, 1042);
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
