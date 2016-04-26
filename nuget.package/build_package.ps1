Set-StrictMode -Version Latest

$frameworkVersion="v4.0.30319"
$frameworkDir="$env:SystemRoot\Microsoft.NET\Framework"
$nugetBinary="$env:programfiles\Unity\Editor\Data\PlaybackEngines\MetroSupport\Tools\Nuget.exe"

if (Test-Path "$env:SystemRoot\Microsoft.NET\Framework64")
{
  $frameworkDir="$env:SystemRoot\Microsoft.NET\Framework64"
}

& (Join-Path (Join-Path $frameworkDir $frameworkVersion) "msbuild.exe") Moduni.csproj
& $nugetBinary Pack Moduni.csproj