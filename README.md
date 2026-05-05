# Sistema de Votación Web

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

## 🐳 Despliegue con Docker (Manual)

>[!IMPORTANT]
> Requisitos Previos:<br>
> - Clonar este repositorio: git clone [https://github.com/tu-usuario/VotoSeguro.git](https://github.com/tu-usuario/VotoSeguro.git)
> - Docker Desktop instalado y en ejecución.<br>
> - Java JDK 21 y Maven (para compilar el proyecto).<br>
> - **Preparación del Artefacto:**<br>
> Antes de construir la imagen de Docker, es necesario generar el archivo ejecutable (.war) que contiene toda la lógica del sistema, incluyendo las dependencias de EclipseLink.
> 
> ``` Bash
> # En la raíz del proyecto
> mvn clean package
> 
> ```
> Verifica que el archivo VotoSeguro.war se haya generado correctamente en la carpeta /target.


### 1. Construir la Imagen
Este comando crea una imagen local llamada `votoseguro-app` basada en **Tomcat 10.1** y **JDK 21**, empaquetando el archivo `.war` generado previamente.

```bash
docker build -t votoseguro-app .  
```

### 2. Ejecutar el Contenedor
Este comando instancia el contenedor, mapea el puerto 8080 y le asigna un nombre para facilitar su gestión.

```Bash
docker run -d -p 8080:8080 --name contenedor-voto-seguro votoseguro-app
```

### 3. Verificación
URL de acceso: http://localhost:8080/

Logs: Para revisar que el despliegue de Tomcat fue exitoso, usa:

```Bash
docker logs -f contenedor-voto-seguro
```

## Tecnologías (tentativo)


---

## Metodología de trabajo
Se utiliza Kanban mediante GitHub Projects para la gestión de tareas.

Flujo de trabajo:
- Issues para tareas
- Ramas `feature/*`
- Pull Requests para integración

---

## Estructura de ramas
- `main`: versión estable
- `develop`: integración
- `feature/*`: desarrollo de funcionalidades

---



## Notas
Este proyecto se desarrolla como parte de las materias de Aplicaciones Web Avanzadas  y Desarrollo de Software Seguro.
