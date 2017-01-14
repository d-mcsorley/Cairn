using System;

namespace Cairn {
    public class ValueParameter : IParameter {
        public string Name { get; set; }
        public Type ValueType { get; set; }        
        public object Value { get; set; }

        public ValueParameter(string name, Type valueType, object value) {
            this.Name = name;
            this.ValueType = valueType;            
            this.Value = value;
        }

        public object GetParameter(IContext context) {
            return this.Value;
        }
    }
}
