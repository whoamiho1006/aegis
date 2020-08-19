using Aegis.Blockchains.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aegis.Blockchains
{
    /// <summary>
    /// A Block which store data.
    /// </summary>
    public partial class Block
    {
        /// <summary>
        /// Parse X/Y written algorithm information.
        /// </summary>
        /// <param name="InString"></param>
        /// <param name="DSA"></param>
        /// <param name="Hasher"></param>
        private static void ParseAlgorithm(string InString, out IDSA DSA, out IHasher Hasher)
        {
            if (InString is null)
                throw new ArgumentNullException(nameof(InString));

            int Slash = InString.IndexOf('/');

            
            if (Slash < 0)
            {
                DSA = Registry.GetDSA(InString);
                Hasher = SHA256.Instance;
            }

            else if(Slash == 0)
            {
                DSA = SECP256K1.Instance;
                Hasher = Registry.GetHasher(InString.Substring(1));
            }

            else
            {
                DSA = Registry.GetDSA(InString.Substring(0, Slash));
                Hasher = Registry.GetHasher(InString.Substring(Slash + 1));
            }
        }

        /// <summary>
        /// Initialize a NEW Block from each parameters.
        /// </summary>
        private Block(Linkage Linkage, Verification Verification, FileInfo Target)
        {
            this.Linkage = Linkage;
            this.Verification = Verification;
            this.Target = Target;

            Hash();
        }

        /// <summary>
        /// Initialize a Block from Serializable.
        /// </summary>
        private Block(ref Serializable Serializable, FileInfo Target)
        {
            IDSA DSA;
            IHasher DSAHasher, Hasher;

            ParseAlgorithm(Serializable.Verification.Algorithm, out DSA, out DSAHasher);
            Hasher = Registry.GetHasher(Serializable.Current.Algorithm);

            Linkage = new Linkage
            {
                Algorithm = Hasher,
                Hash = Convert.FromBase64String(Serializable.Current.Hash),

                PrevAlgorithm = !(Serializable.Previous.Algorithm is null) ? 
                    Registry.GetHasher(Serializable.Previous.Algorithm) : null,

                PrevHash = !(Serializable.Previous.Hash is null) ?
                    Convert.FromBase64String(Serializable.Previous.Hash) : null
            };

            Verification = new Verification
            {
                HashAlgorithm = DSAHasher,
                Algorithm = DSA,

                Hash = Convert.FromBase64String(Serializable.Verification.Hash),
                Signature = Convert.FromBase64String(Serializable.Verification.Signature),
                PublicKey = Convert.FromBase64String(Serializable.Verification.PublicKey)
            };

            this.Target = Target;
        }

        /// <summary>
        /// Initialize a Block from Stream.
        /// </summary>
        private Block(Stream Stream, FileInfo Target)
        {
            using (BinaryReader Reader = new BinaryReader(Stream, Encoding.UTF8, true))
            {
                ushort DSAHasherId = Reader.ReadUInt16();
                ushort DSAId = Reader.ReadUInt16();
                ushort HasherId = Reader.ReadUInt16();

                ushort PubKeyLen = Reader.ReadUInt16();
                ushort SignatureLen = Reader.ReadUInt16();
                ushort DataHashLen = Reader.ReadUInt16();

                ushort PrevHasherId = Reader.ReadUInt16();
                ushort PrevHashLen = Reader.ReadUInt16();

                byte[] PubKey = Reader.ReadBytes(PubKeyLen);
                byte[] Signature = Reader.ReadBytes(SignatureLen);
                byte[] DataHash = Reader.ReadBytes(DataHashLen);

                byte[] PrevHash = Reader.ReadBytes(PrevHashLen);

                ushort HashLen = Reader.ReadUInt16();
                byte[] Hash = Reader.ReadBytes(HashLen);

                Linkage = new Linkage
                {
                    Algorithm = Registry.GetHasher(HasherId),
                    PrevAlgorithm = Registry.GetHasher(PrevHasherId),
                    Hash = Hash,
                    PrevHash = PrevHash
                };

                Verification = new Verification
                {
                    Algorithm = Registry.GetDSA(DSAId),
                    HashAlgorithm = Registry.GetHasher(DSAHasherId),
                    PublicKey = PubKey,
                    Signature = Signature,
                    Hash = DataHash
                };

                this.Target = Target;
            }
        }

        /// <summary>
        /// Serialize block to byte stream.
        /// </summary>
        /// <param name="Stream"></param>
        public int Serialize(Stream Stream)
        {
            int TotalLength = _Serialize(Stream);

            using (BinaryWriter Writer = new BinaryWriter(Stream, Encoding.UTF8, true))
            {
                byte[] Hash = this.Hash();

                Writer.Write((ushort)Hash.Length);
                Writer.Write(Hash);

                TotalLength += 2 + Hash.Length;
            }

            return TotalLength;
        }

        /// <summary>
        /// Serialize block to structure for generating JSON or XML, etc.
        /// </summary>
        /// <returns></returns>
        public Serializable Serialize()
        {
            Serializable Output = new Serializable();

            Output.Verification.Algorithm = string.Join(
                Verification.Algorithm.Name, "/", Verification.HashAlgorithm.Name);

            Output.Verification.PublicKey = Convert.ToBase64String(Verification.PublicKey, Base64FormattingOptions.None);
            Output.Verification.Signature = Convert.ToBase64String(Verification.Signature, Base64FormattingOptions.None);
            Output.Verification.Hash = Convert.ToBase64String(Verification.Hash, Base64FormattingOptions.None);

            Output.Previous.Hash = Linkage.PrevHash is null ? null : Convert.ToBase64String(Linkage.PrevHash, Base64FormattingOptions.None);
            Output.Previous.Algorithm = Linkage.PrevAlgorithm is null ? null : Linkage.PrevAlgorithm.Name;

            Output.Current.Hash = Linkage.Hash is null ? null : Convert.ToBase64String(Linkage.Hash, Base64FormattingOptions.None);
            Output.Current.Algorithm = Linkage.Algorithm is null ? null : Linkage.Algorithm.Name;

            return Output;
        }

        /// <summary>
        /// Serialize block to byte stream.
        /// </summary>
        /// <param name="Stream"></param>
        private int _Serialize(Stream Stream)
        {
            int TotalLength = sizeof(ushort) * 8;

            using (BinaryWriter Writer = new BinaryWriter(Stream, Encoding.UTF8, true))
            {
                Writer.Write(Verification.AntiNull().HashAlgorithm.Id);        // ushort
                Writer.Write(Verification.Algorithm.Id);                       // ushort
                Writer.Write(Linkage.Algorithm.Id);                            // ushort

                Writer.Write((ushort)Verification.PublicKey.Length);
                Writer.Write((ushort)Verification.Signature.Length);
                Writer.Write((ushort)Verification.Hash.Length);

                if (Linkage.PrevAlgorithm == null || Linkage.PrevHash == null)
                {
                    Writer.Write(ushort.MinValue);
                    Writer.Write(ushort.MinValue);
                }

                else
                {
                    Writer.Write(Linkage.PrevAlgorithm.Id);                    // ushort
                    Writer.Write((ushort)Linkage.PrevHash.Length);
                }

                Writer.Write(Verification.PublicKey);
                Writer.Write(Verification.Signature);
                Writer.Write(Verification.Hash);

                TotalLength += Verification.PublicKey.Length;
                TotalLength += Verification.Signature.Length;
                TotalLength += Verification.Hash.Length;

                if (Linkage.PrevAlgorithm != null && 
                    Linkage.PrevHash != null)
                {
                    Writer.Write(Linkage.PrevHash);
                    TotalLength += Linkage.PrevHash.Length;
                }

                Writer.Flush();
            }

            return TotalLength;
        }

        /// <summary>
        /// Calculate hash for the block if no hash calculated.
        /// </summary>
        /// <param name="Block"></param>
        /// <returns></returns>
        private byte[] Hash()
        {
            if (Linkage.Hash != null)
                return Linkage.Hash;

            else 
            {
                Linkage Linkage = this.Linkage;
                Linkage.Hash = HashImmediate();
                this.Linkage = Linkage;
            }

            return Linkage.Hash;
        }

        /// <summary>
        /// Calculate hash which for immediate.
        /// </summary>
        /// <returns></returns>
        private byte[] HashImmediate()
        {
            using (MemoryStream MemStream = new MemoryStream())
            {
                _Serialize(MemStream);

                int Length = (int)MemStream.Position;
                byte[] Buffer = MemStream.GetBuffer();

                return Linkage.Algorithm.Calculate(Buffer, 0, Length);
            }
        }

        /// <summary>
        /// Zero byte for non-target blocks.
        /// </summary>
        private static byte[] ZeroByte = new byte[0];

        /// <summary>
        /// Generate a block with given parameters.
        /// </summary>
        /// <returns></returns>
        public static Block Generate(Parameters Params)
        {
            byte[] Hash;

            if (Params.AntiNull().Target is null)
                Hash = Params.DSAHashAlgorithm.Calculate(ZeroByte, 0, 0);
            else Hash = Params.DSAHashAlgorithm.Calculate(Params.Target);

            Block NewBlock = new Block(
                new Linkage{
                    Algorithm = Params.HashAlgorithm,
                    PrevAlgorithm = Params.Previous != null ? Params.Previous.Linkage.Algorithm : null,
                    Hash = null,
                    PrevHash = Params.Previous != null ? Params.Previous.Linkage.Hash : null
                },
                new Verification{
                    Algorithm = Params.Algorithm,
                    HashAlgorithm = Params.DSAHashAlgorithm,
                    PublicKey = Params.Algorithm.ToPublicKey(Params.PrivateKey),
                    Signature = Params.Algorithm.Sign(Params.PrivateKey, Hash),
                    Hash = Hash
                }, Params.Target);

            return NewBlock;
        }

        /// <summary>
        /// Load a block from stream.
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        public static Block Deserialize(Stream Stream, FileInfo Target = null) => new Block(Stream, Target);

        /// <summary>
        /// Load a block from Serializable.
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        public static Block Deserialize(ref Serializable Serializable, FileInfo Target = null) => new Block(ref Serializable, Target);

        /// <summary>
        /// Verify the block is valid or not.
        /// </summary>
        /// <returns></returns>
        public bool Verify(bool IncludeData = false)
        {
            byte[] DataHash = Verification.Hash;

            if (IncludeData)
            {
                if (Target is null)
                {
                    DataHash = Verification
                        .HashAlgorithm.Calculate(ZeroByte, 0, 0);
                }

                else if (!Target.Exists)
                    return false;

                else DataHash = Verification
                    .HashAlgorithm.Calculate(Target);
            }

            if (Verification.Algorithm.Verify(
                Verification.PublicKey, DataHash,
                Verification.Signature))
            {
                byte[] Hash = HashImmediate();
                byte[] StoredHash = Linkage.Hash;

                if (StoredHash == null ||
                    StoredHash.Length != Hash.Length)
                    return false;

                for (int i = 0; i < StoredHash.Length; i++)
                {
                    if (Hash[i] != StoredHash[i])
                        return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Block Linkage.
        /// </summary>
        public Linkage Linkage { get; private set; }

        /// <summary>
        /// Block Verification.
        /// </summary>
        public Verification Verification { get; private set; }

        /// <summary>
        /// Target File.
        /// </summary>
        public FileInfo Target { get; private set; }
    }
}
