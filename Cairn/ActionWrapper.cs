using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cairn {
    public class ActionWrapper {
        public Type DeclaringType { get; set; }
        int[] ParameterIndexes { get; set; }

        public ActionWrapper(Type declaringType) {
            this.DeclaringType = declaringType;
            this.ParameterIndexes = new int[0] { };
        }

        public ActionWrapper(Type declaringType, int[] inputs) {
            this.DeclaringType = declaringType;
            this.ParameterIndexes = inputs;
        }

        private Func<object[], bool> _condition;
        private Action<object[]> _action;

        public bool Condition(object[] parameters) {
            if (_condition == null)
                BuildAction();

            return _condition(parameters);
        }

        public void Execute(object[] parameters) {
            if (_action == null)
                BuildAction();

            _action(parameters);
        }

        private void BuildAction() {
            // Ensure the type implements IAction
            Type interfaceType = this.DeclaringType.GetInterfaces().FirstOrDefault(x => x.Name.Contains("IAction`") || x.Name.Contains("IAction"));
            if (interfaceType == null) throw new Exception("");

            var actionClass = Activator.CreateInstance(this.DeclaringType);
            Type[] actionGenericArgumants = interfaceType.GetGenericArguments();

            MethodInfo conditionMethodInfo = this.DeclaringType.GetMethod("Condition");
            MethodInfo executeMethodInfo = this.DeclaringType.GetMethod("Execute");

            // If there is no Condition method then the assumption is that this is an action with no parameters, i.e. there is nothing for 
            // a condition method to evaluate. 
            if (conditionMethodInfo == null) {
                MethodCallExpression actionMethod = Expression.Call(Expression.Constant(actionClass), executeMethodInfo);
                System.Action executeFunction = Expression.Lambda<Action>(actionMethod).Compile();
                _condition = p => { return true; };
                _action = x => {
                    executeFunction();
                };
                return;
            }

            // Extract the parameters
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            List<Expression> convertedParameters = new List<Expression>();

            for (int i = 0; i < actionGenericArgumants.Length; i++) {
                ParameterExpression parameter = Expression.Parameter(typeof(object));
                parameters.Add(parameter);
                convertedParameters.Add(Expression.Convert(parameter, actionGenericArgumants[i]));
            }

            MethodCallExpression conditionMethod = Expression.Call(Expression.Constant(actionClass), conditionMethodInfo, convertedParameters);
            MethodCallExpression executeMethodWithParameters = Expression.Call(Expression.Constant(actionClass), executeMethodInfo, convertedParameters);

            if (parameters.Count == 1) {
                Func<object, bool> condition = Expression.Lambda<Func<object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]]); };

                Action<object> executeFunction = Expression.Lambda<Action<object>>(executeMethodWithParameters, parameters).Compile();
                _action = p => { executeFunction(p[this.ParameterIndexes[0]]); };
                return;
            }
            if (parameters.Count == 2) {
                Func<object, object, bool> condition = Expression.Lambda<Func<object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]]); };

                Action<object, object> executeFunction = Expression.Lambda<Action<object, object>>(executeMethodWithParameters, parameters).Compile();
                _action = p => { executeFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]]); };
                return;
            }
            if (parameters.Count == 3) {
                Func<object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]]); };

                Action<object, object, object> executeFunction = Expression.Lambda<Action<object, object, object>>(executeMethodWithParameters, parameters).Compile();
                _action = p => { executeFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]]); };
                return;
            }
            if (parameters.Count == 4) {
                Func<object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]]); };

                Action<object, object, object, object> executeFunction = Expression.Lambda<Action<object, object, object, object>>(executeMethodWithParameters, parameters).Compile();
                _action = p => { executeFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]]); };
                return;
            }
            if (parameters.Count == 5) {
                Func<object, object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]]); };

                Action<object, object, object, object, object> executeFunction = Expression.Lambda<Action<object, object, object, object, object>>(executeMethodWithParameters, parameters).Compile();
                _action = p => { executeFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]]); };
                return;
            }
            if (parameters.Count == 6) {
                Func<object, object, object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]]); };

                Action<object, object, object, object, object, object> executeFunction = Expression.Lambda<Action<object, object, object, object, object, object>>(executeMethodWithParameters, parameters).Compile();
                _action = p => { executeFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]]); };
                return;
            }
            if (parameters.Count == 7) {
                Func<object, object, object, object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]], p[this.ParameterIndexes[6]]); };

                Action<object, object, object, object, object, object, object> executeFunction = Expression.Lambda<Action<object, object, object, object, object, object, object>>(executeMethodWithParameters, parameters).Compile();
                _action = p => { executeFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]], p[this.ParameterIndexes[6]]); };
                return;
            }
            if (parameters.Count == 8) {
                Func<object, object, object, object, object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]], p[this.ParameterIndexes[6]], p[this.ParameterIndexes[7]]); };

                Action<object, object, object, object, object, object, object, object> executeFunction = Expression.Lambda<Action<object, object, object, object, object, object, object, object>>(executeMethodWithParameters, parameters).Compile();
                _action = p => { executeFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]], p[this.ParameterIndexes[6]], p[this.ParameterIndexes[7]]); };
                return;
            }

        }
    }
}
