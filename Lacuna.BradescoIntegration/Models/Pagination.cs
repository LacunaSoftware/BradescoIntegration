using System;
using System.Collections.Generic;
using System.Text;

namespace Lacuna.BradescoIntegration.Models {
	public class Pagination {
		public int Limit { get; set; }
		public int CurrentOffset { get; set; }
		public int NextOffset { get; set; }
	}
}
