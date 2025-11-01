
/********************************************************************************************
    9. CARGA DE INFORMACIÓN DEL EXCEL
    ==========================================================
    Este script crea la estructura base de la base de datos. Para cargar la información
    completa de secciones, subsecciones y requisitos provenientes de la hoja
    "NUEVA MATRIZ REL DIR" del archivo Excel, ejecute a continuación el archivo
    `inserts_operacionales.sql` contenido en la carpeta SQL de este proyecto. Dicho
    archivo contiene los INSERT necesarios para poblar el catálogo OEA con la totalidad
    de los elementos.
********************************************************************************************/

USE OEA_Gestor;
GO

/********************************************************************************************
    2. TABLAS BÁSICAS / SEGURIDAD
*********************************************************************************************/

-- 2.1 PAISES
IF OBJECT_ID('dbo.Paises','U') IS NOT NULL DROP TABLE dbo.Paises;
GO
CREATE TABLE dbo.Paises (
    PaisId          INT IDENTITY(1,1) PRIMARY KEY,
    CodigoIso2      NVARCHAR(5)  NOT NULL,
    Nombre          NVARCHAR(150) NOT NULL,
    Activo          BIT NOT NULL DEFAULT 1,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL
);
GO

-- seed de países mínimos
INSERT INTO dbo.Paises (CodigoIso2, Nombre, Activo, CreadoPor)
VALUES
('GT','Guatemala',1,1),
('SV','El Salvador',1,1),
('HN','Honduras',1,1),
('NI','Nicaragua',1,1),
('CR','Costa Rica',1,1),
('PA','Panamá',1,1);
GO

-- 2.2 PERFILES
IF OBJECT_ID('dbo.Perfiles','U') IS NOT NULL DROP TABLE dbo.Perfiles;
GO
CREATE TABLE dbo.Perfiles (
    PerfilId        INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(100) NOT NULL,
    Descripcion     NVARCHAR(300) NULL,
    EsAdmin         BIT NOT NULL DEFAULT 0,
    Activo          BIT NOT NULL DEFAULT 1,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL
);
GO

INSERT INTO dbo.Perfiles (Nombre, Descripcion, EsAdmin, CreadoPor)
VALUES ('Administrador', 'Perfil con acceso total al sistema', 1, 1),
       ('Evaluador', 'Puede crear y editar evaluaciones', 0, 1),
       ('Cliente', 'Usuario asociado a un cliente, con acceso únicamente a sus evaluaciones', 0, 1);
GO

/********************************************************************************************
    3. CLIENTES
*********************************************************************************************/

IF OBJECT_ID('dbo.Clientes','U') IS NOT NULL DROP TABLE dbo.Clientes;
GO
CREATE TABLE dbo.Clientes (
    ClienteId       INT IDENTITY(1,1) PRIMARY KEY,
    PaisId          INT NOT NULL,
    Nombre          NVARCHAR(200) NOT NULL,
    Nit             NVARCHAR(50) NULL,
    Direccion       NVARCHAR(300) NULL,
    Telefono        NVARCHAR(50) NULL,
    Correo          NVARCHAR(200) NULL,
    Activo          BIT NOT NULL DEFAULT 1,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL,
    CONSTRAINT FK_Clientes_Paises FOREIGN KEY (PaisId) REFERENCES dbo.Paises(PaisId)
);
GO

-- cliente de prueba
INSERT INTO dbo.Clientes (PaisId, Nombre, Nit, Direccion, Telefono, Correo, CreadoPor)
VALUES (1, N'Comercializadora Pahame, S.A.', N'1234567-8', N'Ciudad de Guatemala', N'+502 5555 5555', N'contacto@pahame.gt', 1);
GO
-- 2.3 USUARIOS
IF OBJECT_ID('dbo.Usuarios','U') IS NOT NULL DROP TABLE dbo.Usuarios;
GO
CREATE TABLE dbo.Usuarios (
    UsuarioId       INT IDENTITY(1,1) PRIMARY KEY,
    Usuario         NVARCHAR(100) NOT NULL UNIQUE,
    ClaveHash       NVARCHAR(300) NOT NULL,
    NombreCompleto  NVARCHAR(200) NOT NULL,
    Correo          NVARCHAR(200) NULL,
    PerfilId        INT NOT NULL,
    PaisId          INT NULL,
    ClienteId       INT NULL,
    Activo          BIT NOT NULL DEFAULT 1,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL,
    CONSTRAINT FK_Usuarios_Perfiles FOREIGN KEY (PerfilId) REFERENCES dbo.Perfiles(PerfilId),
    CONSTRAINT FK_Usuarios_Paises   FOREIGN KEY (PaisId)   REFERENCES dbo.Paises(PaisId)
    , CONSTRAINT FK_Usuarios_Clientes FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes(ClienteId)
);
GO

