﻿using AppRpgEtec.Models;
using AppRpgEtec.Services.Armas;
using AppRpgEtec.Services.Personagens;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppRpgEtec.ViewModels.Armas
{
    [QueryProperty("ArmaSelecionadaId", "aId")]
    public class CadastroArmaViewModel : BaseViewModel
    {
        private ArmaService aService;
        private PersonagemService pService;

        public CadastroArmaViewModel()
        {
            string token = Application.Current.Properties["UsuarioToken"].ToString();
            aService = new ArmaService(token);
            pService = new PersonagemService(token);

            ObterPersonagens();

            SalvarCommand = new Command(async () => await SalvarArma());
        }

        public ICommand SalvarCommand { get; set; }



        #region Atributos_Propriedades

        private int id;
        private string nome;
        private int dano;
        private int personagemId;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public string Nome
        {
            get => nome;
            set
            {
                nome = value;
                OnPropertyChanged();
            }
        }
        public int Dano
        {
            get => dano;
            set
            {
                dano = value;
                OnPropertyChanged();

            }
        }
        public int PersonagemId
        {
            get => personagemId;
            set
            {
                personagemId = value;
                OnPropertyChanged();
            }
        }

        private Personagem personagemSelecionado;
        public Personagem PersonagemSelecionado
        {
            get { return personagemSelecionado; }
            set
            {
                if (value != null)
                {
                    personagemSelecionado = value;
                    OnPropertyChanged();
                }
            }
        }

        private string armaSelecionadaId;//CTRL + R,E
        public string ArmaSelecionadaId
        {
            set
            {
                if (value != null)
                {
                    armaSelecionadaId = Uri.UnescapeDataString(value);
                    CarregarArma();
                }
            }
        }

        public ObservableCollection<Personagem> Personagens { get; set; }

        #endregion

        #region Metodos



        public async void ObterPersonagens()
        {
            try
            {
                Personagens = await pService.GetPersonagensAsync();
                OnPropertyChanged(nameof(Personagens));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message, "Ok");
            }
        }

        public async Task SalvarArma()
        {
            try
            {
                Arma model = new Arma()
                {
                    Id = this.id,
                    Nome = this.nome,
                    Dano = this.dano,
                    PersonagemId = this.personagemSelecionado.Id
                };

                if (model.Id == 0)
                    await aService.PostArmaAsync(model);
                else
                    await aService.PutArmaAsync(model);

                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvo com sucesso", "Ok");

                await Shell.Current.GoToAsync("..");
            }
            catch (System.Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", ex.Message, "Ok");
            }
        }

        public async void CarregarArma()
        {
            try
            {
                Arma model = await
                    aService.GetArmaAsync(int.Parse(armaSelecionadaId));

                this.Nome = model.Nome;
                this.Dano = model.Dano;
                this.Id = model.Id;
                this.PersonagemId = model.PersonagemId;

                this.PersonagemSelecionado =
                    Personagens.FirstOrDefault(x => x.Id == model.PersonagemId);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("ops", ex.Message, "Ok");
            }
        }

        #endregion



    }
}
