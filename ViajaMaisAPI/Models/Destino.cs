using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ViajaMaisAPI.Models { 
    public class Destino
    {
        public int Id { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string EstadoOuPais { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Hotel> Hoteis { get; set; } = new List<Hotel>();

        [JsonIgnore]
        public virtual ICollection<PacoteViagem> PacotesViagem { get; set; } = new List<PacoteViagem>();
    }
}

