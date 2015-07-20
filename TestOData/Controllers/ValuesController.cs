using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace TestOData.Controllers
{
    public class ValuesController : ApiController
    {
        public static List<Entidade> _entidades = new List<Entidade>();

        public ValuesController()
        {
            if (_entidades.Count == 0)
            {
                _entidades.Add(new Entidade { Id = 1, Chave = "Chave1", Valor = "Valor1" });
                _entidades.Add(new Entidade { Id = 2, Chave = "Chave2", Valor = "Valor2" });

                _entidades[0].Detalhe = (new Detalhe { Id = 1, Valor = "Detalhe1" });
                _entidades[1].Detalhe = (new Detalhe { Id = 2, Valor = "Detalhe2" });
            }
        }

        // GET api/values
        [Queryable(AllowedQueryOptions=AllowedQueryOptions.All)]
        public IQueryable<Entidade> Get()
        {
            return _entidades.AsQueryable(); ;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}