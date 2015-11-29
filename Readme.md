# OData Select To WebApi 1 #

[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/hermsdorff/ODataSelectToWebAPI)](https://ci.appveyor.com/project/hermsdorff/odataselecttowebapi)
[![NuGet version](https://badge.fury.io/nu/ODataSelectForWebAPI1.svg)](http://badge.fury.io/nu/ODataSelectForWebAPI1)

Add support to $select and $expand odata queries to Microsoft OData version running on WebApi 1.
With this it´s possible to use these queries if you need to use .Net Framework 4.0 using the ApiController

# Usage #

## Configuration ##
ODataSelectToWebAPI provide a custom DelegatingHandler to intercept Http requests.

Note: **This works only with IQueryable responses.**

To use it just add this one on the WebApiConfig as the example above

    public static class WebApiConfig
    {
      public static void Register(HttpConfiguration config)
      {
        //
        // your configurations here...
        //
        
        // Add support to $select and $expand OData queries on your ApiControllers
        config.MessageHandlers.Add(new ODataSelectHandler());
      }
    }

This configuration enable you to use $select and $expand OData expression with ApiController, for example:

	public class TestController : ApiController
	{
		[HttpGet]
		public IQueryable<TestEntity> GetEntities()
		{
			// your code to return the entity IQueryable collection here
		}
	}

with a controller like this you can make queries like
	
	http://myserver/mysystem/api/Test?$select=MyField1,MyCollection1/Property1&$expand=MyCollection1

## Attribute ##
ODataSelectToWebAPI allow you to change the default behavior of a controller with the Attribute `ODataSelect`

**This changes only the default behaviour (requests without $select or $expand query)**. In case of requests with $select or $expand fields, the response will be according the query in the querystring.

with `ODataSelect` attribute it's possible configure the action to return a minimalist version of the object as a default behaviour.

Minimalist version means only primitive type properties and primitive types of the it´s aggregated objects excluding all collections.

Example of usage:
	
	public class TestController : ApiController
	{
		[HttpGet]
		[ODataSelect(DefaultMinimalistObject = true)]
		public IQueryable<TestEntity> GetEntities()
		{
			// your code to return the entity IQueryable collection here
		}
	}

with this attribute a query with no OData $select or $expand queries will return a basic version of the `TestEntity` with only it´s primitive type properties and the primitive type of it´s aggregated objects.
