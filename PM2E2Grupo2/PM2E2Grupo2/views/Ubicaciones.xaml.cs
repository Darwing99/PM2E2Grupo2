using PM2E2Grupo2.clases;
using PM2E2Grupo2.model;
using PM2E2Grupo2.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E2Grupo2.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ubicaciones : ContentPage
    {
       

        List<UbicacionModel> service;
        RestService restService;
        public Ubicaciones()
        {
            InitializeComponent();
            restService = new RestService();
            lista.IsRefreshing = true;
            OnAppearing();
          
        }
        protected async override void OnAppearing()
        {

            base.OnAppearing();

            service = await restService.GetRepositoriesAsync(DataConstants.urlGet);
            if (service == null)
            {
               lista.IsRefreshing = true;
                return;
            }
            else
            {
                lista.IsRefreshing = false;
                lista.ItemsSource = service;
            }



        }

        private async void ShowMapa_Clicked(object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(id.Text)))
            {
                await DisplayAlert("Alerta", "Seleccione una ubicacion", "ok");
                return;
            }
            bool show = await DisplayAlert("Lista de Lugares cercanos", "Ir a foresquare api", "yes", "no");
            if (show)
            {
             
                await Navigation.PushAsync(new ForsquareApiList(Convert.ToDouble(latitud.Text),Convert.ToDouble(longitud.Text)));
            }
          

            
        }

        private  void lista_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {


            var ubicacion = new UbicacionModel();
            var objeto = (UbicacionModel)e.SelectedItem;
            if (!string.IsNullOrEmpty(objeto.id.ToString()))
            {
                var listaSeleccionada = service.Where(c => c.id.ToString().Contains(objeto.id.ToString()));
                if (listaSeleccionada != null)
                {
                    id.Text = (listaSeleccionada.FirstOrDefault().id).ToString();
                    descripcion.Text = listaSeleccionada.FirstOrDefault().descripcion;
                    latitud.Text = (listaSeleccionada.FirstOrDefault().latitud).ToString();
                    longitud.Text = listaSeleccionada.FirstOrDefault().longitud.ToString();
                  
                }

            }

        }

        private async void Eliminar_Clicked(object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(id.Text)))
            {
                await DisplayAlert("Alerta", "Seleccione una ubicacion para eliminar", "ok");
                return;
            }
            bool show = await DisplayAlert("Ver Mapa", "Eliminar esta ubicacion: "+descripcion.Text, "yes", "no");
            if (show)
            {
               
                service = await restService.DeleteTodoItemAsync(DataConstants.urlDelete+id.Text);
               
            
                OnAppearing();
              
            }

        }

        private async void actualizar_Clicked(object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(id.Text)))
            {
                await DisplayAlert("Alerta", "Seleccione una ubicacion", "ok");
                return;
            }
            bool show = await DisplayAlert("Update", "Actualizar Datos", "yes", "no");
            if (show)
            {
                var getLista = new Lista
                {
                    idlista = Convert.ToInt32(id.Text)
                };
                var getMapa = new Actualizar();
                getMapa.BindingContext = getLista;

                await Navigation.PushAsync(getMapa);
            }


        }
    }
}