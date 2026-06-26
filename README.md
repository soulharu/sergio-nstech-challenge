# Orders API

API REST para gestão de Pedidos

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/)

---

## Como testar
No prompt de comanado na pasta do projeto:
```bash
dotnet test nstech-challenge.slnx
```

---

## Como rodar

### Via Docker

```bash
docker compose up
```
O banco de dados será inicializado e a API irá aguardar a confirmação de inicialização do banco para iniciar em seguida.

Após iniciar API estará disponível em **http://localhost:5000** e **https://localhost:5001** para https.  
O Swagger estará em **http://localhost:5000/swagger** e **http://localhost:5001/swagger** para https.

As migrations são aplicadas automaticamente na inicialização.

---


## Autenticação

Todos os endpoints de pedidos requerem o token JWT. 

Obtenha um token em:

```http
POST /auth/token
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

Usuários disponíveis (in-memory):

| Username | Password  | Role  |
|----------|-----------|-------|
| admin    | admin123  | Admin |
| user     | user123   | User  |

Use o token no header: `Authorization: Bearer {token}`
Ou via Swagger adicione o token obtido no formato `Bearer {token}` por meio do botão `Authorize` localizado na parte superior direita.


:warning: **Atenção!**

As rotas de confirmar e cancelar um pedido necessitam da role Admin. 

---

## Endpoints

| Método | Rota                      | Descrição                        |
|--------|---------------------------|----------------------------------|
| POST   | /auth/token               | Gerar JWT                        |
| POST   | /orders                   | Criar pedido                     |
| POST   | /orders/{id}/confirm      | Confirmar pedido                 |
| POST   | /orders/{id}/cancel       | Cancelar pedido                  |
| GET    | /orders/{id}              | Buscar pedido por ID             |
| GET    | /orders                   | Listar pedidos (paginado)        |

### Criar pedido — exemplo

```http
POST /orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "currency": "BRL",
  "items": [
    { "productId": "07f763d0-4acb-41c9-b4bc-23e3f53c73c9", "quantity": 2 },
    { "productId": "e6c0f08b-30f9-4e0e-9b4a-ecce3ca98f41", "quantity": 1 }
  ]
}
```

### Produtos disponíveis para teste (seed automático)

| ID                                   | Nome                                                                  | Preço    | Estoque |
|--------------------------------------|-----------------------------------------------------------------------|----------|---------|
| 07f763d0-4acb-41c9-b4bc-23e3f53c73c9 | Dados de RPG - Conjunto com 7 Dados Translúcidos - Transparent Blue   | 29.20    | 80      |
| e6c0f08b-30f9-4e0e-9b4a-ecce3ca98f41 | Kit de Pincéis Alkimya para Detalhamento - Kolinsky Pure Hair         | 419.90   | 10      |
| 23c87b01-2163-40a0-bf13-69d23164e994 | Lata de Verniz em Spray - Brilhante 300ml                             | 59.50    | 50      |
| 0ad91967-9d83-4137-851a-56329930d83b | Diário de anotações em Couro - Azul                                   | 65.00    | 10      |
| 247df04f-0a77-4677-a1a8-91362a6bb3e3 | Bandeja de Dados em Couro com Revestimento Aveludado - Preto/Vermelho | 42.30    | 40      |

---

## Decisões técnicas

**Resumo:**
- **Mediator** para CQRS — commands e queries bem delimitados, utilizando uma pipeline de validação com FluentValidation;
- **Utilização do pacote Mediator(sourcegenerator)**: escolha deliberada devido à melhor performance sobre o pacote MediatR;
- **Clean Code**: adição das regras de negocio em funções das classes de domínio, evitando classes de domínio anêmicas;
- **Idempotência**: implementada no próprio domínio (`Order.Confirm()` e `Order.Cancel()` são limitados caso o registro já se encontre no estado);
- **Índices criados com EF Core**: indexes criados nos campos `customer_id`, `status` e `created_at` para queries de listagem mais eficientes;
- **AsNoTracking**: utilizado em queries de leitura para melhor performance;
- **JWT com usuários fixos** (in-memory) para manter o foco no domínio;
- **Migrations aplicadas automaticamente**: durante a inicialização via `MigrateAsync()`;
- **Seed de produtos**: Adicionados 5 produtos automaticamente para facilitar testes;


