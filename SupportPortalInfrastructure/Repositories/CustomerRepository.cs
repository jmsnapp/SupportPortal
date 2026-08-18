using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<CustomerEntity>
    {
        public CustomerRepository(SupportPortalDBContext context) : base(context) { }

        protected override IQueryable<CustomerEntity> WithDetail() =>
            _dbSet.Include(c => c.Industry);

        protected override IQueryable<CustomerEntity> WithSummary() =>
            _dbSet.Include(c => c.Industry);

    }

}
