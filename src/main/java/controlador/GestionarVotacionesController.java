package controlador;

import java.io.IOException;
import java.util.List;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import modelo.DAO.IVotacionDAO;
import modelo.DAO.IVotanteDAO;
import modelo.DAO.IVotoDAO;
import modelo.DAO.JDBC.JDBCVotacionDAOImpl;
import modelo.DAO.JDBC.JDBCVotanteDAOImpl;
import modelo.DAO.JDBC.JDBCVotoDAOImpl;
import modelo.Entities.Escrutinio;
import modelo.Entities.Votacion;
import modelo.Entities.Votante;
import modelo.Entities.Voto;

@WebServlet("/GestionarVotacionesController")
public class GestionarVotacionesController extends HttpServlet {

    private static final long serialVersionUID = 1L;

    private IVotacionDAO votacionDAO =
            new JDBCVotacionDAOImpl();

    private IVotanteDAO votanteDAO =
            new JDBCVotanteDAOImpl();

    private IVotoDAO votoDAO =
            new JDBCVotoDAOImpl();
    @Override
    protected void doGet(HttpServletRequest request,
                         HttpServletResponse response)
            throws ServletException, IOException {

        ruteador(request, response);
    }

    @Override
    protected void doPost(HttpServletRequest request,
                          HttpServletResponse response)
            throws ServletException, IOException {

        ruteador(request, response);
    }

    private void ruteador(HttpServletRequest request,
                          HttpServletResponse response)
            throws ServletException, IOException {

        String ruta = request.getParameter("ruta");

        if (ruta == null) {
            ruta = "";
        }

        switch (ruta) {

            case "listarVotaciones":
                listarVotaciones(request, response);
                break;

            case "nuevo":
                nuevo(request, response);
                break;

            case "guardar":
                guardar(request, response);
                break;

            case "modificar":
                modificar(request, response);
                break;

            case "guardarExistente":
                guardarExistente(request, response);
                break;

            case "eliminar":
                eliminar(request, response);
                break;

            case "verResultados":
                verResultados(request, response);
                break;

            case "asignar":
                asignar(request, response);
                break;

            case "guardarAsignacion":
                guardarAsignacion(request, response);
                break;

            default:
                listarVotaciones(request, response);
                break;
        }
    }

    private void listarVotaciones(HttpServletRequest request,
                                  HttpServletResponse response)
            throws ServletException, IOException {
        List<Votacion> votaciones = votacionDAO.listar();

        request.setAttribute("votaciones", votaciones);

        request.getRequestDispatcher("jsp/ListarVotaciones.jsp")
                .forward(request, response);
    }

    private void nuevo(HttpServletRequest request,
                       HttpServletResponse response)
            throws ServletException, IOException {

        request.getRequestDispatcher("jsp/crearVotacion.jsp")
                .forward(request, response);
    }

    private void guardar(HttpServletRequest request,
                         HttpServletResponse response)
            throws ServletException, IOException {

        String titulo = request.getParameter("titulo");

        String descripcion =
                request.getParameter("descripcion");

        String fechaInicio =
                request.getParameter("fechaInicio");

        String fechaCierre =
                request.getParameter("fechaCierre");
        Votacion votacion =
                new Votacion(
                        0,
                        titulo,
                        descripcion,
                        fechaInicio,
                        fechaCierre
                );

        boolean resultado =
                votacionDAO.create(votacion);

        if (resultado) {

            response.sendRedirect(
                    "GestionarVotacionesController?ruta=listarVotaciones");

        } else {

            request.setAttribute(
                    "error",
                    "Error al crear la votación."
            );

            request.getRequestDispatcher("jsp/errorLogin.jsp")
                    .forward(request, response);
            }

    }

    private void modificar(HttpServletRequest request,
                           HttpServletResponse response)
            throws ServletException, IOException {
        int id =
                Integer.parseInt(request.getParameter("id"));

        Votacion votacion =
                votacionDAO.getById(id);

        request.setAttribute("votacion", votacion);

        request.getRequestDispatcher("jsp/modificarVotacion.jsp")
                .forward(request, response);
    }

    private void guardarExistente(HttpServletRequest request,
                                  HttpServletResponse response)
            throws ServletException, IOException {
        int id =
                Integer.parseInt(
                        request.getParameter("idVotacion")
                );

        String titulo =
                request.getParameter("titulo");

        String descripcion =
                request.getParameter("descripcion");

        String fechaInicio =
                request.getParameter("fechaInicio");

        String fechaCierre =
                request.getParameter("fechaCierre");

        Votacion votacion =
                new Votacion(
                        id,
                        titulo,
                        descripcion,
                        fechaInicio,
                        fechaCierre
                );

        boolean resultado =
                votacionDAO.update(votacion);

        if (resultado) {

            response.sendRedirect(
                    "GestionarVotacionesController?ruta=listarVotaciones");

        } else {

            request.setAttribute(
                    "error",
                    "Error al modificar la votación."
            );

            request.getRequestDispatcher("jsp/errorLogin.jsp")
                    .forward(request, response);
        }
    }

    private void eliminar(HttpServletRequest request,
                          HttpServletResponse response)
            throws ServletException, IOException {
        int id =
                Integer.parseInt(request.getParameter("id"));

        boolean resultado =
                votacionDAO.delete(id);

        if (resultado) {

            response.sendRedirect(
                    "GestionarVotacionesController?ruta=listarVotaciones");

        } else {

            request.setAttribute(
                    "error",
                    "Error al eliminar la votación."
            );

            request.getRequestDispatcher("jsp/errorLogin.jsp")
                    .forward(request, response);
        }
    }

    private void verResultados(HttpServletRequest request,
                               HttpServletResponse response)
            throws ServletException, IOException {
        int id =
                Integer.parseInt(request.getParameter("id"));

        Votacion votacion =
                votacionDAO.getById(id);

        List<Voto> votos =
                votoDAO.getByVotacion(id);

        List<Votante> votantes =
                votanteDAO.listar();

        Escrutinio escrutinio =
                new Escrutinio(id);

        escrutinio.calcularResultados(votos, votantes);

        request.setAttribute("escrutinio", escrutinio);

        request.setAttribute("votacion", votacion);

        request.getRequestDispatcher("jsp/verResultados.jsp")
                .forward(request, response);
    }

    private void asignar(HttpServletRequest request,
                         HttpServletResponse response)
            throws ServletException, IOException {
        int id =
                Integer.parseInt(request.getParameter("id"));

        Votacion votacion =
                votacionDAO.getById(id);

        List<Votante> votantes =
                votanteDAO.listar();

        request.setAttribute("votacion", votacion);

        request.setAttribute("votantes", votantes);

        request.getRequestDispatcher("jsp/asignarVotantes.jsp")
                .forward(request, response);
    }

    private void guardarAsignacion(HttpServletRequest request,
                                   HttpServletResponse response)
            throws ServletException, IOException {
        int idVotacion =
                Integer.parseInt(
                        request.getParameter("idVotacion")
                );

        Votacion votacion =
                votacionDAO.getById(idVotacion);

        String[] idsSeleccionados =
                request.getParameterValues("votantes");

        votacion.getVotantesAsignados().clear();

        if (idsSeleccionados != null) {

            for (String id : idsSeleccionados) {

                votacion.asignarVotante(
                        Integer.parseInt(id)
                );
            }
        }

        votacionDAO.update(votacion);

        response.sendRedirect(
                "GestionarVotacionesController?ruta=listarVotaciones"
        );
    }
}