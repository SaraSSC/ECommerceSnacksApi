using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Models
{
    public class ProdutoFavorito
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ProdutoId { get; set; }

        public string? Nome { get; set; }

        public string? Detalhe { get; set; }

        public decimal Preco { get; set; }

        public string? ImagemUrl { get; set; }

        public bool IsFavorito { get; set; }
    }
}
