using System;
using System.Text;
using System.Collections.Generic;

namespace MacLibSe
{
    public static class EncodingHelper
    {
        private static List<EncodingInfo> _availableEncodings;
        public static List<EncodingInfo> GetEncodings()
        {
            if (_availableEncodings == null)
            {
                _availableEncodings = new List<EncodingInfo> ();
                foreach (EncodingInfo ei in Encoding.GetEncodings()) 
                {
                    try
                    {
                        if (ei.GetEncoding() != null)
                        {
                            _availableEncodings.Add(ei);
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
            return _availableEncodings;
        }
    }
}

