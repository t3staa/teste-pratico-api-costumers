![Logo da empresa](https://custec.com.br/wp-content/uploads/2018/09/Logo-custec-80x100-e1538002492448.png)

# Teste Prático – API de Gerenciamento de Clientes

Este repositório contém o template para o desenvolvimento de uma Web API em ASP NET Core. O objetivo é avaliar sua capacidade de projetar, implementar e documentar uma solução completa seguindo boas práticas de arquitetura, padrões de projeto e tratamento de erros.

---

## Como Submeter o Projeto

1. Faça um fork deste repositório na sua conta do GitHub.  

2. Após concluir sua implementação, conceda acesso de leitura ao fork para os e-mails dos revisores:  
   - msantos@custec.com.br 
   - jcoturi@custec.com.br  

3. Certifique-se de que o repositório forkado esteja público ou, se privado, que os revisores tenham permissão de leitura.  

---

## Teste

### Cenário

Você foi contratado para desenvolver uma API de gerenciamento de clientes. Cada cliente tem nome, e-mail e endereço. Em vez de solicitar todos os dados de endereço manualmente, sua API deve usar um serviço externo para preencher o logradouro, cidade e estado a partir do CEP informado.

### Requisitos Funcionais

- Criar um projeto ASP NET Core Web API (.NET 2.1 ou superior).  
- Implementar os seguintes endpoints no `CustomersController`:  
  - `GET /api/customers` – listar todos os clientes  
  - `GET /api/customers/{id}` – obter cliente por ID  
  - `POST /api/customers` – cadastrar novo cliente  
  - `PUT /api/customers/{id}` – atualizar cliente existente  
  - `DELETE /api/customers/{id}` – remover cliente  
- Definir a entidade `Customer` com estes campos:  
  - `Id` (long)  
  - `Name` (string, obrigatório)  
  - `Email` (string, obrigatório, formato válido)  
  - `Cep` (string, obrigatório, apenas dígitos)  
  - `Street` (string)  
  - `City` (string)  
  - `State` (string)  
- Ao cadastrar ou atualizar (`POST`/`PUT`), antes de salvar:  
  - Chamar o serviço externo ViaCEP: `https://viacep.com.br/ws/{cep}/json/`  
  - Se o CEP for válido, preencher `Street`, `City` e `State` com os dados retornados  
  - Em caso de CEP inválido ou erro de serviço, retornar 400 Bad Request com mensagem apropriada  

### Requisitos Técnicos

- Usar Entity Framework Core com banco em memória (InMemory) ou SQL Server local.  
- Implementar Repository + Service Pattern com injeção de dependência.  
- Todas as operações de banco e chamadas HTTP externas devem ser assíncronas (`async`/`await`).  
- Configurar `HttpClient` para consumir o ViaCEP via `IHttpClientFactory`.  
- Tratar erros globais via middleware (por exemplo, `ExceptionHandlerMiddleware`).  

### Entregáveis

- Repositório GitHub (público ou privado com acesso de leitura concedido).  
- Este README contendo:  
  - Instruções para clonar, restaurar pacotes e executar a API  
  - Exemplos de requisições (cURL ou Postman Collection)  
- Código-fonte organizado com commits claros e frequentes.  
- Documentação da API em Swagger/OpenAPI (opcional).  

---

Boa implementação e bons commits!