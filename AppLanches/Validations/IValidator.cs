using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Validations
{
    public interface IValidator
    {
        string NomeError { get; set; }
        string EmailError { get; set; }
        string TelefoneError { get; set; }
        string SenhaError { get; set; }
        Task<bool> Validate(string nome, string email,
                           string telefone, string senha);
    }
}
