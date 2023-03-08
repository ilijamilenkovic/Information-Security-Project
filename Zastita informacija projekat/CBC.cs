using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zastita_informacija_projekat
{
    internal class CBC
    {
        public static byte[] Code(byte[] plainText, byte[] iVector)
        {
            if(iVector.Length == 0)
            {
                throw new Exception("Initialization vector must be set to at least one character");
            }

            if(iVector.Length < plainText.Length) {
                byte[] iVectorNew = new byte[plainText.Length];
                Array.Copy(iVector, iVectorNew, iVector.Length);
                for(int i = iVector.Length; i < plainText.Length; i++)
                {
                    iVectorNew[i] = iVector[i % iVector.Length];
                }
                iVector = iVectorNew;
            
            }

            byte[] coded = new byte[plainText.Length];

            for(int i = 0; i < plainText.Length; i++)
            {
                coded[i] = (byte)(plainText[i] ^ iVector[i]);
            }

            return coded;
            

        }
    }
}
