using System;
using System.Text.Json.Serialization;

namespace ViajaMaisAPI.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public DateTime DataCompra { get; set; } = DateTime.Now;
        public int QuantidadePassageiros { get; set; }
        public string StatusPagamento { get; set; } = "Pendente";
        public int ClienteId { get; set; }
        public int PacoteViagemId { get; set; }

        [JsonIgnore]
        public virtual Cliente? Cliente { get; set; }

        [JsonIgnore]
        public virtual PacoteViagem? PacoteViagem { get; set; }
    }
}