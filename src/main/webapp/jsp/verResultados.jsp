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
<title>VotoSeguro - Resultados</title>
</head>
<body>

	<nav>
		<h1>VotoSeguro</h1>
		<span>Bienvenido, ${sessionScope.autorizado.nombre}</span> <a
			href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
			Cerrar sesión </a>
	</nav>

	<h2>
		Resultados:
		<c:out value="${votacion.titulo}" />
	</h2>
	<p>
		<c:out value="${votacion.descripcion}" />
	</p>
	<p>Fecha inicio: ${votacion.fechaInicio} | Fecha cierre:
		${votacion.fechaCierre}</p>

	<hr />

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
				<td id="votosSi">${escrutinio.votosSi}</td>
			</tr>
			<tr>
				<td>No</td>
				<td id="votosNo">${escrutinio.votosNo}</td>
			</tr>
			<tr>
				<td>Blanco</td>
				<td id="votosBlanco">${escrutinio.votosBlanco}</td>
			</tr>
			<tr>
				<td>Nulo</td>
				<td id="votosNulo">${escrutinio.votosNulo}</td>
			</tr>
		</tbody>
		<tfoot>
			<tr>
				<td><strong>Total</strong></td>
				<td><strong id="totalVotos">${escrutinio.totalVotosEmitidos}</strong></td>
			</tr>
		</tfoot>
	</table>

	<br />
	<p>
		Participación: <span id="participacion">${escrutinio.porcentajeParticipacion}</span>%
	</p>
	<p>
		Última actualización: <span id="ultimaActualizacion"></span>
	</p>
	<p>
		Estado: <span id="estadoWS">Conectando...</span>
	</p>

	<br />
	<a
		href="${pageContext.request.contextPath}/GestionarVotacionesController?ruta=listarVotaciones">
		Volver </a>
	<p>ID Votación: ${votacion.idVotacion}</p>
```html
<script>

    const idVotacion = "<c:out value='${votacion.idVotacion}'/>";

    console.log("ID:", idVotacion);

    const wsUrl = "ws://localhost:8080/resultados/" + idVotacion;

    console.log(wsUrl);

    const ws = new WebSocket(wsUrl);

    ws.onopen = function() {

        console.log("WebSocket conectado");

        document.getElementById('estadoWS').textContent = '🟢 En vivo';
    };

    ws.onmessage = function(event) {

        console.log("Mensaje recibido:", event.data);

        const data = JSON.parse(event.data);

        document.getElementById('votosSi').textContent = data.si;
        document.getElementById('votosNo').textContent = data.no;
        document.getElementById('votosBlanco').textContent = data.blanco;
        document.getElementById('votosNulo').textContent = data.nulo;

        document.getElementById('totalVotos').textContent = data.total;

        document.getElementById('participacion').textContent =
            data.participacion.toFixed(2);

        document.getElementById('ultimaActualizacion').textContent =
            new Date().toLocaleTimeString();
    };

    ws.onclose = function() {

        console.log("WebSocket desconectado");

        document.getElementById('estadoWS').textContent =
            '🔴 Desconectado';
    };

    ws.onerror = function(error) {

        console.error("Error WebSocket:", error);

        document.getElementById('estadoWS').textContent =
            '🔴 Error de conexión';
    };

</script>

</body>
</html>