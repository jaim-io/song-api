using Microsoft.AspNetCore.Mvc;
using Song.API.Models;
using Song.API.Services;

namespace Song.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Produces("application/json")]
  public class SongController : ControllerBase
  {
    private readonly ISongService _service;

    public SongController(ISongService service)
     => _service = service;

    /// <summary>
    /// Gets all Songs.
    /// </summary>
    /// <returns>All Songs</returns>
    /// <response code="200">Returns all songs</response>
    /// <response code="404">If no songs exists</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSongs()
    {
      ServiceResponse<IEnumerable<Models.Song>> response = await _service.GetSongs();

      if (response.Result == Result.NotFound)
        return NotFound();

      return Ok(response.Data);
    }

    /// <summary>
    /// Gets a Song by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A Song matching the given ID</returns>
    /// <response code="200">Returns a song matching the given ID</response>
    /// <response code="404">If no song matching the given ID exists</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSong(int id)
    {
      ServiceResponse<Models.Song> response = await _service.GetSong(id);

      if (response.Result == Result.NotFound)
        return NotFound();

      return Ok(response.Data);
    }

    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    /// <summary>
    /// Modifies an existing Song entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="song"></param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /Song/1
    ///     {
    ///         "id": 1,
    ///         "name": "Unstoppable",
    ///         "artist": "For The Fallen Dreams",
    ///         "imageUrl": "https://picsum.photos/200/300"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">If the modification was succesfull</response>
    /// <response code="400">If URL parameter 'id' and Song.Id do not match</response>
    /// <response code="404">If no song matching the given ID exists</response>
    /// <response code="500">If something went wrong while modifying the Song Entity</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutSong(int id, Models.Song song)
    {
      ServiceResponse response = await _service.UpdateSong(id, song);
      switch (response.Result)
      {
        case Result.BadRequest:
          return BadRequest();

        case Result.NotFound:
          return NotFound();

        case Result.Ok:
          return NoContent();

        default:
          return StatusCode(500);
      }
    }

    /// <summary>
    /// Creates a new Song.
    /// </summary>
    /// <param name="song"></param>
    /// <returns>A newly created Song</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /Song
    ///     {
    ///         "name": "sTraNgeRs",
    ///         "artist": "Bring Me The Horizon",
    ///         "imageUrl": "https://picsum.photos/200/300"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created song</response>
    /// <response code="400">If the song is null</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostSong(Models.Song song)
    {
      ServiceResponse<Models.Song> response = await _service.AddSong(song);

      if (response.Result == Result.Err)
        return Problem("Entity set 'SongContext.Songs' is null.");

      return CreatedAtAction("GetSong", new { id = response.Data?.Id }, response.Data);
    }

    /// <summary>
    /// Deletes a specific Song.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="204">If the deletion was succesfull</response>
    /// <response code="404">If no song matching the given ID exists</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSong(int id)
    {
      ServiceResponse response = await _service.DeleteSong(id);
      if (response.Result == Result.NotFound)
        return NotFound();

      return NoContent();
    }
  }
}
