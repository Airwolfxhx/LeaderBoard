using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderBoard.Application;
using LeaderBoard.Domain;
using LeaderBoard.Domain.Enum;
using LeaderBoard.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LeaderBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderBoardController : ControllerBase
    {
        private readonly ILeaderBoardService _leaderBoardService;

        public LeaderBoardController(ILeaderBoardService leaderBoardService)
        {
            _leaderBoardService = leaderBoardService;
        }

        [HttpGet]
        public ActionResult Get(int start, int end)
        {
            APIResult<List<CustomerScore>> result = new APIResult<List<CustomerScore>>();

            if (start <= 0 || end <= 0)
            {
                result.ErrorCode = EnumError.BadRequest;
                result.ErrorMessage = "The start or end rank cannot be negative.";

                return Ok(result);
            }

            if (start > end)
            {
                result.ErrorCode = EnumError.BadRequest;
                result.ErrorMessage = "The start rank cannot be greater than the end rank.";

                return Ok(result);
            }

            if (end - start > 99)
            {
                result.ErrorCode = EnumError.BadRequest;
                result.ErrorMessage = "You can only query limit 100 rows data.";

                return Ok(result);
            }

            if (start > CacheHandler.CustomerRankList.Count)
            {
                result.ErrorCode = EnumError.NoData;
                result.ErrorMessage = "There is no data with the query condition.";
                return Ok(result);
            }

            try
            {
                var list = _leaderBoardService.GetCustomersByRank(start, end);
                result.Data = list;

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception message

                // return message
                result.ErrorCode = EnumError.SystemError;
                result.ErrorMessage = "Server error. Please contact customer service.";

                return Ok(result);
            }
        }

        [HttpGet("{customerId}")]
        public ActionResult<string> GetCustomersByCustomerId(Int64 customerId, int high = 0, int low = 0)
        {
            APIResult<List<CustomerScore>> result = new APIResult<List<CustomerScore>>();

            if (high < 0 || low < 0)
            {
                result.ErrorCode = EnumError.BadRequest;
                result.ErrorMessage = "The value of high or low cannot be negative.";

                return Ok(result);
            }

            if (high + low > 99)
            {
                result.ErrorCode = EnumError.BadRequest;
                result.ErrorMessage = "You can only query limit 100 rows data.";

                return Ok(result);
            }

            try
            {
                result = _leaderBoardService.GetCustomersByCustomerId(customerId, high, low);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception message

                // return message
                result.ErrorCode = EnumError.SystemError;
                result.ErrorMessage = "Server error. Please contact customer service.";

                return Ok(result);
            }
        }
    }
}
