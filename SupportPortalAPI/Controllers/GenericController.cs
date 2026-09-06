using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Configuration;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class GenericController<TModel, TModel2, TEntity> : ControllerBase
        where TEntity : PortalEntity, new()
        where TModel2 : PortalObject, new()
        where TModel : PortalObject, new()
    {
        protected readonly IGenericRepository<TModel, TModel2, TEntity> _repo;

        protected const int DEFAULT_PAGE_SIZE = 50;

        private int _maxPageSize = 200;

        private readonly PaginationOptions _options;

        protected GenericController(IGenericRepository<TModel, TModel2, TEntity> repo, Microsoft.Extensions.Options.IOptions<PaginationOptions>? options = null)
        {
            _repo = repo;
            // Allow callers (tests) to omit options. Provide a sane default when missing.
            _options = (options ?? Microsoft.Extensions.Options.Options.Create(new PaginationOptions())).Value;

        }

        // GET api/[controller]/{id:long}
        [HttpGet("{id:long}")]
        public virtual async Task<IActionResult> GetById(Int64 id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);

        }

        // GET api/[controller]/getall?page=1&pageSize=50
        [HttpGet("getall")]
        public virtual async Task<ActionResult<PagedResult<TModel2>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = DEFAULT_PAGE_SIZE, CancellationToken ct = default)
        {
            if (pageSize == DEFAULT_PAGE_SIZE) pageSize = _options.DefaultPageSize;

            List<TModel2> lstResult = await _repo.GetAllAsync(ct);

            ActionResult<PagedResult<TModel2>> lstPageResult = Collection(lstResult, page, pageSize, ct);

            return lstPageResult;

        }

        // GET api/[controller]/active?page=1&pageSize=50
        [HttpGet("active")]
        public virtual async Task<ActionResult<PagedResult<TModel2>>> GetAllActive([FromQuery] int page = 1, [FromQuery] int pageSize = DEFAULT_PAGE_SIZE, CancellationToken ct = default)
        {
            if (pageSize == DEFAULT_PAGE_SIZE) pageSize = _options.DefaultPageSize;

            List<TModel2> lstResult = await _repo.GetAllActiveAsync(ct);

            ActionResult<PagedResult<TModel2>> lstPageResult = Collection(lstResult, page, pageSize, ct);

            return lstPageResult;

        }

        // PUT api/[controller]/{id:long}
        //
        // Responds with the saved model rather than 204. The body carries the RowVersion the
        // row now holds, so a caller can save again straight away; with 204 it would have to
        // re-read first, and a caller that did not would replay the token it just superseded
        // and be rejected as a stale write.
        [HttpPut("{id:long}")]
        public virtual async Task<ActionResult<TModel>> Update(Int64 id, [FromBody] TModel? updated, CancellationToken ct = default)
        {
            if (updated == null || id != updated.Id) return BadRequest();

            TModel? existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            TEntity entityUpdate = new TEntity();

            MapModelToEntity(updated, entityUpdate);

            await _repo.UpdateAsync(entityUpdate, ct);

            // Re-read through the repository: the write reissues RowVersion, and it is that
            // new token the caller needs in hand to make a second save without re-reading.
            TModel? saved = await _repo.GetByIdAsync(id, ct);
            if (saved == null) return NotFound();

            return saved;

        }

        // POST api/[controller]
        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TModel? create, CancellationToken ct = default)
        {
            if (create == null) return BadRequest();

            create.Id = -1;

            TEntity entity = ConvertEntityFromObject(create);

            Int64 intReturn = await _repo.CreateAsync(entity, ct);

            // Re-read through the repository so WithDetail() populates the navigations
            // the response mapper needs.
            TModel? saved = await _repo.GetByIdAsync(intReturn, ct);
            if (saved == null) return NotFound();

            return CreatedAtAction(nameof(GetById), new { id = saved.Id }, saved);

        }

        // DELETE api/[controller]/{id:long}
        //
        // Optionally guarded by an If-Match precondition carrying the RowVersion the caller
        // believes the row is at. Delete and Restore have no body, so the header is the only
        // place that expectation can travel. Omit it and the write is unguarded, matching
        // Update's behaviour when a model arrives without a token.
        [HttpDelete("{id:long}")]
        public virtual async Task<IActionResult> Delete(Int64 id, CancellationToken ct = default)
        {
            TModel? existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            IActionResult? malformed = ApplyIfMatch(existing);
            if (malformed is not null) return malformed;

            existing.Deleted = true;

            TEntity existingEntity = new TEntity();

            MapModelToEntity(existing, existingEntity);

            await _repo.UpdateAsync(existingEntity, ct);

            return NoContent();

        }

        // Restore api/[controller]/restore/{id:long}
        //
        // Same optional If-Match precondition as Delete.
        [HttpPut("restore/{id:long}")]
        public virtual async Task<IActionResult> Restore(Int64 id, CancellationToken ct = default)
        {
            TModel? existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            IActionResult? malformed = ApplyIfMatch(existing);
            if (malformed is not null) return malformed;

            existing.Deleted = false;

            TEntity existingEntity = new TEntity();

            MapModelToEntity(existing, existingEntity);

            await _repo.UpdateAsync(existingEntity, ct);

            return NoContent();

        }

        /// <summary>
        /// Applies the optional If-Match precondition to an already-loaded entity.
        /// <para>
        /// Stamps the caller's expected RowVersion onto the entity; the update procedure then carries
        /// it in the UPDATE's WHERE clause, so a superseded token matches no row and is refused by
        /// the database instead of quietly overwriting whoever got there first. Returns null when the
        /// request may proceed, or a 400 when the header is present but unreadable.
        /// </para>
        /// </summary>
        private IActionResult? ApplyIfMatch(TModel existing)
        {
            string raw = Request.Headers.IfMatch.ToString();

            // Absent means no precondition; "*" means "whatever version exists", and the row is
            // already loaded, so that is satisfied. Both clear the token the read brought back:
            // the entity is on its way into the update procedure, which guards on whatever token
            // it is handed, and an unasked-for precondition would turn an opt-in feature into a
            // mandatory one.

            if (string.IsNullOrWhiteSpace(raw) || raw.Trim() == "*")
            {
                existing.RowVersion = Array.Empty<byte>();
                return null;
            }

            string tag = raw.Trim();
            if (tag.StartsWith("W/", StringComparison.Ordinal)) tag = tag.Substring(2);
            tag = tag.Trim('"');

            try
            {
                existing.RowVersion = Convert.FromBase64String(tag);
            }
            catch (FormatException)
            {
                return BadRequest("If-Match must be the quoted base64 RowVersion taken from a prior read.");
            }

            return null;

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

        public ActionResult<PagedResult<TModel2>> Collection(List<TModel2> lstItems, int page, int pageSize, CancellationToken ct)
        {
            _maxPageSize = _options.MaxPageSize;
            page = Math.Max(page, 1);
            // Ensure pageSize is at least 1
            pageSize = Math.Max(pageSize, 1);

            List<TModel2> items = lstItems
                                .OrderBy(x => x.Id)
                                .Skip(Math.Max((page - 1) * pageSize, 0))
                                .Take(Math.Clamp(pageSize, 1, _maxPageSize))
                                .ToList();

            return new PagedResult<TModel2>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = lstItems.Count,
            };

        }

    }

}
