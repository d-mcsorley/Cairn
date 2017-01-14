using System.Configuration;
using System.Xml;

namespace Cairn {
    public class CairnSectionHandler : IConfigurationSectionHandler {
        public object Create(object parent, object configContext, XmlNode section) {
            return section;
        }
    }
}
