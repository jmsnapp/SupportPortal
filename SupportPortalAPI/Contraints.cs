namespace SupportPortalAPI
{
    public class UIntRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string parameterName,
        RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(parameterName, out var value) && value != null)
            {
                return uint.TryParse(value.ToString(), out _);

            }

            return false;

        }

    }

}
