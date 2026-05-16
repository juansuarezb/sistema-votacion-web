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
				    INSERT INTO voto (id_votacion, id_tipo_voto)
				    VALUES (?, (SELECT id_tipo_voto FROM tipo_voto WHERE nombre = ?))
				""";
		try {
			PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
			pstmt.setInt(1, voto.getIdVotacion());
			pstmt.setString(2, voto.getOpcionSeleccionada().name());
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
				    SELECT v.id_voto, v.fecha, v.id_votacion, tv.nombre AS opcion
				    FROM voto v
				    JOIN tipo_voto tv ON v.id_tipo_voto = tv.id_tipo_voto
				    WHERE v.id_votacion = ?
				""";
		try {
			PreparedStatement pstmt = ConexionBD.getConexion().prepareStatement(SQL);
			pstmt.setInt(1, idVotacion);
			ResultSet rs = pstmt.executeQuery();
			while (rs.next()) {
				Voto voto = new Voto();
				voto.setIdVoto(rs.getInt("id_voto"));
				voto.setIdVotacion(rs.getInt("id_votacion"));
				voto.setOpcionSeleccionada(Voto.OpcionVoto.valueOf(rs.getString("opcion")));
				lista.add(voto);
			}
			ConexionBD.cerrar(rs);
			ConexionBD.cerrar(pstmt);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

}