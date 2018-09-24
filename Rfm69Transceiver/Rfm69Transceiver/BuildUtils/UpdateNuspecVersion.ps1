param([string] $newVersion = "0.0.0-local", [string] $relativeFilePath = "./Rfm69Transceiver.Wrapper.Net.nuspec")

Write-Host "newVersion is $newVersion"
(Get-Content $relativeFilePath).Replace("0.0.0-local", $newVersion) | out-file $relativeFilePath 
