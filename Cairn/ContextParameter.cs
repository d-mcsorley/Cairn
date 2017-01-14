using System;

namespace Cairn {
    public class ContextParameter : IParameter {
        public string Name { get; set; }
        public Type ValueType { get; set; }

        public ContextParameter(string name, Type valueType) {
            this.Name = name;
            this.ValueType = valueType;
        }

        public object GetParameter(IContext context) {
            object item;
            if (context.ControlState.TryGetValue(this.Name, out item) && item != null && item.GetType() == this.ValueType) {
                return item;
            }
            return null;
        }
    }
}
