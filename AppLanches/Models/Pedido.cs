using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Models
{
    public class Pedido
    {
        public string? Endereco { get; set; }

        public decimal ValorTotal { get; set; }

        public int UsuarioId { get; set; }
    }
}
