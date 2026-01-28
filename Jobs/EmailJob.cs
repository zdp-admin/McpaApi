using System.Globalization;
using McpaApi.Models;
using McpaApi.Models.Dto.Output;
using McpaApi.Services;
using Quartz;

namespace McpaApi.Jobs
{
    public class EmailJob : IJob
    {
        private readonly ILogger<ReportJob> _logger;
        private readonly IEmailService _emailService;
        private readonly ShopGarageService _shopGarageService;
        private readonly ShopPuntoSurService _shopPuntoSurService;
        private readonly ShopAguaAzulService _shopAguaService;

        public EmailJob(
            IEmailService emailService,
            ShopGarageService shopGarageService,
            ShopPuntoSurService shopPuntoSurService,
            ShopAguaAzulService shopAguaService,
            ILogger<ReportJob> logger
        )
        {
            _emailService = emailService;
            _shopAguaService = shopAguaService;
            _shopGarageService = shopGarageService;
            _shopPuntoSurService = shopPuntoSurService;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            DateTime hoy = new DateTime(2025, 9, 6);//DateTime.Today;
            //hoy = hoy.AddDays(-2);
            DateTime primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime ultimoDiaMes = new DateTime(hoy.Year, hoy.Month, 6);//primerDiaMes.AddMonths(1).AddDays(-1);

            await this.SendFromSureste(hoy, primerDiaMes, ultimoDiaMes);
            //await this.SendFromGarage(hoy, primerDiaMes, ultimoDiaMes);
            //await this.SendFromAguaAzul(hoy, primerDiaMes, ultimoDiaMes);
        }

        private async Task SendFromGarage(DateTime now, DateTime startDate, DateTime endDate)
        {
            CultureInfo cultura = new CultureInfo("es-MX");
            string dateNow = now.ToString("dd MMMM yy", cultura);

            var reportGarage = _shopGarageService.DashboardReport(new Dto.FilterDashboardReport()
            {
                WebSite = 0,
                StartDate = DateOnly.FromDateTime(startDate),
                EndDate = DateOnly.FromDateTime(endDate),
                IsForReportEmail = true,
            });

            var html = this.GenerateHtml(reportGarage, dateNow);

            await _emailService.SendEmailAsync(
                //"mcampos@zonadeprivilegios.com.mx",
                "juan_rivera99@hotmail.com",
                $"Reporte de venta Patria - {dateNow}",
                html
            );
        }

        private async Task SendFromSureste(DateTime now, DateTime startDate, DateTime endDate)
        {
            CultureInfo cultura = new CultureInfo("es-MX");
            string dateNow = now.ToString("dd MMMM yy", cultura);

            var report = _shopPuntoSurService.DashboardReport(new Dto.FilterDashboardReport()
            {
                WebSite = 0,
                StartDate = DateOnly.FromDateTime(startDate),
                EndDate = DateOnly.FromDateTime(endDate),
                Today = now,
                IsForReportEmail = true,
            });

            var html = this.GenerateHtml(report, dateNow);

            await _emailService.SendEmailAsync(
                //"molina@garage290.mx",
                //"mcampos@zonadeprivilegios.com.mx",
                "juan_rivera99@hotmail.com",
                $"Reporte de venta Punto Sur - {dateNow}",
                html
            );
        }

        private async Task SendFromAguaAzul(DateTime now, DateTime startDate, DateTime endDate)
        {
            CultureInfo cultura = new CultureInfo("es-MX");
            string dateNow = now.ToString("dd MMMM yy", cultura);

            var report = _shopAguaService.DashboardReport(new Dto.FilterDashboardReport()
            {
                WebSite = 0,
                StartDate = DateOnly.FromDateTime(startDate),
                EndDate = DateOnly.FromDateTime(endDate),
                IsForReportEmail = true,
            });

            var html = this.GenerateHtml(report, dateNow);

            await _emailService.SendEmailAsync(
                //"molina@garage290.mx",
                //"mcampos@zonadeprivilegios.com.mx",
                "juan_rivera99@hotmail.com",
                $"Reporte de venta Agua Azul - {dateNow}",
                html
            );
        }

