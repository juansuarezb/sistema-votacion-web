package modelo.Entities;

public class Auditor extends Usuario {
    private static final long serialVersionUID = 1L;

   // Constructor para lógica de negocio
    public Auditor(int idUsuario, String nombre, String correoElectronico, String contraseña) {
        super(idUsuario, nombre, correoElectronico, contraseña);
    }

    // Constructor vacío
    public Auditor() {
        super();
    }



    public void generarReporte(){

    }
    public void verLog() {

    }


}