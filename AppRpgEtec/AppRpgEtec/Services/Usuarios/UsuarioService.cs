using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Models;
using System.Collections.ObjectModel;

namespace AppRpgEtec.Services.Usuarios
{
    public class UsuarioService: Request
    {
        private readonly Request _request;
        private const string ApiUrlBase = "https://bsite.net/luizfernando987/Usuarios";
        //private const string ApiUrlBase = "http://lzsouza.somee.com/RpgApi/Usuarios";
        private string _token;
        public UsuarioService(string token)
        {
            _request = new Request();
            _token = token;
        }

        public UsuarioService()
        {
            _request = new Request();
        }

        public async Task<Usuario> PostLoginUsuarioAsync(Usuario u)
        {
            string urlComplementar = "/Autenticar";
            u.Token = await _request.PostReturnStringAsync(ApiUrlBase + urlComplementar, u);
            return u;
        }

        public async Task<Usuario> PutAtualizarLocalizacaoAsync(Usuario u)
        {
            string urlComplementar = "/AtualizarLocalizacao";
            var result = await _request.PutAsync(ApiUrlBase + urlComplementar, u, _token);
            return result;
        }
        public async Task<Usuario> GetUsuarioAsync(string login)
        {
            string urlComplementar = string.Format("/GetByLogin/{0}", login);
            var usuario = await
            _request.GetAsync<Models.Usuario>(ApiUrlBase + urlComplementar, _token);
            return usuario;
        }
        //using System.Collections.ObjectModel
        public async Task<ObservableCollection<Usuario>> GetUsuariosAsync()
        {
            string urlComplementar = string.Format("{0}", "/GetAll");
            ObservableCollection<Models.Usuario> listaUsuarios = await
            _request.GetAsync<ObservableCollection<Models.Usuario>>(ApiUrlBase + urlComplementar,
            _token);
            return listaUsuarios;
        }
    }
}
