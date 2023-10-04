#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EmployeeManagementAPI/EmployeeManagementApi.csproj", "EmployeeManagementAPI/"]
COPY ["EmployeeManagement.Entities/EmployeeManagement.Domain.csproj", "EmployeeManagement.Entities/"]
COPY ["EmployeeManagment.Context/EmployeeManagment.Context.csproj", "EmployeeManagment.Context/"]
COPY ["EmployeeManagment.Data/EmployeeManagment.Services.csproj", "EmployeeManagment.Data/"]
COPY ["EmployeeManagement.Repository/EmployeeManagement.Repository.csproj", "EmployeeManagement.Repository/"]
COPY ["EmployeeMangementApi.Hub/EmployeeMangementApi.Hub.csproj", "EmployeeMangementApi.Hub/"]
RUN dotnet restore "EmployeeManagementAPI/EmployeeManagementApi.csproj"
COPY . .
WORKDIR "/src/EmployeeManagementAPI"
RUN dotnet build "EmployeeManagementApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeeManagementApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagementApi.dll"]