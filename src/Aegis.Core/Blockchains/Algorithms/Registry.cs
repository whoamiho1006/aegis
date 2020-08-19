using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Blockchains.Algorithms
{
    public class Registry
    {
        private static Dictionary<ushort, IHasher> m_Hashers
            = new Dictionary<ushort, IHasher>()
            {
                [SHA256.Instance.Id] = SHA256.Instance
            };

        private static Dictionary<ushort, IDSA> m_DSAs
            = new Dictionary<ushort, IDSA>()
            {
                [SECP256K1.Instance.Id] = SECP256K1.Instance
            };

        public static IDSA GetDSA(ushort Id)
        {
            if (m_DSAs.ContainsKey(Id))
                return m_DSAs[Id];

            return null;
        }

        public static IDSA GetDSA(string Name)
        {
            if (!((Name = !(Name is null) ? Name.ToLower() : null) is null))
            {
                foreach (IDSA Each in m_DSAs.Values)
                {
                    if (Each.Name == Name)
                        return Each;
                }
            }

            return null;
        }

        public static IHasher GetHasher(ushort Id)
        {
            if (m_Hashers.ContainsKey(Id))
                return m_Hashers[Id];

            return null;
        }

        public static IHasher GetHasher(string Name)
        {
            if (!((Name = !(Name is null) ? Name.ToLower() : null) is null))
            {
                foreach (IHasher Each in m_Hashers.Values)
                {
                    if (Each.Name == Name)
                        return Each;
                }
            }

            return null;
        }
    }
}
