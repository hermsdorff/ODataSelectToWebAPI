using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuntimeSelectExpand
{
    public class ODataParser
    {
        private const string SelectCommand = "$select=";
        private const string ExpandCommand = "$expand=";

        public IExpressionTree Parse(string query)
        {
            var parameters = query.Split('&');
            var tree = new ExpressionTree();

            LoadExpandFields(parameters, tree);
            LoadSelectFields(parameters, tree);

            return tree;
        }

        private void LoadExpandFields(IEnumerable<string> parameters, IExpressionTree tree)
        {
            var expandParameters = parameters.SingleOrDefault(p => p.StartsWith(ExpandCommand));
            if(expandParameters == null) return;

            var expandParameter = expandParameters.Split(',');
            expandParameter[0] = expandParameter[0].Replace(ExpandCommand, String.Empty);

            foreach (var param in expandParameter)
            {
                tree.AddCollection(param);
            }
        }

        private void LoadSelectFields(IEnumerable<string> parameters, IExpressionTree tree)
        {
            var selectParameters = parameters.SingleOrDefault(p => p.StartsWith(SelectCommand));
            if (selectParameters == null) return;

            var selectParameter = selectParameters.Split(',');
            selectParameter[0] = selectParameter[0].Replace(SelectCommand, String.Empty);

            foreach (var param in selectParameter.OrderBy(o => o))
            {
                tree.AddProperty(param);
            }
        }
    }
}
