====================================================
  PromiPoints – Deploy en Servidor de Test
====================================================

Servidor: mr-bubotest01.promilab.prominente.com.ar
Usuario RDP: PROMI\epelozo

----------------------------------------------------
ESTRUCTURA DE LA CARPETA DEPLOY
----------------------------------------------------
deploy/
  frontend/        <- archivos del frontend (React build)
  backend/         <- binarios del backend (.NET 8 publish)
  setup-server.ps1 <- script de configuracion automatica
  README.txt       <- este archivo

----------------------------------------------------
PASOS PARA EJECUTAR EN EL SERVIDOR
----------------------------------------------------

1. Conectar al servidor via RDP:
   Host : mr-bubotest01.promilab.prominente.com.ar:3389
   User : PROMI\epelozo

2. Copiar la carpeta deploy/ al servidor.
   Metodo facil: en la sesion RDP, usar "Compartir carpeta"
   o arrastrar y soltar la carpeta al escritorio del servidor.
   (Ej: copiar a C:\deploy\)

3. Abrir PowerShell como Administrador:
   - Clic derecho en el menu Inicio
   - "Windows PowerShell (Administrador)"

4. Habilitar ejecucion de scripts para esta sesion:
   Set-ExecutionPolicy RemoteSigned -Scope Process

5. Ir a la carpeta copiada:
   cd C:\deploy

6. (Opcional) Si la raiz fisica del servidor NO es C:\PSS\,
   editar las variables al comienzo del script:
   $FrontendDest = 'C:\PSS\PromiPoints'
   $BackendDest  = 'C:\PSS\PromiPointsAPI'

7. Ejecutar el script:
   .\setup-server.ps1

   El script realiza automaticamente:
   - Verifica que IIS este habilitado
   - Verifica/instala IIS URL Rewrite Module (si falta)
   - Verifica/instala .NET 8 Hosting Bundle (si falta)
   - Detecta el sitio IIS existente en puerto 80
   - Crea Virtual Application /PromiPoints (frontend estatico)
     -> fisico: C:\PSS\PromiPoints\
   - Crea Virtual Application /PromiPointsAPI (ASP.NET Core via ANCM)
     -> fisico: C:\PSS\PromiPointsAPI\
     -> App Pool "PromiPointsAPIPool" con No Managed Code
     -> Inyecta ASPNETCORE_ENVIRONMENT=Production en web.config

----------------------------------------------------
VERIFICACION POST-DEPLOY
----------------------------------------------------

En el servidor (o desde cualquier PC en la red):

  Frontend (login)  : http://mr-bubotest01.promilab.prominente.com.ar/PromiPoints/
  API (Swagger)     : http://mr-bubotest01.promilab.prominente.com.ar/PromiPointsAPI/swagger

Login de prueba:
  Email    : maria.garcia@promilab.com
  Password : Promi2024!

Otros usuarios de prueba (todos con password Promi2024!):
  juan.perez@promilab.com      (employee, Marketing)
  ana.rodriguez@promilab.com   (people/admin, People & Culture)
  carlos.lopez@promilab.com    (employee)
  laura.martinez@promilab.com  (employee)

En IIS Manager:
  Sitio existente -> Applications -> debe mostrar:
    /PromiPoints
    /PromiPointsAPI

----------------------------------------------------
COMANDOS UTILES EN EL SERVIDOR
----------------------------------------------------

# Listar sitios y aplicaciones IIS
Get-Website
Get-WebApplication

# Reiniciar IIS
iisreset

# Ver App Pool backend
Get-Item IIS:\AppPools\PromiPointsAPIPool

# Reciclar App Pool backend (equivale a reiniciar el proceso)
Restart-WebAppPool PromiPointsAPIPool

# Ver logs de stdout del backend
# (habilitarlos primero: stdoutLogEnabled="true" en web.config del backend)
Get-Content C:\PSS\PromiPointsAPI\logs\stdout*.log -Tail 50

----------------------------------------------------
COMO FUNCIONA (ANCM + Virtual Application)
----------------------------------------------------

Cuando IIS recibe /PromiPointsAPI/auth/login:
  1. Enruta a la Virtual Application /PromiPointsAPI
  2. ANCM elimina el prefijo y pasa /auth/login a Kestrel
  3. El controller [Route("auth")] / [HttpPost("login")] responde

No se requieren puertos adicionales ni reglas de firewall.
Frontend y API comparten el mismo origen -> sin CORS.

----------------------------------------------------
PUERTOS Y SERVICIOS
----------------------------------------------------
Puerto 80   -> IIS -> /PromiPoints/    (frontend React SPA)
                   -> /PromiPointsAPI/ (backend ASP.NET Core 8 via ANCM)
Puerto 2001 -> SQL Server remoto (Mr-BuboTest01, sin cambios)

====================================================
