# CU02 - Aceptar acuerdo de usuario

## 1. Descripción

Permite al votante aceptar los términos y condiciones antes de emitir su voto.

## 2. Actores

* **Principal:** Votante
* **Secundario(s):** Ninguno

## 3. Precondiciones

* El usuario ha iniciado sesión.
* El acuerdo de usuario está disponible.

## 4. Flujo Principal (Escenario de éxito)

1. El sistema muestra el acuerdo de usuario.
2. El votante revisa el contenido.
3. El votante acepta el acuerdo.
4. El sistema registra la aceptación.

## 5. Flujos Alternativos / Excepciones

* **3a. El usuario no acepta:**

  1. El sistema bloquea la opción de votar.
  2. El flujo termina.

## 6. Postcondiciones

* El usuario queda habilitado para votar.