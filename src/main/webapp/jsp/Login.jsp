<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ taglib prefix="c" uri="jakarta.tags.core"%>
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Login</title>
</head>
<body>
	<form method="POST" action="${pageContext.request.contextPath}/AutenticarController?ruta=ingresar">
		<fieldset>
			<legend>Login</legend>
			<label>Usuario:<br></label> <input type="text" name="correo" /><br>
			<label>Password:<br></label> <input type="password"
				name="contraseña" /><br>
			<input type="submit" value="Ingresar" />
		</fieldset>

	</form>
</body>
</html>