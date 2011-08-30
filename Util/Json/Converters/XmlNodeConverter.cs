/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion Header

namespace WebGrid.Util.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using WebGrid.Util.Json.Utilities;

    ///<summary>
    ///</summary>
    public class XmlNodeConverter : JsonConverter
    {
        #region Fields

        private const string CDataName = "#cdata-section";
        private const string CommentName = "#comment";
        private const string DeclarationName = "?xml";
        private const string SignificantWhitespaceName = "#significant-whitespace";
        private const string TextName = "#text";
        private const string WhitespaceName = "#whitespace";

        #endregion Fields

        #region Methods

        ///<summary>
        ///</summary>
        ///<param name="valueType"></param>
        ///<returns></returns>
        public override bool CanConvert(Type valueType)
        {
            return typeof(XmlNode).IsAssignableFrom(valueType);
        }

        ///<summary>
        ///</summary>
        ///<param name="reader"></param>
        ///<param name="objectType"></param>
        ///<returns></returns>
        ///<exception cref="JsonSerializationException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType)
        {
            // maybe have CanReader and a CanWrite methods so this sort of test wouldn't be necessary
              if (objectType != typeof(XmlDocument))
            throw new JsonSerializationException("XmlNodeConverter only supports deserializing XmlDocuments");

              XmlDocument document = new XmlDocument();
              XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
              reader.Read();

              DeserializeNode(reader, document, manager, document);

              return document;
        }

        ///<summary>
        ///</summary>
        ///<param name="writer"></param>
        ///<param name="value"></param>
        ///<exception cref="ArgumentException"></exception>
        public override void WriteJson(JsonWriter writer, object value)
        {
            XmlNode node = value as XmlNode;

              if (node == null)
            throw new ArgumentException("Value must be an XmlNode", "value");

              writer.WriteStartObject();
              SerializeNode(writer, node, true);
              writer.WriteEndObject();
        }

        private static string GetPrefix(string qualifiedName)
        {
            int colonPosition = qualifiedName.IndexOf(':');

              if ((colonPosition == -1 || colonPosition == 0) || (qualifiedName.Length - 1) == colonPosition)
            return string.Empty;
            return qualifiedName.Substring(0, colonPosition);
        }

        private static string GetPropertyName(XmlNode node)
        {
            switch (node.NodeType)
              {
            case XmlNodeType.Attribute:
              return "@" + node.Name;
            case XmlNodeType.CDATA:
              return CDataName;
            case XmlNodeType.Comment:
              return CommentName;
            case XmlNodeType.Element:
              return node.Name;
            case XmlNodeType.ProcessingInstruction:
              return "?" + node.Name;
            case XmlNodeType.XmlDeclaration:
              return DeclarationName;
            case XmlNodeType.SignificantWhitespace:
              return SignificantWhitespaceName;
            case XmlNodeType.Text:
              return TextName;
            case XmlNodeType.Whitespace:
              return WhitespaceName;
            default:
              throw new JsonSerializationException("Unexpected XmlNodeType when getting node name: " + node.NodeType);
              }
        }

        /// <summary>
        /// Checks if the attributeName is a namespace attribute.
        /// </summary>
        /// <param name="attributeName">Attributes name to test.</param>
        /// <param name="prefix">The attribute name prefix if it has one, otherwise an empty string.</param>
        /// <returns>True if attribute name is for a namespace attribute, otherwise false.</returns>
        private static bool IsNamespaceAttribute(string attributeName, out string prefix)
        {
            if (attributeName.StartsWith("xmlns", StringComparison.Ordinal))
              {
            switch (attributeName.Length)
            {
            case 5:
                prefix = string.Empty;
                return true;
            default:
                if (attributeName[5] == ':')
                {
                    prefix = attributeName.Substring(6, attributeName.Length - 6);
                    return true;
                }
                break;
            }
              }
              prefix = null;
              return false;
        }

        private void DeserializeNode(JsonReader reader, XmlDocument document, XmlNamespaceManager manager, XmlNode currentNode)
        {
            do
              {
            switch (reader.TokenType)
            {
              case JsonToken.PropertyName:
            string propertyName = reader.Value.ToString();
            reader.Read();

            if (reader.TokenType == JsonToken.StartArray)
            {
              while (reader.Read() && reader.TokenType != JsonToken.EndArray)
              {
                DeserializeValue(reader, document, manager, propertyName, currentNode);
              }
            }
            else
            {
              DeserializeValue(reader, document, manager, propertyName, currentNode);
            }
            break;
              //case JsonToken.String:
              //    DeserializeValue(reader, document, manager, TextName, currentNode);
              //    break;
              case JsonToken.EndObject:
              case JsonToken.EndArray:
            return;
              default:
            throw new JsonSerializationException("Unexpected JsonToken when deserializing node: " + reader.TokenType);
            }
              } while (reader.TokenType == JsonToken.PropertyName || reader.Read());
              // don't read if current token is a property. token was already read when parsing element attributes
        }

        private void DeserializeValue(JsonReader reader, XmlDocument document, XmlNamespaceManager manager, string propertyName, XmlNode currentNode)
        {
            switch (propertyName)
              {
            case TextName:
              currentNode.AppendChild(document.CreateTextNode(reader.Value.ToString()));
              break;
            case CDataName:
              currentNode.AppendChild(document.CreateCDataSection(reader.Value.ToString()));
              break;
            case WhitespaceName:
              currentNode.AppendChild(document.CreateWhitespace(reader.Value.ToString()));
              break;
            case SignificantWhitespaceName:
              currentNode.AppendChild(document.CreateSignificantWhitespace(reader.Value.ToString()));
              break;
            default:
              // processing instructions and the xml declaration start with ?
              if (propertyName[0] == '?')
              {
            if (propertyName == DeclarationName)
            {
              string version = null;
              string encoding = null;
              string standalone = null;
              while (reader.Read() && reader.TokenType != JsonToken.EndObject)
              {
                switch (reader.Value.ToString())
                {
                  case "@version":
                    reader.Read();
                    version = reader.Value.ToString();
                    break;
                  case "@encoding":
                    reader.Read();
                    encoding = reader.Value.ToString();
                    break;
                  case "@standalone":
                    reader.Read();
                    standalone = reader.Value.ToString();
                    break;
                  default:
                    throw new JsonSerializationException("Unexpected property name encountered while deserializing XmlDeclaration: " + reader.Value);
                }
              }

                if (version != null)
                {
                    XmlDeclaration declaration = document.CreateXmlDeclaration(version, encoding, standalone);
                    currentNode.AppendChild(declaration);
                }
            }
            else
            {
              XmlProcessingInstruction instruction = document.CreateProcessingInstruction(propertyName.Substring(1), reader.Value.ToString());
              currentNode.AppendChild(instruction);
            }
              }
              else
              {
            // deserialize xml element
            bool finishedAttributes = false;
            bool finishedElement = false;
            string elementPrefix = GetPrefix(propertyName);
            Dictionary<string, string> attributeNameValues = new Dictionary<string, string>();

            // a string token means the element only has a single text child
            if (reader.TokenType != JsonToken.String
              && reader.TokenType != JsonToken.Null
              && reader.TokenType != JsonToken.Boolean
              && reader.TokenType != JsonToken.Integer
              && reader.TokenType != JsonToken.Float
              && reader.TokenType != JsonToken.Date)
            {
              // read properties until first non-attribute is encountered
              while (!finishedAttributes && !finishedElement && reader.Read())
              {
                switch (reader.TokenType)
                {
                  case JsonToken.PropertyName:
                    string attributeName = reader.Value.ToString();

                    if (attributeName[0] == '@')
                    {
                      attributeName = attributeName.Substring(1);
                      reader.Read();
                      string attributeValue = reader.Value.ToString();
                      attributeNameValues.Add(attributeName, attributeValue);

                      string namespacePrefix;

                      if (IsNamespaceAttribute(attributeName, out namespacePrefix))
                      {
                        manager.AddNamespace(namespacePrefix, attributeValue);
                      }
                    }
                    else
                    {
                      finishedAttributes = true;
                    }
                    break;
                  case JsonToken.EndObject:
                    finishedElement = true;
                    break;
                  default:
                    throw new JsonSerializationException("Unexpected JsonToken: " + reader.TokenType);
                }
              }
            }

            // have to wait until attributes have been parsed before creating element
            // attributes may contain namespace info used by the element
            XmlElement element = (!string.IsNullOrEmpty(elementPrefix))
                    ? document.CreateElement(propertyName, manager.LookupNamespace(elementPrefix))
                    : document.CreateElement(propertyName);

            currentNode.AppendChild(element);

            // add attributes to newly created element
            foreach (KeyValuePair<string, string> nameValue in attributeNameValues)
            {
              string attributePrefix = GetPrefix(nameValue.Key);

              XmlAttribute attribute = (!string.IsNullOrEmpty(attributePrefix))
                      ? document.CreateAttribute(nameValue.Key, manager.LookupNamespace(attributePrefix))
                      : document.CreateAttribute(nameValue.Key);

              attribute.Value = nameValue.Value;

              element.SetAttributeNode(attribute);
            }

            switch (reader.TokenType)
            {
                case JsonToken.String:
                    element.AppendChild(document.CreateTextNode(reader.Value.ToString()));
                    break;
                case JsonToken.Integer:
                    element.AppendChild(document.CreateTextNode(XmlConvert.ToString((long)reader.Value)));
                    break;
                case JsonToken.Float:
                    element.AppendChild(document.CreateTextNode(XmlConvert.ToString((double)reader.Value)));
                    break;
                case JsonToken.Boolean:
                    element.AppendChild(document.CreateTextNode(XmlConvert.ToString((bool)reader.Value)));
                    break;
                case JsonToken.Date:
            #pragma warning disable 618,612
                    element.AppendChild(document.CreateTextNode(XmlConvert.ToString((System.DateTime)reader.Value)));
            #pragma warning restore 618,612
                    break;
                case JsonToken.Null:
                    break;
                default:
                    if (!finishedElement)
                    {
                        manager.PushScope();

                        DeserializeNode(reader, document, manager, element);

                        manager.PopScope();
                    }
                    break;
            }
              }
              break;
              }
        }

        private void SerializeGroupedNodes(JsonWriter writer, XmlNode node)
        {
            // group nodes together by name
              Dictionary<string, List<XmlNode>> nodesGroupedByName = new Dictionary<string, List<XmlNode>>();

              for (int i = 0; i < node.ChildNodes.Count; i++)
              {
            XmlNode childNode = node.ChildNodes[i];
            string nodeName = GetPropertyName(childNode);

            List<XmlNode> nodes;
            if (!nodesGroupedByName.TryGetValue(nodeName, out nodes))
            {
              nodes = new List<XmlNode>();
              nodesGroupedByName.Add(nodeName, nodes);
            }

            nodes.Add(childNode);
              }

              // loop through grouped nodes. write single name instances as normal,
              // write multiple names together in an array
              foreach (KeyValuePair<string, List<XmlNode>> nodeNameGroup in nodesGroupedByName)
              {
            List<XmlNode> groupedNodes = nodeNameGroup.Value;

            if (groupedNodes.Count == 1)
            {
              SerializeNode(writer, groupedNodes[0], true);
            }
            else
            {
              writer.WritePropertyName(nodeNameGroup.Key);
              writer.WriteStartArray();

              for (int i = 0; i < groupedNodes.Count; i++)
              {
            SerializeNode(writer, groupedNodes[i], false);
              }

              writer.WriteEndArray();
            }
              }
        }

        private void SerializeNode(JsonWriter writer, XmlNode node, bool writePropertyName)
        {
            switch (node.NodeType)
              {
            case XmlNodeType.Document:
            case XmlNodeType.DocumentFragment:
              SerializeGroupedNodes(writer, node);
              break;
            case XmlNodeType.Element:
              if (writePropertyName)
            writer.WritePropertyName(node.Name);

              if (CollectionUtils.IsNullOrEmpty(node.Attributes) && node.ChildNodes.Count == 1
                  && node.ChildNodes[0].NodeType == XmlNodeType.Text)
              {
            // write elements with a single text child as a name value pair
            writer.WriteValue(node.ChildNodes[0].Value);
              }
              else if (node.ChildNodes.Count == 0 && CollectionUtils.IsNullOrEmpty(node.Attributes))
              {
            // empty element
            writer.WriteNull();
              }
              else
              {
            writer.WriteStartObject();

            for (int i = 0; i < node.Attributes.Count; i++)
            {
              SerializeNode(writer, node.Attributes[i], true);
            }

            SerializeGroupedNodes(writer, node);

            writer.WriteEndObject();
              }

              break;
            case XmlNodeType.Comment:
              if (writePropertyName)
            writer.WriteComment(node.Value);
              break;
            case XmlNodeType.Attribute:
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.ProcessingInstruction:
            case XmlNodeType.Whitespace:
            case XmlNodeType.SignificantWhitespace:
              if (writePropertyName)
            writer.WritePropertyName(GetPropertyName(node));
              writer.WriteValue(node.Value);
              break;
            case XmlNodeType.XmlDeclaration:
              XmlDeclaration declaration = (XmlDeclaration)node;
              writer.WritePropertyName(GetPropertyName(node));
              writer.WriteStartObject();

              if (!string.IsNullOrEmpty(declaration.Version))
              {
            writer.WritePropertyName("@version");
            writer.WriteValue(declaration.Version);
              }
              if (!string.IsNullOrEmpty(declaration.Encoding))
              {
            writer.WritePropertyName("@encoding");
            writer.WriteValue(declaration.Encoding);
              }
              if (!string.IsNullOrEmpty(declaration.Standalone))
              {
            writer.WritePropertyName("@standalone");
            writer.WriteValue(declaration.Standalone);
              }

              writer.WriteEndObject();
              break;
            default:
              throw new JsonSerializationException("Unexpected XmlNodeType when serializing nodes: " + node.NodeType);
              }
        }

        #endregion Methods
    }
}