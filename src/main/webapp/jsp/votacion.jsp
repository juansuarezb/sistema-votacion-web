<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
    <%@ taglib prefix="c" uri="jakarta.tags.core" %>
        <% if (session.getAttribute("autorizado")==null) { response.sendRedirect(request.getContextPath()
            + "/jsp/Login.jsp" ); return; } %>
            <!DOCTYPE html>
            <html>

            <head>
                <meta charset="UTF-8">
                <title>VotoSeguro - Emitir Voto</title>
            </head>

            <body>

                <nav>
                    <h1>VotoSeguro</h1>
                    <span>Bienvenido, ${sessionScope.autorizado.nombre}</span>
                    <a href="${pageContext.request.contextPath}/AutenticarController?ruta=cerrarSesion">
                        Cerrar sesión
                    </a>
                </nav>

                <h2>
                    <c:out value="${votacion.titulo}" />
                </h2>
                <p>
                    <c:out value="${votacion.descripcion}" />
                </p>

                <form action="${pageContext.request.contextPath}/EmitirVotoController?ruta=confirmar" method="post">

                    <input type="hidden" name="idVotacion" value="${votacion.idVotacion}" />

                    <label>
                        <input type="radio" name="opcion" value="SI" required /> Sí
                    </label>
                    <br />
                    <label>
                        <input type="radio" name="opcion" value="NO" /> No
                    </label>
                    <br />
                    <label>
                        <input type="radio" name="opcion" value="BLANCO" /> Blanco
                    </label>
                    <br />
                    <label>
                        <input type="radio" name="opcion" value="NULO" /> Nulo
                    </label>
                    <br /><br />

                    <button type="submit"
                        onclick="return confirm('¿Está seguro de emitir este voto? Esta acción no se puede deshacer.')">
                        Emitir voto
                    </button>
                    <a href="${pageContext.request.contextPath}/EmitirVotoController?ruta=listar">
                        Cancelar
                    </a>

                </form>

            </body>

            </html>