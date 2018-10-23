namespace TOne.WhS.RouteSync.Entities
{
    public abstract class ConvertedRoute
    {
    }

    public abstract class ConvertedRouteWithCode : ConvertedRoute
    {
        public string Code { get; set; }

        public abstract string GetCustomer();

        public abstract string GetRouteOptionsIdentifier();
    }

    public interface ICreateConvertedRouteWithCodeContext
    {
        string Customer { get; }

        string Code { get; }

        string RouteOptionIdentifier { get; }
    }

    public class CreateConvertedRouteWithCodeContext : ICreateConvertedRouteWithCodeContext
    {
        public string Customer { get; set; }

        public string Code { get; set; }

        public string RouteOptionIdentifier { get; set; }
    }
}
