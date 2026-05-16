package modelo.DAO.JDBC;

import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import Conexion.ConexionBD;
import modelo.DAO.IVotacionDAO;
import modelo.Entities.Votacion;
import modelo.Entities.Votante;

public class JDBCVotacionDAOImpl implements IVotacionDAO {

    @Override
    public List<Votacion> listar() {

        List<Votacion> lista = new ArrayList<>();

        String SQL = """
            SELECT *
            FROM votacion
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            ResultSet rs = pstmt.executeQuery();

            while (rs.next()) {

                Votacion v = new Votacion(
                        rs.getInt("id_votacion"),
                        rs.getString("titulo"),
                        rs.getString("descripcion"),
                        rs.getString("fecha_inicio"),
                        rs.getString("fecha_cierre")
                );

                lista.add(v);
            }

            ConexionBD.cerrar(rs);
            ConexionBD.cerrar(pstmt);

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return lista;
    }

    @Override
    public Votacion getById(int idVotacion) {

        String SQL = """
            SELECT *
            FROM votacion
            WHERE id_votacion = ?
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            pstmt.setInt(1, idVotacion);

            ResultSet rs = pstmt.executeQuery();

            if (rs.next()) {

                return new Votacion(
                        rs.getInt("id_votacion"),
                        rs.getString("titulo"),
                        rs.getString("descripcion"),
                        rs.getString("fecha_inicio"),
                        rs.getString("fecha_cierre")
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
    public boolean create(Votacion v) {

        String SQL = """
            INSERT INTO votacion
            (
                titulo,
                descripcion,
                fecha_inicio,
                fecha_cierre
            )
            VALUES (?, ?, ?, ?)
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            pstmt.setString(1, v.getTitulo());
            pstmt.setString(2, v.getDescripcion());
            pstmt.setString(3, v.getFechaInicio());
            pstmt.setString(4, v.getFechaCierre());

            return pstmt.executeUpdate() > 0;

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    @Override
    public boolean update(Votacion v) {

        String SQL = """
            UPDATE votacion
            SET
                titulo = ?,
                descripcion = ?,
                fecha_inicio = ?,
                fecha_cierre = ?
            WHERE id_votacion = ?
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            pstmt.setString(1, v.getTitulo());
            pstmt.setString(2, v.getDescripcion());
            pstmt.setString(3, v.getFechaInicio());
            pstmt.setString(4, v.getFechaCierre());
            pstmt.setInt(5, v.getIdVotacion());

            return pstmt.executeUpdate() > 0;

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    @Override
    public boolean delete(int idVotacion) {

        String SQL = """
            DELETE FROM votacion
            WHERE id_votacion = ?
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            pstmt.setInt(1, idVotacion);

            return pstmt.executeUpdate() > 0;

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    @Override
    public boolean asignarVotante(
            int idVotacion,
            int idVotante
    ) {

        String SQL = """
            INSERT INTO votacion_votante
            (
                id_votacion,
                id_votante
            )
            VALUES (?, ?)
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            pstmt.setInt(1, idVotacion);
            pstmt.setInt(2, idVotante);

            return pstmt.executeUpdate() > 0;

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    @Override
    public List<Votante> getVotantesAsignados(
            int idVotacion
    ) {

        List<Votante> lista = new ArrayList<>();

        String SQL = """
            SELECT
                u.id_usuario,
                u.nombre,
                u.correo_electronico,
                u.contrasena
            FROM votacion_votante vv

            JOIN votante v
                ON vv.id_votante = v.id_usuario

            JOIN usuario u
                ON u.id_usuario = v.id_usuario

            WHERE vv.id_votacion = ?
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            pstmt.setInt(1, idVotacion);

            ResultSet rs = pstmt.executeQuery();

            while (rs.next()) {

                Votante votante = new Votante(
                        rs.getInt("id_usuario"),
                        rs.getString("nombre"),
                        rs.getString("correo_electronico"),
                        rs.getString("contrasena")
                );

                lista.add(votante);
            }

            ConexionBD.cerrar(rs);
            ConexionBD.cerrar(pstmt);

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return lista;
    }
    @Override
    public boolean marcarVotoEmitido(int idVotacion, int idVotante) {
        String SQL = """
            UPDATE votacion_votante
            SET ha_votado = TRUE
            WHERE id_votacion = ? AND id_votante = ?
        """;
        try {
            PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
            pstmt.setInt(1, idVotacion);
            pstmt.setInt(2, idVotante);
            boolean resultado = pstmt.executeUpdate() > 0;
            ConexionBD.cerrar(pstmt);
            return resultado;
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return false;
    }
    @Override
    public List<Votacion> getVotacionesByVotante(int idVotante) {
        List<Votacion> lista = new ArrayList<>();
        String SQL = """
            SELECT v.*
            FROM votacion v
            JOIN votacion_votante vv ON v.id_votacion = vv.id_votacion
            WHERE vv.id_votante = ?
        """;
        try {
            PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
            pstmt.setInt(1, idVotante);
            ResultSet rs = pstmt.executeQuery();
            while (rs.next()) {
                lista.add(new Votacion(
                    rs.getInt("id_votacion"),
                    rs.getString("titulo"),
                    rs.getString("descripcion"),
                    rs.getString("fecha_inicio"),
                    rs.getString("fecha_cierre")
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
    public boolean eliminarAsignaciones(int idVotacion) {
        String SQL = """
            DELETE FROM votacion_votante
            WHERE id_votacion = ?
        """;
        try {
            PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
            pstmt.setInt(1, idVotacion);
            pstmt.executeUpdate();
            ConexionBD.cerrar(pstmt);
            return true;
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return false;
    }
    
    
}