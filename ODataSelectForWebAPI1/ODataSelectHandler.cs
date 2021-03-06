﻿using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ODataSelectForWebAPI1
{
    using System;
    using System.Diagnostics;
    using System.Web;

    public class ODataSelectHandler : DelegatingHandler 
    {
        [DebuggerStepThrough]
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
                t =>
                {
                    var response = t.Result;

                    bool minimalist = 
                        response.RequestMessage != null && response.RequestMessage.Properties != null
                        && response.RequestMessage.Properties.ContainsKey(ODataSelectAttribute.MinimalistObject) 
                        && (bool)response.RequestMessage.Properties[ODataSelectAttribute.MinimalistObject];

                    string defaultSelect = response.RequestMessage != null && response.RequestMessage.Properties != null
                        && response.RequestMessage.Properties.ContainsKey(ODataSelectAttribute.DefaultSelectProp)
                        ? (string)response.RequestMessage.Properties[ODataSelectAttribute.DefaultSelectProp] : String.Empty;

                    if(!minimalist && !HasSelectOrExpand(request) && String.IsNullOrEmpty(defaultSelect)) return response;

                    if(!ValidResponse(response)) return response;

                    var lastResult = GetValueFromObjectContent(response.Content);
                    if (!(lastResult is IQueryable)) return response;

                    var result = (lastResult as IQueryable<object>);
                    var parser = new ODataParser();
                    var query = HttpUtility.UrlDecode(request.RequestUri.Query);
                    var tree = parser.Parse(HasSelectOrExpand(request)? query: defaultSelect ?? String.Empty);

                    tree.Bind(result.ElementType);
                    tree.BuildType();

                    var selection = DynamicSelection.Select(result, tree.QueryType.Value);

                    response.Content = CreateObjectContent(
                        selection, ((ObjectContent)response.Content).Formatter, response.Content.Headers.ContentType);

                    return response;
                });
        }

        private object GetValueFromObjectContent(HttpContent content)
        {
            if (!(content is ObjectContent)) return null;
            var valueProperty = typeof(ObjectContent).GetProperty("Value");
            if (valueProperty == null) return null;
            return valueProperty.GetValue(content, null);
        }

        private ObjectContent CreateObjectContent(object value, MediaTypeFormatter mtf, MediaTypeHeaderValue mthv)
        {
            if (value == null) return null;
            return new ObjectContent(value.GetType(), value, mtf, mthv);
        }

        private bool ValidResponse(HttpResponseMessage response)
        {
            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK) return false;

            return response.Content is ObjectContent;
        }

        public bool HasSelectOrExpand(HttpRequestMessage request)
        {
            var queryParams = request.RequestUri.ParseQueryString();

            return queryParams.AllKeys.Any(k => k.Equals("$select") || k.Equals("$expand"));
        }
    }
}
