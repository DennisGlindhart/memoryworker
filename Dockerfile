FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . /src
WORKDIR /src
RUN dotnet restore && dotnet publish -c Release -o publish/
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /src/publish .
ENTRYPOINT ["dotnet", "memoryworker.dll"]
#If needed to run as non-root
#RUN useradd -u 1000 worker
#USER 1000
