# Compilation
Write-Host "--- Compiling Solution ---" -ForegroundColor Cyan
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" "C:\Users\pauli\source\repos\BazaarPlugin\BazaarPlugin.slnx" /p:Configuration=Debug /v:m

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Paths
$sourceDll = "C:\Users\pauli\source\repos\BazaarPlugin\BazaarPlugin\bin\Debug\BazaarPlugin.dll"
$destDir = "E:\Programmes\Steam\steamapps\common\The Bazaar\BepInEx\plugins"
$dbDir = Join-Path $destDir "BazaarDB"
$metaDir = Join-Path $dbDir "meta"

$cleanCardsSource = "C:\Users\pauli\source\repos\BazaarPlugin\BazaarPlugin\BazaarDB\clean_cards.json"
$metaDataSource = "C:\Users\pauli\source\repos\BazaarPlugin\BazaarPlugin\BazaarDB\meta_data.json"
$metaImagesSource = "C:\Users\pauli\source\repos\BazaarPlugin\BazaarPlugin\BazaarDB\meta"
$mobsSource = "C:\Users\pauli\source\repos\BazaarPlugin\mobs"
$mobDestDir = Join-Path $destDir "mob"

# Deployment
Write-Host "--- Deploying to The Bazaar ---" -ForegroundColor Cyan

# Ensure directories exist
if (!(Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force }
if (!(Test-Path $dbDir)) { New-Item -ItemType Directory -Path $dbDir -Force }
if (!(Test-Path $metaDir)) { New-Item -ItemType Directory -Path $metaDir -Force }
if (!(Test-Path $mobDestDir)) { New-Item -ItemType Directory -Path $mobDestDir -Force }

Copy-Item $sourceDll $destDir -Force
Copy-Item $cleanCardsSource $dbDir -Force
Copy-Item $metaDataSource $dbDir -Force

# Copy Meta images (contents of meta folder)
if (Test-Path $metaImagesSource) {
    Copy-Item -Path "$metaImagesSource\*" -Destination $metaDir -Recurse -Force
}

# Copy Mob images (contents of mobs folder)
if (Test-Path $mobsSource) {
    Copy-Item -Path "$mobsSource\*" -Destination $mobDestDir -Recurse -Force
}

Write-Host "Deployment successful!" -ForegroundColor Green
