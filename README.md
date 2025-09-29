# API de Gerenciamento de Clientes

API RESTful para gerenciamento de clientes com integração automática com a API ViaCEP para busca de endereços por CEP.

## 🚀 Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - Para construção da API RESTful
- **Entity Framework Core 8.0.8** - ORM para acesso a dados
- **InMemory Database** - Banco de dados em memória para desenvolvimento/testes
- **Swagger/OpenAPI** - Documentação interativa da API
- **ViaCEP API** - Integração para busca automática de endereços

## 📦 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Editor de código (Visual Studio 2022, VS Code, Rider, etc.)

## 🔧 Como Executar o Projeto

### 1. Clone o repositório
```bash
git clone [url-do-repositorio]
cd teste-pratico-api-costumers
```

### 2. Restaure as dependências
```bash
dotnet restore
```

### 3. Compile o projeto
```bash
dotnet build
```

### 4. Execute a aplicação
```bash
dotnet run
```

### 5. Acesse o Swagger UI

Após executar o projeto, o Swagger **NÃO** abrirá automaticamente no navegador. Acesse manualmente:

- **HTTP:** http://localhost:5005/swagger/index.html
- **HTTPS:** https://localhost:7257/swagger/index.html

## 💾 Banco de Dados

Este projeto utiliza **Entity Framework Core com InMemory Database**.

### Características:
- **Temporário:** Os dados são perdidos quando a aplicação é encerrada
- **Ideal para Desenvolvimento:** Não requer instalação de banco de dados
- **Performance:** Extremamente rápido para operações CRUD
- **Dados Iniciais:** Inclui um cliente de exemplo (João Silva)

## 📡 Endpoints da API

### Base URL
```
http://localhost:5005/api/customers
```

### 1. **GET** `/api/customers`
Lista todos os clientes cadastrados.

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "João Silva",
    "email": "joao.silva@example.com",
    "cep": "01310100",
    "street": "Avenida Paulista",
    "city": "São Paulo",
    "state": "SP",
    "createdAt": "2025-01-28T10:00:00Z",
    "updatedAt": null
  }
]
```

### 2. **GET** `/api/customers/{id}`
Busca um cliente específico por ID.

**Response:** `200 OK` ou `404 Not Found`

### 3. **POST** `/api/customers`
Cria um novo cliente. O endereço é buscado automaticamente via ViaCEP.

**Request Body:**
```json
{
  "name": "Maria Santos",
  "email": "maria.santos@example.com",
  "cep": "01001000"
}
```

**Response:** `201 Created`

### 4. **PUT** `/api/customers/{id}`
Atualiza um cliente existente.

**Request Body:**
```json
{
  "name": "Maria Santos Silva",
  "email": "maria.silva@example.com",
  "cep": "20040020"
}
```

**Response:** `200 OK` ou `404 Not Found`

### 5. **DELETE** `/api/customers/{id}`
Remove um cliente.

**Response:** `204 No Content` ou `404 Not Found`

## 🧪 Exemplos de Requisições (cURL)

```bash
# Listar todos os clientes
curl -X GET "http://localhost:5005/api/customers"

# Buscar cliente por ID
curl -X GET "http://localhost:5005/api/customers/1"

# Criar novo cliente
curl -X POST "http://localhost:5005/api/customers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Teste Silva",
    "email": "teste@example.com",
    "cep": "01310100"
  }'

# Atualizar cliente
curl -X PUT "http://localhost:5005/api/customers/1" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva Atualizado",
    "email": "joao.novo@example.com",
    "cep": "20040020"
  }'

# Deletar cliente
curl -X DELETE "http://localhost:5005/api/customers/1"
```

## 🛠️ Estrutura do Projeto

```
teste-pratico-api-costumers/
├── Controllers/          # Endpoints da API
├── Models/              # Entidades e DTOs
├── Data/                # Contexto do EF Core
├── Repositories/        # Camada de acesso a dados
├── Services/            # Lógica de negócio e integração ViaCEP
├── Middleware/          # Tratamento global de erros
└── Program.cs           # Configuração da aplicação
```

## ✅ Requisitos Implementados

- ✅ ASP.NET Core Web API (.NET 8.0)
- ✅ Entity Framework Core com InMemory Database
- ✅ Repository + Service Pattern com injeção de dependência
- ✅ Operações assíncronas (async/await)
- ✅ HttpClient via IHttpClientFactory para ViaCEP
- ✅ Middleware de tratamento global de erros
- ✅ Documentação Swagger/OpenAPI
- ✅ Validação de dados de entrada
- ✅ Integração automática com ViaCEP

## ⚠️ Tratamento de Erros

A API retorna respostas padronizadas:
- **400 Bad Request**: Dados inválidos, CEP não encontrado
- **404 Not Found**: Cliente não encontrado
- **408 Request Timeout**: Timeout na chamada do ViaCEP
- **500 Internal Server Error**: Erro inesperado no servidor

---

**Teste Prático - API de Gerenciamento de Clientes**