<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ taglib prefix="c" uri="jakarta.tags.core"%>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Voto Seguro Ecuador - Iniciar Sesión</title>
    <link rel="stylesheet" href="${pageContext.request.contextPath}/assets/css/main.css">
</head>
<body>

    <div class="login-screen-container">
        
        <main class="login-main-content">
            
            <div class="login-card">
                
                <img class="login-logo" src="${pageContext.request.contextPath}/assets/icons/icon-votoSeguro.svg" alt="Logo Voto Seguro Ecuador">
                
                <form class="login-form" method="POST" action="${pageContext.request.contextPath}/AutenticarController?ruta=ingresar">
                    
                    <div class="box-large-group">
                        <input type="text" 
                               id="correo" 
                               name="correo" 
                               class="box-large-input" 
                               placeholder="Cédula de identidad" 
                               required 
                               autocomplete="username">
                    </div>
                    
                    <div class="box-large-group">
                        <input type="password" 
                               id="contraseña" 
                               name="contraseña" 
                               class="box-large-input" 
                               placeholder="Contraseña" 
                               required 
                               autocomplete="current-password">
                        <img id="toggleEye"
                             class="password-toggle-icon" 
                             src="${pageContext.request.contextPath}/assets/icons/icon-eye.svg" 
                             alt="Mostrar contraseña" 
                             onclick="togglePasswordVisibility()">
                    </div>
                    
                    <button type="submit" class="btn-medium">
                        Ingresar al Sistema
                    </button>
                    
                </form>
            </div>
        </main>
    </div>

    <script>
        function togglePasswordVisibility() {
            const passwordInput = document.getElementById('contraseña');
            const eyeIcon = document.getElementById('toggleEye');
            
            // Evaluamos el contexto dinámico para actualizar las imágenes sin romper la ruta
            const contextPath = '${pageContext.request.contextPath}';
            
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                eyeIcon.src = contextPath + '/assets/icons/icon-eye-block.svg';
                eyeIcon.alt = 'Ocultar contraseña';
            } else {
                passwordInput.type = 'password';
                eyeIcon.src = contextPath + '/assets/icons/icon-eye.svg';
                eyeIcon.alt = 'Mostrar contraseña';
            }
        }
    </script>
</body>
</html>