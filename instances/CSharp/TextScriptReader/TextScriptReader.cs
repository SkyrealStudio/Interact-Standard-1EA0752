using System;
using System.IO;
using System.Collections.Generic;

namespace Skyreal.Universal.Static
{
    /// <summary>
    /// Reader for text scripts.
    /// </summary>
    public static class TextScriptReader
    {
        /// <summary>
        /// The type of a text script element.
        /// </summary>
        public enum TextScriptElementType
        {
            /// <summary>
            /// Method element.
            /// </summary>
            Method,
            /// <summary>
            /// Parameter element.
            /// </summary>
            Parameter
        }
        /// <summary>
        /// A text script element.
        /// </summary>
        /// <param name="Type">The type of this element.</param>
        /// <param name="Content">The content of this element.</param>
        public readonly record struct TextScriptElement(TextScriptElementType Type, string Content);
        public static Queue<TextScriptElement> GetElements(string _FileName)
        {
            //throw new NotImplementedException();
            FileStream fs = File.OpenRead(_FileName);
            StreamReader sr = new(fs);
            Queue<TextScriptElement> ret = new(0);
            TextScriptElementType type = TextScriptElementType.Method;
            bool multiline = false;
            string content = "";
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine() ?? "";
                if (multiline)
                {
                    switch (type)
                    {
                    case TextScriptElementType.Method:
                        if (line.Length >= 6 && line[..6] == "------")
                            multiline = false;
                        break;
                    case TextScriptElementType.Parameter:
                        if (line.Length >= 6 && line[..6] == "======")
                            multiline = false;
                        break;
                    }
                    if (multiline)
                    {
                        if (content != "")
                            content += '\n';
                        content += line;
                        continue;
                    }
                }
                else if (line.Length >= 6 && line[..6] == "------")
                {
                    content = "";
                    type = TextScriptElementType.Method;
                    multiline = true;
                    continue;
                }
                else if (line.Length >= 6 && line[..6] == "======")
                {
                    content = "";
                    type = TextScriptElementType.Parameter;
                    multiline = true;
                    continue;
                }
                else if (line.Length >= 2 && line[..2] == "--" )
                {
                    type = TextScriptElementType.Method;
                    content = line[2..];
                }
                else if (line.Length >= 2 && line[..2] == "==")
                {
                    type = TextScriptElementType.Parameter;
                    content = line[2..];
                }
                else if (line.Length >= 1 && line[0] == '-')
                {
                    type = TextScriptElementType.Method;
                    content = line[1..].Trim();
                }
                else if (line.Length >= 1 && line[0] == '=')
                {
                    type = TextScriptElementType.Parameter;
                    content = line[1..].Trim();
                }
                else
                {
                    continue;
                }
                ret.Enqueue(new TextScriptElement(type, content));
            }
            sr.Close();
            fs.Close();
            sr.Dispose();
            fs.Dispose();
            return ret;
        }
    }
}