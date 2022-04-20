# make sure latest version of cortside.templates is installed
dotnet new --install cortside.templates

# update powershell scripts from root
dotnet new cortside-api-powershell --force

# update .editorconfig
dotnet new cortside-api-editorconfig --force

# update dockerfile and supporting shell and powershell scripts
#dotnet new enerbank-api-deployment --force
