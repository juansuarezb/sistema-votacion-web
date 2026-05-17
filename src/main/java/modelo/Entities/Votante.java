package modelo.Entities;

import java.util.ArrayList;
import java.util.List;

public class Votante extends Usuario {
    private static final long serialVersionUID = 1L;
    private List<Integer> votacionesVotadas = new ArrayList<>();
    private static List<Votante> votantes = null;

    // Constructor para lógica de negocio
    public Votante(int idUsuario, String nombre, String correoElectronico, String contraseña) {
        super(idUsuario, nombre, correoElectronico, contraseña);
    }

    // Constructor vacío
    public Votante() {
        super();
    }

    // Getters y Setters
    public int getIdUsuario() { return idUsuario; }
    public void setIdUsuario(int idUsuario) { this.idUsuario = idUsuario; }
    public String getNombre() { return nombre; }
    public void setNombre(String nombre) { this.nombre = nombre; }
    public String getCorreoElectronico() { return correoElectronico; }
    public void setCorreoElectronico(String correoElectronico) { this.correoElectronico = correoElectronico; }
    public String getContraseña() { return contraseña; }
    public void setContraseña(String contraseña) { this.contraseña = contraseña; }
    public List<Integer> getVotacionesVotadas() { return votacionesVotadas; }

    @Override
    public Usuario authenticate(String correoElectronico, String contraseña) {
        for (Votante votante : getListaVotantes()) {
            if (votante.getCorreoElectronico().equals(correoElectronico) &&
                votante.getContraseña().equals(contraseña)) {
                return votante;
            }
        }
        return null;
    }
    
    @Override
    public void cerrarSesion() {}
    
    // Métodos de negocio
    public boolean puedeVotar(int idVotacion) {
        return !votacionesVotadas.contains(idVotacion);
    }

    public void marcarComoVotado(int idVotacion) {
        votacionesVotadas.add(idVotacion);
    }

    // Lista en memoria
    public static List<Votante> getListaVotantes() {
        if (votantes == null) {
            votantes = new ArrayList<>();
            votantes.add(new Votante(1, "Juan", "juan@gmail.com", "1234"));
            votantes.add(new Votante(2, "Abi", "abi@gmail.com", "1234"));
            votantes.add(new Votante(3, "Michi", "Michi@gmail.com", "1234"));
            votantes.add(new Votante(4, "Gipsi", "gipsi@gmail.com", "1234"));
        }
        return votantes;
    }

    public static Votante getVotanteById(int idUsuario) {
        for (Votante votante : getListaVotantes()) {
            if (votante.getIdUsuario() == idUsuario) return votante;
        }
        return null;
    }

    public static boolean create(Votante v) {
        int max = 0;
        for (Votante votante : getListaVotantes()) {
            if (max < votante.getIdUsuario()) max = votante.getIdUsuario();
        }
        v.setIdUsuario(max + 1);
        getListaVotantes().add(v);
        return true;
    }

    public static boolean update(Votante v) {
        List<Votante> lista = getListaVotantes();
        for (int i = 0; i < lista.size(); i++) {
            if (lista.get(i).getIdUsuario() == v.getIdUsuario()) {
                lista.set(i, v);
                return true;
            }
        }
        return false;
    }

    public static boolean delete(int idUsuario) {
        return getListaVotantes().removeIf(v -> v.getIdUsuario() == idUsuario);
    }
}