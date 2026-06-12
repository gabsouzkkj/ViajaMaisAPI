using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ViajaMaisAPI.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [JsonIgnore]
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}