using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajaMaisAPI.Data;
using ViajaMaisAPI.Models;

namespace ViajaMaisAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacotesViagemController : ControllerBase
{
    private readonly ViajaMaisContext _context;

    public PacotesViagemController(ViajaMaisContext context)
    {
        _context = context;
    }

    // GET: api/PacotesViagem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PacoteViagem>>> GetPacotes()
    {
        return await _context.PacotesViagem
            .Include(p => p.Destino)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    // GET: api/PacotesViagem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PacoteViagem>> GetPacote(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { erro = "Argumento inválido.", detalhe = "O ID do pacote deve ser maior que 0." });
        }

        var pacote = await _context.PacotesViagem
            .Include(p => p.Destino)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pacote == null)
        {
            return NotFound(new { mensagem = $"Pacote de viagem com o ID {id} não foi encontrado." });
        }

        return pacote;
    }

    // POST: api/PacotesViagem
    [HttpPost]
    public async Task<ActionResult<PacoteViagem>> PostPacote(PacoteViagem pacote)
    {
        if (string.IsNullOrWhiteSpace(pacote.NomePacote))
        {
            return BadRequest(new { erro = "Dados incompletos.", detalhe = "O campo 'NomePacote' é obrigatório." });
        }

        if (pacote.Preco <= 0 || pacote.VagasDisponiveis <= 0)
        {
            return BadRequest(new { erro = "Valores inválidos.", detalhe = "O preço e as vagas disponíveis devem ser maiores que zero." });
        }

        if (pacote.DataSaida >= pacote.DataRetorno)
        {
            return BadRequest(new { erro = "Inconsistência de datas.", detalhe = "A data de saída do pacote deve ser anterior à data de retorno." });
        }

        var destinoExiste = await _context.Destinos.AnyAsync(d => d.Id == pacote.DestinoId);
        if (!destinoExiste)
        {
            return BadRequest(new { erro = "Destino não encontrado.", detalhe = $"Não existe destino cadastrado com o ID {pacote.DestinoId}." });
        }

        _context.PacotesViagem.Add(pacote);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPacotes),
            new { id = pacote.Id },
            new { mensagem = "Pacote de viagem criado com sucesso!", dados = pacote }
        );
    }

    // DELETE: api/PacotesViagem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePacote(int id)
    {
        var pacote = await _context.PacotesViagem.FindAsync(id);
        if (pacote == null)
        {
            return NotFound(new { mensagem = $"Pacote de viagem ID {id} não encontrado." });
        }

        var possuiReservas = await _context.Reservas.AnyAsync(r => r.PacoteViagemId == id);
        if (possuiReservas)
        {
            return BadRequest(new { erro = "Não é possível apagar.", detalhe = "Este pacote possui reservas ativas vinculadas a ele. Delete as reservas antes." });
        }

        _context.PacotesViagem.Remove(pacote);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Pacote de viagem removido com sucesso!", idDeletado = id });
    }
}