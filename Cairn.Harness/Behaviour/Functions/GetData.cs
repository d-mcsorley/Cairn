using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairn.Harness.Component;

namespace Cairn.Harness.Behaviour.Functions {
	public class GetData : IFunction<DummyDataComponent, string[]> {	
		public bool Condition (DummyDataComponent dataComponent) {
			return true;
		}

		public string[] Return (DummyDataComponent dataComponent) {
			return dataComponent.GetData();
		}		
	}
}
