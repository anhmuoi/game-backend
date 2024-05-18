FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
COPY ./*.sln ./

# Copy csproj and restore as distinct layers
COPY ./API/ERPSystem.Api/ERPSystem.Api.csproj ./API/ERPSystem.Api/
COPY ./API/ERPSystem.Common/ERPSystem.Common.csproj ./API/ERPSystem.Common/
COPY ./API/ERPSystem.DataAccess/ERPSystem.DataAccess.csproj ./API/ERPSystem.DataAccess/
COPY ./API/ERPSystem.DataModel/ERPSystem.DataModel.csproj ./API/ERPSystem.DataModel/
COPY ./API/ERPSystem.Repository/ERPSystem.Repository.csproj ./API/ERPSystem.Repository/
COPY ./API/ERPSystem.Service/ERPSystem.Service.csproj ./API/ERPSystem.Service/
RUN dotnet restore --force --ignore-failed-sources

COPY . ./
RUN dotnet publish ./API/ERPSystem.Api/ERPSystem.Api.csproj -c Release -o API/ERPSystem.Api/out

# Build runtime image
FROM 009795078640.dkr.ecr.ap-southeast-1.amazonaws.com/erp-api:runtime AS runtime
WORKDIR /app
COPY --from=build-env /app/API/ERPSystem.Api/out ./
COPY --from=build-env /app/API/ERPSystem.Api/ERPSystem.Api.xml ./

ENTRYPOINT ["dotnet", "ERPSystem.Api.dll"]	
