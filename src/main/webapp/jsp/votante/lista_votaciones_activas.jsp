<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <title>VotoSeguro - Votaciones Activas</title>
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

        /* Sidebar Styles */
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
            line-height: 1.4;
        }

        .menu-group {
            margin-top: 30px;
            flex: 1;
        }

        .menu-title {
            padding: 0 30px;
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 15px;
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

        .sidebar-bottom {
            padding: 30px;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .sidebar-bottom a {
            color: #fff;
            text-decoration: none;
            font-size: 18px;
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

        /* Main Content */
        .content {
            flex: 1;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
        }

        .message {
            font-size: 24px;
            text-align: center;
            max-width: 400px;
            line-height: 1.4;
        }
    </style>
</head>

<body>
    <div class="sidebar">
        <div class="sidebar-header">VOTOSEGURO</div>
        <div class="user-profile">
            <div class="avatar"></div>
            <div class="user-info">
                correoElectronico<br>nombre
            </div>
        </div>
        <div class="menu-group">
            <div class="menu-title">Main Menu</div>
            <a href="#" class="menu-item active">Lista Votaciones</a>
        </div>
        <div class="sidebar-bottom">
            <a href="#">System Settings</a>
            <a href="#">Help Center</a>
        </div>
        <a href="login.jsp" class="logout">Logout Accout</a>
    </div>

    <div class="content">
        <div class="message">
            Check your Inbox to get the certification
        </div>
    </div>
</body>

</html>