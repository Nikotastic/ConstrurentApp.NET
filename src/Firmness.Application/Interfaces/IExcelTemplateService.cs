namespace Firmness.Application.Services;

public interface IExcelTemplateService
{
    byte[] GenerateTemplate();
    byte[] GenerateSampleData();
    byte[] GenerateVehiclesSampleData();
}
