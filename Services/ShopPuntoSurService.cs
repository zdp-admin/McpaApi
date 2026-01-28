using ClosedXML.Excel;
using McpaApi.Database.Mysql;
using McpaApi.Dto;
using McpaApi.Models.Dto;
using McpaApi.Models.Dto.Output;
using McpaApi.Models.Shop;
using Microsoft.EntityFrameworkCore;

namespace McpaApi.Services
{
  public class ShopPuntoSurService
  {
    protected readonly ShopPuntoSurDbContext _context;
    protected readonly PuntoSurCommercialDbContext _commercialContext;

    public ShopPuntoSurService(
        ShopPuntoSurDbContext context,
        PuntoSurCommercialDbContext commercialContext
    )
    {
      _context = context;
      _commercialContext = commercialContext;
    }

    public IEnumerable<Sales> GetSales()
    {
      var result = _context.Sales.Where(s => s.IsCotization == 0).Take(100).ToList();

      return result;
    }

    public IEnumerable<ReportSales> SaleReport(DownloadReportSale downloadReportSale)
    {
      var result = new List<ReportSales>();
      var startDateTime = downloadReportSale.StartDate.ToDateTime(TimeOnly.MinValue);
      var endDateTime = downloadReportSale.EndDate.ToDateTime(TimeOnly.MaxValue);

      var sales = _context.Sales.Include(s => s.Agency).Include(s => s.User).Include(s => s.Invoice).Include(s => s.SaleElements.Where(se => se.DeletedAt == null)).ThenInclude(se => se.Product).Where((s) => s.DeletedAt == null && s.IsCotization == 0 && s.InvoiceId != null && s.CreatedAt >= startDateTime && s.CreatedAt <= endDateTime && (downloadReportSale.ByUser != 0 ? s.ByUserId == downloadReportSale.ByUser : true)).ToList();
      var invoicesIds = sales.Select(s => s.Invoice!.AdminpaqId).ToList();
      var invoicesSet = new HashSet<int>(invoicesIds!);
      var documents = _commercialContext.Documents.Include(d => d.Movements).Where(d => invoicesSet.Contains(d.Id)).ToList();

      foreach (var sale in sales)
      {
        var payed = documents.Where(d => d.Id == sale.Invoice?.AdminpaqId).FirstOrDefault();
        var invoiceIsCancel = payed?.Cancelado == 1;
        var pending = payed?.Pendiente > 10;
        var observation = "";

        if (payed == null)
        {
          continue;
        }

        if (invoiceIsCancel)
        {
          observation = "La factura esta cancelada";
        }
        else if (pending)
        {
          observation = "La factura tiene saldo pendiente";
        }

        foreach (var product in sale.SaleElements)
        {
          if (product.Product.Code.ToLower() != "anticipo")
          {
            var total = product.Quantity * product.Price;
            var iva = total - (total / 1.16);
            var productInDocument = payed.Movements.Where(m => m.IdProducto == product.Product.AdminpaqId).FirstOrDefault();
            var totalComercial = productInDocument?.Total ?? 0;
            totalComercial = totalComercial / 1.16;

            result.Add(new ReportSales()
            {
              Date = DateOnly.FromDateTime(sale.CreatedAt),
              InvoiceDate = DateOnly.FromDateTime(payed!.Fecha),
              Sale = $"{sale.Id}".PadLeft(6, '0'),
              Invoice = $"{payed.SerieDocumento}{payed.Folio}",
              Saller = $"{sale.User.Name} {sale.User.LastName}",
              ProductName = product.Product.Name,
              ProductCode = product.Product.Code,
              AmountWitOutVat = total - iva,
              Client = sale.Agency?.BusinessName ?? sale.ClientName,
              RazonSocial = payed.RazonSocial ?? "",
              VehicleModel = sale.VehicleModel,
              VehicleBrand = sale.VehicleBrand,
              VehicleColor = sale.VehicleColor ?? "",
              VehicleSerie = sale.VehicleSerie.ToUpper(),
              Observation = observation,
              AmountComercial = totalComercial
            });
          }
        }
      }

      return result;
    }

