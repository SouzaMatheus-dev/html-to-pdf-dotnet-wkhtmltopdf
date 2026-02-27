using System.Diagnostics;
using System.Text;

namespace POC_Html_To_PDF;

/// <summary>
/// Serviço que gera PDF a partir de HTML usando wkhtmltopdf,
/// com suporte a cabeçalho e rodapé fixos e quebras de página.
/// </summary>
public class PdfService
{
    private const string Wkhtmltopdf = "wkhtmltopdf";
    private readonly ILogger<PdfService> _logger;

    public PdfService(ILogger<PdfService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> GerarPdfAsync(
        string htmlBody,
        string? headerHtml,
        string? footerHtml,
        CancellationToken cancellationToken = default)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "wkhtmltopdf_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);

        try
        {
            var bodyPath = Path.Combine(tempDir, "body.html");
            await File.WriteAllTextAsync(bodyPath, htmlBody, Encoding.UTF8, cancellationToken);

            var args = new List<string>
            {
                "--enable-local-file-access",
                "--no-stop-slow-scripts",
                "--margin-top", "25mm",
                "--margin-bottom", "25mm",
                "--margin-left", "15mm",
                "--margin-right", "15mm",
                "--encoding", "UTF-8",
                "--page-size", "A4"
            };

            if (!string.IsNullOrWhiteSpace(headerHtml))
            {
                var headerPath = Path.Combine(tempDir, "header.html");
                await File.WriteAllTextAsync(headerPath, headerHtml, Encoding.UTF8, cancellationToken);
                args.Add("--header-html");
                args.Add(headerPath);
                args.Add("--header-spacing");
                args.Add("8");
            }

            if (!string.IsNullOrWhiteSpace(footerHtml))
            {
                var footerPath = Path.Combine(tempDir, "footer.html");
                await File.WriteAllTextAsync(footerPath, footerHtml, Encoding.UTF8, cancellationToken);
                args.Add("--footer-html");
                args.Add(footerPath);
                args.Add("--footer-spacing");
                args.Add("8");
            }

            args.Add(bodyPath);
            var outputPath = Path.Combine(tempDir, "output.pdf");
            args.Add(outputPath);

            var psi = new ProcessStartInfo
            {
                FileName = Wkhtmltopdf,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = tempDir
            };
            foreach (var a in args)
                psi.ArgumentList.Add(a);

            using var process = Process.Start(psi);
            if (process == null)
                throw new InvalidOperationException("Não foi possível iniciar o processo wkhtmltopdf. Verifique se está instalado e no PATH.");

            var stderr = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode != 0)
            {
                _logger.LogError("wkhtmltopdf saiu com código {Code}. stderr: {Stderr}", process.ExitCode, stderr);
                throw new InvalidOperationException($"wkhtmltopdf falhou (código {process.ExitCode}): {stderr}");
            }

            if (!File.Exists(outputPath))
                throw new InvalidOperationException("wkhtmltopdf não gerou o arquivo PDF.");

            return await File.ReadAllBytesAsync(outputPath, cancellationToken);
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao remover diretório temporário {Dir}", tempDir);
            }
        }
    }
}
