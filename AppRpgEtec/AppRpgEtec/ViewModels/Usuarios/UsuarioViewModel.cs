﻿using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms;
using AppRpgEtec.Views;
using Plugin.Geolocator;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class UsuarioViewModel: BaseViewModel
    {
        private UsuarioService uService;
        private Usuario Usuario;

        public ICommand EntrarCommand { get; set; }

        public UsuarioViewModel()
        {
            this.Usuario = new Usuario();
            uService = new UsuarioService();
            RegistrarCommands();
        }

        public void RegistrarCommands()
        {
            EntrarCommand = new Command(async () => { await ConsultarUsuario(); });
        }

        public async Task ConsultarUsuario()
        {
            try
            {
                Usuario u = null;
                u = await uService.PostLoginUsuarioAsync(Usuario);

                if(!string.IsNullOrEmpty(u.Token))
                {
                    Application.Current.Properties["UsuarioId"] = u.Id;
                    Application.Current.Properties["UsuarioUsername"] = u.Username;
                    Application.Current.Properties["UsuarioPerfil"] = u.Perfil;
                    Application.Current.Properties["UsuarioToken"] = u.Token;

                    UsuarioService uServiceLoc = new UsuarioService(u.Token);
                    Usuario uLoc = await uServiceLoc.GetUsuarioAsync(u.Username);

                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;
                    var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

                    uLoc.Latitude = position.Latitude;
                    uLoc.Longitude = position.Longitude;

                    await uServiceLoc.PutAtualizarLocalizacaoAsync(uLoc);

                    string message = string.Format("Bem-vindo {0}", u.Username);
                    await Application.Current.MainPage.DisplayAlert("Informação",message,"OK");

                    Application.Current.MainPage = new FlyoutMenu();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Informação", "Dados incorretos :(", "Ok");
                }
            }
            catch (Exception ex)
            { 
                await Application.Current.MainPage.DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        #region ViewProperties
        public string Login
        {
            get { return this.Usuario.Username; }
            set 
            {
                this.Usuario.Username = value; 
                OnPropertyChanged();
            }
        }

        public string Senha
        {
            get { return this.Usuario.PasswordString; }
            set
            {
                this.Usuario.PasswordString = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
