using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cairn {
    public class Context : IContext {       
        private readonly ConcurrentDictionary<string, object> _controlState;
        private readonly ConcurrentDictionary<string, object> _returnData;
        private readonly List<ErrorMessage> _errors;       

        private readonly CairnApplication _application;

        public Context(CairnApplication application) {
            _controlState = new ConcurrentDictionary<string, object>();
            _returnData = new ConcurrentDictionary<string, object>();
            _errors = new List<ErrorMessage>();
            _application = application;
        }

        public ConcurrentDictionary<string, object> ControlState {
            get { return _controlState; }
        }

        public ConcurrentDictionary<string, object> ReturnData {
            get { return _returnData; }
        }

        public List<ErrorMessage> Errors {
            get { return _errors; }
        } 

        public CairnApplication Application {
            get { return _application; }
        }

        //public void Fire(string message, object paramsAsAnonymousType) {
        //    if (paramsAsAnonymousType != null) {
        //        PropertyInfo[] propertyInfos = paramsAsAnonymousType.GetType().GetProperties();
        //        foreach (PropertyInfo propertyInfo in propertyInfos) {
        //            _controlState.GetOrAdd(propertyInfo.Name, propertyInfo.GetValue(paramsAsAnonymousType, null));
        //        }
        //    }
        //    this.Fire(message);
        //}

        public void Fire(string message) {
            for (int i = 0; i < _application.Behaviours.Count; i++) {
                _application.Behaviours[i].Fire(message, this);
            }
        }
    }
}
