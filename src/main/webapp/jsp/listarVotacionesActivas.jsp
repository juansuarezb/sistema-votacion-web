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
    <title>VotoSeguro - Votaciones Activas</title>
</head>
<body>

    <nav>
        <h1>VotoSeguro</h1>
        <span>Bienvenido, ${sessionScope.autorizado.nombre}</span>
        <a href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
            Cerrar sesión
        </a>
    </nav>

    <h2>Votaciones Activas</h2>

    <table border="1">
        <thead>
            <tr>
                <th>Título</th>
                <th>Descripción</th>
                <th>Fecha Inicio</th>
                <th>Fecha Cierre</th>
                <th>Acción</th>
            </tr>
        </thead>
        <tbody>
            <c:forEach var="votacion" items="${votaciones}">
                <tr>
                    <td><c:out value="${votacion.titulo}"/></td>
                    <td><c:out value="${votacion.descripcion}"/></td>
                    <td>${votacion.fechaInicio}</td>
                    <td>${votacion.fechaCierre}</td>
                    <td>
                        <c:choose>
                            <c:when test="${votante.puedeVotar(votacion.idVotacion)}">
                                <a href="EmitirVotoController?ruta=votar&id=${votacion.idVotacion}">
                                    Votar
                                </a>
                            </c:when>
                            <c:otherwise>
                                <span>Ya votaste</span>
                            </c:otherwise>
                        </c:choose>
                    </td>
                </tr>
            </c:forEach>
        </tbody>
    </table>

</body>
</html>