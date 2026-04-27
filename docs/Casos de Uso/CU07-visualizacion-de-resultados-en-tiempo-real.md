# CU07 - Visualizar resultados en tiempo real

## 1. Descripción

Permite al administrador consultar los resultados de las votaciones en tiempo real.

## 2. Actores

* **Principal:** Administrador
* **Secundario(s):** Sistema de visualización

## 3. Precondiciones

* El administrador ha iniciado sesión.
* Existe al menos una votación activa o finalizada.

## 4. Flujo Principal (Escenario de éxito)

1. El administrador accede a la sección de resultados.
2. El sistema obtiene los datos actualizados.
3. El sistema muestra los resultados en tiempo real.

## 5. Flujos Alternativos / Excepciones

* **2a. Error al obtener datos:**

  1. El sistema muestra un mensaje de error.
  2. El flujo puede reintentarse.

## 6. Postcondiciones

* El administrador visualiza el estado actualizado de la votación.