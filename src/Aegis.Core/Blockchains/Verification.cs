using Aegis.Blockchains.Algorithms;
using System;

namespace Aegis.Blockchains
{
    /// <summary>
    /// Verification of a block.
    /// </summary>
    public struct Verification
    {
        /// <summary>
        /// DSA Algorithm to generate signature.
        /// </summary>
        public IDSA Algorithm;

        /// <summary>
        /// Hash Algorithm which is used for calculating signature.
        /// </summary>
        public IHasher HashAlgorithm;

        /// <summary>
        /// Public Key.
        /// </summary>
        public byte[] PublicKey;

        /// <summary>
        /// Signature.
        /// </summary>
        public byte[] Signature;

        /// <summary>
        /// Data Hash
        /// </summary>
        public byte[] Hash;

        /// <summary>
        /// Anti-Null Operation for preventing null reference.
        /// This makes necessary objects to be default.
        /// 
        /// But, If Public-Key or Signature is null, it will cause 
        /// NullReferenceException.
        /// </summary>
        /// <returns></returns>
        internal Verification AntiNull()
        {
            if (Algorithm is null)
                Algorithm = SECP256K1.Instance;

            if (HashAlgorithm is null)
                HashAlgorithm = SHA256.Instance;

            if (Hash is null)
                Hash = new byte[HashAlgorithm.BitWidth / 8];

            else if (Hash.Length < HashAlgorithm.BitWidth / 8)
                Array.Resize(ref Hash, HashAlgorithm.BitWidth / 8);

            if (Signature is null)
                throw new NullReferenceException(nameof(Signature));

            if (PublicKey is null)
                throw new NullReferenceException(nameof(Signature));

            return this;
        }
    }

}
