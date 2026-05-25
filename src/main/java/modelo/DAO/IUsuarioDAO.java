package modelo.DAO;
import modelo.Entities.Usuario;

public interface IUsuarioDAO {
	Usuario authenticate(String correoElectronico, String contrasena);
}
