<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
	<%@ taglib prefix="c" uri="jakarta.tags.core" %>
		<!DOCTYPE html>
		<html>

		<head>
			<meta charset="UTF-8">
			<title>Actualizar Usuario</title>
		</head>

		<body>
			<h1>Actualizar Usuario</h1>
			<form method="POST" action="GestionarVotantesController">
				<input type="hidden" name="txtId" id="txtId" />
				<label for="txtNombre">Nombre</label>
				<input type="text" name="txtNombre" id="txtNombre" />
				<label for="txtEmail">Correo electrónico</label>
				<input type="email" name="txtEmail" id="txtEmail" />
				<label for="txtContraseña">Contraseña</label>
				<input type="password" name="txtContraseña" id="txtContraseña" />
				<label for="txtEstado">Ha votado</label>
				<input type="checkbox" name="txtEstadoSi" id="txtEstadoSi" value="Si">/
				<input type="checkbox" name="txtEstadoNo" id="txtEstadoNo" value="No">/
				<br><br>
				<input type="submit" value="Guardar" />
			</form>
		</body>

		</html>