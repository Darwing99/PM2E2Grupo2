using Plugin.Geolocator;
using PM2E2Grupo2.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Plugin.Media;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.FilePicker;
using PM2E2Grupo2.views;
using Plugin.Media.Abstractions;
using PM2E2Grupo2.services;
using System.Net.Http;
using Newtonsoft.Json;
using PM2E2Grupo2.clases;
using System.Net;

namespace  PM2E2Grupo2
{
    public partial class MainPage : ContentPage
    {
        List<UbicacionModel> service;
        RestService restService;
        double dlatitud, dlongitud;
        byte[] image;
        public MainPage()
        {
            InitializeComponent();
            restService = new RestService();


        }

        protected  override void OnAppearing()
        {

            base.OnAppearing();
            locationGPS();
        }

        public async void locationGPS()
        {

            var location = CrossGeolocator.Current;
            location.DesiredAccuracy = 50;

            if (!location.IsGeolocationEnabled || !location.IsGeolocationAvailable)
            {

                await DisplayAlert("Warning", " GPS no esta activo", "ok");

            }
            else
            {
                if (!location.IsListening)
                {
                    await location.StartListeningAsync(TimeSpan.FromSeconds(10), 1);


                }
                location.PositionChanged += (posicion, args) =>
                {
                    var ubicacion = args.Position;
                    latitud.Text = ubicacion.Latitude.ToString();
                    dlatitud = Convert.ToDouble(latitud.Text);
                    longitud.Text = ubicacion.Longitude.ToString();
                    dlongitud = Convert.ToDouble(longitud.Text);
                };

            }

        }

        public async void guardarUbicacion()
        {

            if (string.IsNullOrEmpty(descripcion_larga.Text))
            {
                await DisplayAlert("Alerta", "Debe describir la ubicacion", "ok");
                return;
            }
            if (imagefile.Source==null) {
                await DisplayAlert("Alerta", "Seleccione Imagen", "ok");
                return;
            }


            else
            {



                var ubicacion = new UbicacionModel()
                {
                    id = 0,
                    latitud = Convert.ToDouble(latitud.Text),
                    longitud = Convert.ToDouble(longitud.Text),
                    descripcion = descripcion_larga.Text,
                    fotografia=image
                  
                };
                refresc.IsRunning = true;
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(ubicacion);
                var contentJSON = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(DataConstants.urlPost, contentJSON);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await DisplayAlert("Datos", "Se guardo la ubicacion", "OK");
                    descripcion_larga.Text = "";
                    imagefile.Source = "";
                    image=null;

                    refresc.IsRunning = false;
                }


            }


        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            string[] fileTypes = null;

            if (Device.RuntimePlatform == Device.Android)
            {
                fileTypes = new string[] { "image/png", "image/jpeg" };
            }
            await PickAndShowFile(fileTypes);
        }
        private async Task PickAndShowFile(string[] fileTypes)
        {
            
            var file = await CrossFilePicker.Current.PickFile(fileTypes);

            if (file != null)
            {
               
                imagefile.Source = file.FileName;
                imagefile.IsVisible = true;
                if (file.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                       || file.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    imagefile.Source = ImageSource.FromStream(() => {
                        return file.GetStream();
                    });
                    imagefile.IsVisible = true;
                    using (MemoryStream memory = new MemoryStream())
                    {

                        Stream stream = file.GetStream();
                        stream.CopyTo(memory);
                        image = memory.ToArray();
                    }
                  
                }

            }
        }

        private async  void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Ubicaciones());
        }

        private async void buttoncamera_Clicked(object sender, EventArgs e)
        {
            var camera = new StoreCameraMediaOptions();
            camera.PhotoSize = PhotoSize.Full;
            camera.Name = "img";
            camera.Directory = "MiApp";


            var foto = await CrossMedia.Current.TakePhotoAsync(camera);


            if (foto != null)
            {

                imagefile.Source = ImageSource.FromStream(() => {

                    return foto.GetStream();



                });
                imagefile.IsVisible = true;
                using (MemoryStream memory = new MemoryStream())
                {

                    Stream stream = foto.GetStream();
                    stream.CopyTo(memory);
                    image = memory.ToArray();
                }
            }
        }

        private void Salvar_Clicked(object sender, EventArgs e)
        {
                guardarUbicacion();

        }
    }
}
