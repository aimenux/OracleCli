using System.Diagnostics.CodeAnalysis;
using TextCopy;

namespace App.Services.Exporters;

[ExcludeFromCodeCoverage]
public class TextExportService : ITextExportService
{
    public async Task ExportToClipboardAsync(string text, CancellationToken cancellationToken)
    {
        await ClipboardService.SetTextAsync(text, cancellationToken);
    }

    public async Task ExportToFileAsync(string text, string file, CancellationToken cancellationToken)
    {
        await File.WriteAllTextAsync(file, text, cancellationToken);
    }
}