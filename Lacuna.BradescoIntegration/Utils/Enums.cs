using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration.Utils {
	public static class Enums {
		enum RenderType : byte {
			Html = 0,
			TelaComLinkPdf = 1,
			Pdf = 2
		}

		enum AutomaticDebitEndConfig : byte {
			EmiteAviso = 1,
			NaoEmiteAviso = 2
		}
	}
}
