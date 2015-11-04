namespace ODataSelectForWebAPI1
{
    using System.Web.Http.Filters;

    public class ODataSelectAttribute : ActionFilterAttribute
    {
        public const string MinimalistObject = "ODataSelect-MinimalistObject";

        public bool DefaultMinimalistObject { get; set; }
        
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Request.Properties.Add("ODataSelect-MinimalistObject", DefaultMinimalistObject);
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}