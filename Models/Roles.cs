namespace StreamHubApi.Models
{
    // Roles disponibles en el sistema. Se guardan como texto en Usuario.Rol
    // y viajan dentro del JWT como claim de tipo Role.
    public static class Roles
    {
        public const string Usuario = "Usuario";
        public const string Administrador = "Administrador";

        public static readonly string[] Todos = { Usuario, Administrador };
    }
}
