<div align="right">
    <br><br><br><br><br><br><br><br><br><br>
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
      <td>v1.0</td>
      <td>Emisión inicial del documento</td>
      <td>Lingtin</td>
    </tr>
    <tr>
      <td>2026-04-27</td>
      <td>v1.0</td>
      <td>Especificación de requerimientos del sistema (Diagrama de casos de uso)</td>
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
- **Usuario votante:**<br>Persona que participa en el proceso de votación emitiendo su voto.
- **Administrador del sistema:**<br>Persona encargada de gestionar el sistema de votación, incluyendo la configuración y supervisión del proceso.
- **Auditor:**<br> Persona responsable de revisar y verificar la integridad del proceso de votación y los resultados.
### 3.3 Requerimientos del usuario
- **Votante:**<br>
El sistema de votación electrónica permitirá a los usuarios autenticarse mediante credenciales válidas antes de acceder a sus funcionalidades.<br>
El sistema de votación electrónica mostrará a los usuarios las votaciones disponibles en las que pueden participar.<br>
El sistema de votación electrónica permitirá a cada usuario autenticado emitir un único voto en cada proceso de votación activo.<br><br>
- **Administrador del sistema:**<br>
El sistema de votación electrónica permitirá a los administradores configurar y gestionar los procesos de votación, incluyendo la creación de nuevas votaciones, la definición de opciones de voto y la asignación de permisos a los usuarios.<br>
El sistema de votación electrónica proporcionará a los administradores la visualización de los resultados de las votaciones en tiempo real.<br><br>
- **Auditor:**<br>
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
- **Votante:**<br>Usuario del sistema que participa en las votaciones. Su principal objetivo es emitir su voto en procesos activos de forma segura y confiable. Tiene acceso limitado únicamente a las funcionalidades necesarias para autenticarse y votar.<br> 
- **Administrador:**<br>Usuario responsable de la gestión y control del sistema de votaciones. Se encarga de crear, modificar y administrar las votaciones, así como de supervisar su funcionamiento y consultar los resultados.<br>
- **Auditor:**<br>Usuario encargado de supervisar y verificar la transparencia e integridad del sistema. Su función es revisar los resultados de las votaciones y analizar los registros de actividad, sin intervenir en la gestión ni en la emisión de votos.<br>
- **Servicio de correo(actor externo):**<br>Sistema externo que colabora con la aplicación enviando códigos de verificación (OTP) y notificaciones necesarias para procesos como autenticación y recuperación de contraseña.<br>

## 6.3. Casos de uso:
- **Autenticarse:**<br>Permite al usuario acceder al sistema mediante el ingreso de credenciales y la confirmación mediante OTP (correo eletrónico).
- **Emitir Voto:**<br>Permite al votante seleccionar una opción en una votación activa y registrar su elección de forma segura.<br>
- **Recuperar contraseña**<br>Permite al usuario restablecer su contraseña en caso de haberla olvidado, para recuperar el acceso al sistema.<br>
- **Gestionar votaciones:**<br>Permite al administrador crear, modificar, eliminar y controlar el estado de las votaciones disponibles en el sistema.<br>
- **Visualizar resultados:**<br>Permite al administrador consultar los resultados de las votaciones de manera actualizada.<br>
- **Generar reporte de auditoria:**<br>Permite al auditor revisar los registros del sistema para analizar la actividad y garantizar la integridad del proceso.<br>
## 6.4. Descripción de casos de uso:
Las descripciones detalladas de los casos de uso se encuentran en los siguientes documentos:
- [CU-01: Autenticacion de usuario](https://github.com/juansuarezb/sistema-votacion-web/blob/main/docs/Casos%20de%20Uso/CU-01%E2%80%93Autenticarse.md)<br>
- [CU-02: Emitir Voto](https://github.com/juansuarezb/sistema-votacion-web/blob/main/docs/Casos%20de%20Uso/CU-02%E2%80%93EmitirVoto.md)<br>
- [CU-03: Gestionar votaciones](https://github.com/juansuarezb/sistema-votacion-web/blob/main/docs/Casos%20de%20Uso/CU-03%E2%80%93GestionarVotaciones.md)<br>
- [CU-04: Visualizar resultados](https://github.com/juansuarezb/sistema-votacion-web/blob/main/docs/Casos%20de%20Uso/CU-04%E2%80%93VisualizarResultados.md)<br>
- [CU-05: Recuperar contraseña](https://github.com/juansuarezb/sistema-votacion-web/blob/main/docs/Casos%20de%20Uso/CU-05%E2%80%93RecuperarContrase%C3%B1a.md)<br>
- [CU-06: Auditar actividad](https://github.com/juansuarezb/sistema-votacion-web/blob/main/docs/Casos%20de%20Uso/CU-06%E2%80%93Auditar%20actividad.md)<br>
---

## 7. Evolución del sistema

---

## 8. Apéndices
