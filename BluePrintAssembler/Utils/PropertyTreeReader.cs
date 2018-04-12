using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace BluePrintAssembler.Utils
{
    public class PropertyTreeReader
    {
        private System.IO.BinaryReader internalReader;

        public PropertyTreeReader(System.IO.Stream strm)
        {
            internalReader = new BinaryReader(strm, Encoding.UTF8, true);
        }

        public JToken ReadOne()
        {
            string ReadString()
            {
                //internalReader.ReadBytes(4);
                if (internalReader.ReadByte() == 1)
                    return string.Empty;
                else
                {
                    uint len = internalReader.ReadByte();
                    if (len == 255)
                        len = internalReader.ReadUInt32();
                    return Encoding.UTF8.GetString(internalReader.ReadBytes((int) len));
                }
            }

            var t = internalReader.ReadByte();
            var any = internalReader.ReadByte();
            switch (t)
            {
                case 0:
                    return JValue.CreateNull();
                case 1:
                    return new JValue(0 != internalReader.ReadByte());
                case 2:
                    return new JValue(internalReader.ReadDouble());
                case 3:
                    return new JValue(ReadString());
                case 4:
                case 5:
                {
                    var rv = new JObject();
                    uint len = internalReader.ReadUInt32();
                    for (uint i = 0; i < len; i++)
                    {
                        rv.Add(ReadString(), ReadOne());
                    }

                    return rv;
                }
            }

            return null;
        }
    }
}