-- usuario admin inicial (clave en texto plano SOLO de ejemplo)
INSERT INTO dbo.Usuarios (Usuario, ClaveHash, NombreCompleto, Correo, PerfilId, PaisId, CreadoPor)
VALUES ('admin', 'admin123$CAMBIAR', 'Administrador del sistema', 'admin@oea.local', 1, 1, 1);
GO



/********************************************************************************************
    2.4 PERMISOS Y RELACIONES PERFIL-PERMISO
*********************************************************************************************/

-- Tabla de permisos
IF OBJECT_ID('dbo.Permisos','U') IS NOT NULL DROP TABLE dbo.Permisos;
GO
CREATE TABLE dbo.Permisos (
    PermisoId      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre         NVARCHAR(100) NOT NULL,
    Descripcion    NVARCHAR(300) NULL,
    Activo         BIT NOT NULL DEFAULT 1,
    CreadoPor      INT NULL,
    CreadoEn       DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor  INT NULL,
    ModificadoEn   DATETIME NULL
);
GO

-- Tabla puente entre perfiles y permisos (clave compuesta)
IF OBJECT_ID('dbo.PerfilPermiso','U') IS NOT NULL DROP TABLE dbo.PerfilPermiso;
GO
CREATE TABLE dbo.PerfilPermiso (
    PerfilId       INT NOT NULL,
    PermisoId      INT NOT NULL,
    CONSTRAINT PK_PerfilPermiso PRIMARY KEY (PerfilId, PermisoId),
    CONSTRAINT FK_PerfilPermiso_Perfil FOREIGN KEY (PerfilId) REFERENCES dbo.Perfiles(PerfilId),
    CONSTRAINT FK_PerfilPermiso_Permiso FOREIGN KEY (PermisoId) REFERENCES dbo.Permisos(PermisoId)
);
GO

-- Semilla de permisos
INSERT INTO dbo.Permisos (Nombre, Descripcion, Activo, CreadoPor)
VALUES
('ManageClients', 'Administrar clientes', 1, 1),
('ManageUsers', 'Administrar usuarios', 1, 1),
('ManageProfiles', 'Administrar perfiles', 1, 1),
('ManagePermissions', 'Administrar permisos', 1, 1),
('ViewEvaluations', 'Ver evaluaciones', 1, 1),
('CreateEvaluations', 'Crear evaluaciones', 1, 1);
GO

-- Asociaciones perfil-permiso (perfilId, permisoId)
-- Perfil 1 (Administrador): todos los permisos
INSERT INTO dbo.PerfilPermiso (PerfilId, PermisoId)
VALUES (1,1),(1,2),(1,3),(1,4),(1,5),(1,6);
-- Perfil 2 (Evaluador): ver y crear evaluaciones
INSERT INTO dbo.PerfilPermiso (PerfilId, PermisoId)
VALUES (2,5),(2,6);
-- Perfil 3 (Cliente): ver y crear evaluaciones para sí mismo
INSERT INTO dbo.PerfilPermiso (PerfilId, PermisoId)
VALUES (3,5),(3,6);
GO

/********************************************************************************************
    4. CATÁLOGO OEA (MULTIPAÍS)
*********************************************************************************************/

-- 4.1 VERSIONES
IF OBJECT_ID('dbo.OEA_Version','U') IS NOT NULL DROP TABLE dbo.OEA_Version;
GO
CREATE TABLE dbo.OEA_Version (
    VersionId       INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(100) NOT NULL,   -- "OEA-Guatemala-2025"
    PaisId          INT NOT NULL,
    VigenteDesde    DATE NOT NULL DEFAULT (GETDATE()),
    EsVigente       BIT NOT NULL DEFAULT 1,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL,
    CONSTRAINT FK_OEA_Version_Paises FOREIGN KEY (PaisId) REFERENCES dbo.Paises(PaisId)
);
GO

-- versión inicial Guatemala
INSERT INTO dbo.OEA_Version (Nombre, PaisId, EsVigente, CreadoPor)
VALUES (N'OEA-GT-2025', 1, 1, 1);
GO

-- 4.2 SECCIONES
IF OBJECT_ID('dbo.OEA_Seccion','U') IS NOT NULL DROP TABLE dbo.OEA_Seccion;
GO
CREATE TABLE dbo.OEA_Seccion (
    SeccionId       INT IDENTITY(1,1) PRIMARY KEY,
    VersionId       INT NOT NULL,
    Nombre          NVARCHAR(200) NOT NULL,
    Descripcion     NVARCHAR(MAX) NULL,
    Orden           INT NOT NULL DEFAULT 1,
    Activo          BIT NOT NULL DEFAULT 1,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL,
    CONSTRAINT FK_OEA_Seccion_Version FOREIGN KEY (VersionId) REFERENCES dbo.OEA_Version(VersionId)
);
GO

