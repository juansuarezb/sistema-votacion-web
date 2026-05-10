<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
    <%@ taglib prefix="c" uri="jakarta.tags.core" %>
        <!DOCTYPE html>
        <html>

        <head>
            <meta charset="UTF-8">
            <title>VotoSeguro - Registro Administrador</title>
        </head>

        <body>

            <nav>
                <h1>VotoSeguro</h1>
                <a href="jsp/Login.jsp">Volver al Login</a>
            </nav>

            <h2>Registro de Administrador</h2>

            <form action="${pageContext.request.contextPath}/RegistroAdminController?ruta=guardar" method="post">

                <label>Nombre:</label>
                <input type="text" name="nombre" required />

                <br />

                <label>Correo electrónico:</label>
                <input type="email" name="correo" required />

                <br />

                <label>Contraseña:</label>
                <input type="password" name="contraseña" required />

                <br />

                <label>Nivel de acceso:</label>
                <select name="nivelAcceso">
                    <option value="SUPER">SUPER</option>
                    <option value="NORMAL">NORMAL</option>
                </select>

                <br />

                <label>Clave de autorización:</label>
                <input type="password" name="claveMaestra" required />

                <br />

                <button type="submit">Registrarse</button>
                <a href="jsp/Login.jsp">Cancelar</a>

            </form>

        </body>

        </html>