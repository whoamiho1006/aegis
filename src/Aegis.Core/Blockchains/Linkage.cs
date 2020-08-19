using Aegis.Blockchains.Algorithms;
using System.Collections.Generic;

namespace Aegis.Blockchains
{
    /// <summary>
    /// Linkage between blocks.
    /// </summary>
    public struct Linkage
    {
        /// <summary>
        /// Hash Algorithm which is used for hashing this block.
        /// </summary>
        public IHasher Algorithm;

        /// <summary>
        /// Hash Algorithm which derived from previous block.
        /// </summary>
        public IHasher PrevAlgorithm;

        /// <summary>
        /// Hash bytes for this block.
        /// </summary>
        public byte[] Hash;

        /// <summary>
        /// Previous hash bytes for hardening the linkage.
        /// </summary>
        public byte[] PrevHash;
    }
}
