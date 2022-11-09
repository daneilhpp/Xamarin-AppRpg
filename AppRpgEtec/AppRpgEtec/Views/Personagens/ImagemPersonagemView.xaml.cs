using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppRpgEtec.ViewModels;
using AppRpgEtec.Models;

namespace AppRpgEtec.Views.Personagens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagemPersonagemView : ContentPage
    {
        ImagemPersonagemView viewModel;
        public ImagemPersonagemView()
        {
            InitializeComponent();

            viewModel = new ImagemPersonagemView();
            Title = "Imagem do Personagem";
            BindingContext = viewModel;
        }
    }
}