using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static PM2E2Grupo2.clases.Metodos;

namespace PM2E2Grupo2.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForsquareApiList : ContentPage
    {
        List<Venue> service;
        double latitud, longitude;
        public ForsquareApiList(double latitut,double longitud)
        {
            InitializeComponent();
            list.IsRefreshing = true;
            latitud = latitut;
            longitude = longitud;
        }

        private async void list_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            var objeto = (Venue)e.SelectedItem;
            if (!string.IsNullOrEmpty(objeto.id))
            {
                var items = service.Where(c => c.id.Contains(objeto.id));
                if (items != null)
                {
                  
                    var datamap = new Mapa(items.FirstOrDefault().location.lat,items.FirstOrDefault().location.lng, items.FirstOrDefault().name, items.FirstOrDefault().location.city, items.FirstOrDefault().location.country);
                
                   await Navigation.PushAsync(datamap);

                }

            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            
            service = await clases.Metodos.getSites(latitud, longitude);
            list.ItemsSource = service;
          
            list.IsRefreshing = false;

            


        }
    }
}