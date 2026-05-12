package modelo.Entities;

import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import Conexion.ConexionBD;

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


    @Override
    public Usuario authenticate(String correoElectronico, String contraseña) {
    	String SQL = """
    	        SELECT u.id_usuario, u.nombre, u.correo_electronico,
    	               u.contrasena, a.nivel_acceso
    	        FROM administrador a
    	        JOIN usuario u ON a.id_usuario = u.id_usuario
    	        WHERE u.correo_electronico = ? AND u.contrasena = ?
    	    """;
    	    try {
    	        PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
    	        pstmt.setString(1, correoElectronico);
    	        pstmt.setString(2, contraseña);
    	        ResultSet rs = pstmt.executeQuery();
    	        if (rs.next()) {
    	            return new Administrador(
    	                rs.getInt("id_usuario"),
    	                rs.getString("nombre"),
    	                rs.getString("correo_electronico"),
    	                rs.getString("contrasena"),
    	                rs.getString("nivel_acceso")
    	            );
    	        }
    	    } catch (SQLException e) {
    	        e.printStackTrace();
    	    }
    	    return null;
    	/*for (Administrador admin : getListaAdministradores()) {
            if (admin.getCorreoElectronico().equals(correoElectronico) &&
                admin.getContraseña().equals(contraseña)) {
                return admin;
            }
        }
        return null;
        */
    }
	@Override
	public void cerrarSesion() {

	}

    public boolean validarPermisos(String claveMaestra) {
        return CLAVE_MAESTRA.equals(claveMaestra);
    }

    // Lista en memoria
    public static List<Administrador> getListaAdministradores() {
        String SQL = """
                SELECT u.id_usuario, u.nombre, u.correo_electronico,
                       u.contrasena, a.nivel_acceso
                FROM administrador a
                JOIN usuario u ON a.id_usuario = u.id_usuario
            """;
            List<Administrador> administradores = new ArrayList<>();
            try {
                PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
                ResultSet rs = pstmt.executeQuery();
                while (rs.next()) {
                    administradores.add(new Administrador(
                        rs.getInt("id_usuario"),
                        rs.getString("nombre"),
                        rs.getString("correo_electronico"),
                        rs.getString("contrasena"),
                        rs.getString("nivel_acceso")
                    ));
                }
                ConexionBD.cerrar(rs);
                ConexionBD.cerrar(pstmt);
                ConexionBD.cerrar();
                return administradores;
            } catch (SQLException e) {
                e.printStackTrace();
                return null;
            }

    	/*if (administradores == null) {
            administradores = new ArrayList<>();
            administradores.add(new Administrador(1, "Admin Principal",
                "admin@votoseguro.com", "admin123", "SUPER"));
        }
        return administradores;
        */
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

}