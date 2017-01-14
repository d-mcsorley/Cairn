using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cairn {
    public interface IContext {
        ConcurrentDictionary<string, object> ControlState { get; }
        ConcurrentDictionary<string, object> ReturnData { get; }
        List<ErrorMessage> Errors { get; }
        CairnApplication Application { get; }
        void Fire(string message);
    }
}
