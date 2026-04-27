<a name="cu-03"></a>
# CU-03 - Gestionar votaciones

## 1. Descripción
Permite al administrador crear, modificar, eliminar y controlar el estado de las votaciones.

## 2. Actores
* **Principal:** Administrador

## 3. Precondiciones
* El administrador está autenticado.

## 4. Flujo Principal (Escenario de éxito)
1. El administrador accede al módulo de gestión.
2. El sistema muestra las votaciones existentes.
3. El administrador selecciona una acción (crear, editar, eliminar, publicar o cerrar).
4. El sistema solicita los datos necesarios.
5. El administrador ingresa o modifica la información.
6. El sistema valida y guarda los cambios.
7. El sistema actualiza la información.

## 5. Flujos Alternativos / Excepciones
* **5a. Datos inválidos:**
  1. El sistema muestra errores.
  2. El administrador corrige la información.

## 6. Postcondiciones
* Las votaciones quedan actualizadas.