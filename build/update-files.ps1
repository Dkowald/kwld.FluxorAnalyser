param(
    [string]$PackageVersion
)

push-location $PSScriptRoot

if([string]::IsNullOrWhiteSpace($PackageVersion)){throw "Version not set"}

$tagName = "v$($PackageVersion)"

$blobRoot = [io.path]::Combine("https://github.com/Dkowald/kwld.FluxorAnalyser/blob/", $tagName)

$file = "../readme.md"
if(!(test-path -path $file)){throw "Cannot find target file: $file";}
$content = (Get-Content $file)

$src = "(docs/Home.md)"
$target = "(" + [io.path]::Combine($blobRoot, "readme.md") + ")"
$content = $content.replace($src, $target)

$src= "(https://github.com/Dkowald/kwld.FluxorAnalyser)"
$target = "(" + [io.path]::Combine("https://github.com/Dkowald/kwld.FluxorAnalyser/tree/", $tagName) + ")"
$content = $content.replace($src, $target)

$src= "(https://www.nuget.org/packages/kwld.FluxorAnalyser)"
$target = "(" + [io.path]::Combine("https://www.nuget.org/packages/kwld.FluxorAnalyser/", $PackageVersion) + ")"
$content = $content.replace($src, $target)

Set-Content -path $file -Value $content

$file = "../src/kwld.FluxorAnalyser/BuildInfo.cs"
if(!(test-path -path $file)){throw "Cannot find target file: $file";}
$content = (Get-Content $file)

$src = "https://github.com/Dkowald/kwld.FluxorAnalyser/blob/main/docs/rules/"
$target = $src.replace("main", $tagName)
$content = $content.replace($src, $target)

Set-Content -path $file -Value $content

pop-location