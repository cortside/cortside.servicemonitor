[cmdletBinding()]
Param()

Push-Location "$PSScriptRoot/src/cortside.healthmonitor.WebApi"

cmd /c start cmd /k "title cortside.healthmonitor.WebApi & dotnet run"

Pop-Location
