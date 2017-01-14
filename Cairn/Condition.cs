using System;

namespace Cairn {
    public class Condition {
        public string Value { get; set; }
        public Type ValueType { get; set; }

        public Condition(string value, Type valueType) {
            this.Value = value;
            this.ValueType = valueType;
        }
    }
}
