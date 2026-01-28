using System.Globalization;
using McpaApi.Models;
using McpaApi.Models.Dto.Output;

namespace McpaApi.Services
{
    public class ReportService
    {
        private readonly ILogger<ReportService> _logger;
        protected readonly IEmailService _emailService;
        protected readonly ShopGarageService _service;
        protected readonly ShopAguaAzulService _aguaAzulService;
        protected readonly ShopPuntoSurService _puntoSurService;

        public ReportService(
            ShopGarageService service,
            ShopAguaAzulService aguaAzulService,
            ShopPuntoSurService puntoSurService,
            IEmailService emailService,
            ILogger<ReportService> logger
        )
        {
            _service = service;
            _aguaAzulService = aguaAzulService;
            _puntoSurService = puntoSurService;
            _emailService = emailService;
            _logger = logger;
        }
        
        public async Task Execute()
        {
            _logger.LogInformation("Iniciando ReportJob a las {Time}", DateTime.Now);

            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                //var today = DateOnly.FromDateTime(new DateTime(2025, 10, 13));
                var initDate = new DateOnly(today.Year, today.Month, 1);
                var onlySellers = new List<string>(["José Daniel", "Cristian Sanchez", "Barbara Sandoval"]);

                IEnumerable<ReportSales> resultAguaAzul = _aguaAzulService.SaleReport(new Models.Dto.DownloadReportSale()
                {
                    WebSite = WebSite.AguaAzul,
                    StartDate = initDate,
                    EndDate = today
                });
                IEnumerable<ReportSales> resultPuntoSur = _puntoSurService.SaleReport(new Models.Dto.DownloadReportSale()
                {
                    WebSite = WebSite.PuntoSur,
                    StartDate = initDate,
                    EndDate = today
                });
                IEnumerable<ReportSales> resultPatria = _service.SaleReport(new Models.Dto.DownloadReportSale()
                {
                    WebSite = WebSite.Garage,
                    StartDate = initDate,
                    EndDate = today
                });
                resultAguaAzul = resultAguaAzul.Where(r => onlySellers.Contains(r.Saller.Trim()));
                resultPuntoSur = resultPuntoSur.Where(r => onlySellers.Contains(r.Saller.Trim()));
                resultPatria = resultPatria.Where(r => onlySellers.Contains(r.Saller.Trim()));

                var todayResult = new List<ReportSocialSale>();
                var monthResult = new List<ReportSocialSale>();

                foreach (var item in resultAguaAzul)
                {
                    monthResult.Add(new ReportSocialSale()
                    {
                        Amount = item.AmountComercial,
                        Portal = WebSite.AguaAzul,
                        Seller = item.Saller.Trim(),
                    });

                    if (item.Date == today)
                    {
                        todayResult.Add(new ReportSocialSale()
                        {
                            Amount = item.AmountComercial,
                            Portal = WebSite.AguaAzul,
                            Seller = item.Saller.Trim(),
                        });
                    }
                }

                foreach (var item in resultPuntoSur)
                {
                    monthResult.Add(new ReportSocialSale()
                    {
                        Amount = item.AmountComercial,
                        Portal = WebSite.PuntoSur,
                        Seller = item.Saller.Trim(),
                    });

                    if (item.Date == today)
                    {
                        todayResult.Add(new ReportSocialSale()
                        {
                            Amount = item.AmountComercial,
                            Portal = WebSite.PuntoSur,
                            Seller = item.Saller.Trim(),
                        });
                    }
                }

                foreach (var item in resultPatria)
                {
                    monthResult.Add(new ReportSocialSale()
                    {
                        Amount = item.AmountComercial,
                        Portal = WebSite.Garage,
                        Seller = item.Saller.Trim(),
                    });

                    if (item.Date == today)
                    {
                        todayResult.Add(new ReportSocialSale()
                        {
                            Amount = item.AmountComercial,
                            Portal = WebSite.Garage,
                            Seller = item.Saller.Trim(),
                        });
                    }
                }

                var html = this.GenerateHtml(todayResult, monthResult);

                await _emailService.SendEmailAsync(
                    "molina@garage290.mx",
                    //"juan_rivera99@hotmail.com",
                    "Reporte de ventas",
                    html,
                    ["garage290mx@gmail.com", "cord@garage290.mx"]
                );
                
                _logger.LogInformation("ReportJob completado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ReportJob");
            }

