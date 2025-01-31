﻿using DotNetCrudWebApi.Data;
using DotNetCrudWebApi.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCrudWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public MoviesController(AppDbContext AppDbContext)
        {
            _appDbContext = AppDbContext;
        }

        // Get api : api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieModel>>> GetMovies()
        {
            if (_appDbContext.Movies is null)
            {
                return NotFound("No Movies found.");
            }
            return await _appDbContext.Movies.ToListAsync();
        }

        // Get api : api/Movies/2
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieModel>> GetMovie(int id)
        {
            if (_appDbContext.Movies is null)
            {
                return NotFound("No Movies found.");
            }
            var movie = await _appDbContext.Movies.FindAsync(id);
            if (movie is null)
            {
                return NotFound($"Movie with id {id} not found.");
            }
            return movie;
        }

        // Post api : api/Movies
        [HttpPost]
        public async Task<ActionResult<MovieModel>> PostMovie(MovieModel movie)
        {
            _appDbContext.Movies.Add(movie);
            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // Put api : api/Movies/2
        [HttpPut]
        public async Task<ActionResult<MovieModel>> PutMovie(int id, MovieModel movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }
            _appDbContext.Entry(movie).State = EntityState.Modified;
            try
            {
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound($"Movie with id {id} not found.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // Check if movie with the id already exists in the database.
        private bool MovieExists(int id)
        {
            return (_appDbContext.Movies?.Any(movie => movie.Id == id)).GetValueOrDefault();
        }

        // Delete api : api/Movies/2
        [HttpDelete("{id}")]
        public async Task<ActionResult<MovieModel>> DeleteMovie(int id)
        {
            if (_appDbContext.Movies is null)
            {
                return NotFound("No Movies found.");
            }
            var movie = await _appDbContext.Movies.FindAsync(id);
            if (movie is null)
            {
                return NotFound($"Movie with id {id} not found.");
            }
            _appDbContext.Movies.Remove(movie);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
