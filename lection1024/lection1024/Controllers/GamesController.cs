using lection1024.Contexts;
using lection1024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lection1024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController(GameDbContext context) : ControllerBase
    {
        private readonly GameDbContext _context = context;

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            return await _context.Games.ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return game;
        }

        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGamesByCategories(string name)
        {
            var category = await _context.Categories
                .Include(c => c.Games)
                .FirstOrDefaultAsync(c => c.Name == name);

            return category is null ? NotFound() : category.Games.ToList();
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGamesByCategory(string categories)
        {
            var values = categories.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            return await _context.Games
                .Include(g => g.Category)
                .Where(g => values.Contains(g.Category.Name))
                .ToListAsync();
        }

        [HttpGet("price")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGamesByPrice(string price)
        {
            var values = price.Split('-');
            if (values.Length != 2)
                return BadRequest();

            int minPrice, maxPrice;
            if (!int.TryParse(values[0], out minPrice) ||
                !int.TryParse(values[1], out maxPrice))
                return BadRequest();

            return await _context.Games
                .Where(g => g.Price >= minPrice && g.Price <= maxPrice)
                .ToListAsync();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGamesByFilters(string filters)
        {
            var values = filters.Split(',');
            Dictionary<string,string> pairs = new Dictionary<string, string>();
            foreach (var v in values)
            {
                var pair = v.Split(':');
                pairs[pair[0]] = pair[1];
            }

            
            return await _context.Games
                .ToListAsync();
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, Game game)
        {
            if (id != game.GameId)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.GameId }, game);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.GameId == id);
        }
    }
}
