using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.DTOs.LogDto_s;
using UserManagementApiReverb.BusinessLayer.Logging;

namespace UserManagementApiReverb.PresentationLayer.Controllers;

[ApiController] 
[Route("api/logs")]
public class LogController : ControllerBase
{
    private readonly ILogQueryService _svc;
    public LogController(ILogQueryService svc)
    {
        _svc = svc;
    }

    [HttpPost("search")]
    // kodun okunaklığını artırabilmek için kullanılan bir attribute'dur olmasada olur güvenli ve daha clean hale getirir kodu okumayı kolaylaştırır.
    // bu tarz ifadeler büyük yapılarda daha çok kullanılır ve tercih edilir.
    [ProducesResponseType(typeof(IEnumerable<LogEntry>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromBody] LogQueryRequest req, CancellationToken ct)
    {
        return Ok(await _svc.QueryAsync(req, ct));
    }
}