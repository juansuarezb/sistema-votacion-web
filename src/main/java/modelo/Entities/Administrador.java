package modelo.Entities;

import java.util.ArrayList;
import java.util.List;

public class Administrador extends Usuario {
    private static final long serialVersionUID = 1L;
    private String nivelAcceso;
    private static final String CLAVE_MAESTRA = "votoseguro2025";
    private static List<Administrador> administradores = null;

    // Constructor para lógica de negocio
    public Administrador(int idUsuario, String nombre, String correoElectronico,
                         String contraseña, String nivelAcceso) {
        super(idUsuario, nombre, correoElectronico, contraseña);
        this.nivelAcceso = nivelAcceso;
    }

    // Constructor vacío
    public Administrador() {
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
    public String getNivelAcceso() { return nivelAcceso; }
    public void setNivelAcceso(String nivelAcceso) { this.nivelAcceso = nivelAcceso; }

    // Lista en memoria
    public static List<Administrador> getListaAdministradores() {
        if (administradores == null) {
            administradores = new ArrayList<>();
            administradores.add(new Administrador(1, "Admin Principal",
                "admin@votoseguro.com", "admin123", "SUPER"));
        }
        return administradores;
    }

    public boolean validarPermisos(String claveMaestra) {
        return CLAVE_MAESTRA.equals(claveMaestra);
    }

    // Registrar nuevo Admin
    public static boolean create(Administrador a) {
        int max = 0;
        for (Administrador admin : getListaAdministradores()) {
            if (max < admin.getIdUsuario()) {
				max = admin.getIdUsuario();
			}
        }
        a.setIdUsuario(max + 1);
        getListaAdministradores().add(a);
        return true;
    }

    @Override
    public Usuario authenticate(String correoElectronico, String contraseña) {
        for (Administrador admin : getListaAdministradores()) {
            if (admin.getCorreoElectronico().equals(correoElectronico) &&
                admin.getContraseña().equals(contraseña)) {
                return admin;
            }
        }
        return null;
    }

	@Override
	public void cerrarSesion() {
		// TODO Auto-generated method stub

	}
}