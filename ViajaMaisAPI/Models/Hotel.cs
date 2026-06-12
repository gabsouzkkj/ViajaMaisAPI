using System.Text.Json.Serialization;

namespace ViajaMaisAPI.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int Classificacao { get; set; }
        public decimal PrecoDiaria { get; set; }
        public int DestinoId { get; set; }
        [JsonIgnore]
        public virtual Destino? Destino { get; set; }
    }
}
