using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cairn.Harness.Behaviour.Actions {
	public class PrintData : IAction<Cairn.Harness.Program, string[]> {
		public bool Condition (Program program, string[] data) {
			return true;
		}

		public void Execute (Program program, string[] data) {
			for (int i = 0; i < data.Length; i++) {
				program.Print(data[i]);
			}		
		}

	}
}
