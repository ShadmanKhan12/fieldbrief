FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Fieldbrief.slnx ./
COPY src/Fieldbrief.Domain/Fieldbrief.Domain.csproj src/Fieldbrief.Domain/
COPY src/Fieldbrief.Infrastructure/Fieldbrief.Infrastructure.csproj src/Fieldbrief.Infrastructure/
COPY src/Fieldbrief.Api/Fieldbrief.Api.csproj src/Fieldbrief.Api/
RUN dotnet restore src/Fieldbrief.Api/Fieldbrief.Api.csproj
COPY src/ ./src/
RUN dotnet publish src/Fieldbrief.Api/Fieldbrief.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Fieldbrief.Api.dll"]
