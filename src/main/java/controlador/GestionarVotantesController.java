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
	}

	@Override
	protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
	}

	private void listar(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		//1. Obtener parametros
		//2. Hablar con el modelo
		//3. LLamar a la vista
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
