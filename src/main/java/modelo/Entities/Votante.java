package modelo;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class Votante extends Usuario implements Serializable {
    private static final long serialVersionUID = 1L;
    private boolean haVotado;
    
    private static List<Votante> votantes = null;
    // 1. Constructor para lógica de negocio
    public Votante(int idUsuario, String nombre, String correoElectronico, boolean haVotado, String contraseña) {
        // Importante: Los nombres deben coincidir con los parámetros de la madre
        super(idUsuario, nombre, correoElectronico, contraseña); 
        this.haVotado = haVotado;
    }

    // 2. Constructor vacío (Obligatorio para JavaBean)
    public Votante() {
        super();
    }

    // --- Getters y Setters (Puente hacia la madre) ---

    public int getIdUsuario() { 
        return idUsuario; 
    }
    
    public void setIdUsuario(int idUsuario) { 
        this.idUsuario = idUsuario; 
    }

    public String getNombre() { 
        return nombre; 
    }
    
    public void setNombre(String nombre) { 
        this.nombre = nombre; 
    }

    public String getCorreoElectronico() { 
        return correoElectronico;
    }
    
    public void setCorreoElectronico(String correoElectronico) {
        this.correoElectronico = correoElectronico;
    }

    public String getContraseña() {
        return contraseña;
    }
    
    public void setContraseña(String contraseña) {
        this.contraseña = contraseña;
    }

    public boolean isHaVotado() {
        return haVotado;
    }

    public void setHaVotado(boolean haVotado) {
        this.haVotado = haVotado;
    }

    /* === Métodos de Negocio & Persistencia === */
    
    @Override
    public Usuario authenticate(String correoElectronico, String contraseña) {
        // Aquí implementarás la lógica de login más adelante
        return null;
    }

    // Métodos estáticos para gestión (Lógica de DAO integrada)
    
    public static List<Votante> getListaVotantes() {
        if(votantes == null) {
        	votantes = new ArrayList<Votante>();
        	votantes.add(new Votante(1, "Juan", "juan@gmail.com", true, "1234"));
        	votantes.add(new Votante(1, "Abi", "abi@gmail.com", false, "1234"));
        	votantes.add(new Votante(1, "Michi", "Michi@gmail.com", false, "1234"));
        	votantes.add(new Votante(1, "Gipsi", "gipsi@gmail.com", false, "1234"));
        }
    	return votantes; 
    }

    public static Votante getVotanteById(int idUsuario) {
        return null;
    }

    public static boolean create(Votante v) {
        return true;
    }

    public static boolean update(Votante v) {
        return false;
    }

    public static boolean delete(int idUsuario) {
        return true;
    }
}
