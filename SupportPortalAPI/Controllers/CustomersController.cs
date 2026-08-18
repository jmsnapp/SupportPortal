using Microsoft.AspNetCore.Mvc;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class CustomersController : GenericController<CustomerEntity, Customer>
    {
        public CustomersController(IGenericRepository<CustomerEntity> repo) : base(repo)
        { }

        protected override Customer MapEntityToModel(CustomerEntity entity)
        {
            // Use existing Mapper method that wires in industry repository
            var model = DBMapper.MapCustomerEntity2Customer(entity);
            return model;

        }

        protected override void MapModelToEntity(Customer model, CustomerEntity entity) =>
            DBMapper.MapCustomer2CustomerEntity(model, ref entity);

    }

}
