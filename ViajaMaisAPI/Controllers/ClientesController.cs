using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajaMaisAPI.Data;
using ViajaMaisAPI.Models;

namespace ViajaMaisAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ViajaMaisContext _context;

    public ClientesController(ViajaMaisContext context)
    {
        _context = context;
    }

    // GET: api/Clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
    {
        return await _context.Clientes.OrderBy(c => c.Id).ToListAsync();
    }

    // GET: api/Clientes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> GetCliente(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                erro = "Argumento inválido.",
                detalhe = "O ID do cliente deve ser maior que 0."
            });
        }

        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
        {
            return NotFound(new { mensagem = $"Cliente com o ID {id} não foi encontrado no sistema." });
        }

        return cliente;
    }

    // POST: api/Clientes
    [HttpPost]
    public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nome))
        {
            return BadRequest(new { erro = "Dados incompletos.", detalhe = "O campo 'nome' é obrigatório." });
        }

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCliente),
            new { id = cliente.Id },
            new
            {
                mensagem = "Cliente registrado com sucesso!",
                dados = cliente
            }
        );
    }

    // PUT: api/Clientes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCliente(int id, Cliente cliente)
    {
        if (id <= 0 || id != cliente.Id)
        {
            return BadRequest(new
            {
                erro = "Inconsistência nos argumentos.",
                detalhe = "O ID da URL deve ser idêntico ao ID enviado no corpo do objeto e maior que 0."
            });
        }

        _context.Entry(cliente).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Clientes.Any(c => c.Id == id))
            {
                return NotFound(new { mensagem = $"Não foi possível atualizar: Cliente com o ID {id} não existe." });
            }
            throw;
        }

        return Ok(new { mensagem = $"Dados do cliente '{cliente.Nome}' atualizados com sucesso!" });
    }

    // DELETE: api/Clientes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        // Tratamento: Evita buscar IDs inválidos
        if (id <= 0)
        {
            return BadRequest(new
            {
                erro = "Argumento inválido.",
                detalhe = "Informe um ID de cliente válido e maior que 0 para exclusão."
            });
        }

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound(new { mensagem = $"Não foi possível deletar: Cliente com o ID {id} não foi encontrado." });
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Cliente removido com sucesso!",
            detalhes = new { idDeletado = id, nome = cliente.Nome }
        });
    }
}