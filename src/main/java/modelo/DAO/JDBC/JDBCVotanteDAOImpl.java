package modelo.DAO.JDBC;

import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import Conexion.ConexionBD;
import modelo.DAO.IVotanteDAO;
import modelo.Entities.Votante;

public class JDBCVotanteDAOImpl implements IVotanteDAO {

    @Override
    public List<Votante> listar() {
        List<Votante> lista = new ArrayList<>();
        String SQL = """
            SELECT u.*
            FROM usuario u
            JOIN votante v ON u.id_usuario = v.id_usuario
        """;
        try {
            PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
            ResultSet rs = pstmt.executeQuery();
            while (rs.next()) {
                lista.add(new Votante(
                    rs.getInt("id_usuario"),
                    rs.getString("nombre"),
                    rs.getString("correo_electronico"),
                    rs.getString("contrasena")
                ));
            }
            ConexionBD.cerrar(rs);
            ConexionBD.cerrar(pstmt);
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return lista;
    }

    @Override
    public Votante getById(int idUsuario) {
        String SQL = """
            SELECT u.*
            FROM usuario u
            JOIN votante v ON u.id_usuario = v.id_usuario
            WHERE u.id_usuario = ?
        """;
        try {
            PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
            pstmt.setInt(1, idUsuario);
            ResultSet rs = pstmt.executeQuery();
            if (rs.next()) {
                return new Votante(
                    rs.getInt("id_usuario"),
                    rs.getString("nombre"),
                    rs.getString("correo_electronico"),
                    rs.getString("contrasena")
                );
            }
            ConexionBD.cerrar(rs);
            ConexionBD.cerrar(pstmt);
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public boolean create(Votante v) {
        String SQLUsuario = """
            INSERT INTO usuario
            (nombre, correo_electronico, contrasena, id_rol)
            VALUES (?, ?, ?, (SELECT id_rol FROM rol WHERE nombre = 'VOTANTE'))
        """;
        String SQLVotante = """
            INSERT INTO votante(id_usuario)
            VALUES (LAST_INSERT_ID())
        """;
        try {
            PreparedStatement pstmtUsuario = ConexionBD.getConexion().prepareStatement(SQLUsuario);
            pstmtUsuario.setString(1, v.getNombre());
            pstmtUsuario.setString(2, v.getCorreoElectronico());
            pstmtUsuario.setString(3, v.getContraseña());
            pstmtUsuario.executeUpdate();
            ConexionBD.cerrar(pstmtUsuario);

            PreparedStatement pstmtVotante = ConexionBD.getConexion().prepareStatement(SQLVotante);
            boolean resultado = pstmtVotante.executeUpdate() > 0;
            ConexionBD.cerrar(pstmtVotante);
            return resultado;
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return false;
    }

    @Override
    public boolean update(Votante v) {
        String SQL = """
            UPDATE usuario
            SET nombre = ?,
                correo_electronico = ?,
                contrasena = ?
            WHERE id_usuario = ?
        """;
        try {
            PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
            pstmt.setString(1, v.getNombre());
            pstmt.setString(2, v.getCorreoElectronico());
            pstmt.setString(3, v.getContraseña());
            pstmt.setInt(4, v.getIdUsuario());
            boolean resultado = pstmt.executeUpdate() > 0;
            ConexionBD.cerrar(pstmt);
            return resultado;
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return false;
    }

    @Override
    public boolean delete(int idUsuario) {
        String SQL = """
            DELETE FROM usuario
            WHERE id_usuario = ?
        """;
        try {
            PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
            pstmt.setInt(1, idUsuario);
            boolean resultado = pstmt.executeUpdate() > 0;
            ConexionBD.cerrar(pstmt);
            return resultado;
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return false;
    }
}