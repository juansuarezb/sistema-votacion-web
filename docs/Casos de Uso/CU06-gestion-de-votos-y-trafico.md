# CU06 - Gestionar votaciones y tráfico

## 1. Descripción

Permite al administrador configurar, supervisar y controlar las votaciones y el tráfico del sistema.

## 2. Actores

* **Principal:** Administrador
* **Secundario(s):** Sistema

## 3. Precondiciones

* El administrador ha iniciado sesión.

## 4. Flujo Principal (Escenario de éxito)

1. El administrador accede al panel de gestión.
2. El sistema muestra las votaciones activas.
3. El administrador configura o monitorea la votación.
4. El sistema guarda los cambios.

## 5. Flujos Alternativos / Excepciones

* **3a. Error en configuración:**

  1. El sistema muestra un mensaje de error.
  2. El flujo regresa al paso 2.

## 6. Postcondiciones

* Las configuraciones de votación quedan actualizadas.