# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore (tách riêng để tận dụng cache layer)
COPY ClinicManagement.API/ClinicManagement.API.csproj ./ClinicManagement.API/
RUN dotnet restore ClinicManagement.API/ClinicManagement.API.csproj

# Copy toàn bộ source rồi publish
COPY ClinicManagement.API/ ./ClinicManagement.API/
RUN dotnet publish ClinicManagement.API/ClinicManagement.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production
# Render sẽ tự set biến PORT; Program.cs đọc PORT để lắng nghe.
EXPOSE 8080

ENTRYPOINT ["dotnet", "ClinicManagement.API.dll"]
