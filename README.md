# Sistema de Gestão de Inventário

Aplicação desktop em C# (`.NET 9.0` / Windows Forms) estruturada segundo o padrão **MVC (Model-View-Controller)** para gestão de artigos de inventário com persistência local em ficheiro JSON.

## Arquitetura

- **`Gestao/InventoryModel.cs`** (`Item` e `InventoryModel`): modelo de dados, validações de domínio, persistência em `inventory.json` via `System.Text.Json` (com recuperação segura em caso de ficheiro corrompido) e eventos `ItemAdded` / `ItemUpdated`.
- **`Gestao/InventoryController.cs`** (`InventoryController`): mediação entre a interface e o modelo, subscrição dos eventos do modelo, operações CRUD, pesquisa multi-campo e alertas de stock baixo.
- **`Gestao/view.cs`** (`IInventoryView` e `InventoryFormView`): interface gráfica Windows Forms com listagem em `DataGridView`, formulário de detalhes, pesquisa e filtro de stock baixo.
- **`Gestao/Program.cs`**: ponto de entrada da aplicação WinForms.

## Funcionalidades

- Adicionar artigos (com geração automática de `Guid`, validações de campos obrigatórios e valores não negativos).
- Atualizar a quantidade em stock de artigos selecionados.
- Remover artigos selecionados.
- Pesquisar por nome, descrição ou categoria (case-insensitive).
- Filtrar artigos com stock abaixo de um limite configurável.
- Registo de eventos (`ItemAdded`, `ItemUpdated`) na barra de estado e consola.

## Pré-requisitos

- Windows com [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (`net9.0-windows`).

## Como compilar e executar

```bash
dotnet build GestaoInventario.sln
dotnet run --project Gestao/GestaoInventario.csproj
```
