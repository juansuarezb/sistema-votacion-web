package modelo.Entities;

import java.util.ArrayList;
import java.util.List;

public class Votante extends Usuario {
    private static final long serialVersionUID = 1L;
    private List<Integer> votacionesVotadas = new ArrayList<>();

    // Constructor para lógica de negocio
    public Votante(int idUsuario, String nombre, String correoElectronico, String contraseña) {
        super(idUsuario, nombre, correoElectronico, contraseña);
    }

    // Constructor vacío
    public Votante() {
        super();
    }

	public List<Integer> getVotacionesVotadas() {
		return votacionesVotadas;
	}

	public void setVotacionesVotadas(List<Integer> votacionesVotadas) {
		this.votacionesVotadas = votacionesVotadas;
	}

    // Métodos de negocio
    public boolean puedeVotar(int idVotacion) {
        return !votacionesVotadas.contains(idVotacion);
    }

    public void marcarComoVotado(int idVotacion) {
        votacionesVotadas.add(idVotacion);
    }



}