using Aegis.Blockchains.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aegis.Blockchains
{
    /// <summary>
    /// Block chain class.
    /// the final implementation should be marked with `sealed` keyword.
    /// </summary>
    public partial class Blockchain
    {
        private DirectoryInfo m_Directory;
        private DirectoryInfo m_Blocks;

        private Stream m_BlockState;
        private byte[] m_PrivateKey;

        /// <summary>
        /// Initialize a new block chain or load block-chain state.
        /// </summary>
        /// <param name="Directory"></param>
        public Blockchain(DirectoryInfo Directory, byte[] PrivateKey)
        {
            BlockState State = new BlockState
            {
                High = 0,
                Low = 0
            };

            FileInfo Genesis = new FileInfo(Path.Combine(Directory.FullName, "genesis.bin"));

            m_Directory = Directory;
            m_PrivateKey = PrivateKey;

            m_BlockState = (new FileInfo(Path.Combine(Directory.FullName, "block.state")))
                .Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

            (m_Blocks = new DirectoryInfo(Path.Combine(Directory.FullName, "blocks"))).CreateIfNotExisted();

            if (!Genesis.Exists)
            {
                this.Genesis = Block.Generate(new Block.Parameters
                {
                    Algorithm = SECP256K1.Instance,
                    HashAlgorithm = SHA256.Instance,
                    DSAHashAlgorithm = SHA256.Instance,
                    PrivateKey = PrivateKey,
                    Previous = null,
                    Target = null
                });

                using (FileStream Stream = Genesis.Open(
                    FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    this.Genesis.Serialize(Stream);
                    Stream.Flush();
                }
            }

            else
            {
                using (FileStream Stream = Genesis.Open(
                    FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    this.Genesis = Block.Deserialize(Stream);
                }
            }

            try { State = LoadState(); }
            catch { SaveState(ref State, false); }

            // Load latest block.
            if (State.Low != 0 && State.High != 0)
            {
                if (State.Low > 0)
                    --State.Low;

                else
                {
                    --State.High;
                    State.Low = uint.MaxValue - 1;
                }

                Latest = _LoadBlock(ref State) ?? this.Genesis;
            }

            /*
             * If no block created, 
             * m_Latest will point genesis.
             */
            else Latest = this.Genesis;
        }

        /// <summary>
        /// Save block state when this instance disposing.
        /// </summary>
        ~Blockchain()
        {
            m_BlockState.Flush();
            m_BlockState.Dispose();
        }

        /// <summary>
        /// Get genesis block.
        /// </summary>
        public Block Genesis { get; }

        /// <summary>
        /// Get latest block.
        /// </summary>
        public Block Latest { get; private set; }

        /// <summary>
        /// Load a block.
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        private Block _LoadBlock(ref BlockState State)
        {
            try
            {
                string Low = State.Low.ToString("x8");
                string High = State.High.ToString("x8");

                FileInfo BlockFile = new FileInfo(Path.Combine(
                    m_Blocks.FullName, string.Join('-', High, Low, ".bin")));

                using (FileStream Stream = BlockFile.Open(
                    FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return Block.Deserialize(Stream);
                }
            }

            catch { }
            return null;
        }

        /// <summary>
        /// Push Data into block-chain.
        /// This returns block-id.
        /// </summary>
        public string Enstack(FileInfo Target, ref Block OutBlock)
        {
            if (Target is null)
                throw new ArgumentNullException(nameof(Target));

            string BlockId;

            lock (m_BlockState)
            {
                BlockState State = LoadState();
                FileInfo BlockFile;

                string Low = State.Low.ToString("x8");
                string High = State.High.ToString("x8");

                BlockId = string.Join('-', High, Low);
                BlockFile = new FileInfo(Path.Combine(m_Blocks.FullName, BlockId + ".bin"));

                OutBlock = Block.Generate(new Block.Parameters
                {
                    Algorithm = SECP256K1.Instance,
                    HashAlgorithm = SHA256.Instance,
                    DSAHashAlgorithm = SHA256.Instance,
                    PrivateKey = m_PrivateKey,
                    Previous = Latest,
                    Target = Target
                });

                using (FileStream Stream = BlockFile.Open(
                    FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    OutBlock.Serialize(Stream);
                    Stream.Flush();
                }

                SaveState(ref State, true);
                Latest = OutBlock;
            }

            return BlockId;
        }

        /// <summary>
        /// Load a block by Low-high pair.
        /// This will never make cache for any blocks.
        /// </summary>
        /// <param name="Low"></param>
        /// <param name="High"></param>
        /// <returns></returns>
        public Block Load(uint Low, uint High)
        {
            lock (m_BlockState)
            {
                BlockState State = LoadState();

                if (State.Low != Low && State.High != High)
                {
                    State.Low = Low; State.High = High;
                    return _LoadBlock(ref State);
                }

                return null;
            }
        }

        /// <summary>
        /// Load Blockstate.
        /// </summary>
        /// <returns></returns>
        private BlockState LoadState()
        {
            BlockState State = new BlockState();

            lock (m_BlockState)
            {
                m_BlockState.Position = 0;

                using (BinaryReader BR = new BinaryReader(
                    m_BlockState, Encoding.UTF8, true))
                {
                    State.High = BR.ReadUInt32();
                    State.Low = BR.ReadUInt32();
                }
            }

            return State;
        }

        /// <summary>
        /// Save Blockstate.
        /// </summary>
        /// <param name="State"></param>
        private void SaveState(ref BlockState State, bool Increase = false)
        {
            lock (m_BlockState)
            {
                m_BlockState.Position = 0;

                if (Increase && 
                   (++State.Low == uint.MaxValue))
                {
                    State.Low = uint.MinValue;
                    ++State.High;
                }

                using (BinaryWriter BW = new BinaryWriter(
                    m_BlockState, Encoding.UTF8, true))
                {
                    BW.Write(State.High);
                    BW.Write(State.Low);
                }
            }
        }
    }
}
