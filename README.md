# Backend de Patitas

API REST desarrollada para administrar clientes, servicios y citas de una tienda de mascotas.


## Funcionalidades

- Registro e inicio de sesión de usuarios.
- Autenticación mediante JWT.
- Autorización mediante roles.
- CRUD de clientes.
- CRUD de servicios.
- CRUD de citas.


## Tecnologías utilizadas

- C#.
- .NET 10.
- ASP.NET Core Web API.
- Entity Framework Core 10.0.11.
- SQLite.
- JSON Web Token.
- Git y GitHub.
- Postman para las pruebas de la API.lite`

## Requisitos previos

Antes de ejecutar el proyecto se debe instalar:

1. .NET SDK 10.
2. Git.

## Comprobar las instalaciones

Abrir una terminal y ejecutar:

```bash
dotnet --version
```

Debe mostrarse una versión `10.0.x`.

Para comprobar Git:

```bash
git --version
```

Debe mostrarse la versión instalada de Git.

## Descargar el proyecto

Clonar el repositorio:

```bash
git clone <URL_DEL_REPOSITORIO>
```

Ingresar a la carpeta del backend:

```bash
cd practica2-backend
```

Cambiar a la rama estable:

```bash
git checkout main
```

> Reemplazar `<URL_DEL_REPOSITORIO>` por la dirección real del repositorio de GitHub.

## Restaurar las dependencias

Desde la carpeta donde se encuentra `practica2.slnx`, ejecutar:

```bash
dotnet restore practica2.slnx
```

Este comando descarga los paquetes NuGet indicados en el proyecto.

Restaurar también la herramienta local de Entity Framework Core:

```bash
dotnet tool restore
```

Comprobar que la herramienta funciona:

```bash
dotnet ef --version
```

Debe mostrarse una versión `10.0.x`.

## Configurar los secretos locales

El proyecto utiliza secretos locales para evitar guardar la clave JWT y la contraseña del administrador en GitHub.

Cada equipo donde se ejecute el proyecto debe configurar sus propios secretos.

### 1. Generar una clave JWT

En PowerShell ejecutar:

```powershell
$jwtKey = [guid]::NewGuid().ToString("N") + [guid]::NewGuid().ToString("N")
```

Esta instrucción genera una clave aleatoria de 64 caracteres.

Guardar la clave en User Secrets:

```powershell
dotnet user-secrets set "Jwt:Key" "$jwtKey" --project "Practica2.Api/Practica2.Api.csproj"
```

### 2. Configurar la contraseña del administrador

Ejecutar:

```powershell
dotnet user-secrets set "UsuarioInicial:Contrasena" "SU_CONTRASENA_SEGURA" --project "Practica2.Api/Practica2.Api.csproj"
```

Se debe reemplazar `SU_CONTRASENA_SEGURA` por la contraseña que se utilizará durante las pruebas.

Ejemplo:

```powershell
dotnet user-secrets set "UsuarioInicial:Contrasena" "AdminPatitas2026!" --project "Practica2.Api/Practica2.Api.csproj"
```

La contraseña y la clave JWT se guardan fuera del repositorio. Por esta razón, no se suben a GitHub.

## Configuración del administrador inicial

El correo del administrador se encuentra configurado en `appsettings.json`:

```text
admin@patitas.com
```

La contraseña es la que se configuró anteriormente mediante User Secrets.

Cuando la API se ejecuta por primera vez, se crea automáticamente este usuario con el rol:

```text
Administrador
```


## Crear la base de datos

Ejecutar las migraciones existentes:

```bash
dotnet ef database update --project "Practica2.Api/Practica2.Api.csproj" --startup-project "Practica2.Api/Practica2.Api.csproj"
```

Este comando utiliza las migraciones del proyecto para crear las tablas en SQLite.

El archivo generado será:

```text
Practica2.Api/practica2.db
```



## Compilar el proyecto

Ejecutar:

```bash
dotnet build practica2.slnx
```

El resultado esperado es:

```text
Compilación realizada correctamente
```

## Ejecutar la API

Ejecutar:

```bash
dotnet run --project "Practica2.Api/Practica2.Api.csproj"
```

La terminal mostrará una dirección parecida a:

```text
Now listening on: http://localhost:5271
```

La dirección mostrada en la terminal será la dirección base de la API.

Para detener la aplicación se debe presionar:

```text
Ctrl + C
```

## Probar el inicio de sesión

En Postman crear la siguiente petición:

```http
POST http://localhost:5271/api/autenticacion/login
```

Seleccionar `Body`, después `raw` y finalmente `JSON`.

Utilizar:

```json
{
  "correo": "admin@patitas.com",
  "contrasena": "SU_CONTRASENA_CONFIGURADA"
}
```

Se debe utilizar la misma contraseña configurada anteriormente mediante User Secrets.

El resultado esperado es:

```text
200 OK
```

La respuesta contiene un token JWT y los datos básicos del usuario.

## Utilizar el token JWT

Copiar el token recibido durante el inicio de sesión.

En las peticiones protegidas de Postman:

1. Abrir la pestaña `Authorization`.
2. Seleccionar `Bearer Token`.
3. Pegar el token JWT.
4. Enviar la petición.

Si no se envía un token válido, la API responde:

```text
401 Unauthorized
```

Si el token es válido pero el rol no tiene permiso, la API responde:

```text
403 Forbidden
```

## Registrar un empleado

Solamente un administrador puede registrar empleados.

Realizar:

```http
POST http://localhost:5271/api/autenticacion/registro
```

Enviar el token del administrador y el siguiente cuerpo:

```json
{
  "nombre": "Empleado Patitas",
  "correo": "empleado@patitas.com",
  "contrasena": "Empleado2026"
}
```

El usuario se crea automáticamente con el rol:

```text
Empleado
```

## Roles y permisos

### Administrador

Puede:

- Consultar clientes, servicios y citas.
- Crear clientes, servicios y citas.
- Modificar clientes, servicios y citas.
- Eliminar clientes, servicios y citas.
- Registrar cuentas de empleados.

### Empleado

Puede:

- Consultar clientes.
- Consultar servicios.
- Consultar citas.

No puede crear, modificar ni eliminar registros. Tampoco puede registrar otros empleados.

## Rutas de autenticación

| Método | Ruta | Permiso | Descripción |
|---|---|---|---|
| POST | `/api/autenticacion/login` | Público | Iniciar sesión |
| POST | `/api/autenticacion/registro` | Administrador | Registrar un empleado |
| GET | `/api/autenticacion/perfil` | Usuario autenticado | Consultar el perfil actual |

## Rutas de clientes

| Método | Ruta | Permiso | Descripción |
|---|---|---|---|
| GET | `/api/clientes` | Administrador o Empleado | Listar clientes |
| GET | `/api/clientes/{id}` | Administrador o Empleado | Consultar un cliente |
| POST | `/api/clientes` | Administrador | Crear un cliente |
| PUT | `/api/clientes/{id}` | Administrador | Modificar un cliente |
| DELETE | `/api/clientes/{id}` | Administrador | Eliminar un cliente |

## Rutas de servicios

| Método | Ruta | Permiso | Descripción |
|---|---|---|---|
| GET | `/api/servicios` | Administrador o Empleado | Listar servicios |
| GET | `/api/servicios/{id}` | Administrador o Empleado | Consultar un servicio |
| POST | `/api/servicios` | Administrador | Crear un servicio |
| PUT | `/api/servicios/{id}` | Administrador | Modificar un servicio |
| DELETE | `/api/servicios/{id}` | Administrador | Eliminar un servicio |

## Rutas de citas

| Método | Ruta | Permiso | Descripción |
|---|---|---|---|
| GET | `/api/citas` | Administrador o Empleado | Listar citas |
| GET | `/api/citas/{id}` | Administrador o Empleado | Consultar una cita |
| POST | `/api/citas` | Administrador | Crear una cita |
| PUT | `/api/citas/{id}` | Administrador | Modificar una cita |
| DELETE | `/api/citas/{id}` | Administrador | Eliminar una cita |


## Restricciones base de datos

No se puede eliminar un cliente o un servicio que tenga citas relacionadas. En ese caso la API responde con un conflicto para proteger la información.

