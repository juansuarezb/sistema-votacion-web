package modelo.DAO;

import java.util.List;

import modelo.Entities.Votante;

public interface IVotanteDAO {

    List<Votante> listar();

    Votante getById(int idUsuario);

    boolean create(Votante v);

    boolean update(Votante v);

    boolean delete(int idUsuario);
}