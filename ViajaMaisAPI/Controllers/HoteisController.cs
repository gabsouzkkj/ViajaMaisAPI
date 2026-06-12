using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajaMaisAPI.Data;
using ViajaMaisAPI.Models;

namespace ViajaMaisAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HoteisController : ControllerBase
{
    private readonly ViajaMaisContext _context;

    public HoteisController(ViajaMaisContext context)
    {
        _context = context;
    }

    // GET: api/Hoteis
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetHoteis()
    {
        return await _context.Hoteis.Include(h => h.Destino).ToListAsync();
    }

    // GET: api/Hoteis/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetHotel(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { erro = "Argumento inválido.", detalhe = "O ID do hotel deve ser maior que 0." });
        }

        var hotel = await _context.Hoteis
            .Include(h => h.Destino)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
        {
            return NotFound(new { mensagem = $"Hotel com o ID {id} não foi encontrado." });
        }

        return hotel;
    }

    // POST: api/Hoteis
    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
    {
        if (string.IsNullOrWhiteSpace(hotel.Nome) || hotel.DestinoId <= 0)
        {
            return BadRequest(new { erro = "Dados inválidos.", detalhe = "Os campos 'Nome' e 'DestinoId' são obrigatórios." });
        }

        if (hotel.PrecoDiaria <= 0)
        {
            return BadRequest(new { erro = "Preço inválido.", detalhe = "O preço da diária deve ser maior que zero." });
        }

        // Validação: Garante que o DestinoId passado realmente existe
        var destinoExiste = await _context.Destinos.AnyAsync(d => d.Id == hotel.DestinoId);
        if (!destinoExiste)
        {
            return BadRequest(new { erro = "Destino não encontrado.", detalhe = $"Não é possível cadastrar o hotel porque o DestinoId {hotel.DestinoId} não existe no banco." });
        }

        _context.Hoteis.Add(hotel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHoteis), new { id = hotel.Id }, new { mensagem = "Hotel cadastrado com sucesso!", dados = hotel });
    }

    // DELETE: api/Hoteis/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await _context.Hoteis.FindAsync(id);
        if (hotel == null)
        {
            return NotFound(new { mensagem = $"Hotel ID {id} não encontrado." });
        }

        _context.Hoteis.Remove(hotel);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Hotel removido com sucesso!", idDeletado = id });
    }
}