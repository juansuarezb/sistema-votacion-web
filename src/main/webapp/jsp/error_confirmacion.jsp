<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
    <%@ taglib prefix="c" uri="jakarta.tags.core" %>
        <!DOCTYPE html>
        <html>

        <head>
            <meta charset="UTF-8">
            <title>VotoSeguro - Confirmación</title>
        </head>

        <body>

            <nav>
                <h1>VotoSeguro</h1>
            </nav>

            <h2>Confirmación</h2>
            <p>
                <c:out value="${mensaje}" />
            </p>

            <a href="${accionAceptar}">Aceptar</a>
            <a href="${accionCancelar}">Cancelar</a>

        </body>

        </html>