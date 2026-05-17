package websocket;

import java.io.IOException;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;

import jakarta.websocket.OnClose;
import jakarta.websocket.OnError;
import jakarta.websocket.OnOpen;
import jakarta.websocket.Session;
import jakarta.websocket.server.PathParam;
import jakarta.websocket.server.ServerEndpoint;
import modelo.DAO.IVotanteDAO;
import modelo.DAO.IVotoDAO;
import modelo.DAO.JDBC.JDBCVotanteDAOImpl;
import modelo.DAO.JDBC.JDBCVotoDAOImpl;
import modelo.Entities.Escrutinio;
import modelo.Entities.Votante;
import modelo.Entities.Voto;

@ServerEndpoint("/resultados/{idVotacion}")
public class ResultadosWebSocket {

    // Mapa de sesiones por votación
    private static Map<Integer, Set<Session>> sesionesVotacion = new ConcurrentHashMap<>();

    private IVotoDAO votoDAO = new JDBCVotoDAOImpl();
    private IVotanteDAO votanteDAO = new JDBCVotanteDAOImpl();

    @OnOpen
    public void onOpen(Session session, @PathParam("idVotacion") int idVotacion)
            throws IOException {

        System.out.println("Cliente conectado a votación " + idVotacion);

        sesionesVotacion
            .computeIfAbsent(idVotacion, k -> Collections.synchronizedSet(new HashSet<>()))
            .add(session);

        enviarResultados(idVotacion);
    }

    @OnClose
    public void onClose(Session session, @PathParam("idVotacion") int idVotacion) {

        System.out.println("Cliente desconectado");

        Set<Session> sesiones = sesionesVotacion.get(idVotacion);

        if (sesiones != null) {
            sesiones.remove(session);
        }
    }

    @OnError
    public void onError(Session session, Throwable throwable) {
        throwable.printStackTrace();
    }

    // Método estático para que el Controller notifique cuando hay un nuevo voto
    public static void notificarNuevoVoto(int idVotacion) {
        ResultadosWebSocket ws = new ResultadosWebSocket();
        ws.enviarResultados(idVotacion);
    }

    private void enviarResultados(int idVotacion) {
        Set<Session> sesiones = sesionesVotacion.get(idVotacion);
        if (sesiones == null || sesiones.isEmpty()) {
			return;
		}

        List<Voto> votos = votoDAO.getByVotacion(idVotacion);
        List<Votante> votantes = votanteDAO.listar();

        Escrutinio escrutinio = new Escrutinio(idVotacion);
        escrutinio.calcularResultados(votos, votantes);

        String json = String.format(
            "{\"si\":%d,\"no\":%d,\"blanco\":%d,\"nulo\":%d,\"total\":%d,\"participacion\":%.2f}",
            escrutinio.getVotosSi(),
            escrutinio.getVotosNo(),
            escrutinio.getVotosBlanco(),
            escrutinio.getVotosNulo(),
            escrutinio.getTotalVotosEmitidos(),
            escrutinio.getPorcentajeParticipacion()
        );

        for (Session s : sesiones) {
            if (s.isOpen()) {
                try {
                    s.getBasicRemote().sendText(json);
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }
    }
}
