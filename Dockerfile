FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Aspirin.Api/*.csproj ./Aspirin.Api/
RUN dotnet restore

# copy everything else and build app
COPY Aspirin.Api/. ./Aspirin.Api/
WORKDIR /app/Aspirin.Api
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/Aspirin.Api/out ./
ENTRYPOINT ["dotnet", "Aspirin.Api.dll"]