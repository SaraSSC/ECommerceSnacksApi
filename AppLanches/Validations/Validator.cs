using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace AppLanches.Validations
{
    public class Validator : IValidator
    {
        public string NomeError { get; set; }
        public string EmailError { get; set; }
        public string TelefoneError { get; set; }
        public string SenhaError { get; set; }

        private const string NomeEmptyErrorMsg = "Please, insert a name.";
        private const string NomeInvalidErrorMsg = "Please, insert a valid name.";
        private const string EmailEmptyErrorMsg = "Please, insert an email.";
        private const string EmailInvalidErrorMsg = "Please, insert a valid email.";
        private const string TelefoneEmptyErrorMsg = "Please, insert a phone number.";
        private const string TelefoneInvalidErrorMsg = "Please, insert a valid phone number.";
        private const string SenhaEmptyErrorMsg = "Please, insert a password";
        private const string SenhaInvalidErrorMsg = "Password must contain 8 characters, including letters and numbers.";
        public Task<bool> Validate(string nome, string email, string telefone, string senha)
        {
            var isNomeValid = ValidateNome(nome);
            var isEmailValid = ValidateEmail(email);
            var isTelefoneValid = ValidateTelefone(telefone);
            var isSenhaValid = ValidateSenha(senha);

            return Task.FromResult(isNomeValid && isEmailValid && isTelefoneValid && isSenhaValid);
        }

        private bool ValidateNome(string nome)
        {
            if (string.IsNullOrEmpty(nome))
            {
                NomeError = NomeEmptyErrorMsg;
                return false;
            }
            if (nome.Length < 3)
            {
                NomeError = NomeInvalidErrorMsg;
                return false;
            }
            NomeError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmailEmptyErrorMsg;
                return false;
            }
            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = EmailInvalidErrorMsg;
                return false;
            }
            EmailError = "";
            return true;
        }

        private bool ValidateTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
            {
                TelefoneError = TelefoneEmptyErrorMsg;
                return false;
            }

            if (telefone.Length < 8)
            {
                TelefoneError = TelefoneInvalidErrorMsg;
                return false;
            }

            TelefoneError = "";
            return true;

        }
        private bool ValidateSenha(string senha)
        {
            if (string.IsNullOrEmpty(senha))
            {
                SenhaError = SenhaEmptyErrorMsg;
                return false;
            }
            if (senha.Length < 8 || !Regex.IsMatch(senha, @"[a-zA-Z]") || !Regex.IsMatch(senha, @"\d"))
            {
                SenhaError = SenhaInvalidErrorMsg;
                return false;
            }
            SenhaError = "";
            return true;
        }
    }
}