            await Task.Delay(1000);
        }

        public async Task<ReportSalesYear> ReportAnual()
        {
            _logger.LogInformation("Iniciando ReportAnual a las {Time}", DateTime.Now);

            try
            {
                var today = new DateOnly(2025, 12, 31);
                //var today = DateOnly.FromDateTime(new DateTime(2025, 10, 13));
                var initDate = new DateOnly(today.Year, 1, 1);
                var onlySellers = new List<string>(["José Daniel", "Cristian Sanchez", "Barbara Sandoval"]);

                ReportSalesYear resultAguaAzul = _aguaAzulService.SalesYearReport(new Models.Dto.DownloadReportSale()
                {
                    WebSite = WebSite.AguaAzul,
                    StartDate = initDate,
                    EndDate = today
                });
                ReportSalesYear resultPuntoSur = _puntoSurService.SalesYearReport(new Models.Dto.DownloadReportSale()
                {
                    WebSite = WebSite.PuntoSur,
                    StartDate = initDate,
                    EndDate = today
                });
                ReportSalesYear resultPatria = _service.SalesYearReport(new Models.Dto.DownloadReportSale()
                {
                    WebSite = WebSite.Garage,
                    StartDate = initDate,
                    EndDate = today
                });


                var htmlPuntoSur = this.GenerateYearHtml(resultPuntoSur, "PUNTO SUR");

                await _emailService.SendEmailAsync(
                    "molina@garage290.mx",
                    "Reporte de ventas anual 2025 PUNTO SUR",
                    htmlPuntoSur
                );

                var htmlAguaAzul = this.GenerateYearHtml(resultAguaAzul, "AGUA AZUL");

                await _emailService.SendEmailAsync(
                    "molina@garage290.mx",
                    "Reporte de ventas anual 2025 AGUA AZUL",
                    htmlAguaAzul
                );

                var htmlPatria = this.GenerateYearHtml(resultPatria, "PATRIA");

                await _emailService.SendEmailAsync(
                    "molina@garage290.mx",
                    "Reporte de ventas anual 2025 PATRIA",
                    htmlPatria
                );
                
                _logger.LogInformation("ReportYearJob completado correctamente");

                return resultPuntoSur;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ReportYearJob");
                throw;
            }
        }

