namespace Aegis.Blockchains.Algorithms
{
    public interface IDSA
    {
        /// <summary>
        /// Name of DSA Algorithm.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ID of DSA Algorithm.
        /// </summary>
        ushort Id { get; }

        /// <summary>
        /// Generate New Private Key.
        /// </summary>
        /// <returns></returns>
        byte[] NewPrivateKey();

        /// <summary>
        /// Generate Public Key from Private Key.
        /// </summary>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        byte[] ToPublicKey(byte[] PrivateKey);

        /// <summary>
        /// This sign the buffer using public key.
        /// </summary>
        byte[] Sign(byte[] PrivateKey, byte[] HashedData);

        /// <summary>
        /// This verify the signature using public key.
        /// </summary>
        bool Verify(byte[] PublicKey, byte[] HashedData, byte[] Signature);
    }
}
