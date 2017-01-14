using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cairn.Harness.Component {
	public class DummyDataComponent {
		private readonly string _dataSet;

		public DummyDataComponent (string dataSet) {
			this._dataSet = dataSet;
		}

		public string[] GetData () {
			string[] data = null;
			if (this._dataSet == "Colours") {
				data = new string[] { "Orange", "Blue", "Red", "Green" };
			} else if (this._dataSet == "Films") {
				data = new string[] { "Pulp Fiction", "Star Wars", "Taxi Driver", "They Live" };
			} else {
				data = new string[] { "Apple", "Banana", "Strawberry", "Grape" };
			}
			return data;
		}
	}
}
