# CU08 - Gestionar soporte al cliente

## 1. Descripción

Permite al actor Soporte al cliente gestionar las solicitudes enviadas por los votantes, incluyendo su revisión, respuesta y seguimiento, con el fin de resolver incidencias o dudas relacionadas con el sistema de votación.

## 2. Actores

* **Principal:** Soporte al cliente
* **Secundario(s):** Votante, Sistema

## 3. Precondiciones

* El actor Soporte al cliente ha iniciado sesión en el sistema. *(incluye CU01)*
* Existen solicitudes de soporte registradas.

## 4. Flujo Principal (Escenario de éxito)

1. El actor Soporte accede al módulo de soporte.
2. El sistema muestra la lista de solicitudes pendientes.
3. El actor selecciona una solicitud.
4. El sistema muestra los detalles de la solicitud.
5. El actor analiza el caso y redacta una respuesta.
6. El actor envía la respuesta.
7. El sistema registra la respuesta y actualiza el estado de la solicitud.
8. El sistema notifica al votante.

## 5. Flujos Alternativos / Excepciones

* **2a. No existen solicitudes:**

  1. El sistema muestra un mensaje indicando que no hay solicitudes pendientes.
  2. El flujo termina.

* **5a. Información insuficiente en la solicitud:**

  1. El actor solicita más información al votante.
  2. El sistema registra la solicitud de información.
  3. El estado cambia a “En espera”.
  4. El flujo queda en pausa hasta recibir respuesta.

* **6a. Error al enviar la respuesta:**

  1. El sistema muestra un mensaje de error.
  2. El actor puede reintentar el envío.

## 6. Postcondiciones

* La solicitud de soporte queda actualizada con una respuesta o en estado de seguimiento.
* El votante recibe una notificación sobre el estado de su solicitud.