-- 4.3 SUBSECCIONES
IF OBJECT_ID('dbo.OEA_Subseccion','U') IS NOT NULL DROP TABLE dbo.OEA_Subseccion;
GO
CREATE TABLE dbo.OEA_Subseccion (
    SubseccionId    INT IDENTITY(1,1) PRIMARY KEY,
    SeccionId       INT NOT NULL,
    Nombre          NVARCHAR(200) NOT NULL,
    Descripcion     NVARCHAR(MAX) NULL,
    Orden           INT NOT NULL DEFAULT 1,
    Activo          BIT NOT NULL DEFAULT 1,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL,
    CONSTRAINT FK_OEA_Subseccion_Seccion FOREIGN KEY (SeccionId) REFERENCES dbo.OEA_Seccion(SeccionId)
);
GO

-- 4.4 REQUISITOS
IF OBJECT_ID('dbo.OEA_Requisito','U') IS NOT NULL DROP TABLE dbo.OEA_Requisito;
GO
CREATE TABLE dbo.OEA_Requisito (
    RequisitoId     INT IDENTITY(1,1) PRIMARY KEY,
    SubseccionId    INT NOT NULL,
    Codigo          NVARCHAR(50) NULL,
    Descripcion     NVARCHAR(MAX) NOT NULL,
    Orden           INT NOT NULL DEFAULT 1,
    Activo          BIT NOT NULL DEFAULT 1,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL,
    CONSTRAINT FK_OEA_Requisito_Subseccion FOREIGN KEY (SubseccionId) REFERENCES dbo.OEA_Subseccion(SubseccionId)
);
GO

/********************************************************************************************
    5. EVALUACIONES
*********************************************************************************************/

IF OBJECT_ID('dbo.OEA_Evaluacion','U') IS NOT NULL DROP TABLE dbo.OEA_Evaluacion;
GO
CREATE TABLE dbo.OEA_Evaluacion (
    EvaluacionId    INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId       INT NOT NULL,
    VersionId       INT NOT NULL,
    UsuarioId       INT NOT NULL,    -- quién creó
    FechaEval       DATETIME NOT NULL DEFAULT (GETDATE()),
    ObservacionGral NVARCHAR(MAX) NULL,
    Estado          NVARCHAR(30) NOT NULL DEFAULT N'CERRADA',  -- o EN_PROCESO
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL,
    CONSTRAINT FK_OEA_Evaluacion_Cliente FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes(ClienteId),
    CONSTRAINT FK_OEA_Evaluacion_Version FOREIGN KEY (VersionId) REFERENCES dbo.OEA_Version(VersionId),
    CONSTRAINT FK_OEA_Evaluacion_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(UsuarioId)
);
GO

IF OBJECT_ID('dbo.OEA_EvaluacionDetalle','U') IS NOT NULL DROP TABLE dbo.OEA_EvaluacionDetalle;
GO
CREATE TABLE dbo.OEA_EvaluacionDetalle (
    EvaluacionDetId INT IDENTITY(1,1) PRIMARY KEY,
    EvaluacionId    INT NOT NULL,
    RequisitoId     INT NOT NULL,
    Estado          TINYINT NOT NULL DEFAULT 0,  -- 0 = no evaluado, 1 = cumple, 2 = parcial, 3 = no cumple
    Observaciones   NVARCHAR(MAX) NULL,
    EvidenciaUrl    NVARCHAR(500) NULL,
    CreadoPor       INT NULL,
    CreadoEn        DATETIME NOT NULL DEFAULT (GETDATE()),
    ModificadoPor   INT NULL,
    ModificadoEn    DATETIME NULL,
    CONSTRAINT FK_OEA_EvalDet_Eval FOREIGN KEY (EvaluacionId) REFERENCES dbo.OEA_Evaluacion(EvaluacionId),
    CONSTRAINT FK_OEA_EvalDet_Req  FOREIGN KEY (RequisitoId)  REFERENCES dbo.OEA_Requisito(RequisitoId),
    CONSTRAINT UQ_OEA_EvalDet UNIQUE (EvaluacionId, RequisitoId)
);
GO

