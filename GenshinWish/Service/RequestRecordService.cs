using GenshinWish.Dao;
using GenshinWish.Helper;
using GenshinWish.Models.PO;
using Microsoft.AspNetCore.Http;
using System;

namespace GenshinWish.Service
{
    public class RequestRecordService : BaseService
    {
        private RequestRecordDao requestRecordDao;

        public RequestRecordService(RequestRecordDao requestRecordDao)
        {
            this.requestRecordDao = requestRecordDao;
        }

        public int GetRequestTimesToday(int authId)
        {
            DateTime startTime = DateTimeHelper.GetTodayStart();
            DateTime endTime = DateTimeHelper.GetTodayEnd();
            return requestRecordDao.getRequestTimes(authId, startTime, endTime);
        }

        public RequestRecordPO AddRecord(IHttpContextAccessor httpContextAccessor, HttpRequest request, int authId)
        {
            string ipAddr = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString().ToLower() ?? "";
            RequestRecordPO requestRecord = new RequestRecordPO();
            requestRecord.AuthId = authId;
            requestRecord.Path = request.Path.Value?.Trim()?.CutString(100) ?? "";
            requestRecord.IpAddr = ipAddr?.Trim().CutString(100).ToLower() ?? "";
            requestRecord.CreateDate = DateTime.Now;
            return requestRecordDao.Insert(requestRecord);
        }



    }
}
