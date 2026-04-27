<a name="cu-01"></a>
# CU-01 - Autenticarse

## 1. Descripción
Permite a los usuarios acceder al sistema mediante la validación de sus credenciales, pudiendo incluir verificación adicional mediante código OTP enviado al correo.

## 2. Actores
* **Principal:** Votante / Administrador / Auditor
* **Secundario(s):** Sistema de correo (si se utiliza OTP)

## 3. Precondiciones
* El usuario está registrado en el sistema.

## 4. Flujo Principal (Escenario de éxito)
1. El actor ingresa sus credenciales.
2. El sistema valida las credenciales del usuario.
3. El sistema genera un código OTP.
4. El sistema envía el código OTP al correo del usuario.
5. El usuario ingresa el código OTP.
6. El sistema valida el código OTP.
7. El sistema autentica al usuario.
8. El sistema redirige al usuario a su panel correspondiente.

## 5. Flujos Alternativos / Excepciones
* **2a. Credenciales incorrectas:**
  1. El sistema muestra un mensaje de error.
  2. El flujo regresa al paso 1.

* **5a. Código OTP incorrecto:**
  1. El sistema muestra un error.
  2. Permite reintentar desde el paso 5.

* **Extensión – Recuperar contraseña:**
  1. El usuario selecciona “¿Olvidó su contraseña?”.
  2. Se ejecuta el caso de uso CU-05–Recuperar Contraseña.

## 6. Postcondiciones
* El usuario queda autenticado en el sistema.
* Se inicia una sesión activa.