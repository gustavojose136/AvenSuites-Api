# 🧪 Testes Automatizados - AvenSuites-Api

Este diretório contém todos os testes automatizados para o projeto AvenSuites-Api, organizados por camadas da arquitetura DDD.

## 📁 Estrutura dos Testes

```
tests/
├── AvenSuites-Api.Domain.Tests/          # Testes das entidades do domínio
│   └── Entities/
│       ├── UserTests.cs
│       ├── RoleTests.cs
│       └── UserRoleTests.cs
├── AvenSuites-Api.Application.Tests/     # Testes dos serviços da aplicação
│   ├── Services/
│   │   ├── AuthServiceTests.cs
│   │   └── JwtServiceTests.cs
│   └── Utils/
│       └── Argon2PasswordHasherTests.cs
├── AvenSuites-Api.Infrastructure.Tests/ # Testes dos repositórios
│   └── Repositories/
│       ├── UserRepositoryTests.cs
│       └── RoleRepositoryTests.cs
└── AvenSuites-Api.IntegrationTests/      # Testes de integração da API
    └── Controllers/
        ├── AuthControllerIntegrationTests.cs
        └── UsersControllerIntegrationTests.cs
```

## 🚀 Como Executar os Testes

### Executar Todos os Testes
```bash
# Windows PowerShell
.\run-tests.ps1

# Linux/macOS
dotnet test --configuration Release --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### Executar Testes por Projeto
```bash
# Testes do Domínio
dotnet test tests/AvenSuites-Api.Domain.Tests

# Testes da Aplicação
dotnet test tests/AvenSuites-Api.Application.Tests

# Testes da Infraestrutura
dotnet test tests/AvenSuites-Api.Infrastructure.Tests

# Testes de Integração
dotnet test tests/AvenSuites-Api.IntegrationTests
```

### Executar Testes Específicos
```bash
# Por classe
dotnet test --filter "ClassName=UserTests"

# Por método
dotnet test --filter "MethodName=LoginAsync_WithValidCredentials_ShouldReturnLoginResponse"

# Por categoria
dotnet test --filter "Category=Integration"
```

## 📊 Cobertura de Código

### Gerar Relatório de Cobertura
```bash
# Instalar ReportGenerator
dotnet tool install --tool-path tools dotnet-reportgenerator-globaltool

# Gerar relatório HTML
tools/reportgenerator.exe -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:"Html"
```

### Visualizar Relatório
Abra o arquivo `coverage/index.html` no navegador para visualizar o relatório de cobertura.

## 🛠️ Tecnologias Utilizadas

- **xUnit**: Framework de testes
- **FluentAssertions**: Biblioteca para assertions mais legíveis
- **Moq**: Framework para mocking
- **Microsoft.EntityFrameworkCore.InMemory**: Banco em memória para testes
- **Microsoft.AspNetCore.Mvc.Testing**: Testes de integração para API
- **Coverlet**: Coleta de cobertura de código
- **ReportGenerator**: Geração de relatórios de cobertura

## 📋 Tipos de Testes

### 1. **Testes Unitários** (Domain.Tests)
- Testam as entidades do domínio isoladamente
- Verificam validações, comportamentos e regras de negócio
- Não dependem de recursos externos

### 2. **Testes de Serviços** (Application.Tests)
- Testam os serviços da camada de aplicação
- Utilizam mocks para dependências externas
- Verificam lógica de negócio e integração entre serviços

### 3. **Testes de Repositório** (Infrastructure.Tests)
- Testam a implementação dos repositórios
- Utilizam banco de dados em memória
- Verificam operações CRUD e consultas

### 4. **Testes de Integração** (IntegrationTests)
- Testam a API completa end-to-end
- Verificam autenticação, autorização e fluxos completos
- Utilizam banco de dados em memória para isolamento

## 🎯 Padrões de Teste

### Estrutura AAA (Arrange-Act-Assert)
```csharp
[Fact]
public void Method_WithCondition_ShouldReturnExpectedResult()
{
    // Arrange
    var input = "test";
    
    // Act
    var result = MethodUnderTest(input);
    
    // Assert
    result.Should().Be("expected");
}
```

### Nomenclatura Descritiva
- **Método**: `Method_WithCondition_ShouldReturnExpectedResult`
- **Classe**: `ClassNameTests`
- **Namespace**: `ProjectName.Tests.LayerName`

### Mocks e Stubs
```csharp
var mockRepository = new Mock<IUserRepository>();
mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
    .ReturnsAsync(user);
```

## 🔧 Configuração

### Arquivo de Configuração (coverlet.runsettings)
- Exclui projetos de teste da cobertura
- Configura formatos de saída
- Define filtros de exclusão

### Scripts de Automação
- `run-tests.ps1`: Script PowerShell para execução completa
- `.github/workflows/tests.yml`: Pipeline CI/CD

## 📈 Métricas de Qualidade

### Cobertura de Código
- **Meta**: > 80% de cobertura
- **Exclusões**: Migrations, Program.cs, Startup.cs
- **Formato**: HTML, Cobertura, LCOV

### Testes por Camada
- **Domain**: 100% das entidades
- **Application**: 100% dos serviços
- **Infrastructure**: 100% dos repositórios
- **API**: Principais endpoints e cenários

## 🚨 Troubleshooting

### Problemas Comuns

1. **Erro de Compilação**
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

2. **Testes Falhando**
   ```bash
   dotnet test --verbosity normal
   ```

3. **Problemas de Cobertura**
   ```bash
   dotnet tool update --tool-path tools dotnet-reportgenerator-globaltool
   ```

### Logs e Debug
```bash
# Executar com logs detalhados
dotnet test --logger "console;verbosity=detailed"

# Executar apenas testes que falharam
dotnet test --filter "FullyQualifiedName!~Integration"
```

## 📚 Recursos Adicionais

- [Documentação xUnit](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq](https://github.com/moq/moq4)
- [Coverlet](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator)

