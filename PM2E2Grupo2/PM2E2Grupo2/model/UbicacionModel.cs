using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PM2E2Grupo2.model
{
    public class UbicacionModel
    {

        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("latitud")]
        public double latitud { get;  set; }
        [JsonProperty("longitud")]
        public double longitud { get;  set; }
        [JsonProperty("descripcion")]
        public string descripcion { get;  set; }
        [JsonProperty("fotografia")]
        public byte[] fotografia { get; set; }
    }
    
}
