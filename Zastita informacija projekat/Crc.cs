using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zastita_informacija_projekat
{
   
   public  class FileCRC
    {
       
        

        public uint CRC(string filePath)
        {

            uint result;
            byte[] bytes = File.ReadAllBytes(filePath);
            if (Path.GetExtension(filePath) == ".txt")
            {
               
                result= CalculateCrcHash(bytes);
            }
            if (Path.GetExtension(filePath) == ".bmp")
            {
                
                int start = 54;
                int length = (int)(bytes.Length - start);
                byte[] data = new byte[length];
                Array.Copy(bytes, start, data, 0, length);
                result= CalculateCrcHash(data);
            }
            else
                throw new Exception("Invalid file format!");

            return result; 
        }

            public uint CalculateCrcHash(byte[] data)
        {


            uint hash = 0x00000000;
            int n = data.Length;
            for (int i = 0; i < n; i++)
            {
                hash ^= (uint)data[i] << 24;
                for (int j = 0; j < 8; j++)
                {
                    if ((hash & 0x80000000) != 0)
                    {
                        hash = (hash << 1) ^ 0x04C11DB7;
                    }
                    else
                    {
                        hash <<= 1;
                    }
                }
            }
            return hash;
        }




        }
        


    
}
