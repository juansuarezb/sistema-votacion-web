<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
    <%@ taglib prefix="c" uri="jakarta.tags.core" %>
        <% if (session.getAttribute("autorizado")==null) { response.sendRedirect(request.getContextPath()
            + "/jsp/Login.jsp" ); return; } %>
            <!DOCTYPE html>
            <html>

            <head>
                <meta charset="UTF-8">
                <title>VotoSeguro - Listar Votantes</title>
            </head>

            <body>

                <nav>
                    <h1>VotoSeguro</h1>
                    <span>Bienvenido, ${sessionScope.autorizado.nombre}</span>
                    <a href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
                        Cerrar sesión
                    </a>
                </nav>

                <h2>Gestión de Votantes</h2>

                <a href="GestionarVotantesController?ruta=nuevo">Nuevo Votante</a>

                <table border="1">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nombre</th>
                            <th>Correo</th>
                            <th>Ha Votado</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        <c:forEach var="votante" items="${votantes}">
                            <tr>
                                <td>${votante.idUsuario}</td>
                                <td>
                                    <c:out value="${votante.nombre}" />
                                </td>
                                <td>
                                    <c:out value="${votante.correoElectronico}" />
                                </td>
                                <td>${votante.votacionesVotadas.size()} votaciones</td>
                                <td>
                                    <a
                                        href="GestionarVotantesController?ruta=actualizar&id=${votante.idUsuario}">Editar</a>
                                    <a href="GestionarVotantesController?ruta=eliminar&id=${votante.idUsuario}"
                                        onclick="return confirm('¿Está seguro de eliminar este votante?')">
                                        Eliminar
                                    </a>
                                </td>
                            </tr>
                        </c:forEach>
                    </tbody>
                </table>

            </body>

            </html>