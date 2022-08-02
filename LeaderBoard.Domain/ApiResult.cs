using System;
using LeaderBoard.Domain.Enum;

namespace LeaderBoard.Domain
{
    /// <summary>
    /// Leaderboard entity
    /// </summary>
    public class APIResult<T>
    {
        public EnumError ErrorCode = EnumError.OK;
        public string ErrorMessage = "success";
        public T Data { get; set; }
    }
}
