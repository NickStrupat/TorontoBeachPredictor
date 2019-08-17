using Newtonsoft.Json;
using System;

namespace TorontoBeachPredictor.Data
{
	public class BeachSample
	{
		public Int64 Id { get; set; }
		public DateTime SampleDate { get; set; }
		public DateTime PublishDate { get; set; }
		public Int32 EColiCount { get; set; }
		public BeachStatus BeachStatus { get; set; }

		public Beach Beach { get; set; }
		public Int32 BeachId { get; set; }

		public override String ToString() => JsonConvert.SerializeObject(this, Formatting.None);
		public override Int32 GetHashCode() => Id.GetHashCode();
		public override Boolean Equals(Object obj) => obj is Beach beach && Equals(beach);
		public Boolean Equals(Beach other) => Id == other.Id;
	}
}