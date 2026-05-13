package modelo.Entities;

public class Administrador extends Usuario {
    private static final long serialVersionUID = 1L;
    private String nivelAcceso;
    private static final String CLAVE_MAESTRA = "votoseguro2025";

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

    public String getNivelAcceso() { return nivelAcceso; }
    public void setNivelAcceso(String nivelAcceso) { this.nivelAcceso = nivelAcceso; }

	public static String getClaveMaestra() {
		return CLAVE_MAESTRA;
	}
    
    public boolean validarPermisos(String claveMaestra) {
        return CLAVE_MAESTRA.equals(claveMaestra);
    }





}