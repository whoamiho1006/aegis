using Secp256k1Net;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Aegis.Blockchains.Algorithms
{
    public class SECP256K1 : IDSA
    {
        public static SECP256K1 Instance { get; } = new SECP256K1();

        /// <summary>
        /// Name of DSA Algorithm.
        /// </summary>
        public string Name => "secp256k1";

        /// <summary>
        /// ID of DSA Algorithm.
        /// </summary>
        public ushort Id { get; } = Utils.Fletcher16("secp256k1");

        /// <summary>
        /// Generate New Private Key.
        /// </summary>
        /// <returns></returns>
        public byte[] NewPrivateKey()
        {
            var privateKey = new byte[32];

            using (var secp256k1 = new Secp256k1())
            {
                using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
                {
                    do { rnd.GetBytes(privateKey); }
                    while (!secp256k1.SecretKeyVerify(privateKey));
                }
            }

            return privateKey;
        }

        /// <summary>
        /// Generate Public Key from Private Key.
        /// </summary>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        public byte[] ToPublicKey(byte[] PrivateKey)
        {
            var publicKey = new byte[64];

            using (var secp256k1 = new Secp256k1())
            {
                if (!secp256k1.PublicKeyCreate(publicKey, PrivateKey))
                    throw new ArgumentException(nameof(PrivateKey));
            }

            return publicKey;
        }

        /// <summary>
        /// This sign the buffer using public key.
        /// </summary>
        public byte[] Sign(byte[] PrivateKey, byte[] HashedData)
        {
            if (PrivateKey is null)
                throw new ArgumentNullException(nameof(PrivateKey));

            if (HashedData is null)
                throw new ArgumentNullException(nameof(HashedData));

            if (HashedData.Length != 32)
                Array.Resize(ref HashedData, 32);

            var signature = new byte[64];

            using (var secp256k1 = new Secp256k1())
            {
                if (!secp256k1.Sign(signature, HashedData, PrivateKey))
                    throw new ArgumentException(nameof(PrivateKey));
            }

            return signature;
        }

        /// <summary>
        /// This verify the signature using public key.
        /// </summary>
        public bool Verify(byte[] PublicKey, byte[] HashedData, byte[] Signature)
        {
            if (PublicKey is null)
                throw new ArgumentNullException(nameof(PublicKey));

            if (HashedData is null)
                throw new ArgumentNullException(nameof(HashedData));

            if (HashedData.Length != 32)
                Array.Resize(ref HashedData, 32);

            using (var secp256k1 = new Secp256k1())
                return secp256k1.Verify(Signature, HashedData, PublicKey);
        }
    }
}
