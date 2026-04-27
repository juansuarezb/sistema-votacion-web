<a name="cu-04"></a>
# CU-04 - Visualizar resultados

## 1. Descripción
Permite a los usuarios autorizados consultar los resultados de las votaciones con actualización en tiempo real.

## 2. Actores
* **Principal:** Administrador / Auditor

## 3. Precondiciones
* El usuario está autenticado.

## 4. Flujo Principal (Escenario de éxito)
1. El usuario accede a la sección de resultados.
2. El sistema muestra los resultados actuales.
3. El sistema actualiza automáticamente los resultados en tiempo real.

## 5. Flujos Alternativos / Excepciones
* **2a. No hay resultados disponibles:**
  1. El sistema informa la ausencia de datos.

## 6. Postcondiciones
* El usuario visualiza resultados actualizados