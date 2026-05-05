package modelo.Entities;

import java.io.Serializable;

public abstract class Usuario implements Serializable{
	private static final long serialVersionUID = 1L;
	protected int idUsuario;
	protected String nombre;
	protected String correoElectronico;
	protected String contraseña;
	// Contrato: Obliga a Admin y Votante a implementar su propia validación
    public abstract Usuario authenticate(String correoElectronico, String contraseña);
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
}