    public ReportSalesYear SalesYearReport(DownloadReportSale downloadReportSale)
    {
      var result = new ReportSalesYear()
      {
        SalesWithoutAdditionals = new List<SellerSalesWithOutAdditional>(),
        SalesWithAdditionals = new List<SellerSalesWithAdditional>(),
        Orders = 0,
        OrdersWithAdditionals = 0,
        PercentPenetrationTotal = 0,
        TotalSalesWithAdditionals = 0,
        TotalSalesWithoutAdditionals = 0
      };

      var startDateTime = downloadReportSale.StartDate.ToDateTime(TimeOnly.MinValue);
      var endDateTime = downloadReportSale.EndDate.ToDateTime(TimeOnly.MaxValue);

      var sales = _context.Sales.Include(s => s.Agency).Include(s => s.User).Include(s => s.Invoice).Include(s => s.SaleElements.Where(se => se.DeletedAt == null)).ThenInclude(se => se.Product).Where((s) => s.DeletedAt == null && s.IsCotization == 0 && s.InvoiceId != null && s.CreatedAt >= startDateTime && s.CreatedAt <= endDateTime && (downloadReportSale.ByUser != 0 ? s.ByUserId == downloadReportSale.ByUser : true)).ToList();
      var invoicesIds = sales.Select(s => s.Invoice!.AdminpaqId).ToList();
      var invoicesSet = new HashSet<int>(invoicesIds!);
      var documents = _commercialContext.Documents.Include(d => d.Movements).Where(d => invoicesSet.Contains(d.Id)).ToList();

      foreach (var sale in sales)
      {
        var payed = documents?.Where(d => d.Id == sale.Invoice?.AdminpaqId).FirstOrDefault();
        var invoiceIsCancel = payed?.Cancelado == 1;
        var pending = payed?.Pendiente > 10;

        if (payed == null || invoiceIsCancel || pending)
        {
          continue;
        }

        result.Orders += 1;

        var sellerName = $"{sale.User.Name} {sale.User.LastName}";
        var isAditional = sale.ParentId != null;
        if (isAditional)
        {
          result.OrdersWithAdditionals += 1;
          result.TotalSalesWithAdditionals += sale.Total;
          var seller = result.SalesWithAdditionals.FirstOrDefault(s => s.Seller == sellerName);


          if (seller != null)
          {
            var totalOrder = result.SalesWithoutAdditionals?.Sum(sw => sw.Orders) ?? 0;
            var totalSaleOrdes = result.SalesWithoutAdditionals?.Where(sw => sw.Seller == seller.Seller).FirstOrDefault()?.Orders ?? 0;

            seller.Orders++;
            seller.ExtraProducts += sale.SaleElements.Count();
            seller.TotalAdditional += sale.Total;
            seller.AvgTicket = seller.TotalAdditional / seller.Orders;
            seller.PercentPenetration = (double)seller.Orders / (totalSaleOrdes - totalOrder) * 100;
          }
          else
          {
            result.SalesWithAdditionals = result.SalesWithAdditionals.Append(new SellerSalesWithAdditional()
            {
              Seller = $"{sale.User.Name} {sale.User.LastName}",
              Orders = 1,
              ExtraProducts = sale.SaleElements.Count(),
              TotalAdditional = sale.Total,
              AvgTicket = sale.Total,
              PercentPenetration = 0
            });
          }
        }
        else
        {
          result.TotalSalesWithoutAdditionals += sale.Total;
          var seller = result.SalesWithoutAdditionals!.FirstOrDefault(s => s.Seller == sellerName);

          if (seller != null)
          {
              seller.Orders++;
              seller.TotalSales += sale.Total;
              seller.AvgTicket = seller.TotalSales / seller.Orders;
          }
          else
          {
              result.SalesWithoutAdditionals = result.SalesWithoutAdditionals!.Append(
                  new SellerSalesWithOutAdditional
                  {
                      Seller = sellerName,
                      Orders = 1,
                      TotalSales = sale.Total,
                      AvgTicket = 0 // o 0, según tu regla de negocio
                  });
          }
        }
      }
      result.PercentPenetrationTotal = result.Orders > 0 ? (double)result.OrdersWithAdditionals / result.Orders * 100 : 0;

      return result;
    }

