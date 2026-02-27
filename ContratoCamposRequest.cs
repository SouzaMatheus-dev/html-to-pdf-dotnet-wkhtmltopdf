namespace POC_Html_To_PDF;

/// <summary>
/// Campos preenchíveis para geração do contrato em PDF.
/// </summary>
public record ContratoCamposRequest(
    string NomeContratante,
    string NomeContratada,
    string? EnderecoContratante = null,
    string? EnderecoContratada = null,
    string? CnpjContratante = null,
    string? CnpjContratada = null,
    string? NumeroContrato = null,
    string? ValorTotal = null,
    string? DataContrato = null,
    string? Cidade = null,
    string? DescricaoServico = null,
    string? PrazoMeses = null,
    string? FileName = null
);