        private string GenerateYearHtml(ReportSalesYear reportSalesYear, string company)
        {
            var html = @$"
                <!DOCTYPE html>
                <html>
                <head>
                <meta charset='UTF-8'>
                <title>Reporte Anual</title>
                </head>
                <body style='margin:0;padding:0;background-color:#f4f4f4;font-family:Arial, Helvetica, sans-serif;'>

                <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f4f4;padding:20px;'>
                <tr>
                    <td align='center'>

                    <!-- CONTENEDOR -->
                    <table width='600' cellpadding='0' cellspacing='0' style='background-color:#ffffff;border-radius:6px;overflow:hidden;'>

                        <!-- HEADER -->
                        <tr>
                        <td style='background-color:#111827;color:#ffffff;padding:16px;text-align:center;font-size:20px;font-weight:bold;'>
                            Reporte Anual 2025
                        </td>
                        </tr>

                        <!-- RESUMEN GENERAL -->
                        <tr>
                        <td style='padding:20px;'>
                            <table width='100%' cellpadding='0' cellspacing='0'>
                            <tr>
                                <td width='30%' align='center' style='padding:10px;border-right:1px solid #e5e7eb;'>
                                <div style='font-size:13px;color:#6b7280;'>VENTA TOTAL DE TIENDA</div>
                                <div style='font-size:26px;font-weight:bold;margin:10px 0;'>{(reportSalesYear.TotalSalesWithAdditionals + reportSalesYear.TotalSalesWithoutAdditionals).ToString("C")}</div>
                                <img src='https://www.garage290.mx/wp-content/uploads/2026/01/favicon-garage290.png' width='120' style='display:block;margin:10px auto;'>
                                <div style='font-size:14px;font-weight:bold;'>{company}</div>
                                </td>

                                <td width='70%' style='padding:10px;'>
                                <div style='font-size:14px;font-weight:bold;margin-bottom:10px;'>
                                    Venta de órdenes generadas
                                </div>

                                <table width='100%' cellpadding='6' cellspacing='0' style='border-collapse:collapse;font-size:12px;'>
                                    <tr style='background-color:#f3f4f6;font-weight:bold;'>
                                    <td>Asesor</td>
                                    <td align='right'>Órdenes Generadas</td>
                                    <td align='right'>Venta Sin Adicionales</td>
                                    <td align='right'>Ticket Promedio</td>
                                    </tr>";
                                    foreach (var item in reportSalesYear.SalesWithoutAdditionals)
                                    {
                                        html += @$"<tr><td>{item.Seller}</td><td align='right'>{item.Orders}</td><td align='right'>{item.TotalSales.ToString("C")}</td><td align='right'>{item.AvgTicket.ToString("C")}</td></tr>";
                                    }

                                    html += @$"<tr style='font-weight:bold;border-top:1px solid #e5e7eb;'>
                                    <td>TOTAL</td>
                                    <td align='right'>{reportSalesYear.SalesWithoutAdditionals.Sum(x => x.Orders)}</td>
                                    <td align='right'>{reportSalesYear.SalesWithoutAdditionals.Sum(x => x.TotalSales).ToString("C")}</td>
                                    <td align='right'>{(reportSalesYear.SalesWithoutAdditionals.Sum(x => x.TotalSales) / reportSalesYear.SalesWithoutAdditionals.Sum(x => x.Orders)).ToString("C")}</td>
                                    </tr>
                                </table>
                                </td>
                            </tr>
                            </table>
                        </td>
                        </tr>

                        <!-- ADICIONALES -->
                        <tr>
                        <td style='padding:20px;'>
                            <div style='font-size:16px;font-weight:bold;margin-bottom:10px;'>
                            Estadísticas de Venta de Adicionales
                            </div>

                            <table width='100%' cellpadding='6' cellspacing='0' style='border-collapse:collapse;font-size:12px;'>
                            <tr style='background-color:#f3f4f6;font-weight:bold;'>
                                <td>Asesor</td>
                                <td align='right'>Órdenes con Adicionales</td>
                                <td align='right'>Productos Extras</td>
                                <td align='right'>Venta Total</td>
                                <td align='right'>Ticket Promedio</td>
                                <td align='right'>% Penetración</td>
                            </tr>";

                           foreach (var item in reportSalesYear.SalesWithAdditionals)
                            {
                                html += @$"<tr><td>{item.Seller}</td><td align='right'>{item.Orders}</td><td align='right'>{item.ExtraProducts}</td><td align='right'>{item.TotalAdditional.ToString("C")}</td><td align='right'>{item.AvgTicket.ToString("C")}</td><td align='right'>{item.PercentPenetration.ToString("F2")}%</td></tr>";
                            } 


                            html += @$"
                            <tr style='font-weight:bold;border-top:1px solid #e5e7eb;'>
                                <td>Total</td>
                                <td align='right'>{reportSalesYear.SalesWithAdditionals.Sum(x => x.Orders)}</td>
                                <td align='right'>{reportSalesYear.SalesWithAdditionals.Sum(x => x.ExtraProducts)}</td>
                                <td align='right'>{reportSalesYear.SalesWithAdditionals.Sum(x => x.TotalAdditional).ToString("C")}</td>
                                <td align='right'>{reportSalesYear.SalesWithAdditionals.Sum(x => x.AvgTicket).ToString("C")}</td>
                                <td align='right'>{reportSalesYear.SalesWithAdditionals.Sum(x => x.PercentPenetration).ToString("F2")}%</td>
                            </tr>
                            </table>
                        </td>
                        </tr>

                        <!-- RESUMEN TIENDA -->
                        <tr>
                        <td style='padding:20px;'>
                            <div style='font-size:16px;font-weight:bold;margin-bottom:10px;'>
                            Resumen Tienda
                            </div>

                            <table width='100%' cellpadding='6' cellspacing='0' style='border-collapse:collapse;font-size:12px;'>
                            <tr style='background-color:#f3f4f6;font-weight:bold;'>
                                <td>Tienda</td>
                                <td align='right'>Órdenes Generadas</td>
                                <td align='right'>Órdenes con Adicionales</td>
                                <td align='right'>% Penetración</td>
                                <td align='right'>Venta Total</td>
                                <td align='right'>Venta Adicionales</td>
                            </tr>

                            <tr>
                                <td>{company}</td>
                                <td align='right'>{reportSalesYear.Orders}</td>
                                <td align='right'>{reportSalesYear.OrdersWithAdditionals}</td>
                                <td align='right'>{((decimal)reportSalesYear.OrdersWithAdditionals / (decimal)reportSalesYear.Orders).ToString("P2")}</td>
                                <td align='right'>{(reportSalesYear.TotalSalesWithAdditionals + reportSalesYear.TotalSalesWithoutAdditionals).ToString("C")}</td>
                                <td align='right'>{reportSalesYear.TotalSalesWithAdditionals.ToString("C")}</td>
                            </tr>

                            <tr style='font-weight:bold;border-top:1px solid #e5e7eb;'>
                                <td colspan='5'>
                                Porcentaje de la venta de adicionales en la venta total de la tienda
                                </td>
                                <td align='right'>{(reportSalesYear.TotalSalesWithAdditionals / (reportSalesYear.TotalSalesWithAdditionals + reportSalesYear.TotalSalesWithoutAdditionals)).ToString("P2")}</td>
                            </tr>
                            </table>
                        </td>
                        </tr>

                        <!-- FOOTER -->
                        <tr>
                        <td style='background-color:#f9fafb;padding:14px;font-size:12px;color:#6b7280;text-align:center;'>
                            © 2026 Garage290 · Reporte automático
                        </td>
                        </tr>

                    </table>

                    </td>
                </tr>
                </table>

                </body>
                </html>
            ";

            return html;
        }

        private string GenerateHtml(List<ReportSocialSale> todayResult, List<ReportSocialSale> monthResult)
        {
            var today = DateTime.Now;
            var sellers = monthResult.Select(s => s.Seller.Trim()).Distinct().ToArray();
            var totalsByKey = todayResult
            .GroupBy(s => $"{s.Seller}{s.Portal}")
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Amount)
            );
            var totalMonthByKey = monthResult.GroupBy(s => $"{s.Seller}{s.Portal}").ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Amount)
            );
            var totalForDays = new Dictionary<string, double>();
            var totalForMonths = new Dictionary<string, double>();

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


                <table style='border-collapse: collapse;'>
                    <tr>
                    <td><img style='background-color: black;border-radius: 5px;width: 130px;' src='https://garage290.com.mx/cdn/shop/files/Garage-1_300x.png?v=1615321310'/>   </td>
                    <td><p style='width: 60px'></p></td>
                    <td><h2 style='margin: 0px;margin-bottom: 4px;'>Reporte de ventas de redes sociales</h2></td>
                    </tr>
                </table>
                <br/><br/>
                <table style='border-collapse: collapse;background-color:black;width:100%;color:#ffdd01'>
                  <tr>
                      <td style='text-align:center;margin:0;padding:6px 0;'>Ventas del día {today.ToString("dd 'de' MMMM", new CultureInfo("es-ES"))}</td>
                  </tr>
                </table>
                
                <table style='width:100%'>
                <tr style='border-collapse: collapse;background-color:#ffdd01;width:100%;color:black'>
                <td>VENTA DEL DÍA</td>";

            foreach (var item in sellers)
            {
                html += @$"
                    <td>{item}</td>
                ";
            }

            html += @$"
                <td>TOTAL</td>
                </tr>
                <tr>
                <td>Patria</td>";

            double sum = 0;
            double sumTotal = 0;
            foreach (var item in sellers)
            {
                if (totalsByKey.TryGetValue($"{item}{WebSite.Garage}", out double totalAmount))
                {
                    html += @$"<td>{totalAmount.ToString("C", new CultureInfo("es-MX"))}</td>";
                    sum += totalAmount;

                    if (!totalForDays.ContainsKey(item))
                    {
                        totalForDays.Add(item, totalAmount);
                    }
                    else
                    {
                        totalForDays[item] += totalAmount;
                    }
                }
                else
                {
                    html += @$"<td>$0</td>";
                }
            }

            html += @$"
                <td>{sum.ToString("C", new CultureInfo("es-MX"))}</td>
                </tr>
                <tr>
                <td>Gallo</td>";
            sumTotal += sum;
            sum = 0;
            foreach (var item in sellers)
            {
                if (totalsByKey.TryGetValue($"{item}{WebSite.AguaAzul}", out double totalAmount))
                {
                    html += @$"<td>{totalAmount.ToString("C", new CultureInfo("es-MX"))}</td>";
                    sum += totalAmount;

                    if (!totalForDays.ContainsKey(item))
                    {
                        totalForDays.Add(item, totalAmount);
                    }
                    else
                    {
                        totalForDays[item] += totalAmount;
                    }
                }
                else
                {
                    html += @$"<td>$0</td>";
                }
            }
            html += @$"<td>{sum.ToString("C", new CultureInfo("es-MX"))}</td>
                </tr>
                <tr>
                <td>Punto Sur</td>";

            sumTotal += sum;
            sum = 0;
            foreach (var item in sellers)
            {
                if (totalsByKey.TryGetValue($"{item}{WebSite.PuntoSur}", out double totalAmount))
                {
                    html += @$"<td>{totalAmount.ToString("C", new CultureInfo("es-MX"))}</td>";
                    sum += totalAmount;

                    if (!totalForDays.ContainsKey(item))
                    {
                        totalForDays.Add(item, totalAmount);
                    }
                    else
                    {
                        totalForDays[item] += totalAmount;
                    }
                }
                else
                {
                    html += @$"<td>$0</td>";
                }
            }

            sumTotal += sum;
            html += @$"<td>{sum.ToString("C", new CultureInfo("es-MX"))}</td>
                </tr>
                <tr style='border-collapse: collapse;background-color:#ffdd01;width:100%;color:black'>
                <td>Total</td>";
                
                foreach (var item in sellers)
            {
                if (totalForDays.TryGetValue(item, out double totalAmount))
                {
                    html += @$"<td>{totalAmount.ToString("C", new CultureInfo("es-MX"))}</td>";
                }
                else
                {
                    html += @$"<td>$0</td>";
                }
            }

            html += @$"<td>{sumTotal.ToString("C", new CultureInfo("es-MX"))}</td>
                </tr>
                </table>
                <hr />
                <table style='border-collapse: collapse;background-color:black;width:100%;color:#ffdd01'>
                  <tr>
                      <td style='text-align:center;margin:0;padding:6px 0;'>Venta acumulada del mes de {today.ToString("MMMM", new CultureInfo("es-ES"))}</td>
                  </tr>
                </table>
                
                <table style='width:100%'>
                <tr style='border-collapse: collapse;background-color:#ffdd01;width:100%;color:black'>
                <td>{today.ToString("MMMM 'al' dd", new CultureInfo("es-ES"))}</td>";
                foreach (var item in sellers)
                {
                    html += @$"
                        <td>{item}</td>
                    ";
                }


            html += @$"
                <td>TOTAL</td>
                </tr>
                <tr>
                <td>Patria</td>";
            sumTotal = 0;
            sum = 0;
                foreach (var item in sellers)
            {
                if (totalMonthByKey.TryGetValue($"{item}{WebSite.Garage}", out double totalAmount))
                {
                    html += @$"<td>{totalAmount.ToString("C", new CultureInfo("es-MX"))}</td>";
                    sum += totalAmount;

                    if (!totalForMonths.ContainsKey(item))
                    {
                        totalForMonths.Add(item, totalAmount);
                    }
                    else
                    {
                        totalForMonths[item] += totalAmount;
                    }
                }
                else
                {
                    html += @$"<td>$0</td>";
                }
            }
            html += @$"
                <td>{sum.ToString("C", new CultureInfo("es-MX"))}</td>
                </tr>
                <tr>
                <td>Gallo</td>";

            sumTotal += sum;
            sum = 0;
            foreach (var item in sellers)
            {
                if (totalMonthByKey.TryGetValue($"{item}{WebSite.AguaAzul}", out double totalAmount))
                {
                    html += @$"<td>{totalAmount.ToString("C", new CultureInfo("es-MX"))}</td>";
                    sum += totalAmount;

                    if (!totalForMonths.ContainsKey(item))
                    {
                        totalForMonths.Add(item, totalAmount);
                    }
                    else
                    {
                        totalForMonths[item] += totalAmount;
                    }
                }
                else
                {
                    html += @$"<td>$0</td>";
                }
            }

            html += @$"<td>{sum.ToString("C", new CultureInfo("es-MX"))}</td>
                </tr>
                <tr>
                <td>Punto Sur</td>";

            sumTotal += sum;
            sum = 0;
            foreach (var item in sellers)
            {
                if (totalMonthByKey.TryGetValue($"{item}{WebSite.PuntoSur}", out double totalAmount))
                {
                    html += @$"<td>{totalAmount.ToString("C", new CultureInfo("es-MX"))}</td>";
                    sum += totalAmount;

                    if (!totalForMonths.ContainsKey(item))
                    {
                        totalForMonths.Add(item, totalAmount);
                    }
                    else
                    {
                        totalForMonths[item] += totalAmount;
                    }
                }
                else
                {
                    html += @$"<td>$0</td>";
                }
            }

            sumTotal += sum;
            html += @$"<td>{sum.ToString("C", new CultureInfo("es-MX"))}</td>
                </tr>
                <tr style='border-collapse: collapse;background-color:#ffdd01;width:100%;color:black'>
                <td>Total</td>";

            foreach (var item in sellers)
            {
                if (totalForMonths.TryGetValue(item, out double totalAmount))
                {
                    html += @$"<td>{totalAmount.ToString("C", new CultureInfo("es-MX"))}</td>";
                }
                else
                {
                    html += @$"<td>$0</td>";
                }
            }

            int month = DateTime.Now.Month;
            var objectives = new Dictionary<string, List<int>>();
            objectives.Add("José Daniel", [463667,526333,526333,589000,589000,589000,589000,463667,589000,589000,463667,589000]);
            objectives.Add("Cristian Sanchez", [742000,742000,742000,838000,838000,838000,838000,742000,838000,838000,646000,838000]);
            objectives.Add("Barbara Sandoval", [463667,526333,526333,589000,589000,589000,589000,463667,589000,589000,463667,589000]);
            
            var totalObjective = 0; //objective * sellers.Count();

            html += @$"<td>{sumTotal.ToString("C", new CultureInfo("es-Mx"))}</td>
                </tr>
                <tr>
                    <td>Objectivo</td>
                ";

            foreach (var item in sellers)
            {
                totalObjective += objectives[item][month - 1];
                html += @$"<td>{objectives[item][month - 1].ToString("C", new CultureInfo("es-MX"))}</td>";
            }

            html += @$"<td>{totalObjective.ToString("C", new CultureInfo("es-MX"))}</td>
                </tr>
                <tr>
                    <td>LOGRO</td>
            ";

            foreach (var item in sellers)
            {
                if (totalForMonths.TryGetValue(item, out double totalAmount))
                {
                    double porcentaje = (totalAmount / objectives[item][month - 1]) * 100;
                    int porcentajeEntero = (int)Math.Round(porcentaje);
                    html += @$"<td>{porcentajeEntero}%</td>";
                }
                else
                {
                    html += @$"<td>0%</td>";
                }
            }

            double totalPorcentaje = (sumTotal / totalObjective) * 100;
            int totalPorcentajeEntero = (int)Math.Round(totalPorcentaje);
            
            html += @$"<td>{totalPorcentajeEntero}%</td>
                </tr>
                </table>

            </body>
            </html>";

            return html;
        }
    }
}