using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using AppRpgEtec.Services.Usuarios;
using AppRpgEtec.Models;
using System.Collections.ObjectModel;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class LocalizacaoViewModel
    {
        public static Map mapa;
        private UsuarioService uService;

        public LocalizacaoViewModel()
        {
            mapa = new Map();

            string token = Application.Current.Properties["UsuarioToken"].ToString();
            uService = new UsuarioService(token);
        }

        public async void IniciarMapa()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

                if(status != PermissionStatus.Granted)
                {
                    if(await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await Application.Current.MainPage.DisplayAlert("Atenção", "É preciso permissão de localização", "Ok");
                    }
                    status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
                }
                if (status == PermissionStatus.Granted)
                {
                    mapa.MyLocationEnabled = true;
                    mapa.UiSettings.MyLocationButtonEnabled = true;
                }
                else if (status != PermissionStatus.Unknown)
                    throw new Exception("permissão negada");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "Ok");    
            }
        }

        public async void LocalizarEtec()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await Application.Current.MainPage.DisplayAlert("Atenção", "É preciso permissão de localização", "Ok");
                    }
                    status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
                }
                if (status == PermissionStatus.Granted)
                {
                    Pin pinEtec = new Pin()
                    {
                        Type = PinType.Place,
                        Label = "Etec Horácio",
                        Address = "Rua Alcântara, 113, Vila Guilherme",
                        Position = new Position(-23.5200241d, -46.596498d),
                        Tag = "IdEtec",
                    };
                    mapa.Pins.Add(pinEtec);
                    mapa.MoveToRegion(MapSpan.FromCenterAndRadius(pinEtec.Position, Distance.FromMeters(500)));
                }
                else if (status != PermissionStatus.Unknown)
                    throw new Exception("permissão negada");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "Ok");
            }
        }

        public async void ExibirUsuariosMapa()
        {
            try
            {
                ObservableCollection<Usuario> ocUsuarios = await uService.GetUsuariosAsync();

                List<Usuario> listUsuarios = new  List<Usuario>(ocUsuarios);

                foreach (Usuario u in listUsuarios)
                {
                    if(u.Latitude != null && u.Longitude != null)
                    {
                        double latitude = (double)u.Latitude;
                        double longitude = (double)u.Longitude;

                        Pin pinAtual = new Pin()
                        {
                            Type = PinType.Place,
                            Label = u.Username,
                            Position = new Position(latitude, longitude)
                        };
                        mapa.Pins.Add(pinAtual);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "Ok");
            }
        }
    }
}
