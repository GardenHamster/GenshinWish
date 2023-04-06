using GenshinWish.Dao;
using GenshinWish.Models.PO;
using GenshinWish.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Service
{
    public class MemberService : BaseService
    {
        private MemberDao memberDao;

        public MemberService(MemberDao memberDao)
        {
            this.memberDao = memberDao;
        }

        /// <summary>
        /// 通过编号查找成员
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        public MemberPO GetByCode(int authId, string memberCode)
        {
            return memberDao.getMember(authId, memberCode);
        }

        /// <summary>
        /// 根据编号获取成员,成员不存在时,新增并返回一个新成员
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="memberCode"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public MemberPO GetOrInsert(int authId, string memberCode, string memberName = "")
        {
            MemberPO memberInfo = memberDao.getMember(authId, memberCode);
            if (memberName != null)
            {
                memberName = memberName.CutString(20);
            }
            if (memberInfo == null)
            {
                memberInfo = new MemberPO(authId, memberCode, memberName);
                return memberDao.Insert(memberInfo);
            }
            if (string.IsNullOrEmpty(memberName) == false && memberInfo.MemberName != memberName)
            {
                memberInfo.MemberName = memberName;
                memberDao.Update(memberInfo);
            }
            return memberInfo;
        }

        /// <summary>
        /// 更新成员信息
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public int UpdateMember(MemberPO member)
        {
            return memberDao.Update(member);
        }

        /// <summary>
        /// 武器定轨
        /// </summary>
        /// <param name="member"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public MemberPO AssignWeapon(MemberPO member, int goodsId)
        {
            member.AssignId = goodsId;
            member.AssignValue = 0;//重置命定值
            memberDao.Update(member);
            return member;
        }

        /// <summary>
        /// 保底重置
        /// </summary>
        /// <param name="member"></param>
        public MemberPO ResetSurplus(MemberPO member)
        {
            member.ResetSurplus();
            memberDao.Update(member);
            return member;
        }

    }
}
