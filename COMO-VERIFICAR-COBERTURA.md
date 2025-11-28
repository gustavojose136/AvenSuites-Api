# 📊 Como Verificar a Cobertura de Testes

## 🚀 Métodos para Verificar a Cobertura

### Método 1: Comando Direto (Mais Simples - Recomendado)

Abra o **CMD** ou **PowerShell** e execute diretamente:

```bash
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

Este é o método mais simples e funciona sempre!

### Método 2: Script Batch (Windows)

Se você não conseguir executar scripts PowerShell, use o arquivo `.bat`:

**Clique duas vezes** no arquivo `verificar-cobertura.bat` ou execute no CMD:

```cmd
verificar-cobertura.bat
```

### Método 3: Script PowerShell (Se funcionar)

Execute o script `verificar-cobertura.ps1`:

```powershell
.\verificar-cobertura.ps1
```

**Se o script não funcionar**, tente:

```powershell
powershell -ExecutionPolicy Bypass -File .\verificar-cobertura.ps1
```

### Método 4: Script Completo (com relatório HTML)

Execute o script `run-tests.ps1` que gera um relatório HTML completo:

```powershell
.\run-tests.ps1
```

Depois, abra o arquivo `coverage/index.html` no navegador.


## 🔍 Onde Encontrar os Resultados

Após executar os testes, os arquivos de cobertura estarão em:

- **XML de cobertura**: `TestResults/**/coverage.cobertura.xml`
- **Relatório HTML** (após executar `run-tests.ps1`): `coverage/index.html`

## 📈 Interpretando os Resultados

### Cobertura de Código

- **80% ou mais**: ✅ Meta atingida!
- **Menos de 80%**: ⚠️ Ainda faltam testes

### O que é medido?

- **Line Coverage**: Porcentagem de linhas de código executadas pelos testes
- **Branch Coverage**: Porcentagem de branches (if/else, switch) testados
- **Method Coverage**: Porcentagem de métodos testados

## 🛠️ Solução de Problemas

### Problema: Script não executa

**Solução 1**: Verificar política de execução
```powershell
Get-ExecutionPolicy
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Solução 2**: Executar com bypass
```powershell
powershell -ExecutionPolicy Bypass -File .\verificar-cobertura.ps1
```

### Problema: Arquivos de cobertura não encontrados

1. Verifique se os testes foram executados com sucesso
2. Verifique se o arquivo `coverlet.runsettings` existe na raiz do projeto
3. Execute `dotnet test` primeiro para verificar se há erros

### Problema: Cobertura não aparece

1. Certifique-se de que está executando na raiz do projeto
2. Verifique se todos os projetos de teste compilam corretamente
3. Execute `dotnet build` antes de executar os testes

## 📝 Comandos Úteis

### Executar apenas testes específicos
```powershell
dotnet test --filter "ClassName=GuestServiceTests"
```

### Executar com mais detalhes
```powershell
dotnet test --logger "console;verbosity=detailed"
```

### Limpar e executar novamente
```powershell
Remove-Item -Recurse -Force TestResults
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

## ✅ Checklist de Verificação

- [ ] Arquivo `coverlet.runsettings` existe na raiz
- [ ] Todos os projetos de teste compilam (`dotnet build`)
- [ ] Testes executam sem erros (`dotnet test`)
- [ ] Arquivos de cobertura são gerados em `TestResults/`
- [ ] Relatório HTML é gerado em `coverage/index.html` (após `run-tests.ps1`)

## 🎯 Meta de Cobertura

**Meta atual**: 80% de cobertura de código

**Status**: ✅ ~80-85% (estimado)

Para aumentar a cobertura, adicione testes para:
- Controllers que ainda não têm testes
- Repositórios que ainda não têm testes
- Edge cases e cenários de erro

