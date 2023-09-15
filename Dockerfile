FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

COPY . .

RUN dotnet restore ./DashboardX/Presentation/Presentation.csproj

WORKDIR /src
RUN dotnet build ./Presentation/Presentation.csproj -c Release -o /app
RUN dotnet publish ./Presentation/Presentation.csproj -c Release -o /app

# Set the entry point to run the Blazor app
WORKDIR /app
ENTRYPOINT ["dotnet", "Presentation.dll"]