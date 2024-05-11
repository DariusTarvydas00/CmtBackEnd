using System.Xml;
using EntitiesProject.Client.Data.Language.English;
using EntitiesProject.Client.GeneralModels;
using FileWriterProject;

namespace AppLayer.FileWriterLayer;

public class FileWriter : IFileWriter
{
    private void WriteMessage(XmlWriter writer, object message)
    {
        switch (message)
        {
            case List<G1NameEntity> g1Names:
                WriteMessageIdCont(writer, g1Names, "g1_name");
                break;
            case List<G2NameEntity> g2Names:
                WriteMessageIdCont(writer, g2Names, "g2_name");
                break;
            case List<G3NameEntity> g3Names:
                WriteMessageIdCont(writer, g3Names, "g3_name");
                break;
            case List<G4NameEntity> g4Names:
                WriteMessageIdCont(writer, g4Names, "g3_name");
                break;
            case List<G5NameEntity> g5Names:
                WriteMessageIdCont(writer, g5Names, "g3_name");
                break;

            default:
                throw new ArgumentException($"Unsupported message type: {message.GetType().Name}");
        }
    }
    
    public void WriteCabalMsg(List<object> messages)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        string FilePath = "";

        using var writer = XmlWriter.Create(Path.Combine(FilePath,"cabal_msg.dec"), settings);
        writer.WriteStartElement("cabal_msg");

        foreach (var message in messages)
        {
            WriteMessage(writer, message);
        }

        writer.WriteEndElement();
    }
    
    private void WriteMessageIdCont<T>(XmlWriter writer, List<T> messages, string elementName) where T : BaseIdContM
    {
        writer.WriteStartElement(elementName);

        foreach (var msg in messages)
        {
            writer.WriteStartElement("msg");
            writer.WriteAttributeString("id", msg.MsgId.ToString());
            writer.WriteAttributeString("cont", msg.Cont);
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
    
     public List<object> ReadCabalMsg(string filePath)
        {
            var messages = new List<object>();
            using var reader = XmlReader.Create(filePath);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "class_model":
                            messages.AddRange(ReadMessages<ClassEntity>(reader));
                            break;
                        case "g1_name":
                            messages.AddRange(ReadMessages<G1NameEntity>(reader));
                            break;
                        case "g2_name":
                            messages.AddRange(ReadMessages<G2NameEntity>(reader));
                            break;
                        case "g3_name":
                            messages.AddRange(ReadMessages<G3NameEntity>(reader));
                            break;
                        case "g4_name":
                            messages.AddRange(ReadMessages<G4NameEntity>(reader));
                            break;
                        case "g5_name":
                            messages.AddRange(ReadMessages<G5NameEntity>(reader));
                            break;
                        default:
                            // Skip other elements
                            reader.Skip();
                            break;
                    }
                }
            }
            return messages;
        }

        private List<T> ReadMessages<T>(XmlReader reader) where T : BaseIdContM, new()
        {
            var messages = new List<T>();

            while (reader.Read())
            {
                if (reader is { NodeType: XmlNodeType.Element, Name: "msg" })
                {
                    var message = new T();
                    int.TryParse(reader.GetAttribute("id"), out int id);
                    var cont = reader.GetAttribute("cont");

                    message.MsgId = id;
                    message.Cont = cont;

                    messages.Add(message);
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == typeof(T).Name.ToLower())
                {
                    // End of messages for this type
                    break;
                }
            }

            return messages;
        }
}