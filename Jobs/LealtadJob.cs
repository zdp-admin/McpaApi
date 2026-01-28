using McpaApi.Models;
using McpaApi.Services;
using Quartz;

namespace McpaApi.Jobs
{
    public class LealtadJob : IJob
    {
        private readonly ILogger<ReportJob> _logger;
        private readonly ILealtadService _service;

        public LealtadJob(
            ILealtadService service,
            ILogger<ReportJob> logger
        )
        {
            _service = service;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Iniciado LealtadJob a las {Time}", DateTime.Now);

            try
            {
                _service.UploadReportLealtad();

                _logger.LogInformation("LealtadJob completado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en LealtadJob");
            }
            
            await Task.Delay(1000);
        }
    }
}