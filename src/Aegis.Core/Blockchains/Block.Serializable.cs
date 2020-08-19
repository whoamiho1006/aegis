using Newtonsoft.Json;

namespace Aegis.Blockchains
{
    public partial class Block
    {
        /// <summary>
        /// Serializable Block Data.
        /// This is for serializing a block into JSON/BSON.
        /// </summary>
        public struct Serializable
        {
            public struct BlockReference
            {
                [JsonProperty("algorithm")]
                public string Algorithm;

                [JsonProperty("hash")]
                public string Hash;
            }

            public struct DSAInfo
            {
                [JsonProperty("algorithm")]
                public string Algorithm;

                [JsonProperty("pub-key")]
                public string PublicKey;

                [JsonProperty("signature")]
                public string Signature;

                [JsonProperty("hash")]
                public string Hash;
            }

            [JsonProperty("verification")]
            public DSAInfo Verification;

            [JsonProperty("previous")]
            public BlockReference Previous;

            [JsonProperty("current")]
            public BlockReference Current;
        }
    }
}
