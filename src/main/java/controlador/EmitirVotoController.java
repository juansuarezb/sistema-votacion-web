package controlador;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import modelo.Entities.Usuario;
import modelo.Entities.Votacion;
import modelo.Entities.Votante;
import modelo.Entities.Voto;
import modelo.Entities.Voto.OpcionVoto;

@WebServlet("/EmitirVotoController")
public class EmitirVotoController extends HttpServlet {
    private static final long serialVersionUID = 1L;

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        ruteador(req, resp);
    }

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        ruteador(req, resp);
    }

    private void ruteador(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        String ruta = (req.getParameter("ruta") == null) ? "listar" : req.getParameter("ruta");
        switch (ruta) {
            case "listar":
                listar(req, resp);
                break;
            case "votar":
                votar(req, resp);
                break;
            case "confirmar":
                confirmar(req, resp);
                break;
            default:
                listar(req, resp);
                break;
        }
    }

    private void listar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
    	HttpSession sesion = req.getSession(false);
    	if (sesion == null) {
    	    resp.sendRedirect("jsp/Login.jsp");
    	    return;
    	}
    	Usuario usuario = (Usuario) sesion.getAttribute("autorizado");
    	if (!(usuario instanceof Votante)) {
    	    // El usuario no es un Votante; redirigir a Login
    		resp.sendRedirect("jsp/Login.jsp");
    	    return;
    	}
    	Votante votante = (Votante) usuario;
        // Solo votaciones donde el votante está asignado
        List<Votacion> todasVotaciones = Votacion.getListaVotaciones();
        List<Votacion> votacionesAsignadas = new ArrayList<>();
        for (Votacion v : todasVotaciones) {
            if (v.votanteAsignado(votante.getIdUsuario())) {
                votacionesAsignadas.add(v);
            }
        }
        
        req.setAttribute("votaciones", votacionesAsignadas);
        req.setAttribute("votante", votante);
        req.getRequestDispatcher("jsp/listarVotacionesActivas.jsp").forward(req, resp);
    }

    private void votar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        HttpSession sesion = req.getSession(false);
        if (sesion == null) {
            resp.sendRedirect("jsp/Login.jsp");
            return;
        }
        Usuario usuario = (Usuario) sesion.getAttribute("autorizado");
        if (!(usuario instanceof Votante)) {
            resp.sendRedirect("jsp/Login.jsp");
            return;
        }
        Votante votante = (Votante) usuario;
        // 2. Obtener votación
        int idVotacion = Integer.parseInt(req.getParameter("id"));
        Votacion votacion = Votacion.getVotacionById(idVotacion);
        // 3. Verificar si puede votar
        if (!votante.puedeVotar(idVotacion)) {
            req.setAttribute("error", "Ya has emitido tu voto en esta votación.");
            req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
            return;
        }
        // 4. Mandar a la vista
        req.setAttribute("votacion", votacion);
        req.getRequestDispatcher("jsp/votacion.jsp").forward(req, resp);
    }

    private void confirmar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        // 1. Obtener votante de la sesión
        HttpSession sesion = req.getSession(false);
        if (sesion == null) {
            resp.sendRedirect("jsp/Login.jsp");
            return;
        }
        Usuario usuario = (Usuario) sesion.getAttribute("autorizado");
        if (!(usuario instanceof Votante)) {
            resp.sendRedirect("jsp/Login.jsp");
            return;
        }
        Votante votante = (Votante) usuario;
        // 2. Obtener parámetros
        int idVotacion = Integer.parseInt(req.getParameter("idVotacion"));
        String opcionStr = req.getParameter("opcion");
        // 3. Verificar si puede votar
        if (!votante.puedeVotar(idVotacion)) {
            req.setAttribute("error", "Ya has emitido tu voto en esta votación.");
            req.getRequestDispatcher("jsp/errorLogin.jsp").forward(req, resp);
            return;
        }
        // 4. Crear voto
        OpcionVoto opcion = OpcionVoto.valueOf(opcionStr);
        Voto voto = new Voto(0, null, opcion, idVotacion);
        Voto.create(voto);
        // 5. Marcar votante como votado
        votante.marcarComoVotado(idVotacion);
        // 6. Redirigir a confirmación
        req.getRequestDispatcher("jsp/confirmacionVoto.jsp").forward(req, resp);
    }
}