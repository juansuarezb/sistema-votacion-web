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
    <title>VotoSeguro - Asignar Votantes</title>
</head>
<body>

    <nav>
        <h1>VotoSeguro</h1>
        <span>Bienvenido, ${sessionScope.autorizado.nombre}</span>
        <a href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
            Cerrar sesión
        </a>
    </nav>

    <h2>Asignar Votantes a: <c:out value="${votacion.titulo}"/></h2>

    <form action="${pageContext.request.contextPath}/GestionarVotacionesController?ruta=guardarAsignacion" method="post">

        <input type="hidden" name="idVotacion" value="${votacion.idVotacion}"/>

        <table border="1">
            <thead>
                <tr>
                    <th>Seleccionar</th>
                    <th>ID</th>
                    <th>Nombre</th>
                    <th>Correo</th>
                </tr>
            </thead>
            <tbody>
                <c:forEach var="votante" items="${votantes}">
                    <tr>
                        <td>
                            <input type="checkbox" name="votantes" 
                                value="${votante.idUsuario}"
                                <c:if test="${votacion.votantesAsignados.contains(votante.idUsuario)}">
                                    checked
                                </c:if>
                            />
                        </td>
                        <td>${votante.idUsuario}</td>
                        <td><c:out value="${votante.nombre}"/></td>
                        <td><c:out value="${votante.correoElectronico}"/></td>
                    </tr>
                </c:forEach>
            </tbody>
        </table>

        <br/>

        <button type="submit">Guardar asignación</button>
        <a href="${pageContext.request.contextPath}/GestionarVotacionesController?ruta=listarVotaciones">
            Cancelar
        </a>

    </form>

</body>
</html>