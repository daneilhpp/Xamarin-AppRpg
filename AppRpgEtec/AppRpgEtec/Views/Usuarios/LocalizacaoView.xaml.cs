using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.ViewModels.Usuarios;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRpgEtec.Views.Usuarios
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalizacaoView : ContentPage
    {
        LocalizacaoViewModel viewModel;
        public LocalizacaoView()
        {
            InitializeComponent();

            viewModel = new LocalizacaoViewModel();
            BindingContext = viewModel;

            LocalizacaoViewModel.mapa = map;

            viewModel.IniciarMapa();
            viewModel.LocalizarEtec();
            viewModel.ExibirUsuariosMapa();
        }
    }
}