<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
<%-- JSTL actualizado para Jakarta EE 10 --%>
<%@ taglib uri="jakarta.tags.core" prefix="c" %>
<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Voto Seguro Ecuador - Selección de Candidatos</title>

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

            <!-- Contenedor de Dignidades -->
            <div class="dignity-tabs">
                <button type="button" class="dignity-tab active" data-dignity="1">Dignidad 1</button>
                <button type="button" class="dignity-tab" data-dignity="2">Dignidad 2</button>
                <button type="button" class="dignity-tab" data-dignity="3">Dignidad 3</button>
            </div>

            <!-- Contenedor de Candidatos -->
            <form id="candidatosForm" action="${pageContext.request.contextPath}/ConfirmarVotoController" method="POST">

                <div class="candidates-container" id="candidatesContainer">
                    <!-- Opción 1: Unión Europea -->
                    <div class="candidate-card">
                        <input type="checkbox" name="candidato" value="1" class="candidate-card__checkbox">
                        <div class="candidate-card__content">
                            <div class="candidate-card__party">
                                <p class="candidate-card__party-label">Partido</p>
                                <img src="${pageContext.request.contextPath}/assets/icons/icon-bandera.svg"
                                    alt="Unión Europea" class="candidate-card__party-logo">
                                <p class="candidate-card__party-name">Unión Europea</p>
                            </div>
                            <div class="candidate-card__positions">
                                <div class="position">
                                    <p class="position__title">Presidente</p>
                                    <img src="${pageContext.request.contextPath}/assets/icons/icon-person.svg"
                                        alt="Leo Cruz" class="position__image">
                                    <p class="position__name">Leo Cruz</p>
                                </div>
                                <div class="position">
                                    <p class="position__title">Vicepresidenta</p>
                                    <img src="${pageContext.request.contextPath}/assets/icons/icon-person.svg"
                                        alt="Mía Solís" class="position__image">
                                    <p class="position__name">Mía Solís</p>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Opción 2: Construye -->
                    <div class="candidate-card">
                        <input type="checkbox" name="candidato" value="2" class="candidate-card__checkbox">
                        <div class="candidate-card__content">
                            <div class="candidate-card__party">
                                <p class="candidate-card__party-label">Partido</p>
                                <div
                                    style="width: 80px; height: 80px; background-color: #254A77; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin-bottom: 15px; color: white; font-weight: bold; font-size: 24px;">
                                    25
                                </div>
                                <p class="candidate-card__party-name">Construye</p>
                            </div>
                            <div class="candidate-card__positions">
                                <div class="position">
                                    <p class="position__title">Presidente</p>
                                    <img src="${pageContext.request.contextPath}/assets/icons/icon-person.svg"
                                        alt="Enzo Vidal" class="position__image">
                                    <p class="position__name">Enzo Vidal</p>
                                </div>
                                <div class="position">
                                    <p class="position__title">Vicepresidenta</p>
                                    <img src="${pageContext.request.contextPath}/assets/icons/icon-person.svg"
                                        alt="Sara León" class="position__image">
                                    <p class="position__name">Sara León</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Botones de Navegación -->
                <div class="navigation-buttons">
                    <button type="button" class="nav-btn" onclick="volver()">VOLVER</button>
                    <button type="submit" class="nav-btn">SIGUIENTE</button>
                </div>

            </form>

        </main>

        <button class="btn-help-floating" aria-label="Ayuda">
            <img src="${pageContext.request.contextPath}/assets/icons/icon-help.svg" alt="Ayuda">
        </button>

    </div>

    <script>
        // Manejo de tabs de dignidades
        document.querySelectorAll('.dignity-tab').forEach(tab => {
            tab.addEventListener('click', function () {
                document.querySelectorAll('.dignity-tab').forEach(t => t.classList.remove('active'));
                this.classList.add('active');

                // Aquí se cargarían los candidatos de la dignidad seleccionada
                const dignity = this.getAttribute('data-dignity');
                console.log('Dignidad seleccionada:', dignity);
            });
        });

        // Solo un candidato puede ser seleccionado a la vez
        document.querySelectorAll('input[type="checkbox"][name="candidato"]').forEach(checkbox => {
            checkbox.addEventListener('change', function () {
                if (this.checked) {
                    document.querySelectorAll('input[type="checkbox"][name="candidato"]').forEach(cb => {
                        if (cb !== this) {
                            cb.checked = false;
                        }
                    });
                }
            });
        });

        function volver() {
            // Volver a la pantalla anterior
            window.history.back();
        }

        document.getElementById('candidatosForm').addEventListener('submit', function (e) {
            e.preventDefault();
            const candidatoSeleccionado = document.querySelector('input[type="checkbox"][name="candidato"]:checked');
            if (!candidatoSeleccionado) {
                alert('Por favor seleccione un candidato');
                return;
            }
            this.submit();
        });
    </script>

</body>

</html>