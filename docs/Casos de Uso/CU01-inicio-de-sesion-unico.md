# CU01 - Inicio de sesión único

## 1. Descripción

Permite a los usuarios autenticarse en el sistema mediante un mecanismo de inicio de sesión único para acceder a las funcionalidades disponibles según su rol.

## 2. Actores

* **Principal:** Votante / Administrador
* **Secundario(s):** Sistema de autenticación externo (si aplica)

## 3. Precondiciones

* El usuario está registrado en el sistema.
* El sistema de autenticación está disponible.

## 4. Flujo Principal (Escenario de éxito)

1. El actor ingresa sus credenciales.
2. El sistema valida las credenciales.
3. El sistema autentica al usuario.
4. El sistema redirige al usuario a su panel correspondiente.

## 5. Flujos Alternativos / Excepciones

* **2a. Credenciales incorrectas:**

  1. El sistema muestra un mensaje de error.
  2. El flujo regresa al paso 1.

## 6. Postcondiciones

* El usuario queda autenticado en el sistema.
* Se inicia una sesión activa.