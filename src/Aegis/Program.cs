using Aegis.Blockchains.Algorithms;
using Aegis.Blockchains;
using System;
using System.IO;
using System.Threading;

namespace Aegis
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] Pvt;

            if (File.Exists("this.pvt"))
                Pvt = File.ReadAllBytes("this.pvt");
            else Pvt = SECP256K1.Instance.NewPrivateKey();

            Blockchain Bc = new Blockchain(new DirectoryInfo("."), Pvt);
            
            

            while(true)
            {
                Block NewBlock = null;

                File.WriteAllText("hello.dt", DateTime.Now.Ticks + "");
                string BlockId = Bc.Enstack(new FileInfo("hello.dt"), ref NewBlock);

                Console.WriteLine("{0} - {1}, {2}", BlockId, 
                    Convert.ToBase64String(NewBlock.Linkage.Hash),
                    Convert.ToBase64String(NewBlock.Verification.Signature));
                Thread.Sleep(100);
            }
        }
    }
}
