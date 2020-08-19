using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Blockchains.Algorithms
{
    public class Utils
    {
        public static ushort Fletcher16(string Text)
        {
            byte[] TextBytes = Encoding.UTF8.GetBytes(Text);
            ushort Sum1 = 0, Sum2 = 0;

            for (int i = 0; i < TextBytes.Length; ++i)
            {
                Sum1 = (byte)((Sum1 + TextBytes[i]) % 255);
                Sum2 = (byte)((Sum2 + Sum1) % 255);
            }

            return (ushort)((Sum2 << 8) | Sum1);
        }
    }
}
