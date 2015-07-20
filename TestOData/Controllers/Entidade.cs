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
        public List<Detalhe> Detalhes { get; set; }
        public EnumStatus Status { get; set; }

        public Entidade()
        {
            Detalhes = new List<Detalhe>();
            Status = EnumStatus.OK;
        }
    }
}
