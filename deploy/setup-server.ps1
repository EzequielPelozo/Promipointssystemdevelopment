#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Deploys PromiPoints (frontend + backend) as IIS Virtual Applications
    under the existing port-80 site on the server.
    Run from the directory that contains this script.

.EXAMPLE
    Set-ExecutionPolicy RemoteSigned -Scope Process
    .\setup-server.ps1
#>

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# ── Edit these if the server uses a different physical root ──────────────────
$FrontendDest = 'C:\PSS\PromiPoints'
$BackendDest  = 'C:\PSS\PromiPointsAPI'
# ─────────────────────────────────────────────────────────────────────────────

$FrontendSrc = Join-Path $ScriptDir 'frontend'
$BackendSrc  = Join-Path $ScriptDir 'backend'

Write-Host '========================================' -ForegroundColor Cyan
Write-Host ' PromiPoints – Server Setup             ' -ForegroundColor Cyan
Write-Host '========================================' -ForegroundColor Cyan

# ──────────────────────────────────────────────────────────────────────────────
# 1. IIS
# ──────────────────────────────────────────────────────────────────────────────
Write-Host "`n[1/6] Checking IIS..." -ForegroundColor Yellow

$iisFeature = Get-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -ErrorAction SilentlyContinue
if ($null -eq $iisFeature -or $iisFeature.State -ne 'Enabled') {
    Write-Host '      Installing IIS...'
    Enable-WindowsOptionalFeature -Online -FeatureName `
        IIS-WebServerRole, IIS-WebServer, IIS-CommonHttpFeatures, `
        IIS-HttpErrors, IIS-StaticContent, IIS-DefaultDocument, `
        IIS-HttpRedirect, IIS-RequestFiltering, IIS-HttpCompressionStatic `
        -All -NoRestart | Out-Null
    Write-Host '      IIS installed.' -ForegroundColor Green
} else {
    Write-Host '      IIS already enabled.' -ForegroundColor Green
}

# ──────────────────────────────────────────────────────────────────────────────
# 2. URL Rewrite Module
# ──────────────────────────────────────────────────────────────────────────────
Write-Host "`n[2/6] Checking IIS URL Rewrite module..." -ForegroundColor Yellow

$rewriteDll = 'C:\Windows\System32\inetsrv\rewrite.dll'
if (-not (Test-Path $rewriteDll)) {
    $rewriteInstaller = Join-Path $env:TEMP 'rewrite_amd64_en-US.msi'
    Write-Host '      Downloading URL Rewrite...'
    Invoke-WebRequest -Uri 'https://download.microsoft.com/download/1/2/8/128E2E22-C1B9-44A4-BE2A-5859ED1D4592/rewrite_amd64_en-US.msi' `
        -OutFile $rewriteInstaller -UseBasicParsing
    Start-Process msiexec.exe -ArgumentList "/i `"$rewriteInstaller`" /quiet /norestart" -Wait
    Write-Host '      URL Rewrite installed.' -ForegroundColor Green
} else {
    Write-Host '      URL Rewrite already installed.' -ForegroundColor Green
}

# ──────────────────────────────────────────────────────────────────────────────
# 3. .NET 8 Hosting Bundle (required for ASP.NET Core Module v2)
# ──────────────────────────────────────────────────────────────────────────────
Write-Host "`n[3/6] Checking .NET 8 Hosting Bundle..." -ForegroundColor Yellow

