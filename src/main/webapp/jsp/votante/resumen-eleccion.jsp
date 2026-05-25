<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
<%-- JSTL actualizado para Jakarta EE 10 --%>
<%@ taglib uri="jakarta.tags.core" prefix="c" %>
<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Voto Seguro Ecuador - Resumen de Elección</title>

    <link rel="stylesheet" href="${pageContext.request.contextPath}/assets/css/main.css">
</head>

<body>

    <aside class="o-sidebar">
        <div class="o-sidebar__menu">
            <a href="${pageContext.request.contextPath}/dashboard" class="o-sidebar__item">
                <img src="${pageContext.request.contextPath}/assets/icons/icon-home.svg" alt="Inicio"
                    style="width: 40px; height: 40px;">
                <span class="o-sidebar__text">Inicio</span>
            </a>
            <a href="#" class="o-sidebar__item o-sidebar__item--active">
                <img src="${pageContext.request.contextPath}/assets/icons/icon-votar.svg" alt="Votar"
                    style="width: 40px; height: 40px;">
                <span class="o-sidebar__text">Votar</span>
            </a>
            <a href="${pageContext.request.contextPath}/comprobantes" class="o-sidebar__item">
                <img src="${pageContext.request.contextPath}/assets/icons/icon-registro.svg" alt="Comprobantes"
                    style="width: 40px; height: 40px;">
                <span class="o-sidebar__text">Comprobantes</span>
            </a>
        </div>

        <div class="o-sidebar__footer">
            <a href="${pageContext.request.contextPath}/LogoutController" class="o-sidebar__item">
                <img src="${pageContext.request.contextPath}/assets/icons/icon-exit.svg" alt="Cerrar Sesión"
                    style="width: 40px; height: 40px;">
                <span class="o-sidebar__text">Cerrar Sesión</span>
            </a>
        </div>
    </aside>

    <header class="o-header">
        <div class="o-header__brand">
            <img src="${pageContext.request.contextPath}/assets/icons/icon-votoSeguro.svg"
                alt="Logo Voto Seguro Ecuador" class="o-header__logo">
        </div>

        <div class="o-header__status">
            <img src="${pageContext.request.contextPath}/assets/icons/icon-alarm.svg" alt="Campana"
                class="o-header__icon">
            <span class="o-header__text">Verificado por CNE</span>
        </div>
    </header>

    <div class="main-layout-container">

        <main class="page-content">

            <h1 class="resumen-title">RESUMEN DE SU ELECCIÓN</h1>

            <!-- Tarjeta de Resumen -->
            <div class="resumen-card">

                <!-- Dignidad 1 -->
                <div class="resumen-item">
                    <div class="resumen-item__info">
                        <img src="${pageContext.request.contextPath}/assets/icons/icon-bandera.svg" alt="Unión Europea"
                            class="resumen-item__logo">
                        <div class="resumen-item__details">
                            <p class="resumen-item__dignity">Dignidad 1</p>
                            <p class="resumen-item__party">PARTIDO UNIÓN EUROPEA</p>
                        </div>
                    </div>
                    <div class="resumen-item__actions">
                        <button type="button" class="resumen-btn resumen-btn--modificar"
                            data-dignity="1">MODIFICAR</button>
                        <button type="button" class="resumen-btn resumen-btn--eliminar"
                            data-dignity="1">ELIMINAR</button>
                    </div>
                </div>

                <!-- Dignidad 2 -->
                <div class="resumen-item">
                    <div class="resumen-item__info">
                        <div
                            style="width: 80px; height: 80px; background-color: #254A77; border-radius: 50%; display: flex; align-items: center; justify-content: center; color: white; font-weight: bold; font-size: 24px;">
                            25
                        </div>
                        <div class="resumen-item__details">
                            <p class="resumen-item__dignity">Dignidad 2</p>
                            <p class="resumen-item__party">PARTIDO CONSTRUYE</p>
                        </div>
                    </div>
                    <div class="resumen-item__actions">
                        <button type="button" class="resumen-btn resumen-btn--modificar"
                            data-dignity="2">MODIFICAR</button>
                        <button type="button" class="resumen-btn resumen-btn--eliminar"
                            data-dignity="2">ELIMINAR</button>
                    </div>
                </div>

                <!-- Dignidad 3 -->
                <div class="resumen-item">
                    <div class="resumen-item__info">
                        <img src="${pageContext.request.contextPath}/assets/icons/icon-bandera.svg" alt="Unión Europea"
                            class="resumen-item__logo">
                        <div class="resumen-item__details">
                            <p class="resumen-item__dignity">Dignidad 3</p>
                            <p class="resumen-item__party">PARTIDO UNIÓN EUROPEA</p>
                        </div>
                    </div>
                    <div class="resumen-item__actions">
                        <button type="button" class="resumen-btn resumen-btn--modificar"
                            data-dignity="3">MODIFICAR</button>
                        <button type="button" class="resumen-btn resumen-btn--eliminar"
                            data-dignity="3">ELIMINAR</button>
                    </div>
                </div>

            </div>

            <!-- Formulario de confirmación -->
            <form id="confirmationForm" action="${pageContext.request.contextPath}/ConfirmarVotoController"
                method="POST">

                <button type="submit" class="btn-confirm" id="confirmBtn" disabled>
                    CONFIRMAR Y ENVIAR VOTO
                </button>

                <div class="review-section">
                    <input type="checkbox" id="reviewCheckbox" class="review-checkbox">
                    <label for="reviewCheckbox">He revisado mi selección</label>
                </div>

            </form>

        </main>

        <button class="btn-help-floating" aria-label="Ayuda">
            <img src="${pageContext.request.contextPath}/assets/icons/icon-help.svg" alt="Ayuda">
        </button>

    </div>

    <script>
        // Habilitar botón de confirmación solo si el checkbox está marcado
        const reviewCheckbox = document.getElementById('reviewCheckbox');
        const confirmBtn = document.getElementById('confirmBtn');

        reviewCheckbox.addEventListener('change', function () {
            confirmBtn.disabled = !this.checked;
        });

        // Botones de modificar
        document.querySelectorAll('.resumen-btn--modificar').forEach(btn => {
            btn.addEventListener('click', function () {
                const dignity = this.getAttribute('data-dignity');
                // Redirigir a la página de voto para modificar la dignidad
                window.location.href = '${pageContext.request.contextPath}/EmitirVotoController?ruta=votar&dignidad=' + dignity;
            });
        });

        // Botones de eliminar
        document.querySelectorAll('.resumen-btn--eliminar').forEach(btn => {
            btn.addEventListener('click', function () {
                const dignity = this.getAttribute('data-dignity');
                if (confirm('¿Desea eliminar la selección de Dignidad ' + dignity + '?')) {
                    // Aquí iría la lógica para eliminar la selección
                    console.log('Eliminar Dignidad ' + dignity);
                    // Enviar petición al servidor para eliminar
                }
            });
        });

        // Prevenir envío si no está marcado el checkbox
        document.getElementById('confirmationForm').addEventListener('submit', function (e) {
            if (!reviewCheckbox.checked) {
                e.preventDefault();
                alert('Por favor confirme que ha revisado su selección');
            }
        });
    </script>

</body>

</html>