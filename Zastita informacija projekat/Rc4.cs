using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zastita_informacija_projekat
{
    internal class Rc4
    {

        private static int keyLen = 128;
        private byte[] key;
        private byte[] S;

        public Rc4(string textKey)
        {

            key = new byte[keyLen];
            Encoding enc_default = Encoding.Latin1;
            key = enc_default.GetBytes(textKey);
            S = new byte[keyLen];
            // keyStringLen = textKey.Length;
            KeyScheduling();
        }
        public string Encrypt(string plainText, bool cbc = false, string initializationVector = " ", bool encryption = true)
        {

            if (cbc && initializationVector == " ")
            {
                throw new ArgumentException("Argument initializationVector must be set if cbc is true");

            }
           // plainText = plainText.Replace('�', '\0');
            Encoding enc_default = Encoding.Latin1;
            byte[] plainTextBytes = enc_default.GetBytes(plainText);
            byte[] ivBytes = enc_default.GetBytes(initializationVector);

            int i = 0;
            int j = 0;

            for (int offset = 0; offset < plainText.Length; offset++)
            {
                i = (i + 1) % keyLen;
                j = (j + S[i]) % keyLen;

                (S[i], S[j]) = (S[j], S[i]);
                byte xor_value = S[(S[i] + S[j]) % keyLen];

                if (cbc && encryption)
                {
                    //block is only 1 byte long
                    byte[] plainTextByteBlock = new byte[1];
                    plainTextByteBlock[0] = plainTextBytes[offset];
                    ivBytes = CBC.Code(plainTextByteBlock, ivBytes);
                    plainTextBytes[offset] = ivBytes[0];

                }


                //for decryption with cbc
                byte tempIv = plainTextBytes[offset];
                
                plainTextBytes[offset] ^= xor_value;
                
                //new value for iv
                if(cbc && encryption)
                {
                    ivBytes[0] = plainTextBytes[offset];
                }

                //cbc decription
                if (cbc && !encryption)
                {
                    //block is only 1 byte long
                    byte[] plainTextByteBlock = new byte[1];
                    plainTextByteBlock[0] = plainTextBytes[offset];
                    byte[] result = CBC.Code(plainTextByteBlock, ivBytes);
                    plainTextBytes[offset] = result[0];
                    ivBytes[0] = tempIv;
                }
                

                
            }

            char[] outarrchar = new char[enc_default.GetCharCount(plainTextBytes, 0, plainTextBytes.Length)];

            enc_default.GetChars(plainTextBytes, 0, plainTextBytes.Length, outarrchar, 0);

            string retStr = new string(outarrchar);

           // retStr = retStr.Replace('\0', '�');

            return retStr;
        }

        public void KeyScheduling()
        {
            if(key.Length == 0)
            {
                throw new Exception("Key must be provided");
            }

            for (byte i = 0; i < keyLen - 1; i++)
            {
                S[i] = i;
            }
            S[127] = 127;

            int j = 0;
            for (int i = 0; i < keyLen; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % 128;
                //swap s[i],s[j]
                (S[i], S[j]) = (S[j], S[i]);
            }
        }

        public void CitajPisiThreadBK(string putanja, string putanjaUpisa, int myNumber, int totalThreads)
        {

            
            byte[] buffer = new byte[1];
            int blockSize = 0, originalBlockSize = 0;

            ///citanje
            try
            {
                // Open the text file using a stream reader.
                using (var sr = new FileStream(putanja, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    //size of the block which thread need to encrypt
                    blockSize = (int)((sr.Length) / totalThreads); //tends to change, need to keep original
                    originalBlockSize = blockSize;
                    //position from where the block starts
                    sr.Position = myNumber * originalBlockSize;



                    //only for the last thread if the length of plain text isn't divisable with the number of threads
                    if (myNumber == totalThreads - 1 && ((sr.Length) % totalThreads) != 0)
                    {
                        blockSize += (int)((sr.Length) % totalThreads);
                    }
                    buffer = new byte[blockSize];
                    sr.Read(buffer, 0, blockSize);
                    //Console.WriteLine("Thread " + myNumber + ". je procitao: " + System.Text.Encoding.ASCII.GetString(buffer));
                    



                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            string plainText = System.Text.Encoding.UTF8.GetString(buffer);
            
            string encryptedText =this.Encrypt(plainText);

            
            
            buffer = Encoding.UTF8.GetBytes(encryptedText);



            ///upis
            try
            {
                using (var sr = new FileStream(putanjaUpisa, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    sr.Position = myNumber * originalBlockSize;
                    sr.Write(buffer, 0, buffer.Length);
                    //sr.Write(Encoding.UTF8.GetBytes(encryptedText), 0, Encoding.UTF8.GetBytes(encryptedText).Length);
                    
                    //Console.WriteLine("Thread " + myNumber + ". je upisao: " + System.Text.Encoding.ASCII.GetString(buffer));
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(e.Message);
            }


           

        }


        public void CitajPisiThread(string putanjaUpisa, string slice)
        {
            try
            {
                using (var sr = new FileStream(putanjaUpisa, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    
                    //sr.Write(Encoding.UTF8.GetBytes(encryptedText), 0, Encoding.UTF8.GetBytes(encryptedText).Length);

                    //Console.WriteLine("Thread " + myNumber + ". je upisao: " + System.Text.Encoding.ASCII.GetString(buffer));
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(e.Message);
            }







        }



        public void ParallelEncryptBK(string putanja, string putanjaUpisa, int brojThreadova)
        {
            Thread[] threadovi = new Thread[brojThreadova];

            //int blockSize = (int)((sr.Length - 3) / brojThreadova);Fd

            for (int i = 0; i < brojThreadova; i++)
            {
                int myLocalNumber = i;
                threadovi[i] = new Thread(new ThreadStart(() => { CitajPisiThreadBK(putanja, putanjaUpisa, myLocalNumber, brojThreadova); }));
            }
            for (int i = 0; i < brojThreadova; i++)
            {
                threadovi[i].Start();
            }
            for (int i = 0; i < brojThreadova; i++)
            {
                threadovi[i].Join();
            }

        }


        public void ParallelEncrypt(string putanja, string putanjaUpisa, int brojThreadova)
        {


            try
            {
                using(var sr = new StreamReader(putanja))
                {
                    string s = sr.ReadToEnd();
                    int blockSize = s.Length / brojThreadova;
                    int originalBlockSize = blockSize;
                    Thread[] threadovi = new Thread[brojThreadova];

                    //int blockSize = (int)((sr.Length - 3) / brojThreadova);Fd

                    for (int i = 0; i < brojThreadova; i++)
                    {
                        int myLocalNumber = i;
                        int myOriginalBlockSize = blockSize;
                        int myBlockSize = blockSize;
                        if(i == brojThreadova - 1)
                        {
                            myBlockSize += s.Length % brojThreadova;
                        }
                        
                        string slice = s.Substring(myLocalNumber * myOriginalBlockSize, myBlockSize);
                        threadovi[i] = new Thread(new ThreadStart(() => { CitajPisiThread( putanjaUpisa,slice ); }));
                    }
                    for (int i = 0; i < brojThreadova; i++)
                    {
                        threadovi[i].Start();
                    }
                    for (int i = 0; i < brojThreadova; i++)
                    {
                        threadovi[i].Join();
                    }

                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(e.Message);
            }



           

        }






    }
}


