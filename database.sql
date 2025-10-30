-- Base de datos para Sistema de Gestión Académica
-- PostgreSQL

-- Crear la base de datos
CREATE DATABASE sistema_academico;

-- Conectar a la base de datos
\c sistema_academico;

-- ============================================
-- 1. TABLA DE USUARIOS Y ROLES
-- ============================================
CREATE TABLE usuarios (
    id_usuario SERIAL PRIMARY KEY,
    nombre_usuario VARCHAR(50) UNIQUE NOT NULL,
    contrasena VARCHAR(255) NOT NULL, -- almacenar hash
    rol VARCHAR(20) NOT NULL CHECK (rol IN ('administrador', 'docente', 'secretario')),
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- 2. TABLA DE ESTUDIANTES
-- ============================================
CREATE TABLE estudiantes (
    id_estudiante SERIAL PRIMARY KEY,
    documento VARCHAR(20) UNIQUE NOT NULL,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    direccion VARCHAR(200),
    telefono VARCHAR(15),
    email VARCHAR(100),
    foto_url VARCHAR(255),
    activo BOOLEAN DEFAULT true,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- 3. TABLA DE DOCENTES
-- ============================================
CREATE TABLE docentes (
    id_docente SERIAL PRIMARY KEY,
    documento VARCHAR(20) UNIQUE NOT NULL,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    especialidad VARCHAR(100),
    telefono VARCHAR(15),
    email VARCHAR(100),
    foto_url VARCHAR(255),
    id_usuario INTEGER REFERENCES usuarios(id_usuario),
    activo BOOLEAN DEFAULT true,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- 4. TABLA DE GRADOS
-- ============================================
CREATE TABLE grados (
    id_grado SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL, -- ej: "1ro Básico", "2do Año"
    nivel VARCHAR(50), -- ej: "Primaria", "Secundaria", "Bachillerato"
    seccion VARCHAR(10), -- ej: "A", "B", "C"
    activo BOOLEAN DEFAULT true
);

-- ============================================
-- 5. TABLA DE ASIGNATURAS
-- ============================================
CREATE TABLE asignaturas (
    id_asignatura SERIAL PRIMARY KEY,
    codigo VARCHAR(20) UNIQUE NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    creditos INTEGER DEFAULT 1,
    activo BOOLEAN DEFAULT true
);

-- ============================================
-- 6. TABLA DE ASIGNACIÓN DOCENTE-ASIGNATURA-GRADO
-- ============================================
CREATE TABLE asignaciones (
    id_asignacion SERIAL PRIMARY KEY,
    id_docente INTEGER NOT NULL REFERENCES docentes(id_docente),
    id_asignatura INTEGER NOT NULL REFERENCES asignaturas(id_asignatura),
    id_grado INTEGER NOT NULL REFERENCES grados(id_grado),
    ciclo_escolar VARCHAR(10) NOT NULL, -- ej: "2024", "2024-2025"
    activo BOOLEAN DEFAULT true,
    UNIQUE(id_docente, id_asignatura, id_grado, ciclo_escolar)
);

-- ============================================
-- 7. TABLA DE MATRÍCULAS
-- ============================================
CREATE TABLE matriculas (
    id_matricula SERIAL PRIMARY KEY,
    id_estudiante INTEGER NOT NULL REFERENCES estudiantes(id_estudiante),
    id_grado INTEGER NOT NULL REFERENCES grados(id_grado),
    ciclo_escolar VARCHAR(10) NOT NULL,
    fecha_matricula DATE DEFAULT CURRENT_DATE,
    estado VARCHAR(20) DEFAULT 'activo' CHECK (estado IN ('activo', 'retirado', 'finalizado')),
    UNIQUE(id_estudiante, ciclo_escolar)
);

-- ============================================
-- 8. TABLA DE INSCRIPCIONES (estudiante-asignatura)
-- ============================================
CREATE TABLE inscripciones (
    id_inscripcion SERIAL PRIMARY KEY,
    id_matricula INTEGER NOT NULL REFERENCES matriculas(id_matricula),
    id_asignatura INTEGER NOT NULL REFERENCES asignaturas(id_asignatura),
    UNIQUE(id_matricula, id_asignatura)
);

-- ============================================
-- 9. TABLA DE CALIFICACIONES
-- ============================================
CREATE TABLE calificaciones (
    id_calificacion SERIAL PRIMARY KEY,
    id_inscripcion INTEGER NOT NULL REFERENCES inscripciones(id_inscripcion),
    periodo VARCHAR(20) NOT NULL, -- ej: "Parcial 1", "Parcial 2", "Final"
    nota DECIMAL(5,2) NOT NULL CHECK (nota >= 0 AND nota <= 100),
    fecha_registro DATE DEFAULT CURRENT_DATE,
    observaciones TEXT,
    UNIQUE(id_inscripcion, periodo)
);

-- ============================================
-- ÍNDICES PARA MEJORAR RENDIMIENTO
-- ============================================
CREATE INDEX idx_estudiantes_documento ON estudiantes(documento);
CREATE INDEX idx_docentes_documento ON docentes(documento);
CREATE INDEX idx_matriculas_estudiante ON matriculas(id_estudiante);
CREATE INDEX idx_matriculas_ciclo ON matriculas(ciclo_escolar);
CREATE INDEX idx_calificaciones_inscripcion ON calificaciones(id_inscripcion);

-- ============================================
-- VISTA: Resumen de estudiantes con grado actual
-- ============================================
CREATE VIEW v_estudiantes_activos AS
SELECT 
    e.id_estudiante,
    e.documento,
    CONCAT(e.nombres, ' ', e.apellidos) AS nombre_completo,
    e.email,
    e.telefono,
    g.nombre AS grado,
    g.seccion,
    m.ciclo_escolar
FROM estudiantes e
INNER JOIN matriculas m ON e.id_estudiante = m.id_estudiante
INNER JOIN grados g ON m.id_grado = g.id_grado
WHERE e.activo = true AND m.estado = 'activo';

-- ============================================
-- VISTA: Calificaciones con promedios
-- ============================================
CREATE VIEW v_calificaciones_estudiantes AS
SELECT 
    e.id_estudiante,
    CONCAT(e.nombres, ' ', e.apellidos) AS estudiante,
    a.nombre AS asignatura,
    c.periodo,
    c.nota,
    m.ciclo_escolar,
    CASE 
        WHEN c.nota >= 60 THEN 'Aprobado'
        ELSE 'Reprobado'
    END AS estado
FROM estudiantes e
INNER JOIN matriculas m ON e.id_estudiante = m.id_estudiante
INNER JOIN inscripciones i ON m.id_matricula = i.id_matricula
INNER JOIN asignaturas a ON i.id_asignatura = a.id_asignatura
INNER JOIN calificaciones c ON i.id_inscripcion = c.id_inscripcion;

-- ============================================
-- DATOS DE PRUEBA
-- ============================================

-- Usuarios
INSERT INTO usuarios (nombre_usuario, contrasena, rol) VALUES
('admin', '$2a$10$abcdefghijklmnopqrstuv', 'administrador'),
('prof_garcia', '$2a$10$abcdefghijklmnopqrstuv', 'docente'),
('secretaria', '$2a$10$abcdefghijklmnopqrstuv', 'secretario');

-- Grados
INSERT INTO grados (nombre, nivel, seccion) VALUES
('1ro Básico', 'Básica', 'A'),
('2do Básico', 'Básica', 'A'),
('1ro Bachillerato', 'Bachillerato', 'A');

-- Asignaturas
INSERT INTO asignaturas (codigo, nombre, descripcion, creditos) VALUES
('MAT-101', 'Matemáticas', 'Matemáticas básicas', 5),
('ESP-101', 'Español', 'Lenguaje y literatura', 4),
('CIE-101', 'Ciencias Naturales', 'Biología y química básica', 4),
('HIS-101', 'Historia', 'Historia nacional', 3);

-- Docentes
INSERT INTO docentes (documento, nombres, apellidos, especialidad, email, id_usuario) VALUES
('12345678', 'Juan', 'García', 'Matemáticas', 'jgarcia@escuela.edu', 2);

-- Estudiantes
INSERT INTO estudiantes (documento, nombres, apellidos, fecha_nacimiento, email, telefono) VALUES
('98765432', 'María', 'López', '2010-05-15', 'mlopez@estudiante.edu', '7777-8888'),
('98765433', 'Carlos', 'Martínez', '2010-08-20', 'cmartinez@estudiante.edu', '7777-9999');

-- Matrículas
INSERT INTO matriculas (id_estudiante, id_grado, ciclo_escolar) VALUES
(1, 1, '2025'),
(2, 1, '2025');

-- Asignaciones docente-asignatura-grado
INSERT INTO asignaciones (id_docente, id_asignatura, id_grado, ciclo_escolar) VALUES
(1, 1, 1, '2025');

-- Inscripciones
INSERT INTO inscripciones (id_matricula, id_asignatura) VALUES
(1, 1), (1, 2),
(2, 1), (2, 2);

-- Calificaciones
INSERT INTO calificaciones (id_inscripcion, periodo, nota) VALUES
(1, 'Parcial 1', 85.5),
(1, 'Parcial 2', 90.0),
(2, 'Parcial 1', 75.0);

-- ============================================
-- FUNCIÓN: Calcular promedio de estudiante
-- ============================================
CREATE OR REPLACE FUNCTION calcular_promedio_estudiante(
    p_id_estudiante INTEGER,
    p_ciclo_escolar VARCHAR
) RETURNS DECIMAL(5,2) AS $$
DECLARE
    promedio DECIMAL(5,2);
BEGIN
    SELECT AVG(c.nota) INTO promedio
    FROM calificaciones c
    INNER JOIN inscripciones i ON c.id_inscripcion = i.id_inscripcion
    INNER JOIN matriculas m ON i.id_matricula = m.id_matricula
    WHERE m.id_estudiante = p_id_estudiante 
    AND m.ciclo_escolar = p_ciclo_escolar;
    
    RETURN COALESCE(promedio, 0);
END;
$$ LANGUAGE plpgsql;

-- Ejemplo de uso: SELECT calcular_promedio_estudiante(1, '2025');