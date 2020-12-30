$version = "1.0.4"

./clean.ps1
dotnet publish -r alpine-x64 -c Debug -o publish .\src\Cortside.HealthMonitor.WebApi\Cortside.HealthMonitor.WebApi.csproj
docker rm healthmonitor
docker build -t cortside/healthmonitor:$version .
docker push cortside/healthmonitor:$version
docker run --name healthmonitor -d -p 5000:5000 cortside/healthmonitor:$version
docker logs healthmonitor