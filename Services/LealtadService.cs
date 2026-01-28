using System.Net;
using System.Text;
using ClosedXML.Excel;
using McpaApi.Database.Mysql;
using McpaApi.Models;
using McpaApi.Models.Commercial;
using McpaApi.Models.Dto.Output;
using Microsoft.EntityFrameworkCore;

namespace McpaApi.Services
{
    public class LealtadService : ILealtadService
    {
        protected readonly ShopGarageDbContext _shopGarageContext;
        protected readonly ShopAguaAzulDbContext _shopAguaZulContext;
        protected readonly ShopPuntoSurDbContext _shopPuntoSurContext;
        protected readonly GarageCommercialDbContext _garageContext;
        protected readonly AguaZulCommercialDbContext _aguaAzulCommercialContext;
        protected readonly PuntoSurCommercialDbContext _puntoSurCommercialContext;

        public LealtadService(
            ShopGarageDbContext shopGarageContext,
            ShopAguaAzulDbContext shopAguaZulContext,
            ShopPuntoSurDbContext shopPuntoSurContext,
            GarageCommercialDbContext garageCommercialDbContext,
            AguaZulCommercialDbContext aguaAzulCommercialContext,
            PuntoSurCommercialDbContext puntoSurCommercialDbContext
        )
        {
            _shopAguaZulContext = shopAguaZulContext;
            _shopGarageContext = shopGarageContext;
            _shopPuntoSurContext = shopPuntoSurContext;
            _garageContext = garageCommercialDbContext;
            _aguaAzulCommercialContext = aguaAzulCommercialContext;
            _puntoSurCommercialContext = puntoSurCommercialDbContext;
        }

        public IEnumerable<ItemLealtad> SendReportLealtad()
        {
            var ignoreClients = _puntoSurCommercialContext.Clients.Where(c => c.CIdValorClasifClient4 == 17).Select(c => c.Id).ToList();
            var ignoreClientIdsSet = new HashSet<int>(ignoreClients);
            var today = DateTime.Today;
            var invoices = _puntoSurCommercialContext.Documents.Include(d => d.Movements).Where(d => d.Fecha.Date == today && !ignoreClientIdsSet.Contains(d.IdClientProveedor)).ToList();
            var invoiceIds = invoices.Select(s => s.Id).ToList();
            var invoiceIdsSet = new HashSet<int>(invoiceIds);

            var items = new List<ItemLealtad>();
            var saleGarage = _shopPuntoSurContext.Sales.Include(
                s => s.SaleElements.Where(se => se.DeletedAt == null)
            ).ThenInclude(se => se.Product)
            .Include(s => s.SaleElements.Where(se => se.DeletedAt == null))
            .ThenInclude(se => se.ServiceCommissionCode).Include(s => s.User)
            .Include(s => s.Invoice)
            .Where(
                s => s.InvoiceId != null &&
                (s.Invoice != null && invoiceIdsSet.Contains(s.Invoice!.AdminpaqId)) &&
                s.DeletedAt == null &&
                s.IsCotization == 0 &&
                s.VehicleSerie.Length > 4
            ).ToList();

            saleGarage = saleGarage.Where(s => s.VehicleBrandInt > 0 && s.VehicleModelInt > 0).ToList();
            var onlyVehicleBrandIds = saleGarage.Select(s => s.VehicleBrandInt).Distinct().ToList();
            var onlyVehicleModelIds = saleGarage.Select(s => s.VehicleModelInt).Distinct().ToList();
            var vehicleBrands = _shopPuntoSurContext.vehicleBrands.Where(v => onlyVehicleBrandIds.Contains(v.Id)).ToList();
            var vehicleModels = _shopPuntoSurContext.vehicleModels.Where(v => onlyVehicleModelIds.Contains(v.Id)).ToList();

            foreach (var sale in saleGarage)
            {
                var vehicleBrand = vehicleBrands.Find(v => v.Id == sale.VehicleBrandInt);
                var vehicleModel = vehicleModels.Find(v => v.Id == sale.VehicleModelInt);
                var invoice = invoices.Find(i => i.Id == sale.Invoice!.AdminpaqId);

                foreach (var product in sale.SaleElements)
                {
                    if (product.Product.Code.ToLower() != "anticipo") {
                        var moviment = invoice!.Movements.FirstOrDefault(m => m.IdProducto == product.Product.AdminpaqId);
                        items.Add(new ItemLealtad()
                        {
                            Amount = (moviment?.Precio ?? (product.Price / 1.16)) * (moviment?.Unidades ?? product.Quantity),
                            Category = product.ServiceCommissionCode.Code ?? "",
                            Concept = product.Product.Name,
                            Cellphone = (sale.Telephone ?? "").Replace("-", "").Replace(" ", "").Trim(),
                            Email = sale.Email ?? "",
                            Invoice = $"{sale.Invoice?.Serie ?? ""}{sale.Invoice?.Folio.ToString() ?? ""}",
                            Name = sale.ClientName,
                            Seller = $"{sale.User.Name} {sale.User.LastName}",
                            Sucursal = "PUNTO SUR",
                            VehiclePlates = sale.VehiclePlates ?? "",
                            VehicleBrand = vehicleBrand?.Name ?? "",
                            VehicleModel = vehicleModel?.Name ?? "",
                            VehicleVim = sale.VehicleSerie,
                            VehicleYear = sale.VehicleYear ?? 0,
                            Birthday = sale.Birthday?.ToString("dd/MM/yyyy") ?? ""
                        });
                    }
                }
            }

            return items;
        }

