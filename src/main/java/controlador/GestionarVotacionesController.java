package controlador;

import java.io.IOException;
import java.util.List;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import modelo.Entities.Escrutinio;
import modelo.Entities.Votacion;
import modelo.Entities.Votante;

@WebServlet("/GestionarVotacionesController")
public class GestionarVotacionesController extends HttpServlet {
    private static final long serialVersionUID = 1L;

    @Override
    protected void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        ruteador(request, response);
    }

    @Override
    protected void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        ruteador(request, response);
    }

    private void ruteador(HttpServletRequest request, HttpServletResponse response)
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

    private void modificar(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // 1. Obtener id
        int id = Integer.parseInt(request.getParameter("id"));
        // 2. Buscar votacion
        Votacion v = Votacion.getVotacionById(id);
        // 3. Mandar a la vista
        request.setAttribute("votacion", v);
        request.getRequestDispatcher("jsp/modificarVotacion.jsp").forward(request, response);
    }

    private void guardarExistente(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // 1. Obtener parámetros
        int id = Integer.parseInt(request.getParameter("idVotacion"));
        String titulo = request.getParameter("titulo");
        String descripcion = request.getParameter("descripcion");
        String fechaInicio = request.getParameter("fechaInicio");
        String fechaCierre = request.getParameter("fechaCierre");
        // 2. Actualizar
        Votacion v = new Votacion(id, titulo, descripcion, fechaInicio, fechaCierre);
        boolean resultado = Votacion.update(v);
        // 3. Redirigir
        if (resultado) {
            response.sendRedirect("GestionarVotacionesController?ruta=listarVotaciones");
        } else {
            request.setAttribute("error", "Error al modificar la votación.");
            request.getRequestDispatcher("jsp/errorLogin.jsp").forward(request, response);
        }
    }

    private void eliminar(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // 1. Obtener id
        int id = Integer.parseInt(request.getParameter("id"));
        // 2. Eliminar
        boolean resultado = Votacion.delete(id);
        // 3. Redirigir
        if (resultado) {
            response.sendRedirect("GestionarVotacionesController?ruta=listarVotaciones");
        } else {
            request.setAttribute("error", "Error al eliminar la votación.");
            request.getRequestDispatcher("jsp/errorLogin.jsp").forward(request, response);
        }
    }

    private void listarVotaciones(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        List<Votacion> votaciones = Votacion.getListaVotaciones();
        request.setAttribute("votaciones", votaciones);
        request.getRequestDispatcher("jsp/ListarVotaciones.jsp")
               .forward(request, response);
    }

    private void nuevo(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        request.getRequestDispatcher("jsp/crearVotacion.jsp").forward(request, response);
    }

    private void guardar(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // 1. Obtener parámetros
        String titulo = request.getParameter("titulo");
        String descripcion = request.getParameter("descripcion");
        String fechaInicio = request.getParameter("fechaInicio");
        String fechaCierre = request.getParameter("fechaCierre");

        // 2. Crear la votación
        Votacion v = new Votacion(0, titulo, descripcion, fechaInicio, fechaCierre);
        boolean resultado = Votacion.create(v);

        // 3. Redirigir según resultado
        if (resultado) {
            response.sendRedirect("GestionarVotacionesController?ruta=listarVotaciones");
        } else {
            request.setAttribute("error", "Error al crear la votación.");
            request.getRequestDispatcher("jsp/errorLogin.jsp").forward(request, response);
        }
    }
    
    private void verResultados(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // 1. Obtener id de votación
        int id = Integer.parseInt(request.getParameter("id"));
        // 2. Calcular escrutinio
        Escrutinio escrutinio = Escrutinio.obtenerEscrutinio(id);
        Votacion votacion = Votacion.getVotacionById(id);
        // 3. Mandar a la vista
        request.setAttribute("escrutinio", escrutinio);
        request.setAttribute("votacion", votacion);
        request.getRequestDispatcher("jsp/verResultados.jsp").forward(request, response);
    }
    private void asignar(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // 1. Obtener votación
        int id = Integer.parseInt(request.getParameter("id"));
        Votacion votacion = Votacion.getVotacionById(id);
        // 2. Obtener todos los votantes
        List<Votante> votantes = Votante.getListaVotantes();
        // 3. Mandar a la vista
        request.setAttribute("votacion", votacion);
        request.setAttribute("votantes", votantes);
        request.getRequestDispatcher("jsp/asignarVotantes.jsp").forward(request, response);
    }

    private void guardarAsignacion(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // 1. Obtener votación
        int idVotacion = Integer.parseInt(request.getParameter("idVotacion"));
        Votacion votacion = Votacion.getVotacionById(idVotacion);
        // 2. Obtener IDs seleccionados
        String[] idsSeleccionados = request.getParameterValues("votantes");
        // 3. Limpiar asignaciones previas y reasignar
        votacion.getVotantesAsignados().clear();
        if (idsSeleccionados != null) {
            for (String id : idsSeleccionados) {
                votacion.asignarVotante(Integer.parseInt(id));
            }
        }
        // 4. Redirigir
        response.sendRedirect("GestionarVotacionesController?ruta=listarVotaciones");
    }
}
