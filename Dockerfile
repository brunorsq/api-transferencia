# Etapa 1 — Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o arquivo .csproj e restaura dependências
COPY src/Transferencia/*.csproj ./src/Transferencia/
RUN dotnet restore ./src/Transferencia/Transferencia.csproj

# Copia todo o código e faz o build
COPY . .
WORKDIR /app/src/Transferencia
RUN dotnet publish -c Release -o /out

# Etapa 2 — Imagem final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia os arquivos compilados
COPY --from=build /out .

# Define a porta de exposição
EXPOSE 8080

# Define o ponto de entrada
ENTRYPOINT ["dotnet", "Transferencia.dll"]