# POC: HTML para PDF com .NET 8 e wkhtmltopdf

Aplicação de demonstração para o time: geração de PDF a partir de HTML com **cabeçalho e rodapé fixos** e **quebras de página coordenadas**, usando .NET 8 e wkhtmltopdf em container Alpine.

## Funcionalidades

- **Cabeçalho fixo**: repetido em todas as páginas (HTML separado, com `[page]` e `[topage]`).
- **Rodapé fixo**: idem, com numeração de páginas.
- **Quebras de página**: uso de `page-break-before`, `page-break-after` e `page-break-inside: avoid` no CSS do corpo para controlar onde a página quebra.
- **API REST**: endpoint que recebe HTML (corpo, cabeçalho, rodapé) e devolve o PDF.
- **Contrato com campos preenchíveis**: endpoint que recebe nome, endereço, CNPJ, etc., e gera o PDF do contrato já preenchido.

## Como rodar

### Com Docker (recomendado para apresentação)

Na pasta do projeto:

```bash
docker build -t poc-html-to-pdf .
docker run -p 8080:8080 --name poc-html-to-pdf poc-html-to-pdf
```

O `--name poc-html-to-pdf` deixa o nome do container fixo na lista do Docker. Se já existir um container com esse nome (por exemplo de uma execução anterior), remova antes com `docker rm poc-html-to-pdf` ou use outro nome.

- **PDF de exemplo**: abra no navegador  
  `http://localhost:8080/api/pdf/exemplo`  
  (baixa o PDF do contrato de exemplo com 3 páginas, cabeçalho e rodapé em todas).

- **Swagger**: `http://localhost:8080/swagger`  
  Para testar o `POST /api/pdf/gerar` enviando seu próprio HTML.

### Local (com wkhtmltopdf instalado)

1. Instale o [wkhtmltopdf](https://wkhtmltopdf.org/) e deixe no PATH.
2. Restaure e execute:

```bash
dotnet restore
dotnet run
```

A API sobe em `http://localhost:5000` (ou na porta configurada). Use `/api/pdf/exemplo` e `/swagger` como acima, ajustando a porta se necessário.

## Endpoints

| Método | URL | Descrição |
|--------|-----|-----------|
| GET | `/api/pdf/exemplo` | Gera e baixa um PDF de contrato de exemplo (cabeçalho/rodapé fixos, quebras de página). |
| POST | `/api/pdf/gerar` | Recebe JSON com `HtmlBody`, opcionalmente `HeaderHtml`, `FooterHtml` e `FileName`; retorna o PDF. |
| POST | `/api/pdf/gerar-contrato` | Recebe JSON com campos do contrato (nome, endereço, CNPJ, etc.) e retorna o PDF preenchido. |
| GET | `/health` | Health check. |

### Exemplo de body para `POST /api/pdf/gerar`

```json
{
  "htmlBody": "<html><body><h1>Meu contrato</h1><p>Conteúdo...</p></body></html>",
  "headerHtml": "<html><body><div>Minha Empresa - Página [page] de [topage]</div></body></html>",
  "footerHtml": "<html><body><div>Rodapé - Página [page] de [topage]</div></body></html>",
  "fileName": "meu-documento.pdf"
}
```

As variáveis `[page]` e `[topage]` nos HTMLs de cabeçalho/rodapé são substituídas pelo wkhtmltopdf pela página atual e total de páginas.

### Campos para `POST /api/pdf/gerar-contrato`

Envie um JSON com os campos a serem preenchidos no contrato. Obrigatórios: `nomeContratante`, `nomeContratada`. Os demais são opcionais (aparecem como "-" se não informados).

| Campo | Descrição |
|-------|-----------|
| `nomeContratante` | Nome ou razão social do contratante |
| `nomeContratada` | Nome ou razão social da contratada |
| `enderecoContratante` | Endereço completo do contratante |
| `enderecoContratada` | Endereço completo da contratada |
| `cnpjContratante` | CNPJ do contratante |
| `cnpjContratada` | CNPJ da contratada |
| `numeroContrato` | Número do contrato (ex.: 2025/001) |
| `valorTotal` | Valor total (ex.: R$ 120.000,00) |
| `dataContrato` | Data (ex.: 27 de fevereiro de 2025) |
| `cidade` | Cidade (foro e data) |
| `descricaoServico` | Descrição do objeto do contrato |
| `prazoMeses` | Prazo em meses (ex.: 12) |
| `fileName` | Nome do arquivo PDF gerado (opcional) |

Exemplo de body:

```json
{
  "nomeContratante": "Empresa Cliente SA",
  "nomeContratada": "Consultoria Tech Ltda",
  "enderecoContratante": "Av. Paulista, 1000 - São Paulo/SP",
  "enderecoContratada": "Rua das Flores, 50 - São Paulo/SP",
  "cnpjContratante": "00.000.000/0001-00",
  "cnpjContratada": "11.111.111/0001-11",
  "numeroContrato": "2025/042",
  "valorTotal": "R$ 150.000,00 (cento e cinquenta mil reais)",
  "dataContrato": "27 de fevereiro de 2025",
  "cidade": "São Paulo",
  "descricaoServico": "a prestação de serviços de consultoria em tecnologia da informação, conforme escopo em anexo",
  "prazoMeses": "12",
  "fileName": "contrato-2025-042.pdf"
}
```

## Quebras de página no HTML do corpo

No CSS do seu HTML use:

- `page-break-before: always` – força nova página **antes** do elemento.
- `page-break-after: always` – força nova página **depois** do elemento.
- `page-break-inside: avoid` – evita quebrar **dentro** do elemento (ex.: manter um bloco de assinatura na mesma página).

No exemplo em `ContratoExemplo.cs` há um contrato com seções que começam em nova página e blocos que permanecem juntos.

## Estrutura do projeto

- `Program.cs` – endpoints e configuração (Swagger, health).
- `PdfService.cs` – chama o binário `wkhtmltopdf` com arquivos temporários (body, header, footer) e devolve o PDF em bytes.
- `ContratoExemplo.cs` – templates HTML de demonstração (cabeçalho, rodapé, corpo com quebras de página).
- `Dockerfile` – base Alpine com aspnet:8.0 e wkhtmltopdf (surnet), timezone São Paulo, fontes e ICU.

## Observações

- O wkhtmltopdf espera **HTML completo** (com `<!DOCTYPE html>`, `<head>`, `<body>`) nos arquivos de cabeçalho e rodapé.
- Para produção, considere validar/sanitizar o HTML recebido e limitar tamanho e tempo de processamento.
- Em ambiente Linux/Alpine, fontes adicionais podem ser instaladas com `apk add` e `fc-cache -f -v` se precisar de caracteres ou idiomas específicos.
