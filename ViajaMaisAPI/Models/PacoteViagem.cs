using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ViajaMaisAPI.Models
{
    public class PacoteViagem
    {
        public int Id { get; set; }
        public string NomePacote { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int VagasDisponiveis { get; set; }
        public DateTime DataSaida { get; set; }
        public DateTime DataRetorno { get; set; }

        public int DestinoId { get; set; }

        [JsonIgnore]
        public virtual Destino? Destino { get; set; }

        [JsonIgnore]
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
