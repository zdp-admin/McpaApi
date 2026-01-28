using McpaApi.Models.Commercial;
using McpaApi.Models.Dto.Output;

namespace McpaApi.Models
{
    public interface ILealtadService
    {
        IEnumerable<ItemLealtad> SendReportLealtad();
        IEnumerable<Documents> GetDocuments();
        void UploadReportLealtad();
    }
}