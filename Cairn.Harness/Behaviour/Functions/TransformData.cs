using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairn.Harness.Component;

namespace Cairn.Harness.Behaviour.Functions {
	public class TransformData : IFunction<string[], string, string[]> {
		public bool Condition (string[] inputData, string extraData) {
			return true;
		}

		public string[] Return (string[] inputData, string extraData) {
			string[] processedData = new string[inputData.Length];
			for (int i = 0; i < inputData.Length; i++) {
				processedData[i] = String.Format(@"{0}:{1}", extraData, inputData[i]);
			}
			return processedData;
		}		
	}
}
