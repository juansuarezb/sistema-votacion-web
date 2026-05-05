package controlador;

import java.io.IOException;
import java.util.List;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import modelo.Entities.Votante;

@WebServlet("/GestionarVotantesController")
public class GestionarVotantesController extends HttpServlet{
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

	    String ruta = req.getParameter("ruta");

	    if (ruta == null || ruta.isEmpty()) {
	        ruta = "listar";
	    }

	    switch (ruta) {
	        case "listar":
	            this.listar(req, resp);
	            break;
	        case "nuevo":
	            this.nuevo(req, resp);
	            break;
	        case "guardarnuevo":
	        	this.guardarNuevo(req, resp);
	        	break;
	        case "actualizar":
	        	this.actualizar(req, resp);
	        	break;
	        case "guardarExistente":
	        	this.guardarExistente(req, resp);
	        	break;
	        default:
	            resp.sendError(HttpServletResponse.SC_NOT_FOUND);
	            break;
	    }
	}
	
	private void listar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		//1. Obtener parametros
		//2. Hablar con el modelo
		List<Votante> votantes= Votante.getListaVotantes();
		//3. LLamar a la vista
		req.setAttribute("votantes", votantes);
		req.getRequestDispatcher("jsp/ListarVotantes.jsp").forward(req, resp);
	}
	private void nuevo(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		//1. Obtener parametros
		//2. Hablar con el modelo
		//3. LLamar a la vista
	}
	private void guardarNuevo(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		//1. Obtener parametros
		//2. Hablar con el modelo
		//3. LLamar a la vista
	}
	private void actualizar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		//1. Obtener parametros
		//2. Hablar con el modelo
		//3. LLamar a la vista
	}
	private void guardarExistente(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		//1. Obtener parametros
		//2. Hablar con el modelo
		//3. LLamar a la vista
	}
	private void eliminar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		//1. Obtener parametros
		//2. Hablar con el modelo
		//3. LLamar a la vista
	}
}
