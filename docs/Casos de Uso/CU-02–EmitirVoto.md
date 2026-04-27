<a name="cu-02"></a>
# CU-02 - Emitir Voto

## 1. Descripción
Permite al votante seleccionar una opción dentro de una votación activa y registrar su voto.

## 2. Actores
* **Principal:** Votante

## 3. Precondiciones
* El usuario está autenticado.
* Existe una votación activa.
* El usuario no ha votado previamente.

## 4. Flujo Principal (Escenario de éxito)
1. El votante accede a las votaciones activas.
2. El sistema muestra las opciones disponibles.
3. El votante selecciona una opción.
4. El sistema registra el voto.
5. El sistema confirma el registro.

## 5. Flujos Alternativos / Excepciones
* **3a. Ya votó previamente:**
  1. El sistema informa que el voto ya fue registrado.

* **1a. No hay votaciones activas:**
  1. El sistema muestra un mensaje informativo.

## 6. Postcondiciones
* El voto queda registrado correctamente.