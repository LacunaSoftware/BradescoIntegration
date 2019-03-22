using Newtonsoft.Json.Converters;

namespace Lacuna.BradescoIntegration {
	public class CustomDateTimeConverter : IsoDateTimeConverter {
		public CustomDateTimeConverter() {
			DateTimeFormat = "yyyy-MM-dd";
		}
	}
}
