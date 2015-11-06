namespace ODataSelectForWebAPI1
{
    using System.Diagnostics;
    using System.Web.Http.Filters;

    public class ODataSelectAttribute : ActionFilterAttribute
    {
        public const string MinimalistObject = "ODataSelect-MinimalistObject";

        public bool DefaultMinimalistObject { get; set; }

        [DebuggerStepThrough]
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Request.Properties.Add("ODataSelect-MinimalistObject", DefaultMinimalistObject);
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}