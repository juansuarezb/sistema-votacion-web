package Conexion;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

public class ConexionBD {

	private static Connection cnn = null;
	private ConexionBD() {
		String servidor = System.getenv("DB_HOST");
		String puerto = System.getenv("DB_PORT");
		String database = System.getenv("DB_NAME");
		String usuario = System.getenv("DB_USER");
		String contraseña = System.getenv("DB_PASSWORD");
		if (servidor == null || puerto == null ||
			    database == null || usuario == null || contraseña == null) {

			    throw new RuntimeException("Faltan variables de entorno para la BD");
			}
		String url = "jdbc:mysql://" + servidor + ":" + puerto + "/" + database
		        + "?useSSL=false"
		        + "&allowPublicKeyRetrieval=true"
		        + "&serverTimezone=UTC";
		try {
			DriverManager.registerDriver(new com.mysql.cj.jdbc.Driver());
			cnn = DriverManager.getConnection(url, usuario, contraseña);
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	public static Connection getConexion() {
		if (cnn== null) {
			new ConexionBD();
		}
		return cnn;
	}

	public static void cerrar(ResultSet rs) {
		try {
			rs.close();
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public static void cerrar(PreparedStatement pstmt) {
		try {
			pstmt.close();
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	public static void cerrar() {
		if(cnn != null) {
			try {
				cnn.close();
				cnn = null;
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
	}
}
