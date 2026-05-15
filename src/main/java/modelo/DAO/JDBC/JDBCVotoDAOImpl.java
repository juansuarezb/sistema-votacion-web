package modelo.DAO.JDBC;

import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import Conexion.ConexionBD;
import modelo.DAO.IVotoDAO;
import modelo.Entities.Voto;

public class JDBCVotoDAOImpl implements IVotoDAO {

    @Override
    public boolean create(Voto voto) {

        String SQL = """
            INSERT INTO voto
            (id_votacion, id_votante, opcion)
            VALUES (?, ?, ?)
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            pstmt.setInt(1, voto.getIdVotacion());
            pstmt.setInt(2, voto.getIdVotante());
            pstmt.setString(
                    3,
                    voto.getOpcionSeleccionada().name()
            );

            return pstmt.executeUpdate() > 0;

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    @Override
    public List<Voto> getByVotacion(int idVotacion) {

        List<Voto> lista = new ArrayList<>();

        String SQL = """
            SELECT *
            FROM voto
            WHERE id_votacion = ?
        """;

        try {

            PreparedStatement pstmt =
                    ConexionBD.getConexion()
                            .prepareStatement(SQL);

            pstmt.setInt(1, idVotacion);

            ResultSet rs = pstmt.executeQuery();

            while (rs.next()) {

                Voto voto = new Voto();

                voto.setIdVoto(
                        rs.getInt("id_voto")
                );

                voto.setIdVotacion(
                        rs.getInt("id_votacion")
                );

                voto.setIdVotante(
                        rs.getInt("id_votante")
                );

                voto.setOpcionSeleccionada(
                        Voto.OpcionVoto.valueOf(
                                rs.getString("opcion")
                        )
                );

                lista.add(voto);
            }

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return lista;
    }

}