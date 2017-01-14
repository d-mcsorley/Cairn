using System;

namespace Cairn {
    public class ReferenceParameter : IParameter {
        public string Name { get; set; }
        public Type ValueType { get; set; }
        public string Reference { get; set; }

        public ReferenceParameter(string name, Type valueType, string reference) {
            this.Name = name;
            this.ValueType = valueType;
            this.Reference = reference;
        }

        public object GetParameter(IContext context) {
            return context.Application.Parameters[this.Reference].GetParameter(context);
        }
    }
}
