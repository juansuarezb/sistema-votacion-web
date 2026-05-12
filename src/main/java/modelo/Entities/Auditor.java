package modelo.Entities;

import java.util.ArrayList;
import java.util.List;

public class Auditor extends Usuario {
    private static final long serialVersionUID = 1L;

    private static List<Auditor> auditores = null;

    // Constructor para lógica de negocio
    public Auditor(int idUsuario, String nombre, String correoElectronico, String contraseña) {
        super(idUsuario, nombre, correoElectronico, contraseña);
    }

    // Constructor vacío
    public Auditor() {
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

    @Override
    public Usuario authenticate(String correoElectronico, String contraseña) {
        for (Auditor auditor : getListaAuditores()) {
            if (auditor.getCorreoElectronico().equals(correoElectronico) &&
                auditor.getContraseña().equals(contraseña)) {
                return auditor;
            }
        }
        return null;
    }

	@Override
	public void cerrarSesion() {
		// TODO Auto-generated method stub

	}
    // Lista en memoria
    public static List<Auditor> getListaAuditores() {
        if (auditores == null) {
            auditores = new ArrayList<>();
            auditores.add(new Auditor(1, "Auditor Principal",
                "auditor@votoseguro.com", "auditor123"));
        }
        return auditores;
    }
    public void generarReporte(){

    }
    public void verLog() {

    }


}