        private string GenerateHtml(DashboardReport report, string dateNow)
        {
            var html = @$"
            <!doctype html>
            <html xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>

            <head>
            <title>
            </title>
            <!--[if !mso]><!-->
            <meta http-equiv='X-UA-Compatible' content='IE=edge'>
            <!--<![endif]-->
            <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1'>
            <style type='text/css'>
                #outlook a {{
                    padding: 0;
                }}

                body {{
                    margin: 0;
                    padding: 0;
                    -webkit-text-size-adjust: 100%;
                    -ms-text-size-adjust: 100%;
                }}

                table,
                td {{
                    border-collapse: collapse;
                    mso-table-lspace: 0pt;
                    mso-table-rspace: 0pt;
                }}

                img {{
                    border: 0;
                    height: auto;
                    line-height: 100%;
                    outline: none;
                    text-decoration: none;
                    -ms-interpolation-mode: bicubic;
                }}

                p {{
                    display: block;
                    margin: 13px 0;
                }}
            </style>
            <!--[if mso]>
                    <noscript>
                    <xml>
                    <o:OfficeDocumentSettings>
                    <o:AllowPNG/>
                    <o:PixelsPerInch>96</o:PixelsPerInch>
                    </o:OfficeDocumentSettings>
                    </xml>
                    </noscript>
                    <![endif]-->
            <!--[if lte mso 11]>
                    <style type='text/css'>
                    .mj-outlook-group-fix {{ width:100% !important; }}
                    </style>
                    <![endif]-->
            <!--[if !mso]><!-->
            <link href='https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700' rel='stylesheet' type='text/css'>
            <style type='text/css'>
                @import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);
            </style>
            </head>
            <body style='word-spacing:normal;background-color:#ffffff;'>
            <div>
                <table style='border-collapse: collapse;'>
                    <tr>
                    <td><img style='background-color: black;border-radius: 5px;width: 130px;' src='https://garage290.com.mx/cdn/shop/files/Garage-1_300x.png?v=1615321310'/>   </td>
                    <td><p style='width: 60px'></p></td>
                    <td><h2 style='margin: 0px;margin-bottom: 4px;'>{dateNow}</h2></td>
                    </tr>
                </table>
                <h3 style='margin-bottom: 4px;'>Acumulado del mes</h3>
                <h3 style='margin: 0px;'>{((report.TotalSales / 1.16) - report.TotalNotasCredito).ToString("C")}</h3>
                <h4 style='margin-bottom: 4px;'>Venta del día</h4>
                <h3 style='margin: 0px;'>{(report.TotalSalesCurrentDay / 1.16).ToString("C")}</h3>
                <br />
                <table style='border-collapse: collapse;'>
                    <thead>
                        <tr style='background-color: #ffdd04'>
                            <th style='padding: 4px 10px;'>Vendedores</th>
                            <th style='padding: 4px 10px;'>Autos del dia</th>
                            <th style='padding: 4px 10px;'>Venta del día</th>
                            <th style='padding: 4px 10px;'>Autos del mes</th>
                            <th style='padding: 4px 10px;'>Ventas del mes</th>
                        </tr>
                    </thead>
                    <tbody>";
                    var indexSeller = 1;
                    var sellers = report.Sellers.OrderByDescending(s => s.Comission);
                    var TotalSalesCurrentDay = 0d;
                    var ComissionCurrentDay = 0d;
                    var TotalSales = 0d;
                    var Comission = 0d;
                    var color = "#f0f0f0";
                    foreach (var item in sellers)
                    {
                        color = indexSeller % 2 == 0 ? "#f0f0f0" : "#ffffff";
                        html += @$"<tr style='background-color: {color}'>
                            <td style='padding: 4px 10px;'>{item.SellerName}</td>
                            <td style='padding: 4px 10px;'>{item.TotalSalesCurrentDay}</td>
                            <td style='padding: 4px 10px;'>{(item.ComissionCurrentDay / 1.16).ToString("C")}</td>
                            <td style='padding: 4px 10px;'>{item.TotalSales}</td>
                            <td style='padding: 4px 10px;'>{(item.Comission / 1.16).ToString("C")}</td>
                        </tr>";

                        indexSeller++;
                        TotalSalesCurrentDay += item.TotalSalesCurrentDay;
                        ComissionCurrentDay += (item.ComissionCurrentDay / 1.16);
                        TotalSales += item.TotalSales;
                        Comission += (item.Comission / 1.16);
                    }

                    color = indexSeller % 2 == 0 ? "#f0f0f0" : "#ffffff";
                    html += @$"<tr style='background-color: {color}'>
                            <td style='padding: 4px 10px;'>Total</td>
                            <td style='padding: 4px 10px;'>{TotalSalesCurrentDay}</td>
                            <td style='padding: 4px 10px;'>{ComissionCurrentDay.ToString("C")}</td>
                            <td style='padding: 4px 10px;'>{TotalSales}</td>
                            <td style='padding: 4px 10px;'>{Comission.ToString("C")}</td>
                        </tr>";

                    var totalSales = report.AverageSales;
                    var totalComission = report.AverageTotalSale / 1.16;

                    html += @$"</ tbody >
                    </table>
                    <br />
                    <table style='border-collapse: collapse;'>
                        <thead>
                        <tr style='background-color: #ffdd04'>
                            <th style='padding: 4px 10px;'>Promedio</th>
                            <th style='padding: 4px 10px;'>Cantidad</th>
                        </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td style='padding: 4px 10px;'>Autos</td>
                                <td style='padding: 4px 10px;'>{Math.Round(totalSales, 1)}</td>
                            </tr>
                            <tr>
                                <td style='padding: 4px 10px;'>Ticket</td>
                                <td style='padding: 4px 10px;'>{totalComission.ToString("C")}</td>
                            </tr>
                        ";
                    



                    html += @"</ tbody >
                    </table>
                    <br />
                    <table style='border-collapse: collapse;'>
                        <thead>
                        <tr style='background-color: #ffdd04'>
                            <th style='padding: 4px 10px;'>Productos</th>
                            <th style='padding: 4px 10px;'>Cantidad</th>
                            <th style='padding: 4px 10px;'>Total del día</th>
                        </tr>
                        </thead>
                        <tbody>";

                        var products = report.Products.OrderByDescending(p => p.TotalSaleCurrentDay).Where(p => p.QuantityCurrentDay > 0);

                        var index = 1;
                        var totalQuantity = 0;
                        double totalAmount = 0;
                        var colorProduct = "#ffffff";
                        foreach (var producto in products)
                        {
                            colorProduct = index % 2 == 0 ? "#f0f0f0" : "#ffffff";
                            html += @$"<tr style='background-color: {colorProduct}'>
                            <td style='padding: 4px 10px;'>{producto.Name}</td>
                            <td style='padding: 4px 10px;'>{producto.QuantityCurrentDay}</td>
                            <td style='padding: 4px 10px;'>{(producto.TotalSaleCurrentDay / 1.16).ToString("C")}</td>
                            </tr>";

                            totalQuantity += producto.QuantityCurrentDay;
                            totalAmount += producto.TotalSaleCurrentDay / 1.16;

                            index++;
                        }

                        colorProduct = index % 2 == 0 ? "#f0f0f0" : "#ffffff";
                        html += @$"<tr style='background-color: {colorProduct}'>
                            <td style='padding: 4px 10px;'>TOTALES</td>
                            <td style='padding: 4px 10px;'>{totalQuantity}</td>
                            <td style='padding: 4px 10px;'>{totalAmount.ToString("C")}</td>
                            </tr>";


                html += @"</tbody>
                </table>
                </div></body></html>";

            return html;
        }
    }
}