using System;
using System.Collections.Generic;
using System.Text;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Personagens;
using Xamarin.Forms;
using System.IO;
using Plugin.Media;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Personagens
{
    [QueryProperty("PersonagemSelecionadoId", "pId")]
    public class ImagemPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;
        public Personagem Personagem { get; set; }
        public ImagemPersonagemViewModel()
        {
            string token = Application.Current.Properties["UsuarioToken"].ToString();
            pService = new PersonagemService(token);

            AbrirGaleriaCommand = new Command(abrirGaleria);
            SalvarImagemCommand = new Command(SalvarImagem);
            FotografarCommand = new Command(Fotografar);
        }
        public ICommand AbrirGaleriaCommand { get; }
        public ICommand SalvarImagemCommand { get; }
        public ICommand FotografarCommand { get; }

        private string personagemSelecionadoId;//CTRL + R,E
        public string PersonagemSelecionadoId
        {
            set
            {
                if (value != null)
                {
                    personagemSelecionadoId = Uri.UnescapeDataString(value);
                    CarregarPersonagem();
                }
            }
        }
        public async void CarregarPersonagem()
        {
            try
            {
                Personagem p = await
                pService.GetPersonagemAsync(int.Parse(personagemSelecionadoId));

                Personagem = new Personagem();
                Personagem.Nome = p.Nome;
                Personagem.Id = p.Id;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private ImageSource fonteImagem;
        public ImageSource FonteImagem
        {
            get
            {
                return fonteImagem;
            }
            set
            {
                fonteImagem = value;
                OnPropertyChanged();
            }
        }

        private string descricao;
        public string Descricao
        {
            get { return descricao; }
            set
            {
                descricao = value;
                OnPropertyChanged();
            }
        }


        public async void abrirGaleria()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if(!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("Galeria não suportada", "Não existe permissão para acessar a galeria", "Ok");
                    return;
                }
                var file = await CrossMedia.Current.PickPhotoAsync();
                if (file == null)
                    return;

                MemoryStream ms = null;
                using (ms = new MemoryStream())
                {
                    var stream = file.GetStream();
                    stream.CopyTo(ms);
                }
                FonteImagem = ImageSource.FromStream(() => file.GetStream());
                Personagem.FotoPersonagem = ms.ToArray();
                return;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + ". Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void Fotografar()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("Sem Câmera", "A câmera não está disponível", "Ok");
                    await Task.FromResult(false);
                }

                string fileName = String.Format("{0:ddMMyyy_HHmmss}", DateTime.Now) + ".jpg";

                var file = await CrossMedia.Current.TakePhotoAsync
                (
                    new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Fotos",
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                        Name = fileName
                    }
                );

                if(file == null)
                    await Task.FromResult(false);

                MemoryStream ms = null;
                using (ms = new MemoryStream())
                {
                    var stream = file.GetStream();
                    stream.CopyTo(ms);
                }
                FonteImagem = ImageSource.FromStream(() => file.GetStream());
                Personagem.FotoPersonagem = ms.ToArray();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + ". Detalhes: " + ex.InnerException, "Ok");
            }
        }


        public async void SalvarImagem()
        {
            try
            {
                await pService.PutFotoPersonagemAsync(this.Personagem);
                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos com sucesso", "Ok");
                await App.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + ". Detalhes: " + ex.InnerException, "Ok");
            }
        }
    }
}
