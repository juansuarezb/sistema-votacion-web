<!DOCTYPE html>
<html lang="es">

<head>
	<meta charset="UTF-8">
	<title>VotoSeguro - Login</title>
	<style>
		* {
			box-sizing: border-box;
			margin: 0;
			padding: 0;
			font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
		}

		body {
			background-color: #fff;
			color: #000;
			min-height: 100vh;
			padding: 40px;
		}

		header {
			font-size: 32px;
			font-weight: bold;
			margin-bottom: 40px;
		}

		.container {
			display: flex;
			gap: 40px;
			height: 500px;
		}

		.window-mock {
			flex: 1;
			border: 1px solid #000;
			position: relative;
		}

		.dots {
			position: absolute;
			top: 10px;
			left: 10px;
			font-size: 20px;
			letter-spacing: 2px;
			color: #666;
		}

		.login-box {
			flex: 1;
			border: 1px solid #000;
			display: flex;
			flex-direction: column;
			align-items: center;
			justify-content: center;
			padding: 40px;
		}

		h1 {
			font-size: 36px;
			margin-bottom: 40px;
			font-weight: normal;
		}

		.form-group {
			width: 100%;
			max-width: 350px;
			margin-bottom: 20px;
		}

		input[type="text"],
		input[type="password"] {
			width: 100%;
			padding: 15px;
			border: 1px solid #666;
			font-size: 16px;
			outline: none;
		}

		input::placeholder {
			color: #999;
		}

		.btn-submit {
			width: 100%;
			max-width: 350px;
			padding: 15px;
			background-color: #000;
			color: #fff;
			border: none;
			font-size: 16px;
			cursor: pointer;
			margin-top: 10px;
		}

		.btn-submit:hover {
			background-color: #333;
		}
	</style>
</head>

<body>
	<header>VOTOSEGURO</header>
	<div class="container">
		<div class="window-mock">
			<div class="dots">ooo</div>
		</div>
		<div class="login-box">
			<h1>Log In</h1>
			<div class="form-group">
				<input type="text" placeholder="Correo electrónico">
			</div>
			<div class="form-group">
				<input type="password" placeholder="Contraseña">
			</div>
			<button class="btn-submit" onclick="window.location.href='../votante/lista_votaciones_activas.jsp'">Iniciar
				sesión</button>
		</div>
	</div>
</body>

</html>