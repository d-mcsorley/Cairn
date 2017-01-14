using System;
using System.Collections.Generic;

namespace Cairn {
    public interface IBehaviour {
        string Name { get; set; }
        List<IParameter> Parameters { get; }
        List<string> RespondsToMessages { get; }
        List<string> ProcessesMessages { get; }
        List<FunctionWrapper> Functions { get; }
        List<ActionWrapper> Actions { get; }
        void Fire(string message, IContext context);
        void HandleException(string message, IContext context, Exception ex);
    }
}
