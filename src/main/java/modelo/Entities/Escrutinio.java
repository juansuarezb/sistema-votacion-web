package modelo.Entities;

import java.io.Serializable;
import java.time.LocalDateTime;
import java.util.List;

public class Escrutinio implements Serializable {
    private static final long serialVersionUID = 1L;

    private int idVotacion;
    private int totalVotosEmitidos;
    private int votosSi;
    private int votosNo;
    private int votosBlanco;
    private int votosNulo;
    private double porcentajeParticipacion;
    private LocalDateTime fechaHoraCierre;

    public Escrutinio(int idVotacion) {
        this.idVotacion = idVotacion;
        calcularResultados();
    }

    public Escrutinio() {}

    // Getters
    public int getIdVotacion() { return idVotacion; }
    public int getTotalVotosEmitidos() { return totalVotosEmitidos; }
    public int getVotosSi() { return votosSi; }
    public int getVotosNo() { return votosNo; }
    public int getVotosBlanco() { return votosBlanco; }
    public int getVotosNulo() { return votosNulo; }
    public double getPorcentajeParticipacion() { return porcentajeParticipacion; }
    public LocalDateTime getFechaHoraCierre() { return fechaHoraCierre; }

    // Calcula resultados en base a la lista de votos
    public void calcularResultados() {
        List<Voto> listaVotos = Voto.getListaVotos();
        List<Votante> listaVotantes = Votante.getListaVotantes();

        votosSi = 0;
        votosNo = 0;
        votosBlanco = 0;
        votosNulo = 0;

        for (Voto voto : listaVotos) {
            if (voto.getIdVotacion() == idVotacion) {
                switch (voto.getOpcionSeleccionada()) {
                    case SI:     votosSi++;     break;
                    case NO:     votosNo++;     break;
                    case BLANCO: votosBlanco++; break;
                    case NULO:   votosNulo++;   break;
                }
            }
        }

        totalVotosEmitidos = votosSi + votosNo + votosBlanco + votosNulo;

        int totalVotantes = listaVotantes.size();
        porcentajeParticipacion = totalVotantes > 0
            ? (totalVotosEmitidos * 100.0) / totalVotantes
            : 0.0;

        fechaHoraCierre = LocalDateTime.now();
    }

    public static Escrutinio obtenerEscrutinio(int idVotacion) {
        return new Escrutinio(idVotacion);
    }
}