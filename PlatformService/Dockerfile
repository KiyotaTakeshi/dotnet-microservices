# syntax=docker/dockerfile:1
# @see https://docs.docker.com/samples/dotnetcore/
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS Builder 
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=Builder /app/out .
ENTRYPOINT ["dotnet", "PlatformService.dll"]

# docker build -t kiyotakeshiwork/platformservice .
# docker run -p 8080:80 -d --name platformservice kiyotakeshiwork/platformservice
# docker login -u kiyotakeshiwork
# docker push kiyotakeshiwork/platformservice
