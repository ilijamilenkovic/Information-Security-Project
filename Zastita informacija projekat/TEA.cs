using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Zastita_informacija_projekat
{
    internal class TEA
    {
        public TEA()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //v[0]-v[1] data, k[0]-k[3] key
        public static void CodeBlock(UInt32[] v, UInt32[] k)
        {
            UInt32 v0 = v[0], v1 = v[1], sum = 0, i;
            UInt32 delta = 0x9E3779B9;
            UInt32 k0 = k[0], k1 = k[1], k2 = k[2], k3 = k[3];
            for (i = 0; i < 32; i++)
            {
                sum += delta;
                v0 += ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                v1 += ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
            }
            v[0] = v0; v[1] = v1;

        }

        public static void DecodeBlock(UInt32[] v, UInt32[] k)
        {
            UInt32 v0 = v[0], v1 = v[1], i;
            UInt32 delta = 0x9E3779B9;
            UInt32 k0 = k[0], k1 = k[1], k2 = k[2], k3 = k[3];
            UInt32 sum = delta << 5;
            for (i = 0; i < 32; i++)
            {
                v1 -= ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
                v0 -= ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                sum -= delta;
            }
            v[0] = v0; v[1] = v1;

        }

        public static string Decode(string encryptedText, string key, bool cbc=false, string initializationVector=" ")
        {
            if (cbc && initializationVector == " ")
            {
                throw new ArgumentException("Argument initializationVector must be set if cbc is true");

            }


            encryptedText = encryptedText.Replace('�', '\0');
            UInt32[] keyBlock = TEA.ConvertStringToUIntKey(key);
            List<byte> dataBytes = new List<byte>();
            byte[] initializationVectorBytes = Encoding.Latin1.GetBytes(initializationVector);
            if(initializationVectorBytes.Length < 8)
            {
                byte[] temp= new byte[8];
                Array.Copy(initializationVectorBytes, temp, initializationVectorBytes.Length);
                for(int i = initializationVectorBytes.Length; i<8; i++)
                {
                    temp[i] = initializationVectorBytes[i % initializationVectorBytes.Length];
                }
                initializationVectorBytes = temp;
            }

            UInt32[] dataBlock = new UInt32[2];

            //converting initialization vector
            byte[] ivBlock0 = new byte[4];
            byte[] ivBlock1 = new byte[4];
            Array.Copy(initializationVectorBytes, 0, ivBlock0, 0, 4);
            Array.Copy(initializationVectorBytes, 4, ivBlock1, 0, 4);
            
            byte[] ivBlockTemp0 = new byte[4];
            byte[] ivBlockTemp1 = new byte[4];
            for (int i = 0; i < encryptedText.Length; i += 8)
            {
                dataBlock[0] = ConvertStringToUInt(encryptedText.Substring(i, 4));
                ///
                dataBlock[1] = 0;
                if(i + 4 < encryptedText.Length)
                {
                    if(i + 8 < encryptedText.Length)
                        dataBlock[1] = ConvertStringToUInt(encryptedText.Substring(i + 4, 4));
                    else
                    {
                        dataBlock[1] = ConvertStringToUInt(encryptedText.Substring(i + 4, encryptedText.Length - i - 4));
                    }

                }
                ///
                //dataBlock[1] = ConvertStringToUInt(encryptedText.Substring(i + 4, 4));


                if (cbc)
                {
                    ivBlockTemp0 = ConvertUIntToBytes(dataBlock[0]);
                    ivBlockTemp1 = ConvertUIntToBytes(dataBlock[1]);
                }


                TEA.DecodeBlock(dataBlock, keyBlock);

                ///
                byte[] dataBlockBytesTemp0 = TEA.ConvertUIntToBytes(dataBlock[0]);
                byte[] dataBlockBytesTemp1 = TEA.ConvertUIntToBytes(dataBlock[1]);

                if (cbc)
                {
                    dataBlockBytesTemp0 = CBC.Code(dataBlockBytesTemp0, ivBlock0);
                    dataBlockBytesTemp1 = CBC.Code(dataBlockBytesTemp1, ivBlock1);
                    ivBlock0 = ivBlockTemp0;
                    ivBlock1 = ivBlockTemp1;
                }
                ///
                dataBytes.AddRange(dataBlockBytesTemp0);
                dataBytes.AddRange(dataBlockBytesTemp1);
            }

            byte[] dataBytesArr = dataBytes.ToArray();
            string decipheredString = System.Text.Encoding.Latin1.GetString(dataBytesArr, 0, dataBytesArr.Length);
            if (decipheredString[decipheredString.Length - 1] == '\0')
                decipheredString = decipheredString.Substring(0, decipheredString.Length - 1);
            return decipheredString;
        }



        public static string Code(string plainText, string key, bool cbc=false, string initializationVector = " ")
        {
            if(cbc && initializationVector == " ")
            {
                throw new ArgumentException("Argument initializationVector must be set if cbc is true");

            }
            plainText = plainText.Replace('�', '\0');
            byte[] plainTextBytes = System.Text.Encoding.Latin1.GetBytes(plainText);
            byte[] initializationVectorBytes = System.Text.Encoding.Latin1.GetBytes(initializationVector);
            string cipherText = "";
            UInt32[] dataBlock = new UInt32[2];
            UInt32[] keyBlock = TEA.ConvertStringToUIntKey(key);


            for (int i = 0; i < plainTextBytes.Length; i += 8)
            {

                dataBlock[0] = dataBlock[1] = 0;

                //creating first uint
                byte[] buffer0 = new byte[4];
                //if array is long enough fill the whole buffer, else take the whole array
                int lengthToCopy = (plainText.Length - i) > 4 ? 4 : (plainText.Length - i);
                //if it's 0, buffer should stay empty
                if (lengthToCopy > 0)
                    Array.Copy(plainTextBytes, i, buffer0, 0, lengthToCopy);

                

                

                //creating second uint
                byte[] buffer1 = new byte[4];
                lengthToCopy = (plainText.Length - i - 4) > 4 ? 4 : (plainText.Length - i - 4);
                if (lengthToCopy > 0)
                    Array.Copy(plainTextBytes, i + 4, buffer1, 0, lengthToCopy);

                if (cbc)
                {

                    byte[] iv0 = new byte[4];
                    byte[] iv1 = new byte[4];
                    if (initializationVectorBytes.Length < 8)
                    {
                        byte[] ivTemp = new byte[8];
                        for(int k = 0; k < 8; k++)
                        {
                            ivTemp[k] = initializationVectorBytes[k% initializationVectorBytes.Length];

                        }

                        initializationVectorBytes = ivTemp;
                    }


                    for(int k = 0; k < 8; k++)
                    {
                        if(k < 4)
                        {
                            iv0[k] = initializationVectorBytes[k];
                        }
                        else
                        {
                            iv1[k - 4] = initializationVectorBytes[k];
                        }
                    }

                    buffer0 = CBC.Code(buffer0, iv0);
                    buffer1 = CBC.Code(buffer1, iv1);
                }


                dataBlock[0] = TEA.ConvertBytesToUInt(buffer0);
                dataBlock[1] = TEA.ConvertBytesToUInt(buffer1);
                


                TEA.CodeBlock(dataBlock, keyBlock);


                cipherText += ConvertUIntBlockToString(dataBlock);

                if (cbc)
                {
                    //setting new vector
                    byte[] firstHalf = ConvertUIntToBytes(dataBlock[0]);
                    byte[] secondHalf = ConvertUIntToBytes(dataBlock[1]);

                    Array.Copy(firstHalf,0,initializationVectorBytes,0, firstHalf.Length);
                    Array.Copy(secondHalf, 0, initializationVectorBytes, 4, secondHalf.Length);
                }

                

            }
         
            cipherText = cipherText.Replace('\0', '�');

            return cipherText;
        }

        public static UInt32 ConvertBytesToUInt(byte[] data)
        {
            if (data.Length > 4)
            {
                throw new Exception("Data too long");
            }

            UInt32 result = 0;
            int ind = 0;
            result |= data[ind++];
            while (ind < data.Length)
            {
                result = result << 8;
                result |= data[ind++];
            }
            return result;
        }

        public static string ConvertUIntBlockToString(UInt32[] Input)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append((char)((Input[0] & 0xFF)));
            output.Append((char)((Input[0] >> 8) & 0xFF));
            output.Append((char)((Input[0] >> 16) & 0xFF));
            output.Append((char)((Input[0] >> 24) & 0xFF));
            output.Append((char)((Input[1] & 0xFF)));
            output.Append((char)((Input[1] >> 8) & 0xFF));
            output.Append((char)((Input[1] >> 16) & 0xFF));
            output.Append((char)((Input[1] >> 24) & 0xFF));
            return output.ToString();
        }
        public static UInt32[] ConvertStringToUIntKey(string keyString)
        {

            //uses a 64-bit key. 
            if (keyString.Length > 16)
                throw new Exception("Key must be 16 characters or less");
            UInt32[] keyBlock = new UInt32[4];
            int blockIndex = 0, switchBlock = 0;
            keyBlock[blockIndex] |= keyString[0];
            switchBlock++;
            for (int i = 1; i < keyString.Length; i++)
            {
                if (switchBlock == 4)
                {
                    switchBlock = 0;
                    blockIndex++;
                }
                keyBlock[blockIndex] = keyBlock[blockIndex] << 8;
                keyBlock[blockIndex] |= keyString[i];
                switchBlock++;


            }
            return keyBlock;
        }

        public static byte[] ConvertUIntToBytes(UInt32 data)
        {
            byte[] bytes = new byte[4];
            for (int i = 3; i >= 0; i--)
            {
                bytes[i] = (byte)data;
                data >>= 8;

            }
            return bytes;
        }
        public static uint ConvertStringToUInt(string Input)
        {
            uint output;
            output = ((uint)Input[0]);
            output += ((uint)Input[1] << 8);
            output += ((uint)Input[2] << 16);
            output += ((uint)Input[3] << 24);
            return output;
        }

    }
}
