package modelo.Entities;

import java.util.List;

public class Administrador extends Usuario {
	private static final long serialVersionUID = 1L;
    private static List<Votante> votantes = null;
    // 1. Constructor para lógica de negocio
    public Administrador(int idUsuario, String nombre, String correoElectronico, boolean haVotado, String contraseña) {
        // Importante: Los nombres deben coincidir con los parámetros de la madre
        super(idUsuario, nombre, correoElectronico, contraseña); 
    }

    // 2. Constructor vacío (Obligatorio para JavaBean)
    public Administrador() {
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

	@Override
	public Usuario authenticate(String correoElectronico, String contraseña) {
		// TODO Auto-generated method stub
		return null;
	}

	
}
