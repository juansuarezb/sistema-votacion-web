package controlador;

import java.io.IOException;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;
import modelo.Entities.Administrador;
import modelo.Entities.Auditor;
import modelo.Entities.Usuario;
import modelo.Entities.Votante;

@WebServlet("/AutenticarController")
public class AutenticarController extends HttpServlet {
	private static final long serialVersionUID = 1L;

	@Override
	protected void doGet(HttpServletRequest req, HttpServletResponse resp)
	        throws ServletException, IOException {
	    this.ruteador(req, resp);
	}

	@Override
	protected void doPost(HttpServletRequest req, HttpServletResponse resp)
	        throws ServletException, IOException {
	    this.ruteador(req, resp);
	}

	private void ruteador(HttpServletRequest req, HttpServletResponse resp)
	        throws ServletException, IOException {
		String ruta = (req.getParameter("ruta")==null)?"iniciar": req.getParameter("ruta");
	    switch (ruta) {
	        case "ingresar":
	            this.ingresar(req, resp);
	            break;
	        case "cerrarSesion":
	        	this.cerrarSesion(req, resp);
	        	break;
	        default:
	            resp.sendRedirect("jsp/Login.jsp");
	            break;
	    }
	}

	private void ingresar(HttpServletRequest req, HttpServletResponse resp)
	        throws ServletException, IOException {
	    // 1. Obtener parámetros
	    String correo = req.getParameter("correo");
	    String contraseña = req.getParameter("contraseña");

	    // 2. Buscar en las tres listas
	    Usuario resultado = new Votante().authenticate(correo, contraseña);
	    if (resultado == null) {
			resultado = new Administrador().authenticate(correo, contraseña);
		}
	    if (resultado == null) {
			resultado = new Auditor().authenticate(correo, contraseña);
		}

	    // 3. Llamar a la vista
	    if (resultado == null) {
	    	req.setAttribute("error", "Correo o contraseña incorrectos.");
	    	req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
	    } else {
	        HttpSession sesionSitio = req.getSession();
	        sesionSitio.setAttribute("autorizado", resultado);

	        // 4. Redirigir según rol
	        if (resultado instanceof Administrador) {
	            resp.sendRedirect("GestionarVotacionesController?ruta=listarVotaciones");
	        } else if (resultado instanceof Votante) {
	            resp.sendRedirect("EmitirVotoController?ruta=listar");
	        } else if (resultado instanceof Auditor) {
	            resp.sendRedirect("PanelAuditoriaController");
	        }
	    }
	}
	private void cerrarSesion(HttpServletRequest req, HttpServletResponse resp)
	        throws ServletException, IOException {
	    HttpSession sesion = req.getSession(false);
	    if (sesion != null) {
	        Usuario usuario = (Usuario) sesion.getAttribute("autorizado");
	        if (usuario != null) {
	            usuario.cerrarSesion(); // llama al modelo
	        }
	        sesion.invalidate(); // invalida la sesión HTTP
	    }
	    resp.sendRedirect("jsp/Login.jsp");
	}
}
