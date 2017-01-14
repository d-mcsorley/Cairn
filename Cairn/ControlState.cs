using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cairn {
    public class ControlState : ConcurrentDictionary<string, ConcurrentDictionary<string, object>> {

        public ControlState() { }

        public IDictionary<string, object> this[string scope] {
            get { return this.Get(scope); }
        }

        public object this[string scope, string key] {
            get { return this.Get(scope, key); }
            set { this.Add(scope, key, value); }
        }       

        public IDictionary<string, object> Get(string scope) {
            return base[scope];
        }

        public object Get(string scope, string key) {
            return base[scope][key];
        }

        public void Add(string scope, string key, object value) {
            if (!base.ContainsKey(scope))
                base[scope] = new ConcurrentDictionary<string, object>();

            base[scope][key] = value;
        }

        public bool ContainsKey(string scope, string key) {
            if (base.ContainsKey(scope))
                if (base[scope].ContainsKey(key)) return true;

            return false;
        }

    }
}
