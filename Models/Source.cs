namespace GCRAS.Models
{
    public class Source
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public int CreadoPorUsuarioID { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaMod { get; set; }
    }
}