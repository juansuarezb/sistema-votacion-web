<a name="cu-06"></a>
# CU-06 - Auditar actividad

## 1. Descripción
Permite al auditor revisar los registros del sistema para verificar la actividad y garantizar la transparencia.

## 2. Actores
* **Principal:** Auditor

## 3. Precondiciones
* El auditor está autenticado.

## 4. Flujo Principal (Escenario de éxito)
1. El auditor accede al módulo de auditoría.
2. El sistema muestra los registros del sistema.
3. El auditor revisa la información.
4. El auditor puede generar un reporte.

## 5. Flujos Alternativos / Excepciones
* **2a. No hay registros:**
  1. El sistema informa que no existen datos.

## 6. Postcondiciones
* El auditor obtiene información del sistema.