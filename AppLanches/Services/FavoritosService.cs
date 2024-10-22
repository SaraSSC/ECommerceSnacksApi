using AppLanches.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Services
{
    public class FavoritosService
    {
        private readonly SQLiteAsyncConnection _database;

        public FavoritosService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "favoritos.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ProdutoFavorito>().Wait();
        }

        public async Task<ProdutoFavorito> ReadAsync(int id)
        {
            return await _database.Table<ProdutoFavorito>().Where(x => x.ProdutoId == id).FirstOrDefaultAsync();
        }

        public async Task<List<ProdutoFavorito>> ReadAllAsync()
        {
            return await _database.Table<ProdutoFavorito>().ToListAsync();
        }

        public async Task CreateAsync(ProdutoFavorito produtoFavorito)
        {
            await _database.InsertAsync(produtoFavorito);
        }

        public async Task DeleteAsync(ProdutoFavorito produtoFavorito)
        {
            await _database.DeleteAsync(produtoFavorito);
        }

       
    }
    
}
