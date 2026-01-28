using McpaApi.Models;
using McpaApi.Models.Dto;
using McpaApi.Models.Dto.Output;
using McpaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace McpaApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ReportsController : ControllerBase
  {
    protected readonly ShopGarageService _service;
    protected readonly ShopAguaAzulService _aguaAzulService;
    protected readonly ShopPuntoSurService _puntoSurService;
    protected readonly ILealtadService _lealtadService;
    protected readonly ReportService _reportService;

    public ReportsController(
      ShopGarageService service,
      ShopAguaAzulService aguaAzulService,
      ShopPuntoSurService puntoSurService,
      ILealtadService lealtadService,
      ReportService reportService
    )
    {
      _service = service;
      _aguaAzulService = aguaAzulService;
      _puntoSurService = puntoSurService;
      _lealtadService = lealtadService;
      _reportService = reportService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ReportSales>> Get([FromQuery] DownloadReportSale downloadReportSale)
    {
      IEnumerable<ReportSales> result;

      switch (downloadReportSale.WebSite)
      {
        case WebSite.AguaAzul:
          result = _aguaAzulService.SaleReport(downloadReportSale);
          break;
        case WebSite.PuntoSur:
          result = _puntoSurService.SaleReport(downloadReportSale);
          break;
        default:
          result = _service.SaleReport(downloadReportSale);
          break;
      }

      return Ok(result);
    }

    [HttpGet("download")]
    public IActionResult Download([FromQuery] DownloadReportSale downloadReportSale)
    {
      MemoryStream result;

      switch (downloadReportSale.WebSite)
      {
        case WebSite.AguaAzul:
          result = _aguaAzulService.DownloadSaleReport(downloadReportSale);
          break;
        case WebSite.PuntoSur:
          result = _puntoSurService.DownloadSaleReport(downloadReportSale);
          break;
        default:
          result = _service.DownloadSaleReport(downloadReportSale);
          break;
      }

      var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
      var fileName = "Comisiones_Vendedores.xlsx";

      return File(result, contentType, fileName);
    }

    [HttpGet("lealtad")]
    public IActionResult ReportLealtad()
    {
      var result = _lealtadService.SendReportLealtad();

      return Ok(result);
    }

    [HttpGet("all-invoice")]
    public IActionResult GetAllInvoice()
    {
      var result = _lealtadService.GetDocuments();

      return Ok(result);
    }

    [HttpPost("SellerJob")]
    public async Task<IActionResult> SellerJob()
    {
      await _reportService.Execute();
      return Ok(true);
    }

    [HttpGet("YearSellerJob")]
    public async Task<IActionResult> YearSellerJob()
    {
      await _reportService.ReportAnual();
      return Ok(true);
    }

    [HttpPost("LealtadJob")]
    public async Task<IActionResult> LealtadJob()
    {
      _lealtadService.UploadReportLealtad();

      return Ok(true);    
    }
  }
}