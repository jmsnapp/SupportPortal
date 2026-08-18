using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class GenericController<TEntity, TModel> : ControllerBase
        where TEntity : PortalEntity, new()
        where TModel : PortalObject, new()
    {
        protected readonly IGenericRepository<TEntity> _repo;

        protected const int DefaultPageSize = 50;

        protected const int MaxPageSize = 200;

        protected GenericController(IGenericRepository<TEntity> repo)
        {
            _repo = repo;

        }

        // GET api/[controller]?page=1&pageSize=50&includeDeleted=false
        [HttpGet]
        public virtual Task<ActionResult<PagedResult<TModel>>> GetPage([FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
            => Collection(page, pageSize, includeDeleted, ct);

        // GET api/[controller]/{id:long}
        [HttpGet("{id:long}")]
        public virtual async Task<IActionResult> GetById(Int64 id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            var model = MapEntityToModel(entity);
            return Ok(model);

        }

        // GET api/[controller]/by-name/{name}
        [HttpGet("by-name/{name}")]
        public virtual async Task<IActionResult> GetByName(string name, CancellationToken ct = default)
        {
            var entity = await _repo.GetByNameAsync(name, ct);
            if (entity == null) return NotFound();

            var model = MapEntityToModel(entity);
            return Ok(model);

        }

        // GET api/[controller]/getall?page=1&pageSize=50
        [HttpGet("getall")]
        public virtual Task<ActionResult<PagedResult<TModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize, CancellationToken ct = default)
            => Collection(page, pageSize, includeDeleted: true, ct);

        // GET api/[controller]/active?page=1&pageSize=50
        [HttpGet("active")]
        public virtual Task<ActionResult<PagedResult<TModel>>> GetAllActive([FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize, CancellationToken ct = default)
            => Collection(page, pageSize, includeDeleted: false, ct);

        // PUT api/[controller]/{id:long}
        [HttpPut("{id:long}")]
        public virtual async Task<IActionResult> Update(Int64 id, [FromBody] TModel updated, CancellationToken ct = default)
        {
            if (updated == null || id != updated.Id) return BadRequest();

            TEntity? existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            ConvertEntityFromObject(updated, existing);

            _repo.Update(existing);
            await _repo.SaveChangesAsync(ct);
            return NoContent();

        }

        // POST api/[controller]
        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TModel create, CancellationToken ct = default)
        {
            if (create == null) return BadRequest();

            create.Id = -1;

            TEntity entity = ConvertEntityFromObject(create);

            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);

            // Re-read through the repository so WithDetail() populates the navigations
            // the response mapper needs.
            TEntity saved = await _repo.GetByIdAsync(entity.Id, ct) ?? entity;

            var model = MapEntityToModel(saved);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, model);

        }

        // DELETE api/[controller]/{id:long}
        [HttpDelete("{id:long}")]
        public virtual async Task<IActionResult> Delete(Int64 id, CancellationToken ct = default)
        {
            TEntity? existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            existing.Deleted = true;

            _repo.Update(existing);
            await _repo.SaveChangesAsync(ct);
            return NoContent();

        }

        // Restore api/[controller]/restore/{id:long}
        [HttpPut("restore/{id:long}")]
        public virtual async Task<IActionResult> Restore(Int64 id, CancellationToken ct = default)
        {
            TEntity? existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            existing.Deleted = false;

            _repo.Update(existing);
            await _repo.SaveChangesAsync(ct);
            return NoContent();

        }

        protected virtual TModel MapEntityToModel(TEntity entity)
        {
            // Default shallow mapping -> map PortalEntity fields into TModel
            var model = new TModel();
            DBMapper.MapPortalEntity2Object(entity, model);
            return model;

        }

        protected virtual IEnumerable<TModel> MapEntitiesToModels(IEnumerable<TEntity> entities)
        {
            var results = entities.Select(e => MapEntityToModel(e));
            return results;

        }

        // GenericController — default shallow mapping
        protected virtual void MapModelToEntity(TModel model, TEntity entity) =>
            DBMapper.MapPortalObject2Entity(model, entity);

        private TEntity ConvertEntityFromObject(TModel model, TEntity? target = null)
        {
            TEntity entity = target ?? new TEntity();
            MapModelToEntity(model, entity);
            return entity;

        }

        private async Task<ActionResult<PagedResult<TModel>>> Collection(int page, int pageSize, bool includeDeleted, CancellationToken ct)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

            var (entities, total) = await _repo.GetPageAsync((page - 1) * pageSize, pageSize, includeDeleted, ct);

            return new PagedResult<TModel>
            {
                Items = MapEntitiesToModels(entities).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
            };

        }

    }

}
