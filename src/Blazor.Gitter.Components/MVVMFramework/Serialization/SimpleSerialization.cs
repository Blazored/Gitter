using System.Collections.Generic;

//
//  2013-06-01  Mark Stega
//              Created
//              Provide a "dirt simple" serialization for Dictionary<string, string> types
//              by simply producing "|" delimited name-value pairs
//
//  2016-05-27  Mark Stega
//              Added capabilty to handle List<string> types
//

namespace Blazor.Gitter.Components.MVVMFramework.Serialization
{
    public static class SimpleSerialization
    {
        // Dictionary Serialization
        public static string SerializeDictionary(Dictionary<string, string> p_DictionaryToSerialize)
        {
            string serializedDictionary = "";
            foreach (KeyValuePair<string, string> kvp in p_DictionaryToSerialize)
            {
                serializedDictionary += "|" + EscapeString(kvp.Key) + "|" + EscapeString(kvp.Value);
            }
            return serializedDictionary;
        }

        // Dictionary Deserialization
        public static Dictionary<string, string> DeserializeDictionary(string p_SerializedDictionary)
        {
            Dictionary<string, string> returnDictionary = new Dictionary<string, string>();

            if ((p_SerializedDictionary != null) && (p_SerializedDictionary.Length > 0))
            {
                string[] val = p_SerializedDictionary.Split('|');

                // We start at index 1  as we have a null value at the front of the array
                for (int i = 1; i < val.Length; i += 2)
                {
                    returnDictionary.Add(val[i], DeEscapeString(val[i + 1]));
                }
            }

            return returnDictionary;
        }

        // List Serialization
        public static string SerializeList(List<string> p_ListToSerialize)
        {
            string serializedList = "";
            foreach (string val in p_ListToSerialize)
            {
                serializedList += "|" + EscapeString(val);
            }
            return serializedList;
        }

        // Dictionary Deserialization
        public static List<string> DeserializeList(string p_SerializedList)
        {
            List<string> returnList = new List<string>();

            if ((p_SerializedList != null) && (p_SerializedList.Length > 0))
            {
                string[] val = p_SerializedList.Split('|');

                // We start at index 1  as we have a null value at the front of the array
                for (int i = 1; i < val.Length; i++)
                {
                    returnList.Add(DeEscapeString(val[i]));
                }
            }

            return returnList;
        }

        // encode ampersands and vertical bars
        static public string EscapeString(string p_StringToEscape)
        {
            if (p_StringToEscape == null)
            {
                return "";
            }

            string escaped;

            escaped = p_StringToEscape.Replace("&", "&amp;");
            escaped = escaped.Replace("|", "&vbar;");

            return escaped;
        }

        // decode ampersands and vertical bars
        static public string DeEscapeString(string p_StringToDeEscape)
        {
            if (p_StringToDeEscape == null)
            {
                return "";
            }

            string deEscaped;

            deEscaped = p_StringToDeEscape.Replace("&vbar;", "|");
            deEscaped = deEscaped.Replace("&amp;", "&");

            return deEscaped;
        }

    }
}
