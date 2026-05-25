package modelo.DAO;

import java.util.List;

import modelo.Entities.Votacion;
import modelo.Entities.Votante;

public interface IVotacionDAO {

    List<Votacion> listar();
    List<Votacion> getVotacionesByVotante(int idVotante);
    Votacion getById(int idVotacion);
	boolean asignarVotante(int idVotacion, int idVotante);
    boolean create(Votacion v);
    List<Votante> getVotantesAsignados(int idVotacion);
    boolean update(Votacion v);

    boolean delete(int idVotacion);
    boolean marcarVotoEmitido(int idVotacion, int idVotante);
    boolean eliminarAsignaciones(int idVotacion);
}