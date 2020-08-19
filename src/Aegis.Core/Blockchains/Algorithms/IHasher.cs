using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aegis.Blockchains.Algorithms
{
    public interface IHasher
    {
        /// <summary>
        /// Name of Hash Algorithm.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ID of Algorithm.
        /// </summary>
        ushort Id { get; }

        /// <summary>
        /// Bit-Width of this algorithm.
        /// </summary>
        ushort BitWidth { get; }

        /// <summary>
        /// Calculates the hashed bytes from buffer.
        /// </summary>
        byte[] Calculate(byte[] Buffer, int Offset, int Size);

        /// <summary>
        /// Calculate the hashed bytes from file.
        /// </summary>
        byte[] Calculate(FileInfo File);
    }
}
