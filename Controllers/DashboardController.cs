using McpaApi.Dto;
using McpaApi.Models.Dto.Output;
using McpaApi.Models.Shop;
using McpaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace McpaApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class DashboardController : ControllerBase
  {
    protected readonly ShopGarageService _service;
    protected readonly ShopAguaAzulService _aguaAzulService;
    protected readonly ShopPuntoSurService _puntoSurService;

    public DashboardController(
      ShopGarageService service,
      ShopAguaAzulService aguaAzulService,
      ShopPuntoSurService puntoSurService
    )
    {
      _service = service;
      _aguaAzulService = aguaAzulService;
      _puntoSurService = puntoSurService;
    }

    [HttpGet]
    public ActionResult<DashboardReport> Get([FromQuery] FilterDashboardReport filterDashboardReport)
    {
      DashboardReport result;

      switch (filterDashboardReport.WebSite)
      {
        case Models.WebSite.AguaAzul:
          result = _aguaAzulService.DashboardReport(filterDashboardReport);
          break;
        case Models.WebSite.PuntoSur:
          result = _puntoSurService.DashboardReport(filterDashboardReport);
          break;
        default:
          result = _service.DashboardReport(filterDashboardReport);
          break;
      }

      return Ok(result);
    }
  }
}