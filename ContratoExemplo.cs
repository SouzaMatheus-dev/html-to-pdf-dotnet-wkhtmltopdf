namespace POC_Html_To_PDF;

/// <summary>
/// Templates HTML de exemplo para contrato: corpo com quebras de página,
/// cabeçalho e rodapé fixos (repetidos em cada página pelo wkhtmltopdf).
/// </summary>
public static class ContratoExemplo
{
    /// <summary>
    /// Cabeçalho fixo (HTML completo, autocontido). [page] e [topage] são substituídos pelo wkhtmltopdf.
    /// </summary>
    public static string GetHeaderHtml() => """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <style>
                body { margin: 0; padding: 0 15px; font-family: 'Segoe UI', Arial, sans-serif; font-size: 10px; color: #333; }
                .header { border-bottom: 2px solid #1a5276; padding-bottom: 6px; display: flex; justify-content: space-between; align-items: center; }
                .logo { font-weight: bold; color: #1a5276; }
                .doc-title { font-size: 11px; }
                .page-info { color: #666; }
            </style>
        </head>
        <body>
            <div class="header">
                <span class="logo">MINHA EMPRESA LTDA</span>
                <span class="doc-title">CONTRATO DE PRESTAÇÃO DE SERVIÇOS</span>
                <span class="page-info">Página [page] de [topage]</span>
            </div>
        </body>
        </html>
        """;

    /// <summary>
    /// Rodapé fixo (HTML completo, autocontido).
    /// </summary>
    public static string GetFooterHtml() => """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <style>
                body { margin: 0; padding: 0 15px; font-family: 'Segoe UI', Arial, sans-serif; font-size: 9px; color: #666; }
                .footer { border-top: 1px solid #ddd; padding-top: 6px; display: flex; justify-content: space-between; }
            </style>
        </head>
        <body>
            <div class="footer">
                <span>Documento gerado eletronicamente - Válido sem assinatura física</span>
                <span>Página [page] de [topage]</span>
            </div>
        </body>
        </html>
        """;

    /// <summary>
    /// Corpo do contrato com seções e quebras de página coordenadas (page-break-before / page-break-after).
    /// </summary>
    public static string GetBodyHtml() => """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <style>
                body { font-family: 'Segoe UI', Arial, sans-serif; font-size: 11px; line-height: 1.5; color: #222; }
                h1 { font-size: 16px; color: #1a5276; margin-bottom: 8px; }
                h2 { font-size: 13px; color: #1a5276; margin-top: 18px; margin-bottom: 8px; }
                p { margin: 8px 0; text-align: justify; }
                .clause { margin: 12px 0; }
                .page-break-before { page-break-before: always; }
                .page-break-after { page-break-after: always; }
                .keep-together { page-break-inside: avoid; }
                .signature-block { margin-top: 24px; page-break-inside: avoid; }
                ul { margin: 8px 0; padding-left: 24px; }
            </style>
        </head>
        <body>
            <h1>CONTRATO DE PRESTAÇÃO DE SERVIÇOS</h1>
            <p><strong>Contrato nº 2025/001</strong></p>
            <p>Pelo presente instrumento particular, de um lado <strong>CONTRATANTE</strong> e de outro <strong>CONTRATADA</strong>, têm entre si justo e contratado o seguinte:</p>

            <h2>1. DO OBJETO</h2>
            <div class="clause keep-together">
                <p>1.1. O presente contrato tem por objeto a prestação de serviços de consultoria em tecnologia da informação, conforme escopo definido em anexo.</p>
                <p>1.2. Os serviços serão executados no prazo de 12 (doze) meses, podendo ser prorrogado mediante termo aditivo.</p>
            </div>

            <h2>2. DO PRAZO E DO VALOR</h2>
            <div class="clause keep-together">
                <p>2.1. O contrato vigorará a partir da data de assinatura até 31/12/2025.</p>
                <p>2.2. O valor total dos serviços é de R$ 120.000,00 (cento e vinte mil reais), em 12 parcelas mensais de R$ 10.000,00.</p>
            </div>

            <div class="page-break-before"></div>
            <h2>3. DAS OBRIGAÇÕES DO CONTRATANTE</h2>
            <div class="clause keep-together">
                <p>3.1. São obrigações do CONTRATANTE:</p>
                <ul>
                    <li>Fornecer informações e documentos necessários à execução dos serviços;</li>
                    <li>Indicar interlocutor para acompanhamento;</li>
                    <li>Efetuar o pagamento nas datas acordadas.</li>
                </ul>
            </div>

            <h2>4. DAS OBRIGAÇÕES DA CONTRATADA</h2>
            <div class="clause keep-together">
                <p>4.1. São obrigações da CONTRATADA:</p>
                <ul>
                    <li>Executar os serviços com diligência e em conformidade com o escopo;</li>
                    <li>Manter sigilo sobre informações confidenciais;</li>
                    <li>Emitir relatórios mensais de acompanhamento.</li>
                </ul>
            </div>

            <div class="page-break-before"></div>
            <h2>5. CONDIÇÕES GERAIS</h2>
            <div class="clause keep-together">
                <p>5.1. O presente contrato é regido pelas leis brasileiras. Eventuais litígios serão submetidos ao foro da comarca do domicílio do CONTRATANTE.</p>
                <p>5.2. A rescisão antecipada poderá ocorrer por comum acordo ou nas hipóteses legais, com aviso prévio de 30 dias.</p>
            </div>

            <div class="signature-block keep-together">
                <p>E por estarem assim justos e contratados, assinam o presente em 2 vias de igual teor.</p>
                <p><strong>São Paulo, 27 de fevereiro de 2025.</strong></p>
                <p>_________________________________________<br>CONTRATANTE</p>
                <p>_________________________________________<br>CONTRATADA</p>
            </div>
        </body>
        </html>
        """;

