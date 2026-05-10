<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
    <% if (session.getAttribute("autorizado")==null) { response.sendRedirect(request.getContextPath() + "/jsp/Login.jsp"
        ); return; } %>
        <!DOCTYPE html>
        <html>

        <head>
            <meta charset="UTF-8">
            <title>VotoSeguro - Crear Votante</title>
        </head>

        <body>

            <nav>
                <h1>VotoSeguro</h1>
                <a href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
                    Cerrar sesión
                </a>
            </nav>

            <h2>Nuevo Votante</h2>

            <form action="${pageContext.request.contextPath}/GestionarVotantesController?ruta=guardarnuevo"
                method="post">

                <label>Nombre:</label>
                <input type="text" name="nombre" required />

                <br />

                <label>Correo electrónico:</label>
                <input type="email" name="correo" required />

                <br />

                <label>Contraseña:</label>
                <input type="password" name="contraseña" required />

                <br />

                <button type="submit">Guardar</button>
                <a href="${pageContext.request.contextPath}/GestionarVotantesController?ruta=listar">
                    Cancelar
                </a>

            </form>

        </body>

        </html>