        public IEnumerable<Documents> GetDocuments()
        {
            var startDate = new DateTime(2025, 7, 1);
            var endDate = new DateTime(2025, 7, 6);
            var allInvoices = _garageContext.Documents
            .Include(d => d.Client)
            .Include(d => d.Movements)
            .ThenInclude(m => m.Product)
            .ThenInclude(p => p.ClasificacionesValores).Where(
                d =>
                d.Fecha >= startDate &&
                d.Fecha <= endDate &&
                d.Cancelado == 0 &&
                (d.SerieDocumento == "F" || d.SerieDocumento == "NV")
            ).Where(d => !Constant.CompaniesIgnore.Contains(d.Client.RFC)).ToList();

            return allInvoices;
        }

        public void UploadGarageReport()
        {
            var items = this.getReportLealtad(_garageContext.Clients, _garageContext.Documents, _shopGarageContext.Sales, _shopGarageContext.vehicleBrands, _shopGarageContext.vehicleModels);
            var file = this.generateFile(items);
            this.uploadFile("garagepatria", "Xk!1f9i1", file, "123_Garage290_Patria_Servicio");
        }

        public void UploadPuntoSurReport()
        {
            var items = this.getReportLealtad(_puntoSurCommercialContext.Clients, _puntoSurCommercialContext.Documents, _shopPuntoSurContext.Sales, _shopPuntoSurContext.vehicleBrands, _shopPuntoSurContext.vehicleModels);
            var file = this.generateFile(items);
            this.uploadFile("garagesur", "37ayk&E2", file, "123_Garage290_Sur_Servicio");
        }

        public void UploadAguaAzulReport()
        {
            var items = this.getReportLealtad(_aguaAzulCommercialContext.Clients, _aguaAzulCommercialContext.Documents, _shopAguaZulContext.Sales, _shopAguaZulContext.vehicleBrands, _shopAguaZulContext.vehicleModels);
            var file = this.generateFile(items);
            this.uploadFile("garagegallo", "L0eun22*", file, "123_Garage290_Gallo_Servicio");
        }
        
        public void UploadReportLealtad()
        {
            try
            {
                this.UploadGarageReport();
            }
            catch (Exception err)
            {
                Console.WriteLine($"Error al subir reporte de garage: {err.Message}", err);
            }

            try
            {
                this.UploadPuntoSurReport();
            }
            catch (Exception err)
            {
                Console.WriteLine($"Error al subir reporte de punto sur: {err.Message}", err);
            }
            
            try
            {
                this.UploadAguaAzulReport();
            } catch (Exception err)
            {
                Console.WriteLine($"Error al subir reporte de agua azul: {err.Message}", err);
            }
        }

