namespace Firmness.Application.Interfaces;


// Interface for Excel template generation service
public interface IExcelTemplateService
{
    // Generates a formatted Excel template for product bulk import
    byte[] GenerateProductTemplate();

    // Generates a formatted Excel template for vehicle bulk import
    byte[] GenerateVehicleTemplate();

    // Generates a formatted Excel template for customer bulk import
    byte[] GenerateCustomerTemplate();

    // Generates an Excel file with sample data (messy/unordered) to demonstrate import capabilities
    byte[] GenerateSampleDataFile();

    // Generates an Excel file with available categories reference
    byte[] GenerateCategoriesReference();
}
