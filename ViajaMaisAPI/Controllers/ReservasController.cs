using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajaMaisAPI.Data;
using ViajaMaisAPI.Models;

namespace ViajaMaisAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly ViajaMaisContext _context;

    public ReservasController(ViajaMaisContext context)
    {
        _context = context;
    }

    // GET: api/Reservas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
    {
        return await _context.Reservas
            .Include(r => r.Cliente)
            .Include(r => r.PacoteViagem)
            .ToListAsync();
    }

    // GET: api/Reservas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Reserva>> GetReserva(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { erro = "Argumento inválido.", detalhe = "O ID da reserva deve ser maior que 0." });
        }

        var reserva = await _context.Reservas
            .Include(r => r.Cliente)
            .Include(r => r.PacoteViagem)
                .ThenInclude(p => p.Destino) 
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
        {
            return NotFound(new { mensagem = $"Reserva com o ID {id} não encontrada." });
        }

        return reserva;
    }

    // POST: api/Reservas
    [HttpPost]
    public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
    {
        if (reserva.QuantidadePassageiros <= 0)
        {
            return BadRequest(new
            {
                erro = "Quantidade de passageiros inválida.",
                detalhe = "A reserva deve ter pelo menos 1 passageiro."
            });
        }

        if (reserva.ClienteId <= 0)
        {
            return BadRequest(new
            {
                erro = "Argumento inválido para Cliente.",
                detalhe = "Você deve fornecer um 'clienteId' válido e maior que 0 para registrar a reserva."
            });
        }

        if (reserva.PacoteViagemId <= 0)
        {
            return BadRequest(new
            {
                erro = "Argumento inválido para Pacote de Viagem.",
                detalhe = "Você deve fornecer um 'pacoteViagemId' válido e maior que 0 para registrar a reserva."
            });
        }

        var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == reserva.ClienteId);
        if (!clienteExiste)
        {
            return BadRequest(new
            {
                erro = "Cliente não encontrado.",
                detalhe = $"Não encontramos nenhum cliente com o ID {reserva.ClienteId}. Certifique-se de inserir o ID de um cliente cadastrado."
            });
        }

        var pacote = await _context.PacotesViagem.FindAsync(reserva.PacoteViagemId);
        if (pacote == null)
        {
            return BadRequest(new
            {
                erro = "Pacote de viagem não encontrado.",
                detalhe = $"Não encontramos nenhum pacote de viagem com o ID {reserva.PacoteViagemId}. Verifique a lista de pacotes disponíveis."
            });
        }

        if (pacote.VagasDisponiveis < reserva.QuantidadePassageiros)
        {
            return BadRequest(new
            {
                erro = "Vagas insuficientes.",
                detalhe = $"O pacote selecionado possui apenas {pacote.VagasDisponiveis} vagas restantes, mas você tentou reservar {reserva.QuantidadePassageiros}."
            });
        }

        pacote.VagasDisponiveis -= reserva.QuantidadePassageiros;

        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetReserva),
            new { id = reserva.Id },
            new
            {
                mensagem = "Reserva registrada com sucesso!",
                dados = reserva
            }
        );
    }

    // DELETE: api/Reservas/5 (Cancelar Reserva)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReserva(int id)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva == null)
        {
            return NotFound(new { mensagem = $"Não foi possível cancelar: Reserva com o ID {id} não existe." });
        }

        var pacote = await _context.PacotesViagem.FindAsync(reserva.PacoteViagemId);
        if (pacote != null)
        {
            pacote.VagasDisponiveis += reserva.QuantidadePassageiros;
        }

        _context.Reservas.Remove(reserva);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = $"Reserva número {id} foi cancelada com sucesso e as vagas foram devolvidas ao pacote." });
    }
}