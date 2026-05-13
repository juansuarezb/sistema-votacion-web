package controlador;

import java.io.IOException;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

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
	        case "eliminar":
	            this.eliminar(req, resp);
	            break;
	        default:
	            resp.sendError(HttpServletResponse.SC_NOT_FOUND);
	            break;
	    }
	}

	private void listar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		/*//1. Obtener parametros
		//2. Hablar con el modelo
		List<Votante> votantes= Votante.getListaVotantes();
		//3. LLamar a la vista
		req.setAttribute("votantes", votantes);
		req.getRequestDispatcher("jsp/ListarVotantes.jsp").forward(req, resp);
	*/}
	private void nuevo(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		 req.getRequestDispatcher("jsp/CrearVotante.jsp").forward(req, resp);
	}
	private void guardarNuevo(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
       /* // 1. Obtener parámetros
        String nombre = req.getParameter("nombre");
        String correo = req.getParameter("correo");
        String contraseña = req.getParameter("contraseña");
        // 2. Crear votante
        Votante v = new Votante(0, nombre, correo, contraseña);
        boolean resultado = Votante.create(v);
        // 3. Redirigir
        if (resultado) {
            resp.sendRedirect("GestionarVotantesController?ruta=listar");
        } else {
            req.setAttribute("error", "Error al crear el votante.");
            req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
        }*/
	}
	private void actualizar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {

	}
	private void guardarExistente(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        /*// 1. Obtener parámetros
        int id = Integer.parseInt(req.getParameter("idUsuario"));
        String nombre = req.getParameter("nombre");
        String correo = req.getParameter("correo");
        String contraseña = req.getParameter("contraseña");
        // 2. Actualizar
        Votante v = new Votante(0, nombre, correo, contraseña);
        boolean resultado = Votante.update(v);
        // 3. Redirigir
        if (resultado) {
            resp.sendRedirect("GestionarVotantesController?ruta=listar");
        } else {
            req.setAttribute("error", "Error al modificar el votante.");
            req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
        }*/
	}
	private void eliminar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        /*int id = Integer.parseInt(req.getParameter("id"));
        boolean resultado = Votante.delete(id);
        if (resultado) {
            resp.sendRedirect("GestionarVotantesController?ruta=listar");
        } else {
            req.setAttribute("error", "Error al eliminar el votante.");
            req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
        }*/
	}
}
