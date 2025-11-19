using Firmness.Application.Models;

namespace Firmness.Application.Interfaces;

public interface IBulkImportService
{
    Task<BulkImportResult> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default);
}