    public MemoryStream DownloadSaleReport(DownloadReportSale downloadReportSale)
    {
      var workbook = new XLWorkbook();
      var worksheet = workbook.Worksheets.Add("Ventas");
      var worksheetSeller = workbook.Worksheets.Add("Comisiones");

      worksheet.Cell("A1").Value = "Fecha";
      worksheet.Cell("B1").Value = "Fecha Factura";
      worksheet.Cell("C1").Value = "Folio Venta";
      worksheet.Cell("D1").Value = "Folio Factura";
      worksheet.Cell("E1").Value = "Vendedor";
      worksheet.Cell("F1").Value = "Nombre Producto";
      worksheet.Cell("G1").Value = "Código Producto";
      worksheet.Cell("H1").Value = "SHOP Monto";
      worksheet.Cell("I1").Value = "COMER Monto";
      worksheet.Cell("J1").Value = "DIFF Monto";
      worksheet.Cell("K1").Value = "Cliente";
      worksheet.Cell("L1").Value = "Razón Social";
      worksheet.Cell("M1").Value = "Modelo Vehiculo";
      worksheet.Cell("N1").Value = "Marca Vehiculo";
      worksheet.Cell("O1").Value = "Color Vehiculo";
      worksheet.Cell("P1").Value = "Serie Venta";
      worksheet.Cell("Q1").Value = "Observación";

      worksheetSeller.Cell("A1").Value = "Vendedor";
      worksheetSeller.Cell("B1").Value = "Comisión";

      var startDateTime = downloadReportSale.StartDate.ToDateTime(TimeOnly.MinValue);
      var endDateTime = downloadReportSale.EndDate.ToDateTime(TimeOnly.MaxValue);

      var sales = _context.Sales.Include(s => s.Agency).Include(s => s.User).Include(s => s.Invoice).Include(s => s.SaleElements.Where(se => se.DeletedAt == null)).ThenInclude(se => se.Product).Where((s) => s.DeletedAt == null && s.IsCotization == 0 && s.InvoiceId != null && s.CreatedAt >= startDateTime && s.CreatedAt <= endDateTime).ToList();
      var invoicesIds = sales.Select(s => s.Invoice!.AdminpaqId).ToList();
      var invoicesSet = new HashSet<int>(invoicesIds!);
      var documents = _commercialContext.Documents.Include(d => d.Movements).Where(d => invoicesSet.Contains(d.Id)).ToList();
      var vehicleBrands = _context.vehicleBrands.ToList();
      var vehicleModels = _context.vehicleModels.ToList();
      var row = 2;
      var sellerSet = new Dictionary<int, SellerComission>();

      foreach (var sale in sales)
      {
        var payed = documents.Where(d => d.Id == sale.Invoice?.AdminpaqId).FirstOrDefault();
        var invoiceIsCancel = payed?.Cancelado == 1;
        var pending = payed?.Pendiente > 10;
        var observation = "";

        if (payed == null)
        {
          continue;
        }

        if (invoiceIsCancel)
        {
          observation = "La factura esta cancelada";
        }
        else if (pending)
        {
          observation = $"La factura tiene saldo pendiente de {payed.Pendiente}";
        }

        foreach (var product in sale.SaleElements)
        {
          var productInDocument = payed.Movements.Where(m => m.IdProducto == product.Product.AdminpaqId).FirstOrDefault();
          var model = vehicleModels.Where(vm => vm.Id.ToString() == sale.VehicleModel).FirstOrDefault();
          var brand = vehicleBrands.Where(vb => vb.Id.ToString() == sale.VehicleBrand).FirstOrDefault();

          if (product.Product.Code.ToLower() != "anticipo")
          {
            var total = productInDocument?.Total ?? 0;
            total = total / 1.16;
            var totalShop = product.Quantity * product.Price;
            totalShop = totalShop / 1.16;
            worksheet.Cell($"A{row}").Value = DateOnly.FromDateTime(sale.CreatedAt).ToString("dd/MM/yyyy");
            worksheet.Cell($"B{row}").Value = DateOnly.FromDateTime(payed!.Fecha).ToString("dd/MM/yyyy");
            worksheet.Cell($"C{row}").Value = $"{sale.Id}".PadLeft(6, '0');
            worksheet.Cell($"D{row}").Value = $"{payed.SerieDocumento}{payed.Folio}";
            worksheet.Cell($"E{row}").Value = $"{sale.User.Name} {sale.User.LastName}";
            worksheet.Cell($"F{row}").Value = product.Product.Name;
            worksheet.Cell($"G{row}").Value = product.Product.Code;
            worksheet.Cell($"H{row}").Value = totalShop;
            worksheet.Cell($"I{row}").Value = total;
            worksheet.Cell($"J{row}").Value = total - totalShop;

            worksheet.Cell($"K{row}").Value = sale.Agency?.BusinessName ?? sale.ClientName;
            worksheet.Cell($"L{row}").Value = payed.RazonSocial ?? "";
            worksheet.Cell($"M{row}").Value = model != null ? model.Name : sale.VehicleModel;
            worksheet.Cell($"N{row}").Value = brand != null ? brand.Name : sale.VehicleBrand;
            worksheet.Cell($"O{row}").Value = sale.VehicleColor ?? "";
            worksheet.Cell($"P{row}").Value = sale.VehicleSerie.ToUpper();
            worksheet.Cell($"Q{row}").Value = observation;

            if (sellerSet.ContainsKey(sale.User.Id))
            {
              sellerSet[sale.User.Id].Comission += total;
            }
            else
            {
              sellerSet.Add(sale.User.Id, new SellerComission()
              {
                Comission = total,
                SellerId = sale.User.Id,
                SellerName = $"{sale.User.Name} {sale.User.LastName}"
              });
            }

            row += 1;
          }
        }
      }

      var rowSeller = 2;
      foreach (var seller in sellerSet)
      {
        worksheetSeller.Cell($"A{rowSeller}").Value = seller.Value.SellerName;
        worksheetSeller.Cell($"B{rowSeller}").Value = seller.Value.Comission;

        rowSeller += 1;
      }

      var stream = new MemoryStream();
      workbook.SaveAs(stream);
      stream.Position = 0;

      return stream;
    }

