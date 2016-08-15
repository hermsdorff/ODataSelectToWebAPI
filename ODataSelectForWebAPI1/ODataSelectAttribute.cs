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

        //[DebuggerStepThrough]
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Properties.ContainsKey(MinimalistObject))
                actionExecutedContext.Request.Properties.Remove(MinimalistObject);

            actionExecutedContext.Request.Properties.Add(MinimalistObject, DefaultMinimalistObject);

            if (actionExecutedContext.Request.Properties.ContainsKey(DefaultSelectProp))
                actionExecutedContext.Request.Properties.Remove(DefaultSelectProp);

            actionExecutedContext.Request.Properties.Add(DefaultSelectProp, DefaultSelect);

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}