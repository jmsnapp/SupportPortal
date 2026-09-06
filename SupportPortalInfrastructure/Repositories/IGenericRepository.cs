using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure.Repositories
{
    public interface IGenericRepository<PortalObject, PortalObject2, PortalEntity>
    {
        public Task<List<PortalObject2>> GetAllAsync(CancellationToken ct);

        public Task<List<PortalObject2>> GetAllActiveAsync(CancellationToken ct);

        public Task<List<PortalObject2>> GetByParentIdAsync(Int64 parentId, CancellationToken ct);

        public Task<PortalObject> GetByIdAsync(Int64 id, CancellationToken ct);

        public Task<PortalObject> GetByNameAsync(string name, CancellationToken ct);

        public Task<Int64> CreateAsync(PortalEntity entity, CancellationToken ct);

        public Task<Int64> UpdateAsync(PortalEntity entity, CancellationToken ct);

    }

}
