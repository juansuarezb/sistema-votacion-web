-- ================================================
-- VotoSeguro - Script DDL
-- Base de datos: votoseguro
-- Motor: MySQL 8.0
-- ================================================

-- ------------------------------------------------
-- BASE DE DATOS
-- ------------------------------------------------

-- ------------------------------------------------
-- TABLA: rol
-- ------------------------------------------------
CREATE TABLE rol (
    id_rol      INT AUTO_INCREMENT PRIMARY KEY,
    nombre      VARCHAR(30) NOT NULL UNIQUE
);

-- ------------------------------------------------
-- TABLA: usuario
-- ------------------------------------------------
CREATE TABLE usuario (
    id_usuario          INT AUTO_INCREMENT PRIMARY KEY,
    nombre              VARCHAR(100) NOT NULL,
    correo_electronico  VARCHAR(100) NOT NULL UNIQUE,
    contrasena          VARCHAR(255) NOT NULL,
    id_rol              INT NOT NULL,
    CONSTRAINT fk_usuario_rol
        FOREIGN KEY (id_rol) REFERENCES rol(id_rol)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);

-- ------------------------------------------------
-- TABLA: administrador
-- ------------------------------------------------
CREATE TABLE administrador (
    id_usuario      INT PRIMARY KEY,
    nivel_acceso    VARCHAR(50) NOT NULL,
    CONSTRAINT fk_admin_usuario
        FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- ------------------------------------------------
-- TABLA: votante
-- ------------------------------------------------
CREATE TABLE votante (
    id_usuario  INT PRIMARY KEY,
    CONSTRAINT fk_votante_usuario
        FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- ------------------------------------------------
-- TABLA: auditor
-- ------------------------------------------------
CREATE TABLE auditor (
    id_usuario  INT PRIMARY KEY,
    CONSTRAINT fk_auditor_usuario
        FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- ------------------------------------------------
-- TABLA: tipo_voto
-- ------------------------------------------------
CREATE TABLE tipo_voto (
    id_tipo_voto    INT AUTO_INCREMENT PRIMARY KEY,
    nombre          VARCHAR(20) NOT NULL UNIQUE
);

-- ------------------------------------------------
-- TABLA: votacion
-- ------------------------------------------------
CREATE TABLE votacion (
    id_votacion     INT AUTO_INCREMENT PRIMARY KEY,
    titulo          VARCHAR(200) NOT NULL,
    descripcion     TEXT,
    fecha_inicio    DATE NOT NULL,
    fecha_cierre    DATE NOT NULL,
    id_admin        INT NOT NULL,
    CONSTRAINT fk_votacion_admin
        FOREIGN KEY (id_admin) REFERENCES administrador(id_usuario)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT chk_fechas
        CHECK (fecha_cierre > fecha_inicio)
);

-- ------------------------------------------------
-- TABLA: votacion_votante
-- ------------------------------------------------
CREATE TABLE votacion_votante (
    id_votacion INT NOT NULL,
    id_votante  INT NOT NULL,
    ha_votado   BOOLEAN NOT NULL DEFAULT FALSE,
    PRIMARY KEY (id_votacion, id_votante),
    CONSTRAINT fk_vv_votacion
        FOREIGN KEY (id_votacion) REFERENCES votacion(id_votacion)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_vv_votante
        FOREIGN KEY (id_votante) REFERENCES votante(id_usuario)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- ------------------------------------------------
-- TABLA: voto
-- ------------------------------------------------
CREATE TABLE voto (
    id_voto         INT AUTO_INCREMENT PRIMARY KEY,
    fecha           TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    id_votacion     INT NOT NULL,
    id_tipo_voto    INT NOT NULL,
    CONSTRAINT fk_voto_votacion
        FOREIGN KEY (id_votacion) REFERENCES votacion(id_votacion)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT fk_voto_tipo
        FOREIGN KEY (id_tipo_voto) REFERENCES tipo_voto(id_tipo_voto)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);

-- ------------------------------------------------
-- TABLA: escrutinio
-- ------------------------------------------------
CREATE TABLE escrutinio (
    id_escrutinio           INT AUTO_INCREMENT PRIMARY KEY,
    total_votos_emitidos    INT NOT NULL DEFAULT 0,
    porcentaje_participacion DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    fecha_hora_cierre       TIMESTAMP NULL,
    id_votacion             INT NOT NULL UNIQUE,
    CONSTRAINT fk_escrutinio_votacion
        FOREIGN KEY (id_votacion) REFERENCES votacion(id_votacion)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- ------------------------------------------------
-- TABLA: escrutinio_detalle
-- ------------------------------------------------
CREATE TABLE escrutinio_detalle (
    id_escrutinio   INT NOT NULL,
    id_tipo_voto    INT NOT NULL,
    cantidad        INT NOT NULL DEFAULT 0,
    PRIMARY KEY (id_escrutinio, id_tipo_voto),
    CONSTRAINT fk_ed_escrutinio
        FOREIGN KEY (id_escrutinio) REFERENCES escrutinio(id_escrutinio)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_ed_tipo_voto
        FOREIGN KEY (id_tipo_voto) REFERENCES tipo_voto(id_tipo_voto)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);

-- ================================================
-- ÍNDICES
-- ================================================
CREATE INDEX idx_usuario_correo
    ON usuario(correo_electronico);

CREATE INDEX idx_voto_votacion
    ON voto(id_votacion);

CREATE INDEX idx_votacion_admin
    ON votacion(id_admin);

CREATE INDEX idx_votacion_fechas
    ON votacion(fecha_inicio, fecha_cierre);

-- ================================================
-- DATOS INICIALES
-- ================================================

-- Roles
INSERT INTO rol (nombre) VALUES
    ('VOTANTE'),
    ('ADMINISTRADOR'),
    ('AUDITOR');

-- Tipos de voto
INSERT INTO tipo_voto (nombre) VALUES
    ('SI'),
    ('NO'),
    ('BLANCO'),
    ('NULO');

-- Admin Principal
INSERT INTO usuario (nombre, correo_electronico, contrasena, id_rol)
VALUES ('Admin Principal', 'admin@votoseguro.com', 'admin123', 2);

INSERT INTO administrador (id_usuario, nivel_acceso)
VALUES (LAST_INSERT_ID(), 'SUPER');

-- Votantes
INSERT INTO usuario (nombre, correo_electronico, contrasena, id_rol)
VALUES ('Juan', 'juan@gmail.com', '1234', 1);
INSERT INTO votante (id_usuario) VALUES (LAST_INSERT_ID());

INSERT INTO usuario (nombre, correo_electronico, contrasena, id_rol)
VALUES ('Abi', 'abi@gmail.com', '1234', 1);
INSERT INTO votante (id_usuario) VALUES (LAST_INSERT_ID());

INSERT INTO usuario (nombre, correo_electronico, contrasena, id_rol)
VALUES ('Michi', 'michi@gmail.com', '1234', 1);
INSERT INTO votante (id_usuario) VALUES (LAST_INSERT_ID());

INSERT INTO usuario (nombre, correo_electronico, contrasena, id_rol)
VALUES ('Gipsi', 'gipsi@gmail.com', '1234', 1);
INSERT INTO votante (id_usuario) VALUES (LAST_INSERT_ID());

-- Auditor
INSERT INTO usuario (nombre, correo_electronico, contrasena, id_rol)
VALUES ('Auditor Principal', 'auditor@votoseguro.com', 'auditor123', 3);
INSERT INTO auditor (id_usuario) VALUES (LAST_INSERT_ID());

-- ================================================
-- VERIFICACIÓN
-- ================================================
SELECT 'rol' AS tabla, COUNT(*) AS registros FROM rol
UNION ALL
SELECT 'usuario', COUNT(*) FROM usuario
UNION ALL
SELECT 'administrador', COUNT(*) FROM administrador
UNION ALL
SELECT 'votante', COUNT(*) FROM votante
UNION ALL
SELECT 'auditor', COUNT(*) FROM auditor
UNION ALL
SELECT 'tipo_voto', COUNT(*) FROM tipo_voto;

INSERT INTO votacion (
    titulo,
    descripcion,
    fecha_inicio,
    fecha_cierre,
    id_admin
)
VALUES (
    'Elección Consejo',
    'Periodo 2026',
    '2026-05-16',
    '2026-05-20',
    1
);

INSERT INTO votacion_votante (
    id_votacion,
    id_votante,
    ha_votado
)
VALUES (
    1,
    2,
    FALSE
);
