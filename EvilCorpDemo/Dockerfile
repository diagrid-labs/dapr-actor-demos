# Use the .NET 8 SDK as a base image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /back-end

# Copy csproj and restore dependencies
COPY ./EvilCorp.Interfaces/EvilCorp.Interfaces.csproj ./EvilCorp.Interfaces/
COPY ./EvilCorp.Api/EvilCorp.Api.csproj ./EvilCorp.Api/
RUN dotnet restore EvilCorp.Interfaces/EvilCorp.Interfaces.csproj
RUN dotnet restore EvilCorp.Api/EvilCorp.Api.csproj

# Copy the rest of the application code
COPY ./ ./

# Build the application
RUN dotnet publish ./EvilCorp.Api -c Release -o /back-end/out

# Use the .NET 8 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /back-end
COPY --from=build-env /back-end/out ./

# Expose ports 7100 and 3500
EXPOSE 7100
EXPOSE 3500

ENV ASPNETCORE_HTTP_PORTS=7100
ENV ASPNETCORE_URLS=http://+:7100

ARG ABLY_API_KEY
ENV AblyApiKey=$ABLY_API_KEY

# Start the application
ENTRYPOINT ["dotnet", "EvilCorp.Api.dll"]