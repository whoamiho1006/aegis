using Aegis.Blockchains.Algorithms;
using System.IO;

namespace Aegis.Blockchains
{
    public partial class Block
    {
        public class Parameters
        {
            /// <summary>
            /// Hash algorithm for linkage between blocks.
            /// (Default: SHA256)
            /// </summary>
            public IHasher HashAlgorithm { get; set; } = SHA256.Instance;

            /// <summary>
            /// DSA Hash algorithm for verifying block database.
            /// </summary>
            public IHasher DSAHashAlgorithm { get; set; } = SHA256.Instance;

            /// <summary>
            /// DSA Algorithm for generating signature.
            /// </summary>
            public IDSA Algorithm { get; set; } = SECP256K1.Instance;

            /// <summary>
            /// Private Key for DSA Algorithm.
            /// </summary>
            public byte[] PrivateKey { get; set; }

            /// <summary>
            /// Previous Block.
            /// </summary>
            public Block Previous { get; set; }

            /// <summary>
            /// Block Target.
            /// </summary>
            public FileInfo Target { get; set; }

            /// <summary>
            /// Anti-Null Operation for preventing null reference.
            /// This makes necessary objects to be default.
            /// 
            /// But, If Public-Key or Signature is null, it will cause 
            /// NullReferenceException.
            /// </summary>
            /// <returns></returns>
            internal Parameters AntiNull()
            {
                if (Algorithm is null)
                    Algorithm = SECP256K1.Instance;

                if (HashAlgorithm is null)
                    HashAlgorithm = SHA256.Instance;

                if (PrivateKey is null)
                    PrivateKey = Algorithm.NewPrivateKey();

                return this;
            }
        }
    }
}
