namespace App.Services.Exporters;

public interface ITextExportService
{
    Task ExportToClipboardAsync(string text, CancellationToken cancellationToken);
    Task ExportToFileAsync(string text, string file, CancellationToken cancellationToken);
}