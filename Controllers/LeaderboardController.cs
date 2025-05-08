using Leaderboard.Models;
using Leaderboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace Leaderboard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboard _Leaderboard;

        public LeaderboardController(ILeaderboard Leaderboard)
        {
            _Leaderboard = Leaderboard;
        }

        [HttpPost("customer/{customerid}/score/{score}")]
        public ActionResult<decimal> UpdateScore(long customerid, decimal score)
        {
            try
            {
                var newScore = _Leaderboard.UpdateScore(customerid, score);
                return Ok(newScore);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<List<LeaderboardResponse>> GetByRank([FromQuery] int start, [FromQuery] int end)
        {
            try
            {
                var result = _Leaderboard.GetCustomersByRank(start, end);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{customerid}")]
        public ActionResult<NeighborsResponse> GetCustomerWithNeighbors(
            long customerid,
            [FromQuery] int high = 0,
            [FromQuery] int low = 0)
        {
            try
            {
                var result = _Leaderboard.GetCustomerWithNeighbors(customerid, high, low);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}