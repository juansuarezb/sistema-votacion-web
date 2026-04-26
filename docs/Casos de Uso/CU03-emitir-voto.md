# CU03 - Emitir voto

## 1. Descripción

Permite al votante seleccionar una opción y registrar su voto en el sistema.

## 2. Actores

* **Principal:** Votante
* **Secundario(s):** Ninguno

## 3. Precondiciones

* El usuario ha iniciado sesión.
* El usuario ha aceptado el acuerdo.
* Existe una votación activa.

## 4. Flujo Principal (Escenario de éxito)

1. El votante accede a la votación.
2. El sistema muestra las opciones disponibles.
3. El votante selecciona una opción.
4. El sistema valida la selección.
5. El sistema registra el voto.
6. El sistema redirige a la confirmación.

## 5. Flujos Alternativos / Excepciones

* **4a. Selección inválida:**

  1. El sistema muestra un mensaje de error.
  2. El flujo regresa al paso 2.

* **5a. Error al registrar el voto:**

  1. El sistema muestra un mensaje de error.
  2. El flujo termina o se reintenta.

## 6. Postcondiciones

* El voto queda registrado en el sistema.
* Se actualizan los resultados.