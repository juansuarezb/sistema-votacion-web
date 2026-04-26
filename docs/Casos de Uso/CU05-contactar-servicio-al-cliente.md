# CU05 - Contactar servicio al cliente

## 1. Descripción

Permite al votante comunicarse con el servicio de soporte para resolver dudas o problemas.

## 2. Actores

* **Principal:** Votante
* **Secundario(s):** Servicio de soporte

## 3. Precondiciones

* El sistema está disponible.

## 4. Flujo Principal (Escenario de éxito)

1. El votante accede a la opción de contacto.
2. El sistema muestra un formulario o medio de contacto.
3. El votante envía su consulta.
4. El sistema registra la solicitud.

## 5. Flujos Alternativos / Excepciones

* **3a. Información incompleta:**

  1. El sistema solicita completar los campos.
  2. El flujo regresa al paso 2.

## 6. Postcondiciones

* La solicitud de soporte queda registrada.