        public void _UploadReportLealtad()
        {
            var ms = new MemoryStream();

            var writer = new StreamWriter(ms, new UTF8Encoding(true), 1024, leaveOpen: true);
            writer.WriteLine(string.Join(",", new[] {
                "NOMBRE", "TELEFONO_CASA", "PAIS", "ESTADO", "CIUDAD", "DIRECCION", "COLONIA", "CODIGO_POSTAL",
                "CELULAR", "EMAIL", "AÑO", "MARCA", "MODELO", "KILOMETRAJE", "VIN", "PLACAS", "ASESOR", "TIPO",
                "FACTURA", "MONTO", "CONCEPTO", "EMPRESA", "DEPARTAMENTO", "SUCURSAL", "CATEGORÍA", "FECHA CUMPLEAÑOS (dd/mm/aaaa)"
            }));

            var result = this.SendReportLealtad();

            foreach (var item in result)
            {
                var linea = string.Join(",", new[] {
                    Escape(item.Name),
                    Escape(""),
                    Escape("MX"),
                    Escape("JAL"),
                    Escape(""),
                    Escape(""),
                    Escape(""),
                    Escape(""),
                    Escape(item.Cellphone ?? ""),
                    Escape(item.Email),
                    Escape(item.VehicleYear.ToString()),
                    Escape(item.VehicleBrand),
                    Escape(item.VehicleModel),
                    Escape(""),
                    Escape(item.VehicleVim),
                    Escape(item.VehiclePlates),
                    Escape(item.Seller),
                    Escape("VENTAS NUEVOS"),
                    Escape(item.Invoice),
                    Escape(item.Amount.ToString()),
                    Escape(item.Concept),
                    Escape("123"),
                    Escape("3"),
                    Escape(item.Sucursal),
                    Escape(item.Category),
                    Escape(item.Birthday ?? "")
                });

                writer.WriteLine(linea);
            }

            writer.Flush();
            var file = ms.ToArray();

#pragma warning disable SYSLIB0014
            var request = (FtpWebRequest)WebRequest.Create("ftp://misrecompensas.com.mx/123_Garage290_Servicio.csv");
#pragma warning restore SYSLIB0014
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("garage290", "1p_q29Md");
            request.EnableSsl = false;      // Cambia a true si tu servidor requiere FTPS
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = false;
            request.ContentLength = file.Length;

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(file, 0, file.Length);
            }

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Estado de la subida: {response.StatusDescription}");
            }
        }

        private List<ItemLealtad> getReportLealtad(
            DbSet<Clients> clientsDb,
            DbSet<Documents> documentsDb,
            DbSet<Models.Shop.Sales> salesDb,
            DbSet<Models.Mysql.Shop.VehicleBrand> vehicleBrandsDb,
            DbSet<Models.Mysql.Shop.VehicleModel> vehicleModelsDb
        )
        {
            var ignoreClients = clientsDb.Where(c => c.CIdValorClasifClient4 == 17).Select(c => c.Id).ToList();
            var ignoreClientIdsSet = new HashSet<int>(ignoreClients);
            var today = DateTime.Today;
            var invoices = documentsDb.Include(d => d.Movements).Where(d => d.Fecha.Date == today && !ignoreClientIdsSet.Contains(d.IdClientProveedor)).ToList();
            var invoiceIds = invoices.Select(s => s.Id).ToList();
            var invoiceIdsSet = new HashSet<int>(invoiceIds);

            var items = new List<ItemLealtad>();
            var saleGarage = salesDb.Include(
                s => s.SaleElements.Where(se => se.DeletedAt == null)
            ).ThenInclude(se => se.Product)
            .Include(s => s.SaleElements.Where(se => se.DeletedAt == null))
            .ThenInclude(se => se.ServiceCommissionCode).Include(s => s.User)
            .Include(s => s.Invoice)
            .Where(
                s => s.InvoiceId != null &&
                (s.Invoice != null && invoiceIdsSet.Contains(s.Invoice!.AdminpaqId)) &&
                s.DeletedAt == null &&
                s.IsCotization == 0 &&
                s.VehicleSerie.Length > 4
            ).ToList();

            saleGarage = saleGarage.Where(s => s.VehicleBrandInt > 0 && s.VehicleModelInt > 0).ToList();
            var onlyVehicleBrandIds = saleGarage.Select(s => s.VehicleBrandInt).Distinct().ToList();
            var onlyVehicleModelIds = saleGarage.Select(s => s.VehicleModelInt).Distinct().ToList();
            var vehicleBrands = vehicleBrandsDb.Where(v => onlyVehicleBrandIds.Contains(v.Id)).ToList();
            var vehicleModels = vehicleModelsDb.Where(v => onlyVehicleModelIds.Contains(v.Id)).ToList();

            foreach (var sale in saleGarage)
            {
                var vehicleBrand = vehicleBrands.Find(v => v.Id == sale.VehicleBrandInt);
                var vehicleModel = vehicleModels.Find(v => v.Id == sale.VehicleModelInt);
                var invoice = invoices.Find(i => i.Id == sale.Invoice!.AdminpaqId);

                foreach (var product in sale.SaleElements)
                {
                    if (product.Product.Code.ToLower() != "anticipo") {
                        var moviment = invoice!.Movements.FirstOrDefault(m => m.IdProducto == product.Product.AdminpaqId);
                        items.Add(new ItemLealtad()
                        {
                            Amount = (moviment?.Precio ?? (product.Price / 1.16)) * (moviment?.Unidades ?? product.Quantity),
                            Category = product.ServiceCommissionCode.Code ?? "",
                            Concept = product.Product.Name,
                            Cellphone = (sale.Telephone ?? "").Replace("-", "").Replace(" ", "").Trim(),
                            Email = sale.Email ?? "",
                            Invoice = $"{sale.Invoice?.Serie ?? ""}{sale.Invoice?.Folio.ToString() ?? ""}",
                            Name = sale.ClientName,
                            Seller = $"{sale.User.Name} {sale.User.LastName}",
                            Sucursal = "",
                            VehiclePlates = sale.VehiclePlates ?? "",
                            VehicleBrand = vehicleBrand?.Name ?? "",
                            VehicleModel = vehicleModel?.Name ?? "",
                            VehicleVim = sale.VehicleSerie,
                            VehicleYear = sale.VehicleYear ?? 0,
                            Birthday = sale.Birthday?.ToString("dd/MM/yyyy") ?? ""
                        });
                    }
                }
            }

            return items;
        }

        private void uploadFile(string user, string password, byte[] file, string fileName)
        {
#pragma warning disable SYSLIB0014
            var request = (FtpWebRequest)WebRequest.Create($"ftp://misrecompensas.com.mx/{fileName}.csv");
#pragma warning restore SYSLIB0014
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(user, password);
            request.EnableSsl = false;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = false;
            request.ContentLength = file.Length;

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(file, 0, file.Length);
            }

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Estado de la subida({fileName}): {response.StatusDescription}");
            }
        }
        
        private byte[] generateFile(List<ItemLealtad> items)
        {
            var ms = new MemoryStream();

            var writer = new StreamWriter(ms, new UTF8Encoding(true), 1024, leaveOpen: true);
            writer.WriteLine(string.Join(",", new[] {
                "NOMBRE",
                "TELEFONO_CASA",
                "PAIS",
                "ESTADO",
                "CIUDAD",
                "DIRECCION",
                "COLONIA",
                "CODIGO_POSTAL",
                "CELULAR",
                "EMAIL",
                "AÑO",
                "MARCA",
                "MODELO",
                "KILOMETRAJE",
                "VIN",
                "PLACAS",
                "ASESOR",
                "TIPO",
                "FACTURA",
                "MONTO",
                "CONCEPTO",
                "EMPRESA",
                "DEPARTAMENTO",
                "SUCURSAL",
                "CATEGORÍA",
                "FECHA CUMPLEAÑOS (dd/mm/aaaa)"
            }));

            foreach (var item in items)
            {
                var linea = string.Join(",", new[] {
                    Escape(item.Name),
                    Escape(""),
                    Escape("MX"),
                    Escape("JAL"),
                    Escape(""),
                    Escape(""),
                    Escape(""),
                    Escape(""),
                    Escape(item.Cellphone ?? ""),
                    Escape(item.Email),
                    Escape(item.VehicleYear.ToString()),
                    Escape(item.VehicleBrand),
                    Escape(item.VehicleModel),
                    Escape(""),
                    Escape(item.VehicleVim),
                    Escape(item.VehiclePlates),
                    Escape(item.Seller),
                    Escape("VENTAS NUEVOS"),
                    Escape(item.Invoice),
                    Escape(item.Amount.ToString()),
                    Escape(item.Concept),
                    Escape(""), // empresa
                    Escape("3"),
                    Escape(item.Sucursal),
                    Escape(item.Category),
                    Escape(item.Birthday ?? "")
                });

                writer.WriteLine(linea);
            }

            writer.Flush();
            var file = ms.ToArray();

            return file;
        }

        private string Escape(string input)
        {
            if (string.IsNullOrEmpty(input)) return "\"\"";
            return $"\"{input.Replace("\"", "\"\"")}\"";
        }
    }
}