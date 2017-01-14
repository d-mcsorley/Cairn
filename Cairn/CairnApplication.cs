using System.Collections.Generic;

namespace Cairn {
    public class CairnApplication {
        private readonly List<IBehaviour> _behaviours;
        private readonly Dictionary<string, IParameter> _parameters;

        public List<IBehaviour> Behaviours {
            get { return _behaviours; }
        }

        public Dictionary<string, IParameter> Parameters {
            get { return _parameters; }
        }

        public CairnApplication() {
            _behaviours = new List<IBehaviour>();
            _parameters = new Dictionary<string, IParameter>();
        }

        public CairnApplication(List<IBehaviour> behaviours, Dictionary<string, IParameter> parameters) {
            _behaviours = behaviours;
            _parameters = parameters;
        }
    }
}
