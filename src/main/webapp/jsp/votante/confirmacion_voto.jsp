<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <title>VotoSeguro - Confirmación</title>
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
            display: flex;
            flex-direction: column;
        }

        header {
            font-size: 32px;
            font-weight: bold;
            padding: 40px;
        }

        .content {
            flex: 1;
            display: flex;
            flex-direction: column;
            align-items: center;
            margin-top: 40px;
        }

        h1 {
            font-size: 64px;
            font-weight: bold;
            letter-spacing: 2px;
        }

        .check-icon {
            margin: 20px 0 40px 0;
        }

        .certification-box {
            border: 1px solid #000;
            padding: 50px;
            width: 100%;
            max-width: 600px;
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
            border-radius: 4px;
        }

        .mail-icon {
            margin-bottom: 30px;
        }

        p {
            font-size: 20px;
            line-height: 1.5;
            margin-bottom: 40px;
            max-width: 300px;
        }

        .btn-inicio {
            padding: 12px 60px;
            border: 1px solid #000;
            background: transparent;
            font-size: 18px;
            cursor: pointer;
            text-decoration: none;
            color: #000;
            display: inline-block;
            width: 100%;
            max-width: 300px;
        }

        .btn-inicio:hover {
            background-color: #f0f0f0;
        }
    </style>
</head>

<body>
    <header>VOTOSEGURO</header>
    <div class="content">
        <h1>THANK YOU!</h1>
        <div class="check-icon">
            <svg width="80" height="80" viewBox="0 0 24 24" fill="none" stroke="#4CAF50" stroke-width="4"
                stroke-linecap="round" stroke-linejoin="round">
                <polyline points="20 6 9 17 4 12"></polyline>
            </svg>
        </div>
        <div class="certification-box">
            <div class="mail-icon">
                <svg width="60" height="40" viewBox="0 0 24 24" fill="none" stroke="#000" stroke-width="1.5"
                    stroke-linecap="round" stroke-linejoin="round">
                    <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
                    <polyline points="22,6 12,13 2,6"></polyline>
                </svg>
            </div>
            <p>Check your Inbox to get the certification</p>
            <a href="dashboard_inicial.jsp" class="btn-inicio">Inicio</a>
        </div>
    </div>
</body>

</html>