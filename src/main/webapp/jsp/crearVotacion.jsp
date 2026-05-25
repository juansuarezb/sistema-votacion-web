<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ taglib prefix="c" uri="jakarta.tags.core"%>
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
    <title>VotoSeguro - Crear Votación</title>
</head>
<body>

    <nav>
        <h1>VotoSeguro</h1>
        <a href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
            Cerrar sesión
        </a>
    </nav>

    <h2>Nueva Votación</h2>

    <form action="${pageContext.request.contextPath}/GestionarVotacionesController?ruta=guardar" method="post">

        <label>Título:</label>
        <input type="text" name="titulo" required/>

        <br/>

        <label>Descripción:</label>
        <textarea name="descripcion" required></textarea>

        <br/>

        <label>Fecha de inicio:</label>
        <input type="date" name="fechaInicio" required/>

        <br/>

        <label>Fecha de cierre:</label>
        <input type="date" name="fechaCierre" required/>

        <br/>

        <button type="submit">Guardar</button>
        <a href="${pageContext.request.contextPath}/GestionarVotacionesController?ruta=listarVotaciones">
            Cancelar
        </a>

    </form>

</body>
</html>