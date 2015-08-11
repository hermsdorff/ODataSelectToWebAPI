# OData Select To WebApi 1 #
Add support to $select and $expand odata queries to Microsoft OData version running on WebApi 1.
With this it´s possible to use these queries if you need to use .Net Framework 4.0 using the ApiController

# Usage #

ODataSelectToWebAPI provide a custom DelegatingHandler to intercept Http requests.

Note: **This works only with IQueryable responses.**

To use it just add this one on the WebApiConfig as the example above

    public static class WebApiConfig
    {
      public static void Register(HttpConfiguration config)
      {
        //
        // your configurations above...
        //
        
        // Add support to $select and $expand OData queries on your ApiControllers
        config.MessageHandlers.Add(new ODataSelectHandler());
      }
    }
