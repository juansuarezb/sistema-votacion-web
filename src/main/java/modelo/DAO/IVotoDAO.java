package modelo.DAO;

import java.util.List;

import modelo.Entities.Voto;

public interface IVotoDAO {

    boolean create(Voto voto);

    List<Voto> getByVotacion(int idVotacion);

}