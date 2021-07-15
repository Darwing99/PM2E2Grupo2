using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.Geolocator;
using Plugin.Media;
using Plugin.Media.Abstractions;
using PM2E2Grupo2.clases;
using PM2E2Grupo2.model;
using PM2E2Grupo2.services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E2Grupo2.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Actualizar : ContentPage
    {
        List<UbicacionModel> service;
        RestService restService;
        bool estado=true;
        double dlatitud, dlongitud;
        byte[] image;
        public Actualizar()
        {
            InitializeComponent();
            restService = new RestService();
            locationGPS();
            
          


        }

        public async void updateData()
        {
            refresc.IsRunning = true;
            if ((id.Text != null && estado==true))
            {
               
                service = await restService.GetRepositoriesAsync(DataConstants.urlGet);
                var buscar = service.Where(c => c.id.ToString().Contains(id.Text));
                descripcion_larga.Text = buscar.FirstOrDefault().descripcion;
                image = buscar.FirstOrDefault().fotografia;
                imagefile.Source = ImageSource.FromStream(() => new MemoryStream(image));
                refresc.IsRunning = false;
                return;

            }
            else
            {
                refresc.IsRunning = false;
            }
            
        }
        private void Salvar_Clicked(object sender, EventArgs e)
        {
            refresc.IsRunning = false;
            estado = false;
            guardarUbicacion();
        }

        protected override void OnAppearing()
        {

            base.OnAppearing();
            locationGPS();
            updateData();

        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            refresc.IsRunning = false;
            estado = false;
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
            if (imagefile.Source == null)
            {
                await DisplayAlert("Alerta", "Seleccione Imagen", "ok");
                return;
            }


            else
            {

                var ubicacion = new UbicacionModel()
                {
                    id = Convert.ToInt32(id.Text),
                    latitud = Convert.ToDouble(latitud.Text),
                    longitud = Convert.ToDouble(longitud.Text),
                    descripcion = descripcion_larga.Text,
                    fotografia = image

                };
                refrescar.IsRunning = true;
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(ubicacion);
                var contentJSON = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(DataConstants.urlPost, contentJSON);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await DisplayAlert("Datos", "Se actualizo correctamente la información", "OK");
                    refrescar.IsRunning = false;
                }


            }


        }
        private async void buttonfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}