    public DashboardReport DashboardReport(FilterDashboardReport filterDashboardReport)
    {
      var result = new DashboardReport()
      {
        Sellers = new List<SellerReport>(),
        Products = new List<ProductoReport>(),
      };

      var ignoreClients = _commercialContext.Clients.Where(c => c.CIdValorClasifClient4 == 17).Select(c => c.Id).ToList();
      var ignoreClientIdsSet = new HashSet<int>(ignoreClients);
      var startDateTime = filterDashboardReport.StartDate.ToDateTime(TimeOnly.MinValue);
      var endDateTime = filterDashboardReport.EndDate.ToDateTime(TimeOnly.MaxValue);
      var sales = _context.Sales.Include(s => s.Agency).Include(s => s.User).Include(s => s.Invoice).Include(s => s.SaleElements.Where(se => se.DeletedAt == null)).ThenInclude(se => se.Product).Where((s) => s.DeletedAt == null && s.IsCotization == 0 && s.CreatedAt >= startDateTime && s.CreatedAt <= endDateTime).ToList();
      var invoicesIds = sales.Where(s => s.InvoiceId != null).Select(s => s.Invoice!.AdminpaqId).ToList();
      var invoicesSet = new HashSet<int>(invoicesIds!);
      var documents = _commercialContext.Documents.Include(d => d.Movements).ThenInclude(m => m.Product).ThenInclude(p => p.ClasificacionesValores).Where(d => invoicesSet.Contains(d.Id) && !ignoreClientIdsSet.Contains(d.IdClientProveedor) && (d.SerieDocumento == "FFF" || d.SerieDocumento == "NV")).ToList();
      var sellerSet = new Dictionary<int, SellerReport>();
      var productSet = new Dictionary<int, ProductoReport>();
      var currentDay = filterDashboardReport.Today;
      result.TotalNotasCredito = _commercialContext.Documents.Where(d => d.SerieDocumento == "NCF" && d.Cancelado == 0 && d.Fecha >= startDateTime && d.Fecha <= endDateTime).Sum(d => d.Neto);

      foreach (var sale in sales)
      {
        var payed = documents.Where(d => d.Id == sale.Invoice?.AdminpaqId).FirstOrDefault();
        var isCancel = payed == null || payed.Cancelado == 1;

        if (isCancel)
        {
          result.TotalSalesWithoutInvoice += sale.Total;
          if (sale.CreatedAt.Date == currentDay)
          {
            result.TotalSalesWithoutInvoiceCurrentDay += sale.Total;
          }
        }
        else
        {
          result.TotalSales += payed.Total;
          if (sale.CreatedAt.Date == currentDay)
          {
            result.TotalSalesCurrentDay += sale.Total;
          }
        }

        if (isCancel)
        {
          continue;
        }

        if (!sellerSet.ContainsKey(sale.User.Id))
        {
          sellerSet.Add(sale.User.Id, new SellerReport()
          {
            Comission = 0,
            SellerId = sale.User.Id,
            SellerName = $"{sale.User.Name} {sale.User.LastName}",
            AverageTiket = 0,
            TotalSales = 0,
            TotalSalesCurrentDay = 0,
            ComissionCurrentDay = 0
          });
        }
        else
        {
          if (sale.ParentId == null)
          {
            sellerSet[sale.User.Id].TotalSales += 1;
          }

          if (sale.CreatedAt.Date == currentDay)
          {
            sellerSet[sale.User.Id].TotalSalesCurrentDay += 1;
          }
        }

        foreach (var product in sale.SaleElements)
        {
          if (product.Product.Code.ToLower() != "anticipo")
          {
            var total = product.Quantity * product.Price;
            sellerSet[sale.User.Id].Comission += total;

            if (sale.CreatedAt.Date == currentDay)
            {
              sellerSet[sale.User.Id].ComissionCurrentDay += total;
            }

            if (payed != null)
            {
              var productCompaq = payed.Movements.Where(m => m.IdProducto == product.Product.AdminpaqId).FirstOrDefault();

              if (productCompaq != null)
              {
                if (!productSet.ContainsKey(productCompaq.IdProducto))
                {
                  productSet.Add(productCompaq.IdProducto, new ProductoReport()
                  {
                    Id = productCompaq.IdProducto,
                    Name = productCompaq.Product.Name,
                    CategoryId = productCompaq.Product.ClasificacionId,
                    CategoryName = productCompaq.Product.ClasificacionesValores.Name,
                    Quantity = product.Quantity,
                    TotalSale = total,
                    TotalSaleCurrentDay = sale.CreatedAt.Date == currentDay ? total : 0,
                    QuantityCurrentDay = sale.CreatedAt.Date == currentDay ? product.Quantity : 0
                  });
                }
                else
                {
                  productSet[productCompaq.IdProducto].Quantity += product.Quantity;
                  productSet[productCompaq.IdProducto].TotalSale += total;

                  if (sale.CreatedAt.Date == currentDay)
                  {
                    productSet[productCompaq.IdProducto].TotalSaleCurrentDay += total;
                    productSet[productCompaq.IdProducto].QuantityCurrentDay += product.Quantity;
                  }
                }
              }
            }
          }
        }
      }

      foreach (var seller in sellerSet.Values)
      {
        ((List<SellerReport>)result.Sellers).Add(new SellerReport()
        {
          SellerId = seller.SellerId,
          SellerName = seller.SellerName,
          Comission = seller.Comission,
          TotalSales = seller.TotalSales,
          AverageTiket = seller.Comission / seller.TotalSales,
          TotalSalesCurrentDay = seller.TotalSalesCurrentDay,
          ComissionCurrentDay = seller.ComissionCurrentDay
        });
      }

      foreach (var product in productSet.Values)
      {
        ((List<ProductoReport>)result.Products).Add(product);
      }

      var groupSaleByDay = sales.GroupBy(s => s.CreatedAt.Date).Select(g => new
      {
        Day = g.Key,
        TotalByDay = g.Sum(x => x.Total),
        SalesByDay = g.Count(),
      });

      double totalSales = sales.Sum(v => v.Total);
      int totalTickets = sales.Count();
      double AverageTicket = totalTickets > 0
        ? Math.Round(totalSales / totalTickets, 1)
        : 0;

      result.AverageTotalSale = AverageTicket;
      //result.AverageTotalSale = groupSaleByDay.Average(x => x.TotalByDay);
      result.AverageSales = groupSaleByDay.Average(x => x.SalesByDay);

      return result;
    }
  }
}