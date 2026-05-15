package modelo.Entities;

import java.io.Serializable;

public abstract class Usuario implements Serializable{
	private static final long serialVersionUID = 1L;
	private int idUsuario;
	private String nombre;
	private String correoElectronico;
	private String contraseña;
    // 1. Constructor vacío (necesario para que los hijos sean JavaBeans)
    public Usuario() {

    }
    // 2. Constructor con parámetro
    public Usuario(int idUsuario, String nombre, String correoElectronico, String contraseña) {
        this.idUsuario = idUsuario;
        this.nombre = nombre;
        this.correoElectronico = correoElectronico;
        this.contraseña = contraseña;
    }
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

}
