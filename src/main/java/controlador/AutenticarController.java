package controlador;

import java.io.IOException;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;
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
	        default:
	            resp.sendError(HttpServletResponse.SC_NOT_FOUND);
	            break;
	    }
	}
	
	private void ingresar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		// 1. Obtener parametros
		String correo = req.getParameter("correo");
		String contraseña = req.getParameter("contraseña");
		// 2. Hablar con el modelo
		Usuario usuario = new Votante();
		Usuario resultado = usuario.authenticate(correo, contraseña);
		// 3. LLamar a la vista
		if(resultado == null) {
			//Se regresar al login
			resp.sendRedirect("jsp/Login.jsp");
		}else {
			HttpSession sesionSitio = req.getSession();
			sesionSitio.setAttribute("autorizado", resultado);
			resp.sendRedirect("GestionarVotantesController?ruta=listar");
		}
	}
}
