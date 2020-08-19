using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DSHA256 = System.Security.Cryptography.SHA256;

namespace Aegis.Blockchains.Algorithms
{
    /// <summary>
    /// SHA256 Hasher.
    /// </summary>
    public class SHA256 : IHasher
    {
        public static SHA256 Instance { get; } = new SHA256();

        /// <summary>
        /// Constructor.
        /// </summary>
        private SHA256() { }

        /// <summary>
        /// Name of this algorithm.
        /// </summary>
        public string Name => "sha256";

        /// <summary>
        /// Bit-Width of this algorithm.
        /// </summary>
        public ushort BitWidth => 256;

        /// <summary>
        /// Identity of this algorithm.
        /// </summary>
        public ushort Id { get; } = Utils.Fletcher16("sha256");

        /// <summary>
        /// Calculate a hash.
        /// </summary>
        public byte[] Calculate(byte[] Buffer, int Offset, int Size)
        {
            using (DSHA256 Algo = DSHA256.Create())
                return Algo.ComputeHash(Buffer, Offset, Size);
        }

        /// <summary>
        /// Calculate a hash.
        /// </summary>
        public byte[] Calculate(FileInfo File)
        {
            using (FileStream Stream = File.OpenRead())
            {
                using (DSHA256 Algo = DSHA256.Create())
                    return Algo.ComputeHash(Stream);
            }
        }
    }
}
