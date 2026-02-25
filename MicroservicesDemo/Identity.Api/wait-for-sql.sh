#!/bin/sh
set -e

host="$1"
shift
cmd="$@"

echo "Esperando a SQL Server en $host..."

until /opt/mssql-tools/bin/sqlcmd -S "$host" -U "sa" -P "Your_password123" -Q "SELECT 1" > /dev/null 2>&1
do
  echo "SQL Server no disponible, esperando 2s..."
  sleep 2
done

echo "SQL Server listo, aplicando migraciones y arrancando API..."

# Aplicar migraciones EF Core autom�ticamente
dotnet tool install --global dotnet-ef --version 8.0.0 --verbosity quiet || true
export PATH="$PATH:/root/.dotnet/tools"
dotnet ef database update || true

# Ejecutar la API
exec $cmd