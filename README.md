# Sistema de Votación Web
## Tecnologías

![Java](https://img.shields.io/badge/Java-21-ED8B00?style=flat-square&logo=openjdk&logoColor=white)
![Jakarta EE](https://img.shields.io/badge/Jakarta_EE-10-4E9BCD?style=flat-square&logo=jakarta&logoColor=white)
![Apache Tomcat](https://img.shields.io/badge/Apache_Tomcat-10.1-F8DC75?style=flat-square&logo=apachetomcat&logoColor=black)
![Maven](https://img.shields.io/badge/Maven-3.9-C71A36?style=flat-square&logo=apachemaven&logoColor=white)
![JSP](https://img.shields.io/badge/JSP-3.1-007396?style=flat-square&logo=java&logoColor=white)
![JSTL](https://img.shields.io/badge/JSTL-3.0-6DB33F?style=flat-square&logo=java&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-24-2496ED?style=flat-square&logo=docker&logoColor=white)
![HTML5](https://img.shields.io/badge/HTML5-5-E34F26?style=flat-square&logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-3-1572B6?style=flat-square&logo=css3&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-ES6-F7DF1E?style=flat-square&logo=javascript&logoColor=black)
![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat-square&logo=github&logoColor=white)
![Visual Paradigm](https://img.shields.io/badge/Visual_Paradigm-18.0-FF6B35?style=flat-square&logoColor=white)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/juansuarezb/sistema-votacion-web)

## Descripción
>[!NOTE]
>Aplicación web para la gestión de votaciones electrónicas, con autenticación de usuarios y visualización de resultados en tiempo real.
---
## Planteamiento del problema
>[!NOTE]
>En la actualidad, muchos procesos de votación aún dependen de métodos tradicionales que pueden resultar ineficientes, lentos y propensos a errores humanos o manipulación. Además, la falta de acceso remoto y de resultados en tiempo real limita la participación de los usuarios y dificulta la transparencia del proceso. Ante esta problemática, surge la necesidad de desarrollar un sistema de votación web que permita gestionar elecciones de manera segura, accesible y confiable, garantizando la autenticación de los usuarios, la integridad de los votos y la visualización inmediata de los resultados.
---
## Objetivo general
>[!NOTE]
>
>Desarrollar una aplicación web de votación electrónica que permita a usuarios autenticados emitir votos de forma segura, mientras que los administradores puedan gestionar las votaciones y visualizar los resultados en tiempo real.
---
## Alcance
>[!NOTE]
>**Incluye**:<br>
> - Módulo de inicio de sesión único.
> - Módulo de Votación.
> - Pantalla de registro de votación exitoso.
> - Cifrado, protección y privatización total del voto.
> - Pantalla de resultados de votación general en tiempo real.<br>

---
## Contribuidores
> [!NOTE]
> [![May-CR](https://img.shields.io/badge/May--CR-Emilia_Guachamin-black?style=flat-square&logo=github)](https://github.com/May-CR)
> [![P4bloMaldonado](https://img.shields.io/badge/P4bloMaldonado-Pablo_Maldonado-black?style=flat-square&logo=github)](https://github.com/P4bloMaldonado)
> [![XLex0](https://img.shields.io/badge/XLex0-AlexRedes-black?style=flat-square&logo=github)](https://github.com/XLex0)
> [![KealPetu](https://img.shields.io/badge/KealPetu-Kevin_Peñafiel-black?style=flat-square&logo=github)](https://github.com/KealPetu)
> [![juansuarezb](https://img.shields.io/badge/juansuarezb-Juan_Suárez-black?style=flat-square&logo=github)](https://github.com/juansuarezb)
> [![Dsmcamila](https://img.shields.io/badge/Dsmcamila-Domenica_Sánchez-black?style=flat-square&logo=github)](https://github.com/DomenicaSanchez)
> [![greyhatbat](https://img.shields.io/badge/greyhatbat-Álvaro_Zumbana-black?style=flat-square&logo=github)](https://github.com/greyhatbat)
---

## 🐳 Despliegue con Docker

> [!IMPORTANT]
> Requisitos previos:
>
> * Docker Desktop instalado y en ejecución.
> * Git instalado.
> * Puertos `8080` y `3306` disponibles.

---

### 1. Clonar el repositorio

```bash
git clone https://github.com/juansuarezb/sistema-votacion-web.git
cd sistema-votacion-web
```

---

### 2. Configurar variables de entorno

Copiar el archivo `.env.example`:

```bash
cp .env.example .env
```

O crear manualmente el archivo:

```plaintext
.env
```

---

### 3. Levantar el entorno completo

```bash
docker compose up -d --build
```

Este comando automáticamente:

* Compila el proyecto con Maven.
* Genera el archivo `.war`.
* Construye la imagen Docker.
* Levanta el contenedor MySQL.
* Ejecuta el script SQL de inicialización.
* Despliega la aplicación en Apache Tomcat.

---

### 4. Acceder a la aplicación

Aplicación web:

```txt
http://localhost:8080/
```

---

### 5. Ver logs

Aplicación:

```bash
docker logs -f votoseguro-app
```

Base de datos:

```bash
docker logs -f votoseguro-db
```

---

### 6. Detener el entorno

```bash
docker compose down
```

Eliminar también el volumen de MySQL:

```bash
docker compose down -v
```

---

### 7. Reinicio limpio de la base de datos

> [!WARNING]
> Esto eliminará todos los datos almacenados.

```bash
docker compose down -v
docker compose up -d --build
```

---

## Estructura Docker

```plaintext
.
├── Dockerfile
├── compose.yml
├── .env
├── init/
│   └── votoseguro.sql
```

---

## Inicialización automática de MySQL

El contenedor MySQL ejecuta automáticamente los scripts SQL ubicados en:

```plaintext
init/
```

Esto permite:

* Crear tablas.
* Insertar datos semilla.
* Configurar usuarios iniciales.
* Preparar el entorno automáticamente.

---

##  Usuarios de prueba

| Rol           | Correo                                                  | Contraseña |
| ------------- | ------------------------------------------------------- | ---------- |
| Administrador | [admin@votoseguro.com](mailto:admin@votoseguro.com)     | admin123   |
| Votante       | [juan@gmail.com](mailto:juan@gmail.com)                 | 1234       |
| Auditor       | [auditor@votoseguro.com](mailto:auditor@votoseguro.com) | auditor123 |


## Metodología de trabajo
Se utiliza Kanban mediante GitHub Projects para la gestión de tareas.

## Flujo de Trabajo

Se utiliza una estrategia basada en Git Flow simplificado.

### Ramas principales
- `main` → versión estable
- `develop` → integración de desarrollo

### Ramas de funcionalidades
- `feature/login`
- `feature/dashboard`
- `feature/votacion`

### Flujo de desarrollo
1. Crear Issue.
2. Crear rama `feature/*`.
3. Desarrollar funcionalidad.
4. Crear Pull Request hacia `develop`.
5. Realizar revisión.
6. Integrar cambios.
7. Generar nueva imagen Docker estable si aplica.



## Notas
Este proyecto se desarrolla como parte de las materias de Aplicaciones Web Avanzadas  y Desarrollo de Software Seguro.
