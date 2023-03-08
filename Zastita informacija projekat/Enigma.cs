using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zastita_informacija_projekat
{
    internal class Enigma
    {
        Plugboard plugboard;
        Rotor rotorI;
        Rotor rotorII;
        Rotor rotorIII;
        Reflector reflector;

        public Enigma(Plugboard plugboard, Rotor rotorI, Rotor rotorII, Rotor rotorIII, Reflector reflector)
        {
            this.plugboard = plugboard;
            this.rotorI = rotorI;
            this.rotorII = rotorII;
            this.rotorIII = rotorIII;
            this.reflector = reflector;
        }

        public char Encrypt(char symbol)
        {



            //rotate the rotors
            if (rotorII.IsAtNotch() && rotorIII.IsAtNotch())//second condition is there because of double stepping
            {
                rotorI.Rotate();
                rotorII.Rotate();
                rotorIII.Rotate();
            }
            else if (rotorII.IsAtNotch())
            {
                rotorI.Rotate();
                rotorII.Rotate();
                rotorIII.Rotate();
            }
            else if (rotorIII.IsAtNotch())
            {
                rotorII.Rotate();
                rotorIII.Rotate();
            }
            else
            {
                rotorIII.Rotate();
            }



            int signal = plugboard.Forward(symbol);
            signal = rotorIII.Forward(signal);
            signal = rotorII.Forward(signal);
            signal = rotorI.Forward(signal);
            signal = reflector.Forward(signal);
            signal = rotorI.Backward(signal);
            signal = rotorII.Backward(signal);
            signal = rotorIII.Backward(signal);
            return plugboard.Backward(signal);


        }
        public void SetKey(char[] key)
        {
            if (key.Length != 3)
                throw new Exception("You must provide key with a length of 3");

            rotorI.RotateToSymbol(key[0]);

            rotorII.RotateToSymbol(key[1]);

            rotorIII.RotateToSymbol(key[2]);

        }

        public void SetRings(char[] ringSetting)
        {
            if (ringSetting.Length != 3)
                throw new Exception("You must provide ring setting with a length of 3");

            rotorI.SetRing(ringSetting[0]);

            rotorII.SetRing(ringSetting[1]);

            rotorIII.SetRing(ringSetting[2]);

        }


    }


    internal class Plugboard
    {

        private char[] original_letters;
        private char[] encrypted_letters;
        public Plugboard(char[] pairs)
        {
            if (pairs.Length % 2 != 0) throw new ArgumentException("Each provided letter must have it's paired letter");
            original_letters = new char[26];
            encrypted_letters = new char[26];
            char alphabet_char = 'A';
            for (int i = 0; i < 26; i++)
                original_letters[i] = encrypted_letters[i] = alphabet_char++;

            for (int i = 0; i < pairs.Length; i += 2)
            {
                int index_org = Array.IndexOf(encrypted_letters, pairs[i]);
                int index_enc = Array.IndexOf(encrypted_letters, pairs[i + 1]);

                char temp = encrypted_letters[index_org];
                encrypted_letters[index_org] = encrypted_letters[index_enc];
                encrypted_letters[index_enc] = temp;
            }


        }

        public int Forward(char symbol)
        {
            return Array.IndexOf(encrypted_letters, symbol);

        }
        public char Backward(int signal)
        {
            return encrypted_letters[signal];
        }

    }

    internal class Reflector
    {
        private char[] wiring;
        public Reflector(char type)
        {
            wiring = new char[26];



            string code;
            //Reflector wiring for the original enigma machine
            //source: https://en.wikipedia.org/wiki/Enigma_rotor_details
            switch (type)
            {
                case 'A':
                    code = "EJMZALYXVBWFCRQUONTSPIKHGD";
                    break;
                case 'B':
                    code = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
                    break;
                case 'C':
                    code = "FVPJIAOYEDRZXWGCTKUQSBNMHL";
                    break;
                default:
                    //first code
                    code = "EJMZALYXVBWFCRQUONTSPIKHGD";
                    break;
            }
            for (int i = 0; i < 26; i++)
            {
                wiring[i] = code[i];
            }




        }

        public int Forward(int input)
        {
            char input_char = wiring[input];

            //code for output char
            return input_char - 'A';
        }
    }
    internal class Rotor
    {
        private char[,] wiring;
        private char notch;
        public Rotor(int num)
        {

            

            wiring = new char[2, 26];

            //setting up wiring
            char alphabet_char = 'A';
            for (int i = 0; i < 26; i++)
                wiring[0, i] = alphabet_char++;


            //Rotor wiring for the original enigma machine
            //source: https://en.wikipedia.org/wiki/Enigma_rotor_details
            string code;
            switch (num)
            {
                case 1:
                    code = "EKMFLGDQVZNTOWYHXUSPAIBRCJ";
                    this.notch = 'Q';
                    break;
                case 2:
                    code = "AJDKSIRUXBLHWTMCQGZNPYFVOE";
                    this.notch = 'E';
                    break;
                case 3:
                    code = "BDFHJLCPRTXVZNYEIWGAKMUSQO";
                    this.notch = 'V';
                    break;
                case 4:
                    code = "ESOVPZJAYQUIRHXLNFTGKDCMWB";
                    this.notch = 'J';
                    break;
                case 5:
                    code = "VZBRGITYUPSDNHLXAWMJQOFECK";
                    this.notch = 'Z';
                    break;
                default:
                    //first code
                    code = "EKMFLGDQVZNTOWYHXUSPAIBRCJ";
                    this.notch = 'Q';
                    break;
            }

            for (int i = 0; i < 26; i++)
            {
                wiring[1, i] = code[i];
            }

        }


        public int Forward(int input)
        {
            char letter = wiring[1, input];
            for (int i = 0; i < 26; i++)
            {
                if (wiring[0, i] == letter)
                    return i;
            }
            throw new Exception("Letter not found");

        }
        public int Backward(int input)
        {
            char letter = wiring[0, input];
            int index = 0;
            while (index < 26)
            {
                if (wiring[1, index] == letter)
                    return index;
                index++;
            }

            throw new Exception("Letter not found");

        }
        public char Rotate()
        {
            char original_first = wiring[0, 0];
            char ciphered_first = wiring[1, 0];

            Array.Copy(wiring, 1, wiring, 0, wiring.GetLength(1) - 1);
            wiring[0, 25] = original_first;
            Array.Copy(wiring, 27, wiring, 26, wiring.GetLength(1) - 1);
            wiring[1, 25] = ciphered_first;

            return wiring[0, 0];

        }
        public bool IsAtNotch()
        {
            return wiring[0, 0] == notch;
        }

        public void RotateToSymbol(char symbol)
        {

            int index = symbol - 'A';

            char[,] buffer = new char[1, 26];
            Array.Copy(wiring, 0, buffer, 0, index);
            Array.Copy(wiring, index, wiring, 0, wiring.GetLength(1) - index);
            Array.Copy(buffer, 0, wiring, wiring.GetLength(1) - index, index);


            Array.Copy(wiring, 26, buffer, 0, index);
            Array.Copy(wiring, index + 26, wiring, 26, wiring.GetLength(1) - index);
            Array.Copy(buffer, 0, wiring, wiring.GetLength(1) - index + 26, index);


        }


        public void SetRing(char position)
        {
            int setting = position - 'A';

            //Rotate, but in different direction from Rotate function
            char[,] buffer = new char[1, 26];
            Array.Copy(wiring, 26 - setting, buffer, 0, setting);
            Array.Copy(wiring, 0, wiring, setting, 26 - setting);
            Array.Copy(buffer, 0, wiring, 0, setting);


            Array.Copy(wiring, 2 * 26 - setting, buffer, 0, setting);
            Array.Copy(wiring, 26, wiring, 26 + setting, 26 - setting);
            Array.Copy(buffer, 0, wiring, 26, setting);

            //Set notch to equivalent position
            int notch = this.notch - 'A';
            notch = (notch - setting) >= 0 ? notch - setting : notch - setting + 26;

            this.notch = (char)('A' + notch);

        }

       

    }
}
