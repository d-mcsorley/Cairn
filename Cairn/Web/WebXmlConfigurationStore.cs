using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Cairn.Web {
    public class WebXmlConfigurationStore : IConfigurationStore {
        private readonly XmlNode _configSectionNode;

        public WebXmlConfigurationStore() : this("cairn") { }

        public WebXmlConfigurationStore(string sectionName) {
            XmlNode xmlNode = (XmlNode)ConfigurationManager.GetSection(sectionName);
            if (xmlNode == null) {
                string message = string.Format(CultureInfo.InvariantCulture, "Could not find section '{0}' in the configuration file associated with this domain.", new object[]
				{
					sectionName
				});
                throw new ConfigurationErrorsException(message);
            }
            this._configSectionNode = xmlNode;
        }

        public virtual IEnumerable<IBehaviour> GetBehaviours() {
            return this.Deserialize(_configSectionNode);
        }

        protected virtual List<IBehaviour> Deserialize(XmlNode sectionNode) {
            List<IBehaviour> behaviours = new List<IBehaviour>();
            foreach (XmlNode node in sectionNode) {
                if (node.Name == "include") {
                    string uriString = node.Attributes["uri"].Value;

                    Uri uri;
                    if (!Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri)) {
                        throw new Exception(String.Format(@"'{0}' is not a valid URI.", uriString));
                    } else if (!uri.IsFile) {
                        throw new Exception(String.Format(@"'{0}' is not a file.", uriString));
                    }

                    string filePath = uri.LocalPath;
                    if (uri.IsUnc) {
                        string defaultBasePath = AppDomain.CurrentDomain.BaseDirectory;
                        filePath = string.Format(@"{0}{1}", defaultBasePath, uri.LocalPath.Replace(@"\\", ""));
                    }

                    if (!File.Exists(filePath)) {
                        throw new Exception(String.Format(@"'{0}' does not exist.", filePath));
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    using (StreamReader streamReader = new StreamReader(filePath)) {
                        xmlDoc.Load(streamReader);
                    }

                    if (xmlDoc.FirstChild.NodeType != XmlNodeType.XmlDeclaration)
                        behaviours.AddRange(this.Deserialize(xmlDoc.FirstChild));
                    else
                        behaviours.AddRange(this.Deserialize(xmlDoc.ChildNodes[1]));

                }

                if (node.Name == "concern") {
                    foreach (XmlNode behaviourNode in node.ChildNodes) {
                        if (behaviourNode.Name != "behaviour") continue;
                        behaviours.Add(this.DeserialiseBehaviour(behaviourNode));
                    }
                }
            }
            return behaviours;
        }

        protected virtual IBehaviour DeserialiseBehaviour(XmlNode behaviourNode) {
            IBehaviour behaviour = new Behaviour();

            foreach (XmlNode node in behaviourNode) {
                if (node.Name == "respondsTo") {
                    foreach (XmlNode messageNode in node.ChildNodes) {
                        if (messageNode.Name != "message") continue;
                        behaviour.RespondsToMessages.Add(messageNode.InnerText);
                    }
                    continue;
                }
                if (node.Name == "processes") {
                    foreach (XmlNode messageNode in node.ChildNodes) {
                        if (messageNode.Name != "message") continue;
                        behaviour.ProcessesMessages.Add(messageNode.InnerText);
                    }
                    continue;
                }
                if (node.Name == "parameters") {
                    foreach (XmlNode parameterNode in node.ChildNodes) {
                        if (parameterNode.Name != "parameter") continue;

                        string name = parameterNode.Attributes["name"].Value;
                        Type type = Type.GetType(parameterNode.Attributes["type"].Value);
                        behaviour.Parameters.Add(new Parameter(name, type));
                    }
                    continue;
                }
                if (node.Name == "actions") {
                    foreach (XmlNode actionNode in node.ChildNodes) {
                        if (actionNode.Name != "action") continue;
                        behaviour.Actions.Add(this.DeserializeAction(actionNode));
                    }
                    continue;
                }
                if (node.Name == "functions") {
                    foreach (XmlNode functionNode in node.ChildNodes) {
                        if (functionNode.Name != "function") continue;
                        behaviour.Functions.Add(this.DeserializeFunction(functionNode));
                    }
                    continue;
                }

            }
            return behaviour;
        }

        protected virtual ActionWrapper DeserializeAction(XmlNode actionNode) {
            Type type = Type.GetType(actionNode.Attributes["type"].Value);
            int[] parameterIndex = new int[0];

            foreach (XmlNode node in actionNode) {
                if (node.Name == "inputs") {
                    parameterIndex = new int[node.ChildNodes.Count];

                    for (int i = 0; i < node.ChildNodes.Count; i++) {
                        if (node.ChildNodes[i].Name != "parameterIndex") continue;
                        parameterIndex[i] = Convert.ToInt32(node.ChildNodes[i].InnerText);
                    }
                    continue;
                }
            }

            ActionWrapper action = new ActionWrapper(type, parameterIndex);
            return action;
        }

        protected virtual FunctionWrapper DeserializeFunction(XmlNode functionNode) {
            Type type = Type.GetType(functionNode.Attributes["type"].Value);
            int[] parameterIndex = new int[0];
            string outputName = "";
            bool returnAsResult = false;

            foreach (XmlNode node in functionNode) {
                if (node.Name == "inputs") {
                    parameterIndex = new int[node.ChildNodes.Count];

                    for (int i = 0; i < node.ChildNodes.Count; i++) {
                        if (node.ChildNodes[i].Name != "parameterIndex") continue;
                        parameterIndex[i] = Convert.ToInt32(node.ChildNodes[i].InnerText);
                    }

                    continue;
                }
                if (node.Name == "output") {
                    outputName = node.InnerText;

                    if (node.Attributes["returnAsResult"] != null)
                        returnAsResult = Convert.ToBoolean(node.Attributes["returnAsResult"].Value);
                }
            }

            FunctionWrapper function = new FunctionWrapper(type, parameterIndex, outputName, returnAsResult);
            return function;
        }

        protected virtual void DesirializeInclude(XmlNode node) {
            throw new NotImplementedException();
        }
    }
}
