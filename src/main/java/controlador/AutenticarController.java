package controlador;

import java.io.IOException;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

@WebServlet("/AutenticarController")
public class AutenticarController extends HttpServlet {
	private static final long serialVersionUID = 1L;

	@Override
	protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
	}

	@Override
	protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {

	}

	private void iniciar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		// 1. Obtener parametros
		// 2. Hablar con el modelo
		// 3. LLamar a la vista
	}

	private void ingresar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		// 1. Obtener parametros
		// 2. Hablar con el modelo
		// 3. LLamar a la vista
	}
}
