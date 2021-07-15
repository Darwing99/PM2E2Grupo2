using Newtonsoft.Json;
using PM2E2Grupo2.clases;
using PM2E2Grupo2.model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PM2E2Grupo2.services
{
    class RestService
    {
        HttpClient cliente;

        public RestService()
        {
            cliente = new HttpClient();

            if (Device.RuntimePlatform == Device.UWP)
            {
                cliente.DefaultRequestHeaders.Add("User-Agent", "");
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            }
        }

        public async Task<List<UbicacionModel>> GetRepositoriesAsync(string url)
        {
            List<UbicacionModel> lista = null;
            try
            {
                HttpResponseMessage respuesta = await cliente.GetAsync(url);
                if (respuesta.IsSuccessStatusCode)
                {
                    string img = await respuesta.Content.ReadAsStringAsync();
                    lista = JsonConvert.DeserializeObject<List<UbicacionModel>>(img);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error", ex.Message);
            }

            return lista;
        }
        public async Task<List<UbicacionModel>> DeleteTodoItemAsync(string url)
        {
            List<UbicacionModel> lista = null;
            
           
             HttpResponseMessage response = await cliente.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
               
            }
            return lista;
         
            }


    }
}