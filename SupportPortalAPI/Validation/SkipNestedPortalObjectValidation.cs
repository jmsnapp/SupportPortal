using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using SupportPortalDomain.Models;

namespace SupportPortalAPI.Validation
{
    /// <summary>
    /// Stops model validation from recursing into nested PortalObject references.
    /// <para>
    /// A nested reference on a domain model (Ticket.Customer, Customer.Industry, ...) is a
    /// foreign key wearing an object costume — DBMapper reads nothing from it but Id. The
    /// constructors populate those properties with empty-but-non-null instances, so MVC's
    /// default recursion validates them as if they were being created, and a caller ends up
    /// having to invent a Name, a Description and a contact email for a Customer it is only
    /// pointing at. Suppressing child validation lets a caller post {"customer":{"id":3}}.
    /// </para>
    /// <para>
    /// The top-level model is unaffected: its own Name, Description, lengths and email formats
    /// are still validated normally. Referential validity is the FK's job, not the validator's.
    /// </para>
    /// </summary>
    public sealed class SkipNestedPortalObjectValidation : IValidationMetadataProvider
    {
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context.Key.MetadataKind == ModelMetadataKind.Property &&
                typeof(PortalObject).IsAssignableFrom(context.Key.ModelType))
            {
                context.ValidationMetadata.ValidateChildren = false;
            }
        }
    }
}
