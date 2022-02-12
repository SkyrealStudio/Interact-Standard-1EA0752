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
        /// <summary>
        /// A method structure.
        /// </summary>
        /// <param name="MethodName">The name of the method.</param>
        /// <param name="Parameters">The parameters.</param>
        public record struct TextScriptMethod(string MethodName, List<string> Parameters);
        /// <summary>
        /// Get all the elements in a text script file.
        /// </summary>
        /// <param name="_FileStream">The stream of the file, will be disposed.</param>
        /// <returns>All the elements in a queue structure.</returns>
        public static Queue<TextScriptElement> GetElements(FileStream _FileStream)
        {
            StreamReader sr = new(_FileStream);
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
                else if (line.Length >= 2 && line[..2] == "--")
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
                ret.Enqueue(new(type, content));
            }
            sr.Close();
            _FileStream.Close();
            sr.Dispose();
            _FileStream.Dispose();
            return ret;
        }
        /// <summary>
        /// Get all the elements in a text script file.
        /// </summary>
        /// <param name="_FileName">The name of the file.</param>
        /// <returns>All the elements in a queue structure.</returns>
        public static Queue<TextScriptElement> GetElements(string _FileName)
        {
            return GetElements(File.OpenRead(_FileName));
        }
        /// <summary>
        /// Get all the elements organized as methods in a text script file.
        /// </summary>
        /// <param name="_FileStream">The stream of the file, will be disposed.</param>
        /// <returns>All the methods in a queue structure.</returns>
        public static Queue<TextScriptMethod> GetMethods(FileStream _FileStream)
        {
            Queue<TextScriptMethod> ret = new(0);
            Queue<TextScriptElement> datas = GetElements(_FileStream);
            while (datas.Count > 0)
            {
                TextScriptElement tse = datas.Dequeue();
                switch (tse.Type)
                {
                case TextScriptElementType.Method:
                    ret.Enqueue(new(tse.Content, new(0)));
                    break;
                case TextScriptElementType.Parameter:
                    try
                    {
                        ret.Last().Parameters.Add(tse.Content);
                    }
                    catch (InvalidOperationException) { }
                    break;
                }
            }
            return ret;
        }
        /// <summary>
        /// Get all the elements organized as methods in a text script file.
        /// </summary>
        /// <param name="_FileName">The name of the file.</param>
        /// <returns>All the methods in a queue structure.</returns>
        public static Queue<TextScriptMethod> GetMethods(string _FileName)
        {
            return GetMethods(File.OpenRead(_FileName));
        }
    }
}