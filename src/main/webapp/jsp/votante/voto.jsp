<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <title>VotoSeguro - Emitir Voto</title>
    <style>
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
            font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
        }

        body {
            display: flex;
            min-height: 100vh;
            background-color: #fff;
        }

        /* Sidebar Layout - Reutilizado para consistencia de la plataforma */
        .sidebar {
            width: 300px;
            background-color: #000;
            color: #fff;
            display: flex;
            flex-direction: column;
        }

        .sidebar-header {
            padding: 30px;
            font-size: 28px;
            font-weight: bold;
        }

        .user-profile {
            padding: 20px 30px;
            display: flex;
            align-items: center;
            gap: 15px;
            border-bottom: 1px solid #333;
        }

        .avatar {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            border: 1px solid #fff;
        }

        .user-info {
            font-size: 16px;
            font-weight: bold;
        }

        .menu-group {
            margin-top: 30px;
            flex: 1;
        }

        .menu-item {
            padding: 15px 30px;
            display: block;
            color: #fff;
            text-decoration: none;
            font-size: 18px;
            font-weight: bold;
        }

        .menu-item.active {
            background-color: #444;
        }

        .logout {
            border-top: 1px solid #333;
            padding: 25px 30px;
            font-size: 18px;
            font-weight: bold;
            color: #fff;
            text-decoration: none;
            display: block;
        }

        /* Box Content (Wireframe Votación) */
        .content {
            flex: 1;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 40px;
        }

        .vote-box {
            border: 1px solid #000;
            width: 100%;
            max-width: 700px;
            padding: 50px;
            position: relative;
            border-radius: 4px;
        }

        h2 {
            font-size: 20px;
            font-weight: normal;
            margin-bottom: 40px;
        }

        .radio-group {
            display: flex;
            flex-direction: column;
            gap: 30px;
            margin-bottom: 80px;
            margin-left: 20px;
        }

        .radio-label {
            display: flex;
            align-items: center;
            gap: 20px;
            font-size: 20px;
            cursor: pointer;
        }

        input[type="radio"] {
            width: 24px;
            height: 24px;
            cursor: pointer;
            accent-color: #000;
        }

        .btn-next {
            position: absolute;
            bottom: 50px;
            right: 50px;
            padding: 10px 40px;
            border: 1px solid #000;
            background: transparent;
            font-size: 18px;
            cursor: pointer;
        }

        .btn-next:hover {
            background: #f0f0f0;
        }
    </style>
</head>

<body>
    <div class="sidebar">
        <div class="sidebar-header">VOTOSEGURO</div>
        <div class="user-profile">
            <div class="avatar"></div>
            <div class="user-info">correo<br>nombre</div>
        </div>
        <div class="menu-group"><a href="#" class="menu-item active">Votación Activa</a></div>
        <a href="login.jsp" class="logout">Logout</a>
    </div>

    <div class="content">
        <div class="vote-box">
            <h2>Question 1.</h2>
            <form action="${pageContext.request.contextPath}/EmitirVotoController" method="post">

                <input type="hidden" name="ruta" value="confirmar">

                <input type="hidden" name="idVotacion" value="${votacion.idVotacion}">

                <div class="radio-group">

                    <label class="radio-label">
                        <input type="radio" name="opcion" value="SI">
                        Sí
                    </label>

                    <label class="radio-label">
                        <input type="radio" name="opcion" value="NO">
                        No
                    </label>

                </div>

                <button type="submit" class="btn-next">
                    Votar
                </button>

            </form>
        </div>
    </div>
</body>

</html>