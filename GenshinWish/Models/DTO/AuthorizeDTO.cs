using GenshinWish.Models.PO;

namespace GenshinWish.Models.DTO
{
    public class AuthorizeDto
    {
        public AuthorizePO Authorize { get; set; }

        public int ApiCalledToday { get; set; }

        public int ApiCallSurplus { get; set; }

        public AuthorizeDto() { }

        public AuthorizeDto(AuthorizePO Authorize, int apiCalledToday)
        {
            this.Authorize = Authorize;
            ApiCalledToday = apiCalledToday;
            ApiCallSurplus = Authorize.DailyCall - apiCalledToday < 0 ? 0 : Authorize.DailyCall - apiCalledToday;
        }


    }
}
