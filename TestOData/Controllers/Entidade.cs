using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestOData.Controllers
{
    public class Entidade
    {
        public int Id { get; set; }

        public string Chave { get; set; }

        public string Valor { get; set; }

        public Detalhe Detalhe { get; set; }
    }
}
