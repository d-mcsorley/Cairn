using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cairn {
    public class FunctionWrapper {
        public Type DeclaringType { get; set; }                
        int[] ParameterIndexes { get; set; }
        public string OutputName { get; set; }
        public bool ReturnAsResult { get; set; }

        public FunctionWrapper(Type declaringType, string outputName) {
            this.DeclaringType = declaringType;
            this.ParameterIndexes = new int[0] { };
            this.OutputName = outputName;
            this.ReturnAsResult = false;
        }

        public FunctionWrapper(Type declaringType, int[] inputs, string outputName) {
            this.DeclaringType = declaringType;
            this.ParameterIndexes = inputs;
            this.OutputName = outputName;
            this.ReturnAsResult = false;
        }

        public FunctionWrapper(Type declaringType, int[] inputs, string outputName, bool returnAsResult) {
            this.DeclaringType = declaringType;
            this.ParameterIndexes = inputs;
            this.OutputName = outputName;
            this.ReturnAsResult = returnAsResult;
        }

        private Func<object[], bool> _condition;
        private Func<object[], object> _function;

        public bool Condition(object[] parameters) {
            if (_condition == null)
                BuildFunction();

            return _condition(parameters);
        }

        public object Return(object[] parameters) {
            if (_function == null)
                BuildFunction();

            return _function(parameters);
        }

        private void BuildFunction() {
            // Ensure the type implements IFunction
            Type interfaceType = this.DeclaringType.GetInterfaces().FirstOrDefault(x => x.Name.Contains("IFunction`"));
            if (interfaceType == null) throw new Exception("");

            var functionClass = Activator.CreateInstance(this.DeclaringType);
            Type[] functionGenericArgumants = interfaceType.GetGenericArguments();

            MethodInfo conditionMethodInfo = this.DeclaringType.GetMethod("Condition");
            MethodInfo returnMethodInfo = this.DeclaringType.GetMethod("Return");

            // If there is no Condition method then the assumption is that this is a function with no parameters, i.e. there is nothing for 
            // a condition method to evaluate. 
            if (conditionMethodInfo == null) {
                MethodCallExpression returnMethod = Expression.Call(Expression.Constant(functionClass), returnMethodInfo);
                UnaryExpression returnMethodAsObject = Expression.Convert(returnMethod, typeof(object));
                Func<object> returnFunction = Expression.Lambda<Func<object>>(returnMethodAsObject).Compile();
                _condition = p => { return true; };
                _function = x => {
                    return returnFunction();
                };
                return;
            }

            // Extract the parameters
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            List<Expression> convertedParameters = new List<Expression>();

            for (int i = 0; i < functionGenericArgumants.Length - 1; i++) {
                ParameterExpression parameter = Expression.Parameter(typeof(object));
                parameters.Add(parameter);
                convertedParameters.Add(Expression.Convert(parameter, functionGenericArgumants[i]));
            }

            MethodCallExpression conditionMethod = Expression.Call(Expression.Constant(functionClass), conditionMethodInfo, convertedParameters);
            MethodCallExpression returnMethodWithParameters = Expression.Call(Expression.Constant(functionClass), returnMethodInfo, convertedParameters);

            UnaryExpression returnMethodWithParametersAsObject = Expression.Convert(returnMethodWithParameters, typeof(object));

            // There is probably a more eloquent way to achieve what is happening below but this code runs once before being cached I'm not inclined to find it.
            if (parameters.Count == 1) {
                Func<object, bool> condition = Expression.Lambda<Func<object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]]); };

                Func<object, object> returnFunction = Expression.Lambda<Func<object, object>>(returnMethodWithParametersAsObject, parameters).Compile();
                _function = p => { return returnFunction(p[this.ParameterIndexes[0]]); };
                return;
            }
            if (parameters.Count == 2) {
                Func<object, object, bool> condition = Expression.Lambda<Func<object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]]); };

                Func<object, object, object> returnFunction = Expression.Lambda<Func<object, object, object>>(returnMethodWithParametersAsObject, parameters).Compile();
                _function = p => { return returnFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]]); };
                return;
            }
            if (parameters.Count == 3) {
                Func<object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]]); };

                Func<object, object, object, object> returnFunction = Expression.Lambda<Func<object, object, object, object>>(returnMethodWithParametersAsObject, parameters).Compile();
                _function = p => { return returnFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]]); };
                return;
            }
            if (parameters.Count == 4) {
                Func<object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]]); };

                Func<object, object, object, object, object> returnFunction = Expression.Lambda<Func<object, object, object, object, object>>(returnMethodWithParametersAsObject, parameters).Compile();
                _function = p => { return returnFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]]); };
                return;
            }
            if (parameters.Count == 5) {
                Func<object, object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]]); };

                Func<object, object, object, object, object, object> returnFunction = Expression.Lambda<Func<object, object, object, object, object, object>>(returnMethodWithParametersAsObject, parameters).Compile();
                _function = p => { return returnFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]]); };
                return;
            }
            if (parameters.Count == 6) {
                Func<object, object, object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]]); };

                Func<object, object, object, object, object, object, object> returnFunction = Expression.Lambda<Func<object, object, object, object, object, object, object>>(returnMethodWithParametersAsObject, parameters).Compile();
                _function = p => { return returnFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]]); };
                return;
            }
            if (parameters.Count == 7) {
                Func<object, object, object, object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]], p[this.ParameterIndexes[6]]); };

                Func<object, object, object, object, object, object, object, object> returnFunction = Expression.Lambda<Func<object, object, object, object, object, object, object, object>>(returnMethodWithParametersAsObject, parameters).Compile();
                _function = p => { return returnFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]], p[this.ParameterIndexes[6]]); };
                return;
            }
            if (parameters.Count == 8) {
                Func<object, object, object, object, object, object, object, object, bool> condition = Expression.Lambda<Func<object, object, object, object, object, object, object, object, bool>>(conditionMethod, parameters).Compile();
                _condition = p => { return condition(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]], p[this.ParameterIndexes[6]], p[this.ParameterIndexes[7]]); };

                Func<object, object, object, object, object, object, object, object, object> returnFunction = Expression.Lambda<Func<object, object, object, object, object, object, object, object, object>>(returnMethodWithParametersAsObject, parameters).Compile();
                _function = p => { return returnFunction(p[this.ParameterIndexes[0]], p[this.ParameterIndexes[1]], p[this.ParameterIndexes[2]], p[this.ParameterIndexes[3]], p[this.ParameterIndexes[4]], p[this.ParameterIndexes[5]], p[this.ParameterIndexes[6]], p[this.ParameterIndexes[7]]); };
                return;
            }

        }
    }
}
