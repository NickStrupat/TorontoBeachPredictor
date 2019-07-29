using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TorontoBeachPredictor.Data
{
    public class Beach : IEquatable<Beach>
    {
        public Int32 Id { get; set; }
        [Required] public String Name { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
        public override Int32 GetHashCode() => Id.GetHashCode();
        public override Boolean Equals(Object obj) => obj is Beach beach && Equals(beach);
        public Boolean Equals(Beach other) => Id == other.Id;
    }
}
