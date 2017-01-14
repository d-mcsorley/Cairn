using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Cairn {
    public class ComponentParameter : IParameter {
        public string Name { get; set; }
        public Type ValueType { get; set; }
        public Lifestyle Lifestyle { get; set; }
        public List<IParameter> Parameters { get; set; }

        private delegate object ObjectActivator(params object[] args);
        private ObjectActivator _createdActivator;
        private static Dictionary<string, object> _singletonObjects = new Dictionary<string,object>();

        public ComponentParameter(string name, Type valueType, Lifestyle lifestyle, List<IParameter> parameters) {
            this.Name = name;
            this.ValueType = valueType;            
            this.Lifestyle = lifestyle;
            this.Parameters = parameters;

            ConstructorInfo[] ctors = valueType.GetConstructors();

            for (int i = 0; i < ctors.Length; i++) {
                ParameterInfo[] pters = ctors[i].GetParameters();

                bool isConstructor = true;
                if (pters.Length == parameters.Count) {
                    for (int j = 0; j < pters.Length; j++) {
                        if (pters[j].ParameterType != parameters[j].ValueType) {
                            isConstructor = false;
                        }
                    }
                } else { isConstructor = false; }

                if (isConstructor) {
                    _createdActivator = GetActivator(ctors[i]);
                    break;
                }
            }

            if (this._createdActivator == null)
                throw new ArgumentException(String.Format(@"Could not match a constuctor for component: {0}.", name));        
        }

        public object GetParameter(IContext context) {
            if (this.Lifestyle == Cairn.Lifestyle.Singleton) {
                if (!_singletonObjects.ContainsKey(this.Name)) {
                    _singletonObjects.Add(this.Name, this._createdActivator(this.GetArguments(context)));
                }
                return _singletonObjects[this.Name];
            }

            return this._createdActivator(this.GetArguments(context));
        }

        private object[] GetArguments(IContext context) {
            object[] arguments = new object[this.Parameters.Count];

            for (int i = 0; i < this.Parameters.Count; i++) {
                arguments[i] = this.Parameters[i].GetParameter(context);
            }

            return arguments;
        }

        private ObjectActivator GetActivator(ConstructorInfo ctor) {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            // Create a single param of type object[]
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            // Pick each arg from the params array and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++) {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            // Make a NewExpression that calls the ctor with the args we just created.
            NewExpression newExp = Expression.New(ctor, argsExp);

            // Create a lambda with the New Expression as body and our param object[] as arg.
            LambdaExpression lambda =
                Expression.Lambda(typeof(ObjectActivator), newExp, param);

            // Compile it.
            ObjectActivator compiled = (ObjectActivator)lambda.Compile();
            return compiled;
        }
    }
}
