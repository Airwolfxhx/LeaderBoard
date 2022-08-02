using System;
using System.Collections.Generic;
using System.Text;

namespace LeaderBoard.Domain.Enum
{
    /// <summary>
    /// the error code enum of return value
    /// </summary>
    public enum EnumError
    {
        OK=0,                
        NoData = 100,
        BadRequest = 400,
        SystemError = 500,
        OutOfRange = 600            
    }
}
