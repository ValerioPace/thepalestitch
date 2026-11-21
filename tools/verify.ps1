<#
.SYNOPSIS
  Full verification of The Pale Stitch: build, unit tests, gameplay scenarios.

.DESCRIPTION
  1. Builds the game's C# with warnings treated as errors.
  2. Runs the unit tests (tests/ThePaleStitch.Tests).
  3. Imports the project's resources (needed after new files).
  4. Runs the gameplay scenarios headless, in simulated time.
  Exit code 0 only when every step is green. Report and logs in verify-report\.

.PARAMETER Godot
  Path of the Godot .NET *console* executable (it prints to the terminal).

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File tools\verify.ps1
#>
param(
    [string]$Godot = $(if ($env:GODOT) { $env:GODOT } else { "X:\Installations\Godot_v4.7.2-stable_mono_win64\Godot_v4.7.2-stable_mono_win64_console.exe" })
)

$Root = Split-Path -Parent $PSScriptRoot
$Project = Join-Path $Root "thepalestitch_project"
$Report = Join-Path $Root "verify-report"
New-Item -ItemType Directory -Force -Path $Report | Out-Null
$Failed = $false

function Step($text) { Write-Host ""; Write-Host "== $text" -ForegroundColor Cyan }
function Run($exe, [string[]]$arguments, $log) {
    & $exe @arguments *>&1 | ForEach-Object { "$_" } | Out-File -Encoding utf8 $log
    return $LASTEXITCODE
}

if (-not (Test-Path $Godot)) { Write-Host "Godot not found: $Godot (use -Godot or set GODOT)" -ForegroundColor Red; exit 2 }

Step "1/4 Build (warnings are errors)"
$code = Run "dotnet" @("build", (Join-Path $Project "The Pale Stitch.csproj"), "-warnaserror", "-nologo", "-v", "q") (Join-Path $Report "build.log")
if ($code -eq 0) { Write-Host "OK" } else { Write-Host "FAILED (see verify-report\build.log)" -ForegroundColor Red; $Failed = $true }

Step "2/4 Unit tests"
$log = Join-Path $Report "unit-tests.log"
$code = Run "dotnet" @("run", "--project", (Join-Path $Root "tests\ThePaleStitch.Tests")) $log
if ($code -eq 0) { Get-Content $log -Tail 1 } else { Select-String -Path $log -Pattern "FAILED|Unit tests" | ForEach-Object { $_.Line }; $Failed = $true }

Step "3/4 Import resources"
Run $Godot @("--headless", "--path", $Project, "--import") (Join-Path $Report "import.log") | Out-Null
Write-Host "done"

Step "4/4 Gameplay scenarios (headless, simulated time)"
$log = Join-Path $Report "scenarios.log"
$code = Run $Godot @("--headless", "--fixed-fps", "144", "--path", $Project, "res://dev/ScenarioRunner.tscn", "--", "--report=$Report") $log
Select-String -Path $log -Pattern "FAIL|Scenarios:" | ForEach-Object { $_.Line }
$noise = @(Select-String -Path $log -Pattern "^(ERROR|WARNING|SCRIPT ERROR)")
if ($code -ne 0) { $Failed = $true }
if ($noise.Count -gt 0) {
    Write-Host "Engine errors/warnings in the log: $($noise.Count)" -ForegroundColor Red
    $noise | ForEach-Object { $_.Line } | Group-Object | ForEach-Object { "{0,4} {1}" -f $_.Count, $_.Name }
    $Failed = $true
}

Write-Host ""
if ($Failed) { Write-Host "VERIFY: RED" -ForegroundColor Red; exit 1 } else { Write-Host "VERIFY: GREEN" -ForegroundColor Green; exit 0 }
