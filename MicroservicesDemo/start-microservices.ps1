# start-microservices.ps1
# ------------------------------------------------------
# Script para limpiar y levantar microservicios Docker
# Funciona en Windows PowerShell
# ------------------------------------------------------

Write-Host "==============================="
Write-Host "🔹 Deteniendo contenedores existentes..."
Write-Host "==============================="

# Detener todos los contenedores que estén corriendo
docker ps -q | ForEach-Object { docker stop $_ }

Write-Host "==============================="
Write-Host "🔹 Eliminando contenedores existentes..."
Write-Host "==============================="

# Eliminar todos los contenedores (detenidos o corriendo)
docker ps -a -q | ForEach-Object { docker rm -f $_ }

Write-Host "==============================="
Write-Host "🔹 Limpiando imágenes antiguas de Docker..."
Write-Host "==============================="

# Limpiar todas las imágenes de los servicios del proyecto
docker compose down --rmi all --volumes

# Limpiar caché de build
docker builder prune -a -f

Write-Host "==============================="
Write-Host "🔹 Levantando todo el stack de microservicios..."
Write-Host "==============================="

docker compose up --build