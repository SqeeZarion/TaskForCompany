Write-Host "Cleaning old TestResults and coverage report..." -ForegroundColor Yellow
Remove-Item -Recurse -Force ".\coveragereport" -ErrorAction SilentlyContinue

Write-Host "Running tests with coverage..." -ForegroundColor Cyan
dotnet test DogsHouseService.Tests/DogsHouseService.Tests.csproj --collect:"XPlat Code Coverage" | Out-Host

Start-Sleep -Seconds 2  # даємо часу collector-у записати coverage файл

Write-Host "Generating coverage report..." -ForegroundColor Green

# знаходимо найновіший coverage.cobertura.xml
$coverageFile = Get-ChildItem -Path ".\DogsHouseService.Tests\TestResults" -Recurse -Filter "coverage.cobertura.xml" `
    | Sort-Object LastWriteTime -Descending `
    | Select-Object -First 1

if ($coverageFile) {
    Write-Host "Found coverage file at: $($coverageFile.FullName)" -ForegroundColor Green
    reportgenerator -reports:$coverageFile.FullName -targetdir:"coveragereport"

    if (Test-Path "coveragereport/index.html") {
        Write-Host "Opening coverage report in browser..." -ForegroundColor Magenta
        start coveragereport/index.html
    } else {
        Write-Host "Report generation failed." -ForegroundColor Red
    }
} else {
    Write-Host "Could not find coverage.cobertura.xml file after tests!" -ForegroundColor Red
}