/********************************************************************************************
    6. IMPORTAR 100% DEL EXCEL
    ==========================================================
    NOTA: Este bloque explica cómo importar los requisitos desde un archivo
    Excel. Ajuste la ruta del archivo y la hoja según corresponda. El archivo
    debe estar disponible en el servidor de bases de datos y requiere que
    OPENROWSET esté habilitado. El comando está comentado para que pueda
    activarlo manualmente.
*********************************************************************************************/

-- HABILITAR AD HOC SOLO SI NO LO TIENES (ejecuta una sola vez)
-- EXEC sp_configure 'show advanced options', 1;
-- RECONFIGURE;
-- EXEC sp_configure 'Ad Hoc Distributed Queries', 1;
-- RECONFIGURE;
-- GO

/*
-- 6.1 Carga bruta del Excel a una tabla staging
-- Asegúrate de que el archivo esté accesible desde el servidor SQL (ruta de servidor)

IF OBJECT_ID('dbo.Stg_MatrizOEA','U') IS NOT NULL DROP TABLE dbo.Stg_MatrizOEA;
GO

CREATE TABLE dbo.Stg_MatrizOEA (
    Pais           NVARCHAR(150) NULL,
    Seccion        NVARCHAR(500) NULL,
    Subseccion     NVARCHAR(500) NULL,
    Codigo         NVARCHAR(100) NULL,
    Requisito      NVARCHAR(MAX) NULL
);
GO

-- Importa el contenido del archivo Excel
INSERT INTO dbo.Stg_MatrizOEA (Seccion, Subseccion, Codigo, Requisito)
SELECT
    [Seccion],
    [Subseccion],
    [Codigo],
    [Requisito]
FROM OPENROWSET(
    'Microsoft.ACE.OLEDB.12.0',
    'Excel 12.0;HDR=YES;Database=C:\\Import\\250611 - Matriz de controles operacionales - diagnostico inicial.xlsx;',
    'SELECT * FROM [Hoja1$]'
);
GO

-- 6.2 Pasar de staging a secciones/subsecciones/requisitos

DECLARE @VersionGT INT = (SELECT TOP 1 VersionId FROM dbo.OEA_Version WHERE PaisId = 1 AND EsVigente = 1 ORDER BY VersionId);

;WITH DistSecciones AS (
    SELECT DISTINCT LTRIM(RTRIM(Seccion)) AS Seccion
    FROM dbo.Stg_MatrizOEA
    WHERE ISNULL(Seccion,'') <> ''
)
INSERT INTO dbo.OEA_Seccion (VersionId, Nombre, Descripcion, Orden, Activo, CreadoPor)
SELECT
    @VersionGT,
    s.Seccion,
    N'Importado desde Excel',
    ROW_NUMBER() OVER (ORDER BY s.Seccion),
    1,
    1
FROM DistSecciones s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.OEA_Seccion x
    WHERE x.VersionId = @VersionGT AND x.Nombre = s.Seccion
);

;WITH DistSub AS (
    SELECT DISTINCT LTRIM(RTRIM(Seccion)) AS Seccion,
                    LTRIM(RTRIM(Subseccion)) AS Subseccion
    FROM dbo.Stg_MatrizOEA
    WHERE ISNULL(Subseccion,'') <> ''
)
INSERT INTO dbo.OEA_Subseccion (SeccionId, Nombre, Descripcion, Orden, Activo, CreadoPor)
SELECT
    sct.SeccionId,
    ds.Subseccion,
    N'Importado desde Excel',
    ROW_NUMBER() OVER (PARTITION BY sct.SeccionId ORDER BY ds.Subseccion),
    1,
    1
FROM DistSub ds
INNER JOIN dbo.OEA_Seccion sct
    ON sct.VersionId = @VersionGT
   AND sct.Nombre = ds.Seccion
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.OEA_Subseccion ss
    WHERE ss.SeccionId = sct.SeccionId AND ss.Nombre = ds.Subseccion
);

-- Finalmente los requisitos
INSERT INTO dbo.OEA_Requisito (SubseccionId, Codigo, Descripcion, Orden, Activo, CreadoPor)
SELECT
    sub.SubseccionId,
    NULLIF(LTRIM(RTRIM(s.Codigo)), ''),
    s.Requisito,
    1,
    1,
    1
FROM dbo.Stg_MatrizOEA s
INNER JOIN dbo.OEA_Seccion sec
    ON sec.VersionId = @VersionGT
   AND sec.Nombre = LTRIM(RTRIM(s.Seccion))
LEFT JOIN dbo.OEA_Subseccion sub
    ON sub.SeccionId = sec.SeccionId
   AND sub.Nombre = LTRIM(RTRIM(s.Subseccion))
WHERE ISNULL(s.Requisito,'') <> '';
GO
*/
