# --- BAZAAR PLUGIN PROJECT CONFIGURATION ---
$projectRoot = Get-Location
$configFile = Join-Path $projectRoot "config.json"

# Function to open a modern folder picker
function Get-Folder() {
    Add-Type -AssemblyName System.Windows.Forms
    $FolderBrowser = New-Object System.Windows.Forms.FolderBrowserDialog
    $FolderBrowser.Description = "Select 'The Bazaar' installation folder`n(Usually in C:\Program Files (x86)\Steam\steamapps\common\The Bazaar)"
    $FolderBrowser.ShowNewFolderButton = $false
    
    # We use a simple ShowDialog without the broken NativeWindow logic
    # This will open the standard Windows folder picker
    $result = $FolderBrowser.ShowDialog()

    if ($result -eq [System.Windows.Forms.DialogResult]::OK) {
        return $FolderBrowser.SelectedPath
    }
    return $null
}

# 1. Check or ask for game location
if (Test-Path $configFile) {
    $config = Get-Content $configFile | ConvertFrom-Json
    $gamePath = $config.GamePath
} else {
    Write-Host "--- INITIAL CONFIGURATION ---" -ForegroundColor Cyan
    Write-Host "A window will open. Please select the folder where the game is installed." -ForegroundColor Yellow
    Write-Host "Example: C:\Program Files (x86)\Steam\steamapps\common\The Bazaar" -ForegroundColor Gray
    
    $gamePath = Get-Folder
    
    if ($gamePath) {
        Write-Host "Selected Path: $gamePath" -ForegroundColor Green
    } else {
        Write-Host "Selection cancelled. Setup aborted." -ForegroundColor Red
        pause
        exit
    }

    if (!(Test-Path $gamePath)) {
        Write-Host "Error: The specified path does not exist." -ForegroundColor Red
        pause
        exit
    }

    $config = @{ GamePath = $gamePath }
    $config | ConvertTo-Json | Set-Content $configFile
    Write-Host "Configuration saved to config.json" -ForegroundColor Green
}

$streamingAssets = Join-Path $gamePath "TheBazaar_Data\StreamingAssets\cards.json"
$pluginDir = Join-Path $gamePath "BepInEx\plugins"

# 2. Fetch cards.json from game folder
Write-Host "`n--- FETCHING GAME DATA ---" -ForegroundColor Cyan
if (Test-Path $streamingAssets) {
    Copy-Item $streamingAssets (Join-Path $projectRoot "data_source\cards.json") -Force
    Write-Host "cards.json successfully retrieved!" -ForegroundColor Green
} else {
    Write-Host "Warning: cards.json not found in game folder." -ForegroundColor Red
    Write-Host "Make sure you selected the ROOT folder of 'The Bazaar'." -ForegroundColor Yellow
}

# 3. Update Meta Data
Write-Host "`n--- UPDATING META DATA (JOTA) ---" -ForegroundColor Cyan
$python = "python"
& $python (Join-Path $projectRoot "scripts\download_meta_excel.py")
& $python (Join-Path $projectRoot "scripts\extract_xlsx_images.py")
& $python (Join-Path $projectRoot "scripts\clean_cards.py")
& $python (Join-Path $projectRoot "scripts\extract_meta.py")

# 4. Update build_and_deploy.ps1
Write-Host "`n--- UPDATING DEPLOYMENT SCRIPT ---" -ForegroundColor Cyan
$buildScript = Join-Path $projectRoot "build_and_deploy.ps1"
if (Test-Path $buildScript) {
    $content = Get-Content $buildScript
    # Create the exact string for the PowerShell variable assignment
    $newPathLine = '$destDir = "' + $gamePath + '\BepInEx\plugins"'
    # Regex to find the line starting with $destDir = and replace it entirely
    $newContent = $content -replace '^\$destDir = ".*"', $newPathLine
    $newContent | Set-Content $buildScript
    Write-Host "Deployment script updated with: $newPathLine" -ForegroundColor Green
} else {
    Write-Host "Error: build_and_deploy.ps1 not found in $projectRoot" -ForegroundColor Red
}

Write-Host "`n--- SETUP COMPLETE! ---" -ForegroundColor Cyan
Write-Host "Triggering first build and deployment..." -ForegroundColor Yellow
& (Join-Path $projectRoot "build_and_deploy.ps1")

Write-Host "`nAll done! You can now use 'build_and_deploy.ps1' for future updates." -ForegroundColor White
pause
