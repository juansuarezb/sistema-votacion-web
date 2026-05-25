package controlador;

import java.io.IOException;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;
import modelo.DAO.IUsuarioDAO;
import modelo.DAO.JDBC.JDBCUsuarioDAOImpl;
import modelo.Entities.Administrador;
import modelo.Entities.Auditor;
import modelo.Entities.Usuario;
import modelo.Entities.Votante;

@WebServlet("/AutenticarController")
public class AutenticarController extends HttpServlet {
	private static final long serialVersionUID = 1L;

	@Override
	protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		this.ruteador(req, resp);
	}

	@Override
	protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		this.ruteador(req, resp);
	}

	private void ruteador(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		String ruta = (req.getParameter("ruta") == null) ? "iniciar" : req.getParameter("ruta");
		switch (ruta) {
			case "ingresar":
				this.ingresar(req, resp);
				break;
			case "cerrarSesion":
				this.cerrarSesion(req, resp);
				break;
			default:
				resp.sendRedirect("jsp/publicas/login.jsp");
				break;
		}
	}

	private void ingresar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {

		String correo = req.getParameter("correo");
		String contrasena = req.getParameter("contrasena");

		IUsuarioDAO usuarioDAO = new JDBCUsuarioDAOImpl();

		Usuario resultado = usuarioDAO.authenticate(correo, contrasena);

		if (resultado == null) {

			req.setAttribute("error", "Correo o contraseña incorrectos.");

			req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);

		} else {

			HttpSession sesionSitio = req.getSession();

			sesionSitio.setAttribute("autorizado", resultado);

			if (resultado instanceof Administrador) {

				resp.sendRedirect("GestionarVotacionesController?ruta=listarVotaciones");

			} else if (resultado instanceof Votante) {

				resp.sendRedirect("EmitirVotoController?ruta=listar");

			} else if (resultado instanceof Auditor) {

				resp.sendRedirect("PanelAuditoriaController");
			}
		}
	}

	private void cerrarSesion(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		HttpSession sesion = req.getSession(false);
		if (sesion != null) {
			sesion.invalidate();
		}
		resp.sendRedirect(req.getContextPath() + "/jsp/publicas/login.jsp");
	}

}
