using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderBoard.Domain;
using LeaderBoard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using LeaderBoard.Domain.Enum;
using LeaderBoard.Application;

namespace LeaderBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public ActionResult Get(int startCreate)
        {
            if (startCreate == 1)
            {
                int n = 10000;
                // create test data
                for (int i = 0; i < 500000; i++)
                {
                    Int64 customerId = n + i;

                    Random random = new Random();
                    decimal score = Convert.ToDecimal(Math.Round(random.NextDouble() * 1000, 2));

                    if (score > 1000)
                        score = 1000;
                    _customerService.SaveScore(customerId, score);
                }
            }

            return Ok();
        }

        [HttpGet("clear")]
        public ActionResult ClearData()
        {
            CacheHandler.CustomerRankList.Clear();

            for (int i = 0; i < 11; i++)
            {
                CacheHandler.CustomerIDList[i].Clear();
                CacheHandler.ScoreList[i].Clear();
            }

            return Ok();
        }

        /// <summary>
        /// add or update score
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        [HttpPost("{customerId}/score/{score}")]
        public ActionResult Post(Int64 customerId, decimal score)
        {
            APIResult<dynamic> result = new APIResult<dynamic>();

            // check parameter
            if (score > 1000 || score < -1000)
            {
                result.ErrorCode = EnumError.BadRequest;
                result.ErrorMessage = "The score is out of range.";
            }

            if (customerId <= 0)
            {
                result.ErrorCode = EnumError.BadRequest;
                result.ErrorMessage = "The value of customerId must be positive.";

                return Ok(result);
            }

            try
            {
                if (!_customerService.SaveScore(customerId, score))
                {
                    result.ErrorCode = EnumError.OutOfRange;
                    result.ErrorMessage = "Congratulations! You are the 2147483648th customer!";
                }

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
