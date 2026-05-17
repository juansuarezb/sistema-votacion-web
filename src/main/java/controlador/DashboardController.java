package controlador;

import java.io.IOException;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;

@WebServlet("/DashboardController")
public class DashboardController extends HttpServlet{
	private static final long serialVersionUID = 1L;
	@Override
	protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
	    this.ruteador(req, resp);
	}

	@Override
	protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
	    this.ruteador(req, resp);
	}

	private void ruteador(HttpServletRequest req, HttpServletResponse resp)
	        throws ServletException, IOException {
		String ruta = (req.getParameter("ruta")==null)?"iniciar": req.getParameter("ruta");
	    switch (ruta) {
	        case "iniciar":
	            this.iniciar(req, resp);
	            break;
	        default:
	            resp.sendError(HttpServletResponse.SC_NOT_FOUND);
	            break;
	        case "iniciarSesion":
	        	this.iniciarSesion(req, resp);
	        	break;
	    }
	}

	private void iniciar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		renderDashboardInicial(req, resp);
	}
	private void iniciarSesion(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException{
		renderLogin(req, resp);
	}
	private void renderDashboardInicial(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
	    HttpSession session = req.getSession(false);

	    // Implementación de la Guarda [session == null]
	    if (session == null || session.getAttribute("usuario") == null) {
	        // Transición al estado Dashboard Inicial (Login)
	        req.getRequestDispatcher("jsp/DashboardInicial.jsp").forward(req, resp);
	    } else {
	        // Transición al estado Dashboard Principal (Si ya existe sesión)
	        resp.sendRedirect("jsp/ListarVotantes.jsp");
	    }
	}

	private void renderLogin(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		resp.sendRedirect("jsp/Login.jsp");
	}
}
