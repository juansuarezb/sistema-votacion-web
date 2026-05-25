<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%-- JSTL actualizado para Jakarta EE 10 --%>
<%@ taglib uri="jakarta.tags.core" prefix="c" %>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Voto Seguro Ecuador - Proceso de Votación</title>
    
    <link rel="stylesheet" href="${pageContext.request.contextPath}/assets/css/main.css">
</head>
<body>

    <aside class="o-sidebar">
        <div class="o-sidebar__menu">
            <a href="${pageContext.request.contextPath}/dashboard" class="o-sidebar__item">
                <img src="${pageContext.request.contextPath}/assets/icons/icon-home.svg" alt="Inicio" style="width: 40px; height: 40px;">
                <span class="o-sidebar__text">Inicio</span>
            </a>
            <a href="#" class="o-sidebar__item o-sidebar__item--active">
                <img src="${pageContext.request.contextPath}/assets/icons/icon-votar.svg" alt="Votar" style="width: 40px; height: 40px;">
                <span class="o-sidebar__text">Votar</span>
            </a>
            <a href="${pageContext.request.contextPath}/comprobantes" class="o-sidebar__item">
                <img src="${pageContext.request.contextPath}/assets/icons/icon-registro.svg" alt="Comprobantes" style="width: 40px; height: 40px;">
                <span class="o-sidebar__text">Comprobantes</span>
            </a>
        </div>
        
        <div class="o-sidebar__footer">
            <a href="${pageContext.request.contextPath}/LogoutController" class="o-sidebar__item">
                <img src="${pageContext.request.contextPath}/assets/icons/icon-exit.svg" alt="Cerrar Sesión" style="width: 40px; height: 40px;">
                <span class="o-sidebar__text">Cerrar Sesión</span>
            </a>
        </div>
    </aside>

    <header class="o-header">
        <div class="o-header__brand">
            <img src="${pageContext.request.contextPath}/assets/icons/icon-votoSeguro.svg" alt="Logo Voto Seguro Ecuador" class="o-header__logo">
        </div>

        <div class="o-header__status">
            <img src="${pageContext.request.contextPath}/assets/icons/icon-alarm.svg" alt="Campana" class="o-header__icon">
            <span class="o-header__text">Verificado por CNE</span>
        </div>
    </header>

    <div class="main-layout-container">
        
        <main class="page-content">
            <%-- Extrae el nombre directamente del objeto 'autorizado' guardado en sesión --%>
            <h1 class="page-content__welcome">Bienvenido, ${not empty sessionScope.autorizado.nombre ? sessionScope.autorizado.nombre : usuarioNombre}</h1>

            <div class="card-election">
                <h2 class="card-election__title">${not empty votacion.titulo ? votacion.titulo : 'Elección no disponible'}</h2>
                <p class="card-election__subtitle">${not empty votacion.descripcion ? votacion.descripcion : ''} - (Cierre: ${votacion.fechaCierre})</p>
                
                <div class="card-election__countdown">
                    <p class="card-election__countdown-label">Tiempo restante para la elección</p>
                    <div id="countdownTimer" class="card-election__timer">08 : 06 : 10</div>
                </div>
                
                <%-- El botón envía el ID real de la votación al EmitirVotoController --%>
                <button type="button" class="btn-primary-action" onclick="location.href='${pageContext.request.contextPath}/EmitirVotoController?ruta=votar&id=${votacion.idVotacion}'">
                    INICIAR PROCESO DE VOTACIÓN
                </button>
            </div>
        </main>

        <button class="btn-help-floating" aria-label="Ayuda">
            <img src="${pageContext.request.contextPath}/assets/icons/icon-help.svg" alt="Ayuda">
        </button>

    </div>

    <script>
        function iniciarCuentaRegresiva() {
            // Recibimos los valores dinámicos desde el controlador (por defecto 0 si no llegan)
            const horasIniciales = Number("${not empty horasRestantes ? horasRestantes : 0}");
            const minutosIniciales = Number("${not empty minutosRestantes ? minutosRestantes : 0}");
            const segundosIniciales = Number("${not empty segundosRestantes ? segundosRestantes : 0}");

            const duracionMilisegundos = ((horasIniciales * 60 * 60) + (minutosIniciales * 60) + segundosIniciales) * 1000;
            const tiempoMeta = Date.now() + duracionMilisegundos;
            const timerElement = document.getElementById('countdownTimer');

            const intervalo = setInterval(function() {
                const ahora = Date.now();
                const distancia = tiempoMeta - ahora;

                if (distancia <= 0) {
                    clearInterval(intervalo);
                    timerElement.innerHTML = "00 : 00 : 00";
                    return;
                }

                const horas = Math.floor(distancia / (1000 * 60 * 60));
                const minutos = Math.floor((distancia % (1000 * 60 * 60)) / (1000 * 60));
                const segundos = Math.floor((distancia % (1000 * 60)) / 1000);

                const horasFormateadas = String(horas).padStart(2, '0');
                const minutosFormateadas = String(minutos).padStart(2, '0');
                const segundosFormateados = String(segundos).padStart(2, '0');

                // Se usa concatenación en lugar de \${} para evitar conflictos con JSP EL
                timerElement.innerHTML = horasFormateadas + " : " + minutosFormateadas + " : " + segundosFormateados;
            }, 1000);
        }

        document.addEventListener('DOMContentLoaded', iniciarCuentaRegresiva);
    </script>

</body>
</html>