    public static (string Body, string Header, string Footer) GetTemplates() =>
        (GetBodyHtml(), GetHeaderHtml(), GetFooterHtml());

    private static string EscapeHtml(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "-" : System.Net.WebUtility.HtmlEncode(value.Trim());

    /// <summary>
    /// Retorna o HTML do corpo do contrato com os placeholders substituídos pelos campos informados.
    /// </summary>
    public static string GetBodyHtmlPreenchido(ContratoCamposRequest campos)
    {
        var body = GetBodyHtmlComPlaceholders();
        body = body.Replace("{{NomeContratante}}", EscapeHtml(campos.NomeContratante));
        body = body.Replace("{{NomeContratada}}", EscapeHtml(campos.NomeContratada));
        body = body.Replace("{{EnderecoContratante}}", EscapeHtml(campos.EnderecoContratante));
        body = body.Replace("{{EnderecoContratada}}", EscapeHtml(campos.EnderecoContratada));
        body = body.Replace("{{CnpjContratante}}", EscapeHtml(campos.CnpjContratante));
        body = body.Replace("{{CnpjContratada}}", EscapeHtml(campos.CnpjContratada));
        body = body.Replace("{{NumeroContrato}}", EscapeHtml(campos.NumeroContrato));
        body = body.Replace("{{ValorTotal}}", EscapeHtml(campos.ValorTotal));
        body = body.Replace("{{DataContrato}}", EscapeHtml(campos.DataContrato));
        body = body.Replace("{{Cidade}}", EscapeHtml(campos.Cidade));
        body = body.Replace("{{DescricaoServico}}", EscapeHtml(campos.DescricaoServico));
        body = body.Replace("{{PrazoMeses}}", EscapeHtml(campos.PrazoMeses));
        return body;
    }

    /// <summary>
    /// Cabeçalho com nome da contratada (para personalizar).
    /// </summary>
    public static string GetHeaderHtmlComNome(string? nomeEmpresa)
    {
        var nome = string.IsNullOrWhiteSpace(nomeEmpresa) ? "MINHA EMPRESA LTDA" : EscapeHtml(nomeEmpresa);
        return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ margin: 0; padding: 0 15px; font-family: 'Segoe UI', Arial, sans-serif; font-size: 10px; color: #333; }}
        .header {{ border-bottom: 2px solid #1a5276; padding-bottom: 6px; display: flex; justify-content: space-between; align-items: center; }}
        .logo {{ font-weight: bold; color: #1a5276; }}
        .doc-title {{ font-size: 11px; }}
        .page-info {{ color: #666; }}
    </style>
</head>
<body>
    <div class=""header"">
        <span class=""logo"">{nome}</span>
        <span class=""doc-title"">CONTRATO DE PRESTAÇÃO DE SERVIÇOS</span>
        <span class=""page-info"">Página [page] de [topage]</span>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Corpo do contrato com placeholders para preenchimento ({{Campo}}).
    /// </summary>
    public static string GetBodyHtmlComPlaceholders() => """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <style>
                body { font-family: 'Segoe UI', Arial, sans-serif; font-size: 11px; line-height: 1.5; color: #222; }
                h1 { font-size: 16px; color: #1a5276; margin-bottom: 8px; }
                h2 { font-size: 13px; color: #1a5276; margin-top: 18px; margin-bottom: 8px; }
                p { margin: 8px 0; text-align: justify; }
                .clause { margin: 12px 0; }
                .page-break-before { page-break-before: always; }
                .keep-together { page-break-inside: avoid; }
                .signature-block { margin-top: 24px; page-break-inside: avoid; }
                ul { margin: 8px 0; padding-left: 24px; }
                .field { font-weight: bold; }
            </style>
        </head>
        <body>
            <h1>CONTRATO DE PRESTAÇÃO DE SERVIÇOS</h1>
            <p><strong>Contrato nº {{NumeroContrato}}</strong></p>
            <p>Pelo presente instrumento particular, de um lado <strong>CONTRATANTE</strong>: <span class="field">{{NomeContratante}}</span>, inscrito no CNPJ sob o nº {{CnpjContratante}}, com sede em {{EnderecoContratante}}; e de outro <strong>CONTRATADA</strong>: <span class="field">{{NomeContratada}}</span>, inscrita no CNPJ sob o nº {{CnpjContratada}}, com sede em {{EnderecoContratada}}; têm entre si justo e contratado o seguinte:</p>

            <h2>1. DO OBJETO</h2>
            <div class="clause keep-together">
                <p>1.1. O presente contrato tem por objeto {{DescricaoServico}}</p>
                <p>1.2. Os serviços serão executados no prazo de {{PrazoMeses}} meses, podendo ser prorrogado mediante termo aditivo.</p>
            </div>

            <h2>2. DO PRAZO E DO VALOR</h2>
            <div class="clause keep-together">
                <p>2.1. O contrato vigorará a partir da data de assinatura.</p>
                <p>2.2. O valor total dos serviços é de {{ValorTotal}}, conforme parcelas acordadas entre as partes.</p>
            </div>

            <div class="page-break-before"></div>
            <h2>3. DAS OBRIGAÇÕES DO CONTRATANTE</h2>
            <div class="clause keep-together">
                <p>3.1. São obrigações do CONTRATANTE:</p>
                <ul>
                    <li>Fornecer informações e documentos necessários à execução dos serviços;</li>
                    <li>Indicar interlocutor para acompanhamento;</li>
                    <li>Efetuar o pagamento nas datas acordadas.</li>
                </ul>
            </div>

            <h2>4. DAS OBRIGAÇÕES DA CONTRATADA</h2>
            <div class="clause keep-together">
                <p>4.1. São obrigações da CONTRATADA:</p>
                <ul>
                    <li>Executar os serviços com diligência e em conformidade com o escopo;</li>
                    <li>Manter sigilo sobre informações confidenciais;</li>
                    <li>Emitir relatórios mensais de acompanhamento.</li>
                </ul>
            </div>

            <div class="page-break-before"></div>
            <h2>5. CONDIÇÕES GERAIS</h2>
            <div class="clause keep-together">
                <p>5.1. O presente contrato é regido pelas leis brasileiras. Eventuais litígios serão submetidos ao foro da comarca de {{Cidade}}.</p>
                <p>5.2. A rescisão antecipada poderá ocorrer por comum acordo ou nas hipóteses legais, com aviso prévio de 30 dias.</p>
            </div>

            <div class="signature-block keep-together">
                <p>E por estarem assim justos e contratados, assinam o presente em 2 vias de igual teor.</p>
                <p><strong>{{Cidade}}, {{DataContrato}}.</strong></p>
                <p>_________________________________________<br>{{NomeContratante}}</p>
                <p>_________________________________________<br>{{NomeContratada}}</p>
            </div>
        </body>
        </html>
        """;
}
