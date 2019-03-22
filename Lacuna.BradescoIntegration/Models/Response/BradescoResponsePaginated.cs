using System;
using System.Collections.Generic;
using System.Text;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class BradescoResponsePaginated : BradescoResponse {
		public Pagination Paging { get; set; }
	}
}
