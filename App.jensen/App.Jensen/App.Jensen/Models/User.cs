namespace App.Jensen.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!; //No aceptar nulos, es obligatorio.

        public string? URLProfilPic { get; set; } //Creo una propiedad para subir el url de foto perfil

        public string Email { get; set; } = null!; //Propiedad para guardar los correos

        public string Password { get; set; } = null!; //Propiedad para guardar las contrasenas


    }
}
