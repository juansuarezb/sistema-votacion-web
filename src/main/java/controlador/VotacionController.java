package controlador;

import java.io.IOException;
import java.time.Duration;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;

import modelo.DAO.IVotacionDAO;
import modelo.DAO.JDBC.JDBCVotacionDAOImpl;
import modelo.Entities.Usuario;
import modelo.Entities.Votacion;

@WebServlet("/votacion")
public class VotacionController extends HttpServlet {
    private static final long serialVersionUID = 1L;

    private IVotacionDAO votacionDAO = new JDBCVotacionDAOImpl();

    @Override
    protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
        HttpSession session = request.getSession(false);
        
        // 1. Obtener usuario real autenticado desde la sesión
        if (session == null || session.getAttribute("autorizado") == null) {
            response.sendRedirect("jsp/Login.jsp");
            return;
        }

        Usuario usuario = (Usuario) session.getAttribute("autorizado");

        // 2. Consulta a base de datos a través del DAO
        String idParam = request.getParameter("id");
        if (idParam == null || idParam.isEmpty()) {
            response.sendRedirect("EmitirVotoController?ruta=listar"); // Regresar al listado si no hay ID
            return;
        }

        int idVotacion = Integer.parseInt(idParam);
        Votacion votacion = votacionDAO.getById(idVotacion);

        // Calculamos la duración basados en la fecha de cierre (asumiendo cierre al final del día 23:59:59)
        LocalDate fechaCierre = LocalDate.parse(votacion.getFechaCierre());
        LocalDateTime fechaFinEleccion = fechaCierre.atTime(LocalTime.MAX);
        LocalDateTime ahora = LocalDateTime.now();

        long horas = 0, minutos = 0, segundos = 0;

        if (fechaFinEleccion != null && fechaFinEleccion.isAfter(ahora)) {
            Duration duracion = Duration.between(ahora, fechaFinEleccion);
            horas = duracion.toHours(); // Total de horas restantes (pueden ser > 24)
            minutos = duracion.toMinutesPart(); // Minutos sobrantes (0-59)
            segundos = duracion.toSecondsPart(); // Segundos sobrantes (0-59)
        }

        request.setAttribute("horasRestantes", horas);
        request.setAttribute("minutosRestantes", minutos);
        request.setAttribute("segundosRestantes", segundos);
        request.setAttribute("votacion", votacion);
        request.setAttribute("usuarioNombre", usuario.getNombre());

        // 3. Pasamos el control al archivo JSP (La vista)
        request.getRequestDispatcher("jsp/votacion.jsp").forward(request, response);
    }
}
