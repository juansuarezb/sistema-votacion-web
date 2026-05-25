<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
    <%@ taglib prefix="c" uri="jakarta.tags.core" %>
        <!DOCTYPE html>
        <html>

        <head>
            <meta charset="UTF-8">
            <title>VotoSeguro - Error</title>
        </head>

        <body>

            <nav>
                <h1>VotoSeguro</h1>
            </nav>

            <h2>Error de autenticación</h2>
            <p>
                <c:out value="${error}" />
            </p>

            <a href="jsp/Login.jsp">Regresar</a>

        </body>

        </html>