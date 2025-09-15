# Script para executar todos os testes com cobertura de código
# AvenSuites-Api - Testes Automatizados

Write-Host "🧪 Executando Testes Automatizados - AvenSuites-Api" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

# Limpar resultados anteriores
if (Test-Path "TestResults") {
    Remove-Item -Recurse -Force "TestResults"
}

if (Test-Path "coverage") {
    Remove-Item -Recurse -Force "coverage"
}

Write-Host "`n📦 Restaurando pacotes..." -ForegroundColor Yellow
dotnet restore

Write-Host "`n🔨 Compilando solução..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro na compilação!" -ForegroundColor Red
    exit 1
}

Write-Host "`n🧪 Executando testes unitários..." -ForegroundColor Green

# Executar testes com cobertura
dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage" --settings coverlet.runsettings --logger "console;verbosity=normal" --results-directory TestResults

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Alguns testes falharam!" -ForegroundColor Red
    exit 1
}

Write-Host "`n📊 Gerando relatório de cobertura..." -ForegroundColor Green

# Instalar reportgenerator se não estiver instalado
$reportGeneratorPath = "tools/reportgenerator"
if (-not (Test-Path $reportGeneratorPath)) {
    Write-Host "📦 Instalando ReportGenerator..." -ForegroundColor Yellow
    dotnet tool install --tool-path tools dotnet-reportgenerator-globaltool
}

# Gerar relatório HTML
& "$reportGeneratorPath/reportgenerator.exe" -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:"Html" -assemblyfilters:"-*.Tests"

Write-Host "`n✅ Testes concluídos com sucesso!" -ForegroundColor Green
Write-Host "📁 Relatório de cobertura: coverage/index.html" -ForegroundColor Cyan
Write-Host "📁 Resultados dos testes: TestResults/" -ForegroundColor Cyan

Write-Host "`n📈 Resumo dos Testes:" -ForegroundColor Yellow
Write-Host "===================" -ForegroundColor Yellow

# Contar testes por projeto
$testProjects = @(
    "tests/AvenSuites-Api.Domain.Tests",
    "tests/AvenSuites-Api.Application.Tests", 
    "tests/AvenSuites-Api.Infrastructure.Tests",
    "tests/AvenSuites-Api.IntegrationTests"
)

foreach ($project in $testProjects) {
    $projectName = Split-Path $project -Leaf
    Write-Host "🔍 $projectName" -ForegroundColor White
}

Write-Host "`n🎯 Para visualizar o relatório de cobertura:" -ForegroundColor Cyan
Write-Host "   Abra: coverage/index.html" -ForegroundColor White

Write-Host "`n🚀 Para executar testes específicos:" -ForegroundColor Cyan
Write-Host "   dotnet test tests/AvenSuites-Api.Domain.Tests" -ForegroundColor White
Write-Host "   dotnet test tests/AvenSuites-Api.Application.Tests" -ForegroundColor White
Write-Host "   dotnet test tests/AvenSuites-Api.Infrastructure.Tests" -ForegroundColor White
Write-Host "   dotnet test tests/AvenSuites-Api.IntegrationTests" -ForegroundColor White

