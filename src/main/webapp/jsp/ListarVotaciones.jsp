<%@ page language="java" contentType="text/html; charset=UTF-8"
	pageEncoding="UTF-8"%>
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
<title>VotoSeguro - Listar Votaciones</title>
</head>
<body>

	<nav>
		<h1>VotoSeguro</h1>
		<span>Bienvenido, ${sessionScope.autorizado.nombre}</span> <a
			href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
			Cerrar sesión </a>
	</nav>

	<h2>Gestión de Votaciones</h2>

	<a href="GestionarVotacionesController?ruta=nuevo">Nueva Votación</a>

	<table border="1">
		<thead>
			<tr>
				<th>ID</th>
				<th>Título</th>
				<th>Descripción</th>
				<th>Fecha Inicio</th>
				<th>Fecha Cierre</th>
				<th>Acciones</th>
			</tr>
		</thead>
		<tbody>
			<c:forEach var="votacion" items="${votaciones}">
				<tr>
					<td>${votacion.idVotacion}</td>
					<td>${votacion.titulo}</td>
					<td>${votacion.descripcion}</td>
					<td>${votacion.fechaInicio}</td>
					<td>${votacion.fechaCierre}</td>
					<td>
						<a
						href="GestionarVotacionesController?ruta=modificar&id=${votacion.idVotacion}">Editar</a>
						<a
						href="GestionarVotacionesController?ruta=eliminar&id=${votacion.idVotacion}">Eliminar</a>
						<a
						href="GestionarVotacionesController?ruta=verResultados&id=${votacion.idVotacion}">
							Ver Resultados </a>
						<a href="GestionarVotacionesController?ruta=asignar&id=${votacion.idVotacion}">
    					Asignar Votantes
						</a>	
					</td>
				</tr>
			</c:forEach>
		</tbody>
	</table>

</body>
</html>