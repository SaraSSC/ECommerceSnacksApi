using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Models
{
    public class PedidoPorUsuario
    {
        public int Id { get; set; }

        public decimal PedidoTotal { get; set; }


        public DateTime DataPedido { get; set; }
    }
}
