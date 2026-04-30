<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ taglib prefix="c" uri="jakarta.tags.core"%>
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Listar Votantes</title>
</head>
<body>
	<div>
		<a href="">Nuevo</a>
	</div>
	<table>
		<thead>
			<tr>
				<th>Id Votante</th>
				<th>Nombre</th>
				<th>Correo electrónico</th>
				<th>Contraseña</th>
				<th>HaVotado</th>
				<th>Acciones</th>
			</tr>
		</thead>
		<tbody>
			<tr>
				<td>1</td>
				<td>Juan</td>
				<td>juan@gmail.com</td>
				<td>false</td>
				<td><a href="">Actualizar</a> / <a href="">Eliminar</a></td>
			</tr>
		</tbody>
	</table>
</body>
</html>