using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairn;
using System.Threading;
using System.Reflection;

namespace Cairn.Harness {      
    public class Program {
        static void Main(string[] args) {

			Program p = new Program();

			IBehaviourService service = new XmlBehaviourService();
			Context context = service.CreateContext();
			context.ControlState["program"] = p;

			context.Fire("get-data");

			Console.Read();
			

        }

		public void Print (string value) {
			Console.WriteLine(value);
		}
    }
}
