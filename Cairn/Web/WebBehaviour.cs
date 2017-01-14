using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cairn.Web {
    public class WebBehaviour : Behaviour, IBehaviour {
        private List<string> _authorisedRoles;

        public WebBehaviour() : base() {
            _authorisedRoles = new List<string>();
        }

        public List<string> AuthorisedRoles {
            get { return _authorisedRoles; }
        }

        public override void Fire(string message, IContext context) {

        }
    }
}
