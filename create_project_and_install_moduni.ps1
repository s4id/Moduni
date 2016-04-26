Param(
   [Parameter(Mandatory=$True)]
   [string]$projectPath
)

Set-StrictMode -Version Latest

$unityBinary="$env:programfiles\Unity\Editor\Unity.exe"
$nugetBinary="C:\Users\corentin.lemasson\Downloads\nuget.exe"
$pluginsPath=Join-Path $projectPath "Assets\Plugins"
$libGit2SharpNativeBinariesPath=Join-Path $pluginsPath "LibGit2Sharp.NativeBinaries.Unity"
$libGit2SharpPath=Join-Path $pluginsPath "LibGit2Sharp.Unity"
$moduniPath=Join-Path $pluginsPath "Moduni"
$restSharpPath=Join-Path $pluginsPath "RestSharp"
$tasksUnofficialPath=Join-Path $pluginsPath "System.Threading.Tasks.Unofficial"

if ( !(Test-Path $projectPath) )
{
	New-Item $projectPath -type directory
}
Start-Process -FilePath $unityBinary -ArgumentList ("-createProject",$projectPath,"-quit","-batchmode") -NoNewWindow -Wait
New-Item $pluginsPath -type directory
Start-Process -FilePath $nugetBinary -ArgumentList ("install","-ExcludeVersion","-OutputDirectory",$pluginsPath,"Moduni") -NoNewWindow -Wait


Remove-Item -Force (Join-Path $libGit2SharpNativeBinariesPath "LibGit2Sharp.NativeBinaries.Unity.nupkg")
Remove-Item -Force -Recurse (Join-Path $libGit2SharpNativeBinariesPath "build")
Remove-Item -Force (Join-Path $libGit2SharpNativeBinariesPath "[Content_Types].xml")
Remove-Item -Force (Join-Path $libGit2SharpPath "LibGit2Sharp.Unity.nupkg")
Remove-Item -Force (Join-Path $moduniPath "Moduni.nupkg")
Remove-Item -Force (Join-Path $restSharpPath "RestSharp.nupkg")
$allItemsToDelete = Get-ChildItem (Join-Path $restSharpPath "lib") | Where {$_.FullName -notlike "*net35*"}
foreach($item in $allItemsToDelete)
{
    Remove-Item -Force -Recurse $item.FullName
}
Remove-Item -Force (Join-Path $tasksUnofficialPath "System.Threading.Tasks.Unofficial.nupkg")
$allItemsToDelete = Get-ChildItem (Join-Path $tasksUnofficialPath "lib") | Where {$_.FullName -notlike "*net35*"}
foreach($item in $allItemsToDelete)
{
    Remove-Item -Force -Recurse $item.FullName
}

Start-Process -FilePath $unityBinary -ArgumentList ("-projectPath",$projectPath,"-quit","-batchmode") -NoNewWindow -Wait
Start-Process -FilePath $unityBinary -ArgumentList ("-projectPath",$projectPath)