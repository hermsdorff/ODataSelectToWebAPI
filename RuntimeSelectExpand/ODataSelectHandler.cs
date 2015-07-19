using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuntimeSelectExpand
{
    public class ODataSelectHandler : DelegatingHandler 
    {
        [DebuggerStepThrough]
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!HasSelectOrExpand(request)) return base.SendAsync(request, cancellationToken);

            return base.SendAsync(request, cancellationToken).ContinueWith(
                t =>
                {
                    var response = t.Result;
                    if(!ValidResponse(response)) return response;

                    var lastResult = GetValueFromObjectContent(response.Content);
                    if (!(lastResult is Queryable)) return response;

                    var result = (lastResult as IQueryable<object>);
                    var parser = new ODataParser();
                    var tree = parser.Parse(request.RequestUri.Query);
                    tree.Bind(result.ElementType);
                    tree.BuildType();

                    var selection = DynamicSelection.Select(result, tree.QueryType);

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

            return queryParams.AllKeys.Any(k => k.StartsWith("$select=") || k.StartsWith("$expand="));
        }
    }
}
