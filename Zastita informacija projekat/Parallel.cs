using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zastita_informacija_projekat;

namespace Zastita_informacija_projekat
{
    internal class Parallel
    {
        private static void SingleThread(string srcPath, string destPath, int myNumber, int totalThreads, string key)
        {


            byte[] buffer = new byte[1];
            int blockSize = 0, originalBlockSize = 0;

            ///citanje
            try
            {
                // Open the text file using a stream reader.
                using (var sr = new FileStream(srcPath, FileMode.Open, FileAccess.Read, FileShare.Read))
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




                }
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }

            Rc4 rc = new Rc4(key);
           
            string res = rc.Encrypt(System.Text.Encoding.Latin1.GetString(buffer));
           

            res.Replace('?', '�');
            buffer = Encoding.Latin1.GetBytes(res);



            ///upis
            try
            {
                using (var sr = new FileStream(destPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    sr.Position = myNumber * originalBlockSize;
                    sr.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("Thread " + myNumber + ". je upisao: " + System.Text.Encoding.Default.GetString(buffer));
                }
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void ParallelEncryptionRc4(string srcPath, string destPath, int numOfThreads, string key)
        {
            Thread[] threads = new Thread[numOfThreads];

            //int blockSize = (int)((sr.Length - 3) / brojThreadova);

            for (int i = 0; i < numOfThreads; i++)
            {
                int myLocalNumber = i;
                threads[i] = new Thread(new ThreadStart(() => { SingleThread(srcPath, destPath, myLocalNumber, numOfThreads, key); }));
            }
            for (int i = 0; i < numOfThreads; i++)
            {
                threads[i].Start();
            }
            for (int i = 0; i < numOfThreads; i++)
            {
                threads[i].Join();
            }

        }
    }
}