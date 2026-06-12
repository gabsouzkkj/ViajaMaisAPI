using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajaMaisAPI.Data;
using ViajaMaisAPI.Models;

namespace ViajaMaisAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DestinosController : ControllerBase
{
    private readonly ViajaMaisContext _context;

    public DestinosController(ViajaMaisContext context)
    {
        _context = context;
    }

    // GET: api/Destinos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Destino>>> GetDestinos()
    {
        return await _context.Destinos.OrderBy(d => d.Cidade).ToListAsync();
    }

    // GET: api/Destinos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Destino>> GetDestino(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { erro = "Argumento inválido.", detalhe = "O ID do destino deve ser maior que 0." });
        }

        var destino = await _context.Destinos.FindAsync(id);
        if (destino == null)
        {
            return NotFound(new { mensagem = $"Destino com o ID {id} não foi encontrado." });
        }

        return destino;
    }

    // POST: api/Destinos
    [HttpPost]
    public async Task<ActionResult<Destino>> PostDestino(Destino destino)
    {
        if (string.IsNullOrWhiteSpace(destino.Cidade) || string.IsNullOrWhiteSpace(destino.EstadoOuPais))
        {
            return BadRequest(new { erro = "Dados incompletos.", detalhe = "Os campos 'Cidade' e 'EstadoOuPais' são obrigatórios." });
        }

        var destinoDuplicado = await _context.Destinos.AnyAsync(d =>
            d.Cidade.ToLower() == destino.Cidade.ToLower() &&
            d.EstadoOuPais.ToLower() == destino.EstadoOuPais.ToLower());

        if (destinoDuplicado)
        {
            return BadRequest(new { erro = "Destino já cadastrado.", detalhe = $"Já existe um destino salvo para '{destino.Cidade} - {destino.EstadoOuPais}'." });
        }

        _context.Destinos.Add(destino);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDestino), new { id = destino.Id }, new { mensagem = "Destino cadastrado com sucesso!", dados = destino });
    }

    // DELETE: api/Destinos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDestino(int id)
    {
        if (id <= 0) return BadRequest(new { erro = "ID inválido." });

        var destino = await _context.Destinos.FindAsync(id);
        if (destino == null)
        {
            return NotFound(new { mensagem = $"Destino ID {id} não encontrado." });
        }

        var possuiPacotes = await _context.PacotesViagem.AnyAsync(p => p.DestinoId == id);
        var possuiHoteis = await _context.Hoteis.AnyAsync(h => h.DestinoId == id);
        if (possuiPacotes || possuiHoteis)
        {
            return BadRequest(new { erro = "Não é possível deletar.", detalhe = "Existem hotéis ou pacotes de viagem vinculados a este destino. Remova-os primeiro." });
        }

        _context.Destinos.Remove(destino);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Destino removido com sucesso!", idDeletado = id });
    }
}