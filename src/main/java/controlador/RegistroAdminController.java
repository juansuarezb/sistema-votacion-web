package controlador;

import java.io.IOException;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import modelo.Entities.Administrador;

@WebServlet("/RegistroAdminController")
public class RegistroAdminController extends HttpServlet {
    private static final long serialVersionUID = 1L;

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        ruteador(req, resp);
    }

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        ruteador(req, resp);
    }

    private void ruteador(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        String ruta = (req.getParameter("ruta") == null) ? "nuevo" : req.getParameter("ruta");
        switch (ruta) {
            case "nuevo":
                this.nuevo(req, resp);
                break;
            case "guardar":
                this.guardar(req, resp);
                break;
            default:
                resp.sendRedirect("jsp/Login.jsp");
                break;
        }
    }

    private void nuevo(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        req.getRequestDispatcher("jsp/registroAdmin.jsp").forward(req, resp);
    }

    private void guardar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        // 1. Obtener parámetros
        String nombre = req.getParameter("nombre");
        String correo = req.getParameter("correo");
        String contraseña = req.getParameter("contraseña");
        String nivelAcceso = req.getParameter("nivelAcceso");
        String claveMaestra = req.getParameter("claveMaestra");

        // 2. Validar clave maestra
        Administrador admin = new Administrador();
        if (!admin.validarPermisos(claveMaestra)) {
        	req.setAttribute("error", "Clave de autorización incorrecta.");
        	req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
        }
        /*
        // 3. Crear y registrar el Admin
        Administrador nuevoAdmin = new Administrador(0, nombre, correo, contraseña, nivelAcceso);
        Administrador.create(nuevoAdmin);
         */
        // 4. Redirigir al Login
        resp.sendRedirect("jsp/Login.jsp");
    }
}