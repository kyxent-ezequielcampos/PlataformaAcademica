# Sistema de Gestión Académica

Una aplicación de escritorio desarrollada con **AvaloniaUI** (frontend) y **Carter .NET** (backend) para la gestión de estudiantes, docentes y asignaturas.

## 🚀 Características

- **Frontend**: AvaloniaUI con C# (.NET 9)
- **Backend**: Carter framework con .NET 9
- **Base de datos**: SQL Server
- **Autenticación**: Sistema de login con sesiones persistentes
- **CRUD completo**: Gestión de estudiantes, docentes y asignaturas
- **UI moderna**: Interfaz limpia y responsiva con tema Fluent

## 📋 Requisitos

- .NET 9 SDK
- SQL Server (LocalDB o instancia completa)
- Visual Studio 2022 o VS Code

## 🛠️ Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone <tu-repositorio>
cd PlataformaAcademica
```

### 2. Configurar la base de datos
```bash
# Ejecutar el script SQL para crear la base de datos
sqlcmd -S (localdb)\MSSQLLocalDB -i database.sql
```

### 3. Configurar el backend
```bash
cd backend
dotnet restore
dotnet build
```

### 4. Configurar el frontend
```bash
cd frontend
dotnet restore
dotnet build
```

## 🚀 Ejecución

### Ejecutar el Backend
```bash
cd backend
dotnet run
```
El backend estará disponible en: `http://localhost:5030`

### Ejecutar el Frontend
```bash
cd frontend
dotnet run
```

## 📱 Uso de la Aplicación

### Login
- Usuario por defecto: `admin`
- Contraseña por defecto: `admin123`

### Funcionalidades
1. **Dashboard**: Vista general con estadísticas
2. **Estudiantes**: CRUD completo de estudiantes
3. **Docentes**: CRUD completo de docentes  
4. **Asignaturas**: CRUD completo de asignaturas

## 🏗️ Estructura del Proyecto

```
PlataformaAcademica/
├── backend/                 # API Backend (Carter .NET)
│   ├── src/
│   │   ├── Controllers/     # Controladores de negocio
│   │   ├── Models/          # Modelos de datos
│   │   ├── Repositories/    # Acceso a datos
│   │   ├── Routes/          # Rutas de la API
│   │   └── Config/          # Configuración
│   └── Program.cs           # Punto de entrada
├── frontend/                # Aplicación Desktop (AvaloniaUI)
│   ├── src/
│   │   ├── Views/           # Ventanas y vistas
│   │   ├── Services/        # Servicios (API, Auth)
│   │   ├── Models/          # Modelos de datos
│   │   ├── Elements/        # Componentes UI personalizados
│   │   ├── Layout/          # Layouts (Header, Sidebar)
│   │   └── Config/          # Configuración
│   ├── App.cs               # Aplicación principal
│   └── Program.cs           # Punto de entrada
└── database.sql             # Script de base de datos
```

## 🔧 Configuración

### Backend (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SistemaAcademico;Trusted_Connection=true;"
  }
}
```

### Frontend (AppConfig.cs)
```csharp
public static class AppConfig
{
    public static class Api
    {
        public const string BaseUrl = "http://localhost:5030/api";
    }
}
```

## 🎨 Personalización

### Colores (Colors.cs)
Los colores de la aplicación se pueden personalizar en `frontend/src/Elements/Colors.cs`:

```csharp
public static readonly SolidColorBrush Primary = new(Color.Parse("#2563eb"));
public static readonly SolidColorBrush Success = new(Color.Parse("#10b981"));
// ... más colores
```

## 🐛 Solución de Problemas

### Error de conexión al backend
1. Verificar que el backend esté ejecutándose en el puerto 5030
2. Comprobar la configuración de CORS en `Program.cs`
3. Verificar la URL en `AppConfig.cs`

### Error de base de datos
1. Verificar que SQL Server esté ejecutándose
2. Comprobar la cadena de conexión en `appsettings.json`
3. Ejecutar el script `database.sql`

## 📝 API Endpoints

### Estudiantes
- `GET /api/estudiantes` - Obtener todos los estudiantes
- `GET /api/estudiantes/{id}` - Obtener estudiante por ID
- `POST /api/estudiantes` - Crear estudiante
- `PUT /api/estudiantes/{id}` - Actualizar estudiante
- `DELETE /api/estudiantes/{id}` - Eliminar estudiante

### Docentes
- `GET /api/docentes` - Obtener todos los docentes
- `GET /api/docentes/{id}` - Obtener docente por ID
- `POST /api/docentes` - Crear docente
- `PUT /api/docentes/{id}` - Actualizar docente
- `DELETE /api/docentes/{id}` - Eliminar docente

### Asignaturas
- `GET /api/asignaturas` - Obtener todas las asignaturas
- `GET /api/asignaturas/{id}` - Obtener asignatura por ID
- `POST /api/asignaturas` - Crear asignatura
- `PUT /api/asignaturas/{id}` - Actualizar asignatura
- `DELETE /api/asignaturas/{id}` - Eliminar asignatura

### Usuarios
- `POST /api/usuarios/login` - Iniciar sesión

## 🤝 Contribución

1. Fork el proyecto
2. Crear una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.