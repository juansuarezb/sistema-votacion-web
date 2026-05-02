<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Inicio</title>
<style>
  * { box-sizing: border-box; margin: 0; padding: 0; }
  body { font-family: var(--font-sans); }
  .page { display: flex; flex-direction: column; min-height: 600px; background: var(--color-background-tertiary); border-radius: var(--border-radius-lg); overflow: hidden; border: 0.5px solid var(--color-border-tertiary); }

  /* NAV */
  nav { background: var(--color-background-primary); border-bottom: 0.5px solid var(--color-border-tertiary); padding: 0 2rem; height: 56px; display: flex; align-items: center; justify-content: space-between; position: sticky; top: 0; z-index: 10; }
  .nav-logo { font-weight: 500; font-size: 16px; color: var(--color-text-primary); display: flex; align-items: center; gap: 8px; }
  .nav-logo-dot { width: 10px; height: 10px; border-radius: 50%; background: #378ADD; }
  .nav-links { display: flex; align-items: center; gap: 4px; }
  .nav-link { font-size: 13px; color: var(--color-text-secondary); padding: 6px 10px; border-radius: var(--border-radius-md); cursor: pointer; transition: background 0.15s; text-decoration: none; }
  .nav-link:hover { background: var(--color-background-secondary); color: var(--color-text-primary); }
  .nav-auth { display: flex; align-items: center; gap: 8px; }
  .btn-login { font-size: 13px; color: var(--color-text-secondary); padding: 6px 14px; border-radius: var(--border-radius-md); cursor: pointer; border: 0.5px solid var(--color-border-secondary); background: transparent; transition: background 0.15s; }
  .btn-login:hover { background: var(--color-background-secondary); }
  .btn-signup { font-size: 13px; color: #fff; padding: 6px 14px; border-radius: var(--border-radius-md); cursor: pointer; border: none; background: #185FA5; transition: opacity 0.15s; }
  .btn-signup:hover { opacity: 0.88; }

  /* FOOTER */
  footer { background: var(--color-background-primary); border-top: 0.5px solid var(--color-border-tertiary); padding: 1.25rem 2rem; display: flex; align-items: center; justify-content: space-between; }
  .footer-copy { font-size: 12px; color: var(--color-text-tertiary); }
  .footer-links { display: flex; gap: 16px; }
  .footer-link { font-size: 12px; color: var(--color-text-tertiary); text-decoration: none; cursor: pointer; }
  .footer-link:hover { color: var(--color-text-secondary); }

  /* LABELS */
  .label { position: absolute; font-size: 10px; font-weight: 500; color: var(--color-text-info); background: var(--color-background-info); padding: 2px 7px; border-radius: 4px; pointer-events: none; white-space: nowrap; }
</style>
</head>
<body>
<div class="page" style="position: relative;">

  <!-- NAV -->
  <nav>
    <div class="nav-logo">
      <div class="nav-logo-dot"></div>
      AppName
    </div>
    <div class="nav-links">
      <a class="nav-link">Inicio</a>
      <a class="nav-link">Feature</a>
      <a class="nav-link">Feature</a>
      <a class="nav-link">Feature</a>
    </div>
    <div class="nav-auth">
      <button class="btn-login"><a href="${pageContext.request.contextPath}/DashboardController?ruta=iniciarSesion">Log in</a></button>
      <button class="btn-signup">Sign up</button>
    </div>
  </nav>
  <!-- FOOTER -->
  <footer>
    <span class="footer-copy">© 2026 AppName. Todos los derechos reservados.</span>
    <div class="footer-links">
      <a class="footer-link">Términos</a>
      <a class="footer-link">Privacidad</a>
      <a class="footer-link">Contacto</a>
    </div>
  </footer>

</div>
</body>
</html>