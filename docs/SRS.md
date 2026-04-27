<div align="right">
    <br><br><br><br><br><br><br><br><br><br><br><br><br>
    <h1>VotoSeguro</h1><br>
    <h2>Especificación de Requerimientos de Software (SRS)</h2>
    <h2>Versión 1.0</h2>
</div>

---
<div align="center" style="margin-top: 50px; margin-bottom: 50px;">
    <h2>Historial de versiones</h2>
    <table>
  <thead>
    <tr>
      <th>Fecha</th>
      <th>Versión</th>
      <th>Descripción</th>
      <th>Autor</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>2026-04-23</td>
      <td>v1.0.0</td>
      <td>Emisión inicial del documento</td>
      <td>Lingtin</td>
    </tr>
    <tr>
      <td></td>
      <td></td>
      <td></td>
      <td></td>
    </tr>
  </tbody>
</table>
</div>


---

## 1. Introducción
### 1.1 Propósito
Este documento S.R.S. describe el comportamiento y las funcionalidades del sistema "VotoSeguro".
### 1.2 Alcance
El documento aplica a la primera versión (v1.0) del sistema "VotoSeguro".
### 1.3 Definiciones, acrónimos y abreviaturas

---

## 3. Definición de requerimientos del usuario
### 3.1 Descripción general
En esta sección se describen los servicios que el sistema proporciona a cada tipo de usuario. Los requerimientos están expresados en lenguaje natural para facilitar su comprensión por parte de los stakeholders.
### 3.2 Tipos de usuarios
- Usuario votante: Persona que participa en el proceso de votación emitiendo su voto.
- Administrador del sistema: Persona encargada de gestionar el sistema de votación, incluyendo la configuración y supervisión del proceso.
- Auditor: Persona responsable de revisar y verificar la integridad del proceso de votación y los resultados.
### 3.3 Requerimientos del usuario
- Votante:<br>
El sistema de votación electrónica permitirá a los usuarios autenticarse mediante credenciales válidas antes de acceder a sus funcionalidades.<br>
El sistema de votación electrónica mostrará a los usuarios las votaciones disponibles en las que pueden participar.<br>
El sistema de votación electrónica permitirá a cada usuario autenticado emitir un único voto en cada proceso de votación activo.<br><br>
- Administrador del sistema::<br>
El sistema de votación electrónica permitirá a los administradores configurar y gestionar los procesos de votación, incluyendo la creación de nuevas votaciones, la definición de opciones de voto y la asignación de permisos a los usuarios.<br>
El sistema de votación electrónica proporcionará a los administradores la visualización de los resultados de las votaciones en tiempo real.<br><br>
- Auditor::<br>
El sistema de votación electrónica permitirá a los auditores acceder a registros detallados de las votaciones, incluyendo información sobre los votos emitidos y los resultados, para garantizar la transparencia y la integridad del proceso de votación.<br>

---

## 4. Arquitectura del sistema

---

## 5. Especificación de requerimientos del sistema

---

## 6. Modelos del sistema
## 6.1. Diagrama de casos de uso
Se presenta el diagrama de casos de uso que describen las interacciones entre los usuarios y el sistema.<br>
![Diagrama](https://raw.githubusercontent.com/juansuarezb/sistema-votacion-web/main/docs/Casos%20de%20Uso/CasosDeUso.png)

## 6.2. Actores:
Votante
Administrador
Auditor
Base de Datos
Servicio de correo

## 6.3. Descripción de casos de uso:
Las descripciones detalladas de los casos de uso se encuentran en los siguientes documentos:
- [CU-01: Autenticacion de usuario](./casos-uso/CU01-inicio-de-sesion-unico.md)
---

## 7. Evolución del sistema

---

## 8. Apéndices
