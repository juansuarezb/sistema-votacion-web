<!DOCTYPE html>
<html lang="es">

<head>
  <meta charset="UTF-8">
  <title>VotoSeguro - Inicio</title>
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

    nav {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 20px 40px;
      border-bottom: 1px solid #ddd;
    }

    .nav-logo {
      font-size: 24px;
      font-weight: bold;
    }

    .nav-links {
      display: flex;
      gap: 30px;
      align-items: center;
      font-size: 14px;
    }

    .nav-links a {
      text-decoration: none;
      color: #000;
      text-transform: uppercase;
    }

    .main-container {
      padding: 60px 40px;
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 60px;
    }

    .hero {
      display: flex;
      gap: 40px;
    }

    .hero-text {
      flex: 1;
    }

    .hero-text h1 {
      font-size: 32px;
      margin-bottom: 20px;
    }

    .hero-text p {
      line-height: 1.6;
      color: #333;
    }

    .hero-box {
      flex: 1;
      border: 1px solid #000;
      min-height: 300px;
      position: relative;
    }

    .dots {
      position: absolute;
      top: 10px;
      left: 10px;
      font-size: 20px;
      letter-spacing: 2px;
      color: #666;
    }

    .features {
      display: flex;
      gap: 40px;
    }

    .feature-item {
      flex: 1;
    }

    .feature-box {
      border: 1px solid #000;
      height: 150px;
      margin-bottom: 20px;
      position: relative;
    }

    .feature-item p {
      text-align: center;
      font-size: 14px;
      line-height: 1.5;
      padding: 0 20px;
    }

    .bottom-section {
      display: flex;
      align-items: center;
      border-top: 1px solid #ccc;
      padding-top: 40px;
    }

    .bottom-text {
      flex: 1;
      padding-right: 40px;
      font-size: 14px;
      text-align: center;
    }

    .bottom-grid {
      flex: 1;
      height: 100px;
      background-image: linear-gradient(#ccc 1px, transparent 1px), linear-gradient(90deg, #ccc 1px, transparent 1px);
      background-size: 40px 40px;
      transform: skewX(-20deg);
    }

    footer {
      border-top: 1px solid #eee;
      padding: 20px 40px;
      display: flex;
      justify-content: space-between;
      font-size: 12px;
      color: #666;
    }

    .footer-cols {
      display: flex;
      gap: 60px;
    }

    .footer-col strong {
      display: block;
      margin-bottom: 10px;
      color: #000;
    }
  </style>
</head>

<body>
  <nav>
    <div class="nav-logo">VOTOSEGURO</div>
    <div class="nav-links">
      <a href="#">About</a>
      <a href="#">Services</a>
      <a href="login.jsp">Log In / Sign in</a>
    </div>
  </nav>
  <div class="main-container">
    <div class="hero">
      <div class="hero-text">
        <h1>Lorem ipsum dolor<br>sit amet</h1>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore
          magna aliqua.</p>
      </div>
      <div class="hero-box">
        <div class="dots">ooo</div>
      </div>
    </div>
    <div class="features">
      <div class="feature-item">
        <div class="feature-box">
          <div class="dots">ooo</div>
        </div>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore
          magna aliqua.</p>
      </div>
      <div class="feature-item">
        <div class="feature-box">
          <div class="dots">ooo</div>
        </div>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore
          magna aliqua.</p>
      </div>
    </div>
    <div class="bottom-section">
      <div class="bottom-text">
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore
          magna aliqua.</p>
      </div>
      <div class="bottom-grid"></div>
    </div>
  </div>
  <footer>
    <div style="width: 30%;">
      <strong style="color:#000; font-size:14px;">VOTOSEGURO</strong><br><br>
      <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p>
    </div>
    <div class="footer-cols">
      <div class="footer-col">
        <strong>Product</strong>
        <p>Product</p>
        <p>Product</p>
        <p>Product</p>
      </div>
      <div class="footer-col">
        <strong>Company</strong>
        <p>Company</p>
        <p>Company</p>
        <p>Company</p>
      </div>
    </div>
    <div>
      <p>© 2010 — 2020</p>
      <p>Privacy — Terms</p>
    </div>
  </footer>
</body>

</html>