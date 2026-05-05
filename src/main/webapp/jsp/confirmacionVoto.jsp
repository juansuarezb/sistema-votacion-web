<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%
    if (session.getAttribute("autorizado") == null) {
        response.sendRedirect(request.getContextPath() + "/jsp/Login.jsp");
        return;
    }
%>
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>VotoSeguro - Voto Registrado</title>
</head>
<body>

    <nav>
        <h1>VotoSeguro</h1>
    </nav>

    <h2>¡Su voto ha sido registrado exitosamente!</h2>
    <p>Gracias por participar en VotoSeguro.</p>

    <a href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
        Cerrar sesión
    </a>

</body>
</html>