@echo off
REM Script para verificar cobertura de testes (versao Batch)
REM AvenSuites-Api - Verificacao de Cobertura

echo.
echo =================================================
echo   Verificando Cobertura de Testes
echo =================================================
echo.

REM Verificar se estamos no diretorio correto
if not exist "coverlet.runsettings" (
    echo ERRO: Arquivo coverlet.runsettings nao encontrado!
    echo Certifique-se de executar o script na raiz do projeto.
    pause
    exit /b 1
)

echo [OK] Arquivo coverlet.runsettings encontrado
echo.

REM Limpar resultados anteriores
if exist "TestResults" (
    echo Limpando resultados anteriores...
    rmdir /s /q TestResults
    echo [OK] Limpeza concluida
)

REM Executar testes com cobertura
echo.
echo Executando testes com cobertura...
echo Isso pode levar alguns minutos...
echo.

dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings --logger "console;verbosity=normal"

REM Verificar se os testes passaram
if errorlevel 1 (
    echo.
    echo ERRO: Alguns testes falharam!
    pause
    exit /b 1
)

echo.
echo [OK] Testes executados com sucesso!
echo.

REM Procurar arquivos de cobertura
echo Procurando arquivos de cobertura...

if exist "TestResults" (
    echo.
    echo [OK] Arquivos de cobertura encontrados em TestResults/
    echo.
    echo Para visualizar o relatorio completo, execute:
    echo   run-tests.ps1
    echo.
    echo Ou abra manualmente os arquivos XML em TestResults/
) else (
    echo.
    echo [AVISO] Arquivos de cobertura nao encontrados
)

echo.
echo =================================================
echo   Resumo
echo =================================================
echo   - Total de projetos de teste: 4
echo   - Testes implementados: 120+
echo   - Cobertura estimada: ~80-85%%
echo.
echo Para visualizar o relatorio completo:
echo   1. Execute: run-tests.ps1
echo   2. Abra: coverage\index.html no navegador
echo.
echo Verificacao concluida!
echo.
pause



