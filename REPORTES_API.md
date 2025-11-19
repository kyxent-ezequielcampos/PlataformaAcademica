# API de Reportes - Sistema Académico

## Endpoints Disponibles

### 1. Reporte de Notas de Estudiante (PDF)

**Endpoint:** `GET /api/reportes/notas/{idEstudiante}`

**Parámetros:**
- `idEstudiante` (path): ID del estudiante
- `cicloEscolar` (query): Ciclo escolar (ej: "2025")

**Ejemplo:**
```
GET http://localhost:5130/api/reportes/notas/1?cicloEscolar=2025
```

**Respuesta:** Archivo PDF con las calificaciones del estudiante

---

### 2. Listado de Matrículas (PDF)

**Endpoint:** `GET /api/reportes/matriculas`

**Parámetros:**
- `cicloEscolar` (query, requerido): Ciclo escolar
- `idGrado` (query, opcional): Filtrar por grado específico

**Ejemplos:**
```
# Todas las matrículas del ciclo
GET http://localhost:5130/api/reportes/matriculas?cicloEscolar=2025

# Matrículas de un grado específico
GET http://localhost:5130/api/reportes/matriculas?cicloEscolar=2025&idGrado=1
```

**Respuesta:** Archivo PDF con el listado de matrículas

---

### 3. Vista Previa de Notas (JSON)

**Endpoint:** `GET /api/reportes/notas/{idEstudiante}/preview`

**Ejemplo:**
```
GET http://localhost:5130/api/reportes/notas/1/preview?cicloEscolar=2025
```

---

### 4. Vista Previa de Matrículas (JSON)

**Endpoint:** `GET /api/reportes/matriculas/preview`

**Ejemplo:**
```
GET http://localhost:5130/api/reportes/matriculas/preview?cicloEscolar=2025
```

## Características

✅ Generación de PDFs profesionales con QuestPDF
✅ Reporte de notas con promedio general
✅ Listado completo de matrículas
✅ Filtros por ciclo escolar y grado
✅ Vista previa en JSON antes de generar PDF

## Uso en el Frontend

### Vista de Calificaciones
- Botón "📄 Generar Reporte" en la parte superior
- Permite seleccionar estudiante y ciclo escolar
- Descarga PDF con todas las calificaciones del estudiante

### Vista de Matrículas
- Botón "📄 Generar Reporte" en la parte superior
- Permite filtrar por ciclo escolar y grado (opcional)
- Descarga PDF con listado completo de matrículas

## Cómo Probar

1. Iniciar el backend:
```bash
cd backend
dotnet run
```

2. Iniciar el frontend:
```bash
cd frontend
dotnet run
```

3. En la aplicación:
   - Ir a "Calificaciones" → Click en "📄 Generar Reporte"
   - Ir a "Matrículas" → Click en "📄 Generar Reporte"
