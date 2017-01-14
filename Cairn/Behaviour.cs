using System;
using System.Collections.Generic;

namespace Cairn {
    public class Behaviour : IBehaviour {
        private string _name;
        private List<IParameter> _parameters;
        private List<string> _respondsToMessages;
        private List<string> _processesMessages;
        private List<FunctionWrapper> _functions;
        private List<ActionWrapper> _actions;

        public Behaviour() {
            _parameters = new List<IParameter>();
            _respondsToMessages = new List<string>();
            _processesMessages = new List<string>();
            _functions = new List<FunctionWrapper>();
            _actions = new List<ActionWrapper>();
        }

        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        public List<IParameter> Parameters {
            get { return _parameters; }
        }

        public List<string> RespondsToMessages {
            get { return _respondsToMessages; }
        }

        public List<string> ProcessesMessages {
            get { return _processesMessages; }
        }

        public List<FunctionWrapper> Functions {
            get { return _functions; }
        }

        public List<ActionWrapper> Actions {
            get { return _actions; }
        }

        public virtual void Fire(string message, IContext context) {
            try {
                if (!_respondsToMessages.Contains(message))
                    return;

                object[] parameters = new object[_parameters.Count];
                for (int i = 0; i < _parameters.Count; i++) {
                    object item = _parameters[i].GetParameter(context);

                    if (item == null)
                        return;
                    else
                        parameters[i] = item;
                }

                for (int i = 0; i < _actions.Count; i++) {
                    if (_actions[i].Condition(parameters))
                        _actions[i].Execute(parameters);
                }

                for (int i = 0; i < _functions.Count; i++) {
                    if (_functions[i].Condition(parameters)) {
                        object returnObject = _functions[i].Return(parameters);
                        context.ControlState[_functions[i].OutputName] = returnObject;

                        if (_functions[i].ReturnAsResult)
                            context.ReturnData[_functions[i].OutputName] = returnObject;
                    }
                }
                for (int i = 0; i < _processesMessages.Count; i++) {
                    context.Fire(_processesMessages[i]);
                }
            } catch (Exception ex) {
                this.HandleException(message, context, ex);
            }
        }

        public virtual void HandleException(string message, IContext context, Exception ex) {
            context.Errors.Add(new ErrorMessage(message, ex));
        }
    }
}