$dotnetVersion = & dotnet --version 2>$null
if ($dotnetVersion -notmatch '^8\.') {
    $bundleInstaller = Join-Path $env:TEMP 'dotnet-hosting-8-win.exe'
    Write-Host '      Downloading .NET 8 Hosting Bundle...'
    Invoke-WebRequest -Uri 'https://aka.ms/dotnet/8.0/dotnet-hosting-win.exe' `
        -OutFile $bundleInstaller -UseBasicParsing
    Start-Process $bundleInstaller -ArgumentList '/quiet /norestart' -Wait
    Write-Host '      .NET 8 Hosting Bundle installed.' -ForegroundColor Green
} else {
    Write-Host "      .NET $dotnetVersion already installed." -ForegroundColor Green
}

Import-Module WebAdministration -ErrorAction Stop

# ──────────────────────────────────────────────────────────────────────────────
# 4. Detect the existing port-80 IIS site
# ──────────────────────────────────────────────────────────────────────────────
Write-Host "`n[4/6] Detecting existing IIS site on port 80..." -ForegroundColor Yellow

$port80Site = Get-Website | Where-Object {
    $_.Bindings.Collection | Where-Object { $_.bindingInformation -match ':80:' }
} | Select-Object -First 1

if ($null -eq $port80Site) {
    Write-Error 'No IIS site bound to port 80 found. Create one first and re-run.'
}

$SiteName = $port80Site.Name
Write-Host "      Found site: '$SiteName'" -ForegroundColor Green

# ──────────────────────────────────────────────────────────────────────────────
# 5. Virtual Application: /PromiPoints  (static frontend)
# ──────────────────────────────────────────────────────────────────────────────
Write-Host "`n[5/6] Deploying frontend as Virtual Application /PromiPoints..." -ForegroundColor Yellow

if (-not (Test-Path $FrontendDest)) { New-Item -ItemType Directory -Path $FrontendDest | Out-Null }
Copy-Item -Path "$FrontendSrc\*" -Destination $FrontendDest -Recurse -Force
Write-Host "      Files copied to $FrontendDest" -ForegroundColor Green

$frontendAppPath = "IIS:\Sites\$SiteName\PromiPoints"
$existingFrontend = Get-WebApplication -Site $SiteName -Name 'PromiPoints' -ErrorAction SilentlyContinue
if ($existingFrontend) {
    Write-Host '      Updating existing Virtual Application PromiPoints...'
    Set-ItemProperty $frontendAppPath -Name physicalPath -Value $FrontendDest
} else {
    Write-Host '      Creating Virtual Application PromiPoints...'
    New-WebApplication -Site $SiteName -Name 'PromiPoints' -PhysicalPath $FrontendDest | Out-Null
}
Write-Host '      Virtual Application /PromiPoints ready.' -ForegroundColor Green

# ──────────────────────────────────────────────────────────────────────────────
# 6. Virtual Application: /PromiPointsAPI  (ASP.NET Core 8 backend via ANCM)
# ──────────────────────────────────────────────────────────────────────────────
Write-Host "`n[6/6] Deploying backend as Virtual Application /PromiPointsAPI..." -ForegroundColor Yellow

if (-not (Test-Path $BackendDest)) { New-Item -ItemType Directory -Path $BackendDest | Out-Null }
Copy-Item -Path "$BackendSrc\*" -Destination $BackendDest -Recurse -Force
Write-Host "      Files copied to $BackendDest" -ForegroundColor Green

# Application Pool: must use No Managed Code for ANCM in-process hosting
$PoolName = 'PromiPointsAPIPool'
$existingPool = Get-Item "IIS:\AppPools\$PoolName" -ErrorAction SilentlyContinue
if ($existingPool) {
    Write-Host "      App Pool '$PoolName' already exists."
} else {
    Write-Host "      Creating App Pool '$PoolName'..."
    New-Item "IIS:\AppPools\$PoolName" | Out-Null
    Set-ItemProperty "IIS:\AppPools\$PoolName" -Name managedRuntimeVersion -Value ''
    Write-Host "      App Pool '$PoolName' created (No Managed Code)." -ForegroundColor Green
}

$existingBackend = Get-WebApplication -Site $SiteName -Name 'PromiPointsAPI' -ErrorAction SilentlyContinue
if ($existingBackend) {
    Write-Host '      Updating existing Virtual Application PromiPointsAPI...'
    Set-ItemProperty "IIS:\Sites\$SiteName\PromiPointsAPI" -Name physicalPath -Value $BackendDest
    Set-ItemProperty "IIS:\Sites\$SiteName\PromiPointsAPI" -Name applicationPool -Value $PoolName
} else {
    Write-Host '      Creating Virtual Application PromiPointsAPI...'
    New-WebApplication -Site $SiteName -Name 'PromiPointsAPI' -PhysicalPath $BackendDest -ApplicationPool $PoolName | Out-Null
}

# Inject ASPNETCORE_ENVIRONMENT=Production into the backend web.config
$backendWebConfig = Join-Path $BackendDest 'web.config'
if (Test-Path $backendWebConfig) {
    [xml]$cfg = Get-Content $backendWebConfig
    $aspNetCore = $cfg.SelectSingleNode('//aspNetCore')
    if ($null -ne $aspNetCore) {
        # Ensure <environmentVariables> child exists
        $envVarsNode = $aspNetCore.SelectSingleNode('environmentVariables')
        if ($null -eq $envVarsNode) {
            $envVarsNode = $cfg.CreateElement('environmentVariables')
            $aspNetCore.AppendChild($envVarsNode) | Out-Null
        }
        # Remove existing entry if present, then add fresh
        $existing = $envVarsNode.SelectSingleNode("environmentVariable[@name='ASPNETCORE_ENVIRONMENT']")
        if ($null -ne $existing) { $envVarsNode.RemoveChild($existing) | Out-Null }

        $envVar = $cfg.CreateElement('environmentVariable')
        $envVar.SetAttribute('name', 'ASPNETCORE_ENVIRONMENT')
        $envVar.SetAttribute('value', 'Production')
        $envVarsNode.AppendChild($envVar) | Out-Null

        $cfg.Save($backendWebConfig)
        Write-Host '      ASPNETCORE_ENVIRONMENT=Production set in web.config.' -ForegroundColor Green
    }
}

Write-Host '      Virtual Application /PromiPointsAPI ready.' -ForegroundColor Green

# ──────────────────────────────────────────────────────────────────────────────
# Summary
# ──────────────────────────────────────────────────────────────────────────────
Write-Host ''
Write-Host '========================================' -ForegroundColor Cyan
Write-Host ' Deployment complete!                   ' -ForegroundColor Green
Write-Host '========================================' -ForegroundColor Cyan
Write-Host "  IIS Site (port 80): $SiteName"
Write-Host "  App Pool (backend): $PoolName  [No Managed Code]"
Write-Host ''
Write-Host '  Frontend  : http://mr-bubotest01.promilab.prominente.com.ar/PromiPoints/'
Write-Host '  API       : http://mr-bubotest01.promilab.prominente.com.ar/PromiPointsAPI/swagger'
Write-Host ''
Write-Host '  Login con : maria.garcia@promilab.com / Promi2024!'
Write-Host '========================================' -ForegroundColor Cyan
