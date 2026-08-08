using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class GenericController<TEntity, TModel> : ControllerBase
        where TEntity : PortalEntity
        where TModel : PortalObject, new()
    {
        protected readonly IGenericRepository<TEntity> _repo;
        protected readonly DBMapper _mapper;

        protected GenericController(IGenericRepository<TEntity> repo, DBMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;

        }

        // GET api/[controller]/{id:long}
        [HttpGet("{id:long}")]
        public virtual async Task<IActionResult> GetById(Int64 id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            var model = await MapEntityToModelAsync(entity);
            return Ok(model);

        }

        // GET api/[controller]/byname/{name}
        [HttpGet("byname/{name}")]
        public virtual async Task<IActionResult> GetByName(string name, CancellationToken ct = default)
        {
            var entity = await _repo.GetByNameAsync(name, ct);
            if (entity == null) return NotFound();

            var model = await MapEntityToModelAsync(entity);
            return Ok(model);

        }

        // GET api/[controller]/getall
        [HttpGet("getall")]
        [HttpGet]
        public virtual async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var entities = await _repo.GetAllAsync(ct);
            var models = await MapEntitiesToModelsAsync(entities, ct);
            return Ok(models);

        }

        // GET api/[controller]/active
        [HttpGet("active")]
        public virtual async Task<IActionResult> GetAllActive(CancellationToken ct = default)
        {
            var entities = await _repo.GetAllActiveAsync(ct);
            var models = await MapEntitiesToModelsAsync(entities, ct);
            return Ok(models);

        }

        // PUT api/[controller]/{id:long}
        [HttpPut("{id:long}")]
        public virtual async Task<IActionResult> Update(Int64 id, [FromBody] TEntity updated, CancellationToken ct = default)
        {
            if (updated == null || id != updated.Id) return BadRequest();
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            _repo.Update(updated);
            await _repo.SaveChangesAsync(ct);
            return NoContent();

        }

        // POST api/[controller]
        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TEntity create, CancellationToken ct = default)
        {
            if (create == null) return BadRequest();

            await _repo.AddAsync(create, ct);
            await _repo.SaveChangesAsync(ct);

            var model = await MapEntityToModelAsync(create);
            return CreatedAtAction(nameof(GetById), new { id = create.Id }, model);

        }

        protected virtual Task<TModel> MapEntityToModelAsync(TEntity entity)
        {
            // Default shallow mapping -> map PortalEntity fields into TModel
            var model = new TModel();
            DBMapper.MapPortalEntity2Object(entity, model);
            return Task.FromResult(model);

        }

        protected virtual async Task<IEnumerable<TModel>> MapEntitiesToModelsAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            var tasks = entities.Select(e => MapEntityToModelAsync(e));
            var results = await Task.WhenAll(tasks);
            return results;

        }

        protected IActionResult OkOrNotFound(object? o) => o == null ? NotFound() : Ok(o);

    }

}
