package modelo.DAO.JDBC;

import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import Conexion.ConexionBD;
import modelo.DAO.IUsuarioDAO;
import modelo.Entities.Administrador;
import modelo.Entities.Auditor;
import modelo.Entities.Usuario;
import modelo.Entities.Votante;

public class JDBCUsuarioDAOImpl implements IUsuarioDAO {

    @Override
    public Usuario authenticate(String correoElectronico, String contraseña) {
    	String SQL = """
    		    SELECT
    		        u.id_usuario,
    		        u.nombre,
    		        u.correo_electronico,
    		        u.contrasena,
    		        r.nombre AS rol,
    		        a.nivel_acceso
    		    FROM usuario u
    		    JOIN rol r ON u.id_rol = r.id_rol
    		    LEFT JOIN administrador a ON u.id_usuario = a.id_usuario
    		    LEFT JOIN votante v ON u.id_usuario = v.id_usuario
    		    LEFT JOIN auditor au ON u.id_usuario = au.id_usuario
    		    WHERE u.correo_electronico = ?
    		    AND u.contrasena = ?
    		""";

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion().prepareStatement(SQL);

            pstmt.setString(1, correoElectronico);
            pstmt.setString(2, contraseña);

            ResultSet rs = pstmt.executeQuery();

            if (rs.next()) {

                int idUsuario = rs.getInt("id_usuario");
                String nombre = rs.getString("nombre");
                String correo = rs.getString("correo_electronico");
                String contrasena = rs.getString("contrasena");
                String rol = rs.getString("rol");

                switch (rol.toUpperCase()) {

                    case "ADMINISTRADOR":

                        return new Administrador(
                                idUsuario,
                                nombre,
                                correo,
                                contrasena,
                                rs.getString("nivel_acceso")
                        );

                    case "VOTANTE":

                        return new Votante(
                                idUsuario,
                                nombre,
                                correo,
                                contrasena
                        );

                    case "AUDITOR":

                        return new Auditor(
                                idUsuario,
                                nombre,
                                correo,
                                contrasena
                        );

                    default:
                        return null;
                }
            }

            ConexionBD.cerrar(rs);
            ConexionBD.cerrar(pstmt);

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }
}