package controlador;

import java.io.IOException;
import java.util.List;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import modelo.DAO.IVotanteDAO;
import modelo.DAO.JDBC.JDBCVotanteDAOImpl;
import modelo.Entities.Votante;

@WebServlet("/GestionarVotantesController")
public class GestionarVotantesController extends HttpServlet {
    private static final long serialVersionUID = 1L;

    private IVotanteDAO votanteDAO = new JDBCVotanteDAOImpl();

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
        String ruta = req.getParameter("ruta");
        if (ruta == null || ruta.isEmpty()) ruta = "listar";
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

    private void listar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        List<Votante> votantes = votanteDAO.listar();
        req.setAttribute("votantes", votantes);
        req.getRequestDispatcher("jsp/ListarVotantes.jsp").forward(req, resp);
    }

    private void nuevo(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        req.getRequestDispatcher("jsp/CrearVotante.jsp").forward(req, resp);
    }

    private void guardarNuevo(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        String nombre = req.getParameter("nombre");
        String correo = req.getParameter("correo");
        String contraseña = req.getParameter("contraseña");
        Votante v = new Votante(0, nombre, correo, contraseña);
        boolean resultado = votanteDAO.create(v);
        if (resultado) {
            resp.sendRedirect("GestionarVotantesController?ruta=listar");
        } else {
            req.setAttribute("error", "Error al crear el votante.");
            req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
        }
    }

    private void actualizar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        int id = Integer.parseInt(req.getParameter("id"));
        Votante v = votanteDAO.getById(id);
        req.setAttribute("votante", v);
        req.getRequestDispatcher("jsp/modificarVotante.jsp").forward(req, resp);
    }

    private void guardarExistente(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        int id = Integer.parseInt(req.getParameter("idUsuario"));
        String nombre = req.getParameter("nombre");
        String correo = req.getParameter("correo");
        String contraseña = req.getParameter("contraseña");
        Votante v = new Votante(id, nombre, correo, contraseña);
        boolean resultado = votanteDAO.update(v);
        if (resultado) {
            resp.sendRedirect("GestionarVotantesController?ruta=listar");
        } else {
            req.setAttribute("error", "Error al modificar el votante.");
            req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
        }
    }

    private void eliminar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        int id = Integer.parseInt(req.getParameter("id"));
        boolean resultado = votanteDAO.delete(id);
        if (resultado) {
            resp.sendRedirect("GestionarVotantesController?ruta=listar");
        } else {
            req.setAttribute("error", "Error al eliminar el votante.");
            req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
        }
    }
}