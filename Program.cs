using POC_Html_To_PDF;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<PdfService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "POC Html to PDF", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || true) // Swagger sempre disponível para demonstração
    app.UseSwagger().UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "POC-Html-to-PDF" }));

// Gera PDF a partir de HTML com cabeçalho e rodapé fixos
app.MapPost("/api/pdf/gerar", async (PdfRequest request, PdfService pdfService, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.HtmlBody))
        return Results.BadRequest("HtmlBody é obrigatório.");

    try
    {
        var pdfBytes = await pdfService.GerarPdfAsync(
            request.HtmlBody,
            request.HeaderHtml,
            request.FooterHtml,
            ct);

        return Results.File(pdfBytes, "application/pdf", request.FileName ?? "contrato.pdf");
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

// Endpoint de demonstração: retorna PDF de contrato de exemplo
app.MapGet("/api/pdf/exemplo", async (PdfService pdfService, CancellationToken ct) =>
{
    var (body, header, footer) = ContratoExemplo.GetTemplates();
    var pdfBytes = await pdfService.GerarPdfAsync(body, header, footer, ct);
    return Results.File(pdfBytes, "application/pdf", "contrato-exemplo.pdf");
});

// Gera PDF do contrato preenchendo os campos enviados (nome, endereço, CNPJ, etc.)
app.MapPost("/api/pdf/gerar-contrato", async (ContratoCamposRequest request, PdfService pdfService, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.NomeContratante))
        return Results.BadRequest("NomeContratante é obrigatório.");
    if (string.IsNullOrWhiteSpace(request.NomeContratada))
        return Results.BadRequest("NomeContratada é obrigatório.");

    try
    {
        var body = ContratoExemplo.GetBodyHtmlPreenchido(request);
        var header = ContratoExemplo.GetHeaderHtmlComNome(request.NomeContratada);
        var footer = ContratoExemplo.GetFooterHtml();
        var pdfBytes = await pdfService.GerarPdfAsync(body, header, footer, ct);
        return Results.File(pdfBytes, "application/pdf", request.FileName ?? "contrato.pdf");
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

app.Run();

/// <summary>
/// Request para geração de PDF com corpo, cabeçalho e rodapé em HTML.
/// </summary>
public record PdfRequest(
    string HtmlBody,
    string? HeaderHtml = null,
    string? FooterHtml = null,
    string? FileName = null);
