using MarketingContactManager.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MarketingContactManager.SchemaFilters
{
    public class ContactModelFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(ContactModel))
            {
                schema.Example = new OpenApiObject()
                {
                    ["FirstName"] = new OpenApiString("John"),
                    ["LastName"] = new OpenApiString("Doe"),
                    ["Email"] = new OpenApiString("john.doe@example.com"),
                    ["PhoneNumber"] = new OpenApiString("1234567890")
                };
            }
        }
    }
}
