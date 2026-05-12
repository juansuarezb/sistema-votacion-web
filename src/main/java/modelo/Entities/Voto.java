package modelo.Entities;

import java.io.Serializable;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

public class Voto implements Serializable {
    private static final long serialVersionUID = 1L;
    private int idVoto;
    private LocalDateTime fecha;
    private OpcionVoto opcionSeleccionada;
    private int idVotacion;

    private static List<Voto> votos = null;

    public enum OpcionVoto {
        SI, NO, BLANCO, NULO
    }

    public Voto(int idVoto, LocalDateTime fecha, OpcionVoto opcionSeleccionada, int idVotacion) {
        this.idVoto = idVoto;
        this.fecha = fecha;
        this.opcionSeleccionada = opcionSeleccionada;
        this.idVotacion = idVotacion;
    }

    public Voto() {}

    // Getters y Setters
    public int getIdVoto() { return idVoto; }
    public void setIdVoto(int idVoto) { this.idVoto = idVoto; }
    public LocalDateTime getFecha() { return fecha; }
    public void setFecha(LocalDateTime fecha) { this.fecha = fecha; }
    public OpcionVoto getOpcionSeleccionada() { return opcionSeleccionada; }
    public void setOpcionSeleccionada(OpcionVoto opcionSeleccionada) { this.opcionSeleccionada = opcionSeleccionada; }
    public int getIdVotacion() { return idVotacion; }
    public void setIdVotacion(int idVotacion) { this.idVotacion = idVotacion; }

    // Lista en memoria
    public static List<Voto> getListaVotos() {
        if (votos == null) {
			votos = new ArrayList<>();
		}
        return votos;
    }

    public static boolean create(Voto v) {
        int max = 0;
        for (Voto voto : getListaVotos()) {
            if (max < voto.getIdVoto()) {
				max = voto.getIdVoto();
			}
        }
        v.setIdVoto(max + 1);
        v.setFecha(LocalDateTime.now());
        getListaVotos().add(v);
        return true;
    }
}