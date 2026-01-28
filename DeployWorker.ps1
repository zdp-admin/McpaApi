# ------------------------------
# Configuración
# ------------------------------
$projectPath = "C:\COMPAC PROGRAMAS\McpaApi"
$publishPath = "C:\McpaApiWorker"
$serviceName = "McpaApiWorker"
$runtime = "win-x64"
$exeName = "McpaApi.exe"  # nombre del exe generado
$logPath = Join-Path $publishPath "Logs"

# ------------------------------
# Publicar proyecto
# ------------------------------
Write-Host "Publicando proyecto..."
dotnet publish "`"$projectPath`"" -c Release -o $publishPath --runtime $runtime --self-contained true

# ------------------------------
# Crear carpeta de publicación si no existe
# ------------------------------
if (-not (Test-Path $publishPath)) {
    Write-Host "Creando carpeta de publicación $publishPath"
    New-Item -ItemType Directory -Path $publishPath
}

# ------------------------------
# Crear carpeta de Logs si no existe
# ------------------------------
if (-not (Test-Path $logPath)) {
    Write-Host "Creando carpeta de logs $logPath"
    New-Item -ItemType Directory -Path $logPath
}

# ------------------------------
# Configurar permisos
# ------------------------------
#Write-Host "Asignando permisos de lectura y ejecución..."
#$acl = Get-Acl $publishPath
#$permission = "BUILTIN\Users","ReadAndExecute","ContainerInherit,ObjectInherit","None","Allow"
#$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
#$acl.SetAccessRule($accessRule)
#Set-Acl $publishPath $acl

# Permisos para la carpeta de logs
#$aclLogs = Get-Acl $logPath
#$accessRuleLogs = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
#$aclLogs.SetAccessRule($accessRuleLogs)
#Set-Acl $logPath $aclLogs

# ------------------------------
# Ruta completa del exe
# ------------------------------
$exePath = Join-Path $publishPath $exeName

# ------------------------------
# Crear o reiniciar Windows Service
# ------------------------------
if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) {
    Write-Host "Servicio '$serviceName' ya existe, reiniciando..."
    Restart-Service -Name $serviceName
} else {
    Write-Host "Creando servicio '$serviceName'..."
    sc.exe create $serviceName binPath= "`"$exePath`"" start= auto
    Start-Service -Name $serviceName
}

Write-Host "---------------------------------------------"
Write-Host "Despliegue completado. Servicio '$serviceName' en ejecución."
Write-Host "Logs en: $logPath"
