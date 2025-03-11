# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the solution and restore dependencies
COPY *.sln ./
COPY Bulk-Price-Lists-v2/Bulk-Price-Lists-v2.csproj ./Bulk-Price-Lists-v2/
RUN dotnet restore

# Copy the rest of the application files
COPY . ./

# Build and publish the app
RUN dotnet publish -c Release -o /out

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Set the entry point
CMD ["dotnet", "Bulk-Price-Lists-v2.dll"]
