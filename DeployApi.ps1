# Variables de configuración
$projectPath = "C:\COMPAC PROGRAMAS\McpaApi"
$publishPath = "C:\inetpub\wwwroot\McpaApi"
$siteName = "McpaApi"
$port = 5000
$appPoolName = "McpaApiAppPool"
$runtime = "win-x64"

# Publicar proyecto
Write-Host "Publicando el proyecto..."
dotnet publish "`"$projectPath`"" -c Release -o $publishPath #--runtime $runtime#--self-contained true -p:PublishSingleFile=true

# Importar módulo WebAdministration si no está
Import-Module WebAdministration

# Crear Application Pool
Write-Host "Creando Application Pool..."
if (-not (Test-Path IIS:\AppPools\$appPoolName)) {
    Write-Host "Creando Application Pool $appPoolName"
    New-WebAppPool -Name $appPoolName
} else {
    Write-Host "El Application Pool $appPoolName ya existe"
}
Set-ItemProperty IIS:\AppPools\$appPoolName -Name managedRuntimeVersion -Value ""

# Crear el sitio web en IIS
Write-Host "Creando el sitio en IIS..."
if (!(Test-Path "IIS:\Sites\$siteName")) {
    New-Item "IIS:\Sites\$siteName" -bindings @{protocol="http";bindingInformation="*:${port}:"} -physicalPath $publishPath
    Set-ItemProperty "IIS:\Sites\$siteName" -Name applicationPool -Value $appPoolName
} else {
    Write-Host "El sitio '$siteName' ya existe, actualizando ruta física y pool..."
    Set-ItemProperty "IIS:\Sites\$siteName" -Name physicalPath -Value $publishPath
    Set-ItemProperty "IIS:\Sites\$siteName" -Name applicationPool -Value $appPoolName
}

# Permisos de carpeta
Write-Host "Asignando permisos de lectura a IIS_IUSRS..."
$acl = Get-Acl $publishPath
$permission = "IIS_IUSRS","ReadAndExecute","ContainerInherit,ObjectInherit","None","Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.SetAccessRule($accessRule)
Set-Acl $publishPath $acl

# (Opcional) Crear binding HTTPS - descomenta y configura si tienes un certificado válido
<# 
$certThumbprint = "THUMBPRINT_DEL_CERTIFICADO"  # reemplaza por el real
$cert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.Thumbprint -eq $certThumbprint }
if ($cert) {
    New-WebBinding -Name $siteName -Protocol https -Port 443 -SslFlags 1
    netsh http add sslcert ipport=0.0.0.0:443 certhash=$certThumbprint appid='{00112233-4455-6677-8899-AABBCCDDEEFF}'
}
#>

# Reiniciar sitio
Restart-WebAppPool $appPoolName
Restart-WebItem "IIS:\Sites\$siteName"

Write-Host "Despliegue completado. Accede a http://localhost:$port"
