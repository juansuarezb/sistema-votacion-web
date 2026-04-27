<a name="cu-05"></a>
# CU-05 - Recuperar contraseña

## 1. Descripción
Permite al usuario restablecer su contraseña mediante verificación con código OTP enviado al correo.

## 2. Actores
* **Principal:** Votante / Administrador / Auditor
* **Secundario(s):** Sistema de correo

## 3. Precondiciones
* El usuario está registrado.

## 4. Flujo Principal (Escenario de éxito)
1. El usuario solicita recuperar contraseña.
2. El sistema solicita el correo electrónico.
3. El sistema genera un código OTP.
4. El sistema envía el código al correo.
5. El usuario ingresa el código.
6. El sistema valida el código.
7. El usuario ingresa una nueva contraseña.
8. El sistema actualiza la contraseña.

## 5. Flujos Alternativos / Excepciones
* **5a. Código incorrecto:**
  1. El sistema muestra error.
  2. Permite reintentar.

## 6. Postcondiciones
* La contraseña queda actualizada.