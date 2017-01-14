using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Cairn {
    public class XmlBehaviourService : IBehaviourService {
        private readonly XmlNode _configSectionNode;
        private static readonly string _defaultConfigSection = "cairn";
        private CairnApplication _application;

        public XmlBehaviourService() : this(_defaultConfigSection) { }

        public XmlBehaviourService(string sectionName) {
            this.Deserialize(sectionName);
        }

        private void Deserialize(string sectionName) {
            XmlNode xmlNode = (XmlNode)ConfigurationManager.GetSection(sectionName);
            if (xmlNode == null) {
                string message = string.Format(CultureInfo.InvariantCulture, "Could not find section '{0}' in the configuration file associated with this domain.", new object[]
				{
					sectionName
				});
                throw new ConfigurationErrorsException(message);
            }

            List<XmlNode> xmlNodes = new List<XmlNode>() { xmlNode };
            xmlNodes.AddRange(DeserializeIncludes(xmlNodes));

            CairnApplication cairnApplication = new CairnApplication();
            foreach (XmlNode node in xmlNodes) {
                DeserializeParameters(node, ref cairnApplication);
            }

            foreach (XmlNode node in xmlNodes) {
                DeserializeBehaviours(node, ref cairnApplication);
            }

            _application = cairnApplication;
        }

        protected virtual List<XmlNode> DeserializeIncludes(List<XmlNode> documentNodes) {
            List<XmlNode> includeNodes = new List<XmlNode>();

            foreach (XmlNode documentNode in documentNodes) {
                XmlNode configNode = documentNode.FirstChild.NodeType == XmlNodeType.XmlDeclaration ? documentNode.ChildNodes[1] : documentNode;

                foreach (XmlNode node in configNode) {
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

                        includeNodes.Add(xmlDoc);
                    }
                }
            }

            if (includeNodes.Count > 0)
                includeNodes.AddRange(DeserializeIncludes(includeNodes));

            return includeNodes;
        }

        protected virtual void DeserializeParameters(XmlNode documentNode, ref CairnApplication cairnApplication) {
            XmlNode configNode = documentNode.FirstChild.NodeType == XmlNodeType.XmlDeclaration ? documentNode.ChildNodes[1] : documentNode;

            foreach (XmlNode node in configNode) {
                if (node.Name == "parameters") {
                    // First pass
                    foreach (XmlNode parameterNode in node.ChildNodes) {
                        if (parameterNode.NodeType != XmlNodeType.Element) continue;

                        string name = parameterNode.Attributes["name"].Value;
                        ParameterType parameterType = (ParameterType)Enum.Parse(typeof(ParameterType), parameterNode.Attributes["parameterType"].Value);

                        if (parameterType == ParameterType.Context) {
                            Type type = Type.GetType(parameterNode.Attributes["type"].Value);
                            IParameter param = new ContextParameter(name, type);
                            cairnApplication.Parameters.Add(param.Name, param);
                            continue;
                        }

                        if (parameterType == ParameterType.Value) {
                            Type type = Type.GetType(parameterNode.Attributes["type"].Value);
                            TypeConverter converter = TypeDescriptor.GetConverter(type);
                            object value = converter.ConvertFrom(parameterNode.Attributes["value"].Value);
                            IParameter param = new ValueParameter(name, type, value);
                            cairnApplication.Parameters.Add(param.Name, param);
                            continue;
                        }                       

                        if (parameterType == ParameterType.Dictionary) {
                            Type keyType = Type.GetType(parameterNode.Attributes["keyType"].Value);
                            Type valueType = Type.GetType(parameterNode.Attributes["valueType"].Value);

                            if (!(keyType.IsValueType || keyType == typeof(string)) && !(valueType.IsValueType || valueType == typeof(string))) {
                                throw new ArgumentException("It's an arbitary restriction but dictionary key's and values must be either a Value Type or a String.");
                            }

                            Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                            IDictionary dictionary = (IDictionary)Activator.CreateInstance(dictionaryType);

                            TypeConverter keyConverter = TypeDescriptor.GetConverter(keyType);
                            TypeConverter valueConverter = TypeDescriptor.GetConverter(valueType);

                            foreach (XmlNode keyValueNode in parameterNode.ChildNodes) {
                                object itemKey = keyConverter.ConvertFrom(keyValueNode.Attributes["key"].Value);
                                object itemValue = valueConverter.ConvertFrom(keyValueNode.Attributes["value"].Value);
                                dictionary.Add(itemKey, itemValue);
                            }
                            continue;
                        }

                        if (parameterType == ParameterType.List) {
                            Type valueType = Type.GetType(parameterNode.Attributes["valueType"].Value);

                            if (!valueType.IsValueType && valueType != typeof(string)) {
                                throw new ArgumentException("It's an arbitary restriction but items must be either a Value Type or a String.");
                            }

                            Type listType = typeof(List<>).MakeGenericType(valueType);
                            IList list = (IList)Activator.CreateInstance(listType);

                            TypeConverter valueConverter = TypeDescriptor.GetConverter(valueType);

                            foreach (XmlNode itemNode in parameterNode.ChildNodes) {
                                object itemValue = valueConverter.ConvertFrom(itemNode.Attributes["value"].Value);
                                list.Add(itemValue);
                            }
                            continue;
                        }
                    }

                    // Second pass
                    foreach (XmlNode parameterNode in node.ChildNodes) {
                        if (parameterNode.NodeType != XmlNodeType.Element) continue;

                        string name = parameterNode.Attributes["name"].Value;
                        ParameterType parameterType = (ParameterType)Enum.Parse(typeof(ParameterType), parameterNode.Attributes["parameterType"].Value);

                        if (parameterType == ParameterType.Component) {
                            Type type = Type.GetType(parameterNode.Attributes["type"].Value);
                            Lifestyle lifestyle = (Lifestyle)Enum.Parse(typeof(Lifestyle), parameterNode.Attributes["lifestyle"].Value);

                            List<IParameter> argumentParamaters = new List<IParameter>();
                            foreach (XmlNode constructorArgumentNode in parameterNode.ChildNodes) {
                                if (constructorArgumentNode.NodeType == XmlNodeType.Element) {
                                    string argumentName = constructorArgumentNode.Name;

                                    if (constructorArgumentNode.Attributes["reference"] != null) {
                                        string reference = constructorArgumentNode.Attributes["reference"].Value;

                                        if (!cairnApplication.Parameters.ContainsKey(reference))
                                            throw new ArgumentException(String.Format(@"Reference: '{0}' has not been defined.", reference));

                                        Type argumentType = cairnApplication.Parameters[reference].ValueType;

                                        IParameter referenceParameter = new ReferenceParameter(argumentName, argumentType, reference);
                                        argumentParamaters.Add(referenceParameter);
                                        continue;
                                    }

                                    if (constructorArgumentNode.Attributes["value"] != null) {
                                        Type argumentType = Type.GetType(constructorArgumentNode.Attributes["type"].Value);
                                        var argumentTypeConverter = TypeDescriptor.GetConverter(argumentType);
                                        object value = argumentTypeConverter.ConvertFrom(constructorArgumentNode.Attributes["value"].Value);
                                        IParameter valueParameter = new ValueParameter(name, argumentType, value);
                                        argumentParamaters.Add(valueParameter);
                                        continue;
                                    }

                                }
                            }
                            IParameter param = new ComponentParameter(name, type, lifestyle, argumentParamaters);
                            cairnApplication.Parameters.Add(param.Name, param);
                            continue;
                        }
                    }
                }
            }
        }

        protected virtual void DeserializeBehaviours(XmlNode documentNode, ref CairnApplication cairnApplication) {
            XmlNode configNode = documentNode.FirstChild.NodeType == XmlNodeType.XmlDeclaration ? documentNode.ChildNodes[1] : documentNode;

            foreach (XmlNode elementNode in configNode) {
                if (elementNode.Name == "behaviours") {
                    foreach (XmlNode behaviourNode in elementNode.ChildNodes) {
                        if (behaviourNode.Name == "behaviour") {
                            IBehaviour behaviour = new Behaviour();

                            if (behaviourNode.Attributes["name"] != null) behaviour.Name = behaviourNode.Attributes["name"].Value;

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

                                        if (!cairnApplication.Parameters.ContainsKey(name)) throw new Exception(String.Format(@"Parameter: '{0}' has not been registered, please check the configuration.", name));
                                        behaviour.Parameters.Add(cairnApplication.Parameters[name]);
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
                            cairnApplication.Behaviours.Add(behaviour);
                        }
                    }
                }
            }
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

        public Context CreateContext() {
            return new Context(_application);
        }
    }
}
