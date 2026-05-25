package controlador;

import java.io.IOException;
import java.util.List;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;
import modelo.DAO.IVotacionDAO;
import modelo.DAO.IVotoDAO;
import modelo.DAO.JDBC.JDBCVotacionDAOImpl;
import modelo.DAO.JDBC.JDBCVotoDAOImpl;
import modelo.Entities.Usuario;
import modelo.Entities.Votacion;
import modelo.Entities.Votante;
import modelo.Entities.Voto;
import modelo.Entities.Voto.OpcionVoto;
import websocket.ResultadosWebSocket;

@WebServlet("/EmitirVotoController")
public class EmitirVotoController extends HttpServlet {
    private static final long serialVersionUID = 1L;

    private IVotacionDAO votacionDAO = new JDBCVotacionDAOImpl();
    private IVotoDAO votoDAO = new JDBCVotoDAOImpl();

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

    private Votante getVotanteFromSession(HttpServletRequest req, HttpServletResponse resp)
            throws IOException {
        HttpSession sesion = req.getSession(false);
        if (sesion == null) {
            resp.sendRedirect("jsp/publicas/login.jsp");
            return null;
        }
        Usuario usuario = (Usuario) sesion.getAttribute("autorizado");
        if (!(usuario instanceof Votante)) {
            resp.sendRedirect("jsp/publicas/login.jsp");
            return null;
        }
        return (Votante) usuario;
    }

    private void listar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        Votante votante = getVotanteFromSession(req, resp);
        if (votante == null)
            return;

        // Sincronizar votaciones ya votadas desde BD
        List<Integer> yaVotadas = votoDAO.getVotacionesVotadasByVotante(votante.getIdUsuario());
        votante.setVotacionesVotadas(yaVotadas);

        List<Votacion> asignadas = votacionDAO.getVotacionesByVotante(votante.getIdUsuario());
        req.setAttribute("votaciones", asignadas);
        req.setAttribute("votante", votante);
        req.getRequestDispatcher("jsp/votante/lista_votaciones_activas.jsp").forward(req, resp);
    }

    private void votar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        Votante votante = getVotanteFromSession(req, resp);
        if (votante == null) {
            return;
        }

        int idVotacion = Integer.parseInt(req.getParameter("id"));
        Votacion votacion = votacionDAO.getById(idVotacion);

        // Verificar si ya votó — consultar BD
        List<Voto> votosExistentes = votoDAO.getByVotacion(idVotacion);
        boolean yaVoto = !votante.puedeVotar(idVotacion);

        if (yaVoto) {
            req.setAttribute("error", "Ya has emitido tu voto en esta votación.");
            req.getRequestDispatcher("jsp/error_login.jsp").forward(req, resp);
            return;
        }

        req.setAttribute("votacion", votacion);
        req.getRequestDispatcher("jsp/votante/voto.jsp").forward(req, resp);
    }

    private void confirmar(HttpServletRequest req, HttpServletResponse resp)
            throws ServletException, IOException {
        Votante votante = getVotanteFromSession(req, resp);
        if (votante == null) {
            return;
        }

        int idVotacion = Integer.parseInt(req.getParameter("idVotacion"));
        String opcionStr = req.getParameter("opcion");

        // Verificar si ya votó
        if (!votante.puedeVotar(idVotacion)) {
            req.setAttribute("error", "Ya has emitido tu voto en esta votación.");
            req.getRequestDispatcher("jsp/error_login.jsp").forward(req, resp);
            return;
        }

        // Registrar voto en BD
        OpcionVoto opcion = OpcionVoto.valueOf(opcionStr);
        Voto voto = new Voto(0, null, opcion, idVotacion);
        boolean resultado = votoDAO.create(voto);

        if (resultado) {
            // Marcar en sesión que ya votó
            votante.marcarComoVotado(idVotacion);
            // Notificar a todos los que están viendo resultados de esta votación
            ResultadosWebSocket.notificarNuevoVoto(idVotacion);
            // Actualizar ha_votado en BD
            votacionDAO.marcarVotoEmitido(idVotacion, votante.getIdUsuario());

            req.getRequestDispatcher("jsp/votante/confirmacion_voto.jsp").forward(req, resp);
        } else {
            req.setAttribute("error", "Error al registrar el voto.");
            req.getRequestDispatcher("jsp/error_login.jsp").forward(req, resp);
        }
    }
}