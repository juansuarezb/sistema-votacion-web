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
    <title>VotoSeguro - Resultados</title>
    <!-- Auto-refresh cada 5 segundos para simular tiempo real -->
    <meta http-equiv="refresh" content="5">
</head>
<body>

    <nav>
        <h1>VotoSeguro</h1>
        <span>Bienvenido, ${sessionScope.autorizado.nombre}</span>
        <a href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
            Cerrar sesión
        </a>
    </nav>

    <h2>Resultados: <c:out value="${votacion.titulo}"/></h2>
    <p><c:out value="${votacion.descripcion}"/></p>
    <p>Fecha inicio: ${votacion.fechaInicio} | Fecha cierre: ${votacion.fechaCierre}</p>

    <hr/>

    <h3>Escrutinio</h3>

    <table border="1">
        <thead>
            <tr>
                <th>Opción</th>
                <th>Votos</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Sí</td>
                <td>${escrutinio.votosSi}</td>
            </tr>
            <tr>
                <td>No</td>
                <td>${escrutinio.votosNo}</td>
            </tr>
            <tr>
                <td>Blanco</td>
                <td>${escrutinio.votosBlanco}</td>
            </tr>
            <tr>
                <td>Nulo</td>
                <td>${escrutinio.votosNulo}</td>
            </tr>
        </tbody>
        <tfoot>
            <tr>
                <td><strong>Total</strong></td>
                <td><strong>${escrutinio.totalVotosEmitidos}</strong></td>
            </tr>
        </tfoot>
    </table>

    <br/>

    <p>Participación: ${escrutinio.porcentajeParticipacion}%</p>
    <p>Última actualización: ${escrutinio.fechaHoraCierre}</p>

    <br/>

    <a href="${pageContext.request.contextPath}/GestionarVotacionesController?ruta=listarVotaciones">
        Volver
    </a>

</body>
</html>