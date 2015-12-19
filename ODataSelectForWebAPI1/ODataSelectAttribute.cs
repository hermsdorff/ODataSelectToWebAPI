namespace ODataSelectForWebAPI1
{
    using System.Diagnostics;
    using System.Web.Http.Filters;

    public class ODataSelectAttribute : ActionFilterAttribute
    {                                          
        public const string MinimalistObject = "ODataSelect-MinimalistObject";
        public const string DefaultSelectProp = "ODataSelect-Default";

        public bool DefaultMinimalistObject { get; set; }
        public string DefaultSelect { get; set; }

        [DebuggerStepThrough]
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Request.Properties.Add(MinimalistObject, DefaultMinimalistObject);
            actionExecutedContext.Request.Properties.Add(DefaultSelectProp, DefaultSelect);

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}