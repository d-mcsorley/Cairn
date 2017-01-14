using System;

namespace Cairn {
    public interface IParameter {
        string Name { get; set; }
        Type ValueType { get; set; }
        object GetParameter(IContext context);
    }
}
