using System.Collections.Generic;

namespace Cairn {
    public interface IConfigurationStore {
        IEnumerable<IBehaviour> GetBehaviours();
        //IEnumerable<Parameter> GetParameters();
        //void AddBehaviour(Behaviour behaviour);
        //void AddParameter(Parameter parameter);
    }
}
