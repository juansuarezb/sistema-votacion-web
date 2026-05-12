package modelo.Entities;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class Votacion implements Serializable {
	private static final long serialVersionUID = 1L;
    private int idVotacion;
    private String titulo;
    private String descripcion;
    private String fechaInicio;
    private String fechaCierre;

    private static List<Votacion> votaciones = null;
    private List<Integer> votantesAsignados = new ArrayList<>();
    // Constructor para lógica de negocio
    public Votacion(int idVotacion, String titulo, String descripcion,
                    String fechaInicio, String fechaCierre) {
        this.idVotacion = idVotacion;
        this.titulo = titulo;
        this.descripcion = descripcion;
        this.fechaInicio = fechaInicio;
        this.fechaCierre = fechaCierre;
    }

    // Constructor vacío
    public Votacion() {}

    // Getters y Setters
    public int getIdVotacion() { return idVotacion; }
    public void setIdVotacion(int idVotacion) { this.idVotacion = idVotacion; }
    public String getTitulo() { return titulo; }
    public void setTitulo(String titulo) { this.titulo = titulo; }
    public String getDescripcion() { return descripcion; }
    public void setDescripcion(String descripcion) { this.descripcion = descripcion; }
    public String getFechaInicio() { return fechaInicio; }
    public void setFechaInicio(String fechaInicio) { this.fechaInicio = fechaInicio; }
    public String getFechaCierre() { return fechaCierre; }
    public void setFechaCierre(String fechaCierre) { this.fechaCierre = fechaCierre; }

    // Lista en memoria
    public static List<Votacion> getListaVotaciones() {
        if (votaciones == null) {
            votaciones = new ArrayList<>();
            votaciones.add(new Votacion(1, "Elección Presidencial",
                "Vota por tu candidato", "2025-01-01", "2025-01-31"));
            votaciones.add(new Votacion(2, "Reforma Estatutaria",
                "Aprueba o rechaza la reforma", "2025-02-01", "2025-02-28"));
            votaciones.add(new Votacion(3, "Elección Representantes",
                "Elige tus representantes", "2025-03-01", "2025-03-31"));
        }
        return votaciones;
    }

    public static Votacion getVotacionById(int idVotacion) {
        for (Votacion v : getListaVotaciones()) {
            if (v.getIdVotacion() == idVotacion) {
				return v;
			}
        }
        return null;
    }
    public static boolean create(Votacion v) {
        int max = 0;
        for (Votacion votacion : getListaVotaciones()) {
            if (max < votacion.getIdVotacion()) {
                max = votacion.getIdVotacion();
            }
        }
        v.setIdVotacion(max + 1);
        getListaVotaciones().add(v);
        return true;
    }
    public static boolean update(Votacion v) {
        List<Votacion> lista = getListaVotaciones();
        for (int i = 0; i < lista.size(); i++) {
            if (lista.get(i).getIdVotacion() == v.getIdVotacion()) {
                lista.set(i, v);
                return true;
            }
        }
        return false;
    }

    public static boolean delete(int idVotacion) {
        return getListaVotaciones().removeIf(v -> v.getIdVotacion() == idVotacion);
    }
    public List<Integer> getVotantesAsignados() {
        return votantesAsignados;
    }

    public void asignarVotante(int idVotante) {
        if (!votantesAsignados.contains(idVotante)) {
            votantesAsignados.add(idVotante);
        }
    }

    public void desasignarVotante(int idVotante) {
        votantesAsignados.removeIf(id -> id == idVotante);
    }

    public boolean votanteAsignado(int idVotante) {
        return votantesAsignados.contains(idVotante);
    }
}
