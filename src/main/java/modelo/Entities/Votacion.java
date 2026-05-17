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
    private int idAdmin;
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
    public int getIdAdmin() { return idAdmin; }
    public void setIdAdmin(int idAdmin) { this.idAdmin = idAdmin; }
    public List<Integer> getVotantesAsignados() { return votantesAsignados; }

    // Lógica de negocio pura — no toca BD
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