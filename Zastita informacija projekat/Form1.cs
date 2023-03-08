using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;

namespace Zastita_informacija_projekat
{
    public partial class Form1 : Form
    {
        private int bitmap_width;
        private int bitmap_height;
        public Form1()
        {
            InitializeComponent();
        }

        private void EncryptButton_Click(object sender, EventArgs e)
        {
            if (this.keyTextBox.Text.ToString() == "" && !radioButton6.Checked)
            {
                return;
            }
            if (radioButton4.Checked)
            {


                if (radioButton5.Checked)
                {
                    //rc4+cbc
                    Rc4 cipher = new Rc4(keyTextBox.Text.ToString());
                    string encriptedText = cipher.Encrypt(this.plainTextBox.Text.ToString(), true, textBox1.Text.ToString(), true);
                    this.encryptedTextBox.Text = encriptedText;
                }
                else
                {
                    Rc4 cipher = new Rc4(keyTextBox.Text.ToString());
                    string encriptedText = cipher.Encrypt(this.plainTextBox.Text.ToString());
                    this.encryptedTextBox.Text = encriptedText;
                }
            }
            if (radioButton2.Checked)
            {

                if (radioButton5.Checked)
                {
                    //tea+cbc
                    encryptedTextBox.Text = TEA.Code(plainTextBox.Text.ToString(), keyTextBox.Text.ToString(), true, textBox1.Text.ToString());
                }
                else
                {
                    //tea
                    encryptedTextBox.Text = TEA.Code(plainTextBox.Text.ToString(), keyTextBox.Text.ToString());
                }
            }
            if (radioButton3.Checked)
            {


                //crc
            }
            if (radioButton6.Checked)
            {
                //enigma
                string rotors = rotorBox.Text.ToString().ToUpper();
                string reflectorString = reflektorBox.Text.ToString().ToUpper();
                char[] ringSettings = ringSettingsBox.Text.ToCharArray();
                char[] keySettings = keySettingsBox.Text.ToCharArray();


                Rotor rotorI = new Rotor(rotors[0] - '0');
                Rotor rotorII = new Rotor(rotors[1] - '0');
                Rotor rotorIII = new Rotor(rotors[2] - '0');


                Reflector reflector = new Reflector(reflectorString[0]);

                Plugboard pb = new Plugboard(plugboardBox.Text.ToCharArray());

                Enigma enigma = new Enigma(pb, rotorI, rotorII, rotorIII, reflector);
                enigma.SetRings(ringSettings);
                enigma.SetKey(keySettings);

                string plainText = plainTextBox.Text.ToString().ToUpper().Trim();
                plainText = plainText.Replace(" ", "");

                char[] plainTextArray = plainText.ToCharArray();
                for (int i = 0; i < plainTextArray.Length; i++)
                {
                    plainTextArray[i] = enigma.Encrypt(plainTextArray[i]);
                }
                encryptedTextBox.Text = new string(plainTextArray);
            }



            //textBox1.Enabled = false;
            //radioButton5.Checked= false;
            //radioButton4.Checked = false;
            //radioButton2.Checked= false;
            //radioButton3.Checked= false;

        }

        private void DecryptBtn_Click(object sender, EventArgs e)
        {
            if (this.keyTextBox.Text.ToString() == "" && !radioButton6.Checked)
            {
                return;
            }
            //string s = 
            if (radioButton4.Checked)
            {
                if (radioButton5.Checked)
                {
                    Rc4 cipher = new Rc4(keyTextBox.Text.ToString());
                    string decriptedText = cipher.Encrypt(this.encryptedTextBox.Text.ToString(), true, textBox1.Text.ToString(), false);
                    this.plainTextBox.Text = decriptedText;
                }
                else
                { //rc4
                    Rc4 cipher = new Rc4(keyTextBox.Text.ToString());
                    string decriptedText = cipher.Encrypt(this.encryptedTextBox.Text.ToString());
                    this.plainTextBox.Text = decriptedText;

                }
            }
            if (radioButton2.Checked)
            {
                if (radioButton5.Checked)
                {
                    //tea+cbc
                    plainTextBox.Text = TEA.Decode(encryptedTextBox.Text.ToString(), keyTextBox.Text.ToString(), true, textBox1.Text.ToString());
                }
                else
                {
                    plainTextBox.Text = TEA.Decode(encryptedTextBox.Text.ToString(), keyTextBox.Text.ToString());
                }
            }
            if (radioButton3.Checked)
            {
                //crc
            }

            if (radioButton6.Checked)
            {
                //enigma
                string rotors = rotorBox.Text.ToString();
                string reflectorString = reflektorBox.Text.ToString();
                char[] ringSettings = ringSettingsBox.Text.ToCharArray();
                char[] keySettings = keySettingsBox.Text.ToCharArray();


                Rotor rotorI = new Rotor(rotors[0] - '0');
                Rotor rotorII = new Rotor(rotors[1] - '0');
                Rotor rotorIII = new Rotor(rotors[2] - '0');


                Reflector reflector = new Reflector(reflectorString[0]);

                Plugboard pb = new Plugboard(plugboardBox.Text.ToCharArray());

                Enigma enigma = new Enigma(pb, rotorI, rotorII, rotorIII, reflector);
                enigma.SetRings(ringSettings);
                enigma.SetKey(keySettings);

                string encryptedText = encryptedTextBox.Text.ToString().ToUpper().Trim();
                encryptedText = encryptedText.Replace(" ", "");

                char[] encryptedTextArray = encryptedText.ToCharArray();
                for (int i = 0; i < encryptedTextArray.Length; i++)
                {
                    encryptedTextArray[i] = enigma.Encrypt(encryptedTextArray[i]);
                }
                plainTextBox.Text = new string(encryptedTextArray);
            }


            //textBox1.Enabled = false;
            //radioButton5.Checked = false;
            //radioButton4.Checked = false;
            //radioButton2.Checked = false;
            //radioButton3.Checked = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            //paralelno kriptovanje
            if(keyTextBox.Text.ToString() == "")
            {
                throw new Exception("Must provide a key");
            }

            Parallel.ParallelEncryptionRc4(srcPathTextBox.Text.ToString(), destPathTextBox.Text.ToString(), 5, keyTextBox.Text.ToString());

        }

        


        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog o = new OpenFileDialog())
            {
                o.Filter = "txt files (*.txt)|*.txt";
                if (o.ShowDialog() == DialogResult.OK)
                {

                    string filePath = o.FileName;


                    var fileStream = o.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        plainTextBox.Text = reader.ReadToEnd();
                    }
                }

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog o = new OpenFileDialog())
            {
                o.Filter = "txt files (*.txt)|*.txt";
                if (o.ShowDialog() == DialogResult.OK)
                {

                    string filePath = o.FileName;

                    File.WriteAllText(filePath, encryptedTextBox.Text);
                }

            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)//crc
        {
            if (radioButton3.Checked)
            {

                parallelGroupBox.Visible = false;

                radioButton5.Enabled = false;
                groupBox3.Enabled = true;
            }
            else
            {
                radioButton5.Enabled = true;
            }
            groupBox1.Visible = false;
            groupBox5.Visible = false;
            groupBox3.Visible = true;
            radioButton5.Checked = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)//tea
        {
            parallelGroupBox.Visible = false;
            radioButton5.Enabled = true;
            radioButton5.Checked = false;
            textBox1.Enabled = true;

            groupBox1.Visible = true;
            groupBox5.Visible = false;
            groupBox3.Visible = false;
        }



        private void radioButton5_CheckedChanged(object sender, EventArgs e)//cbc
        {
            if (radioButton5.Checked)
                textBox1.Enabled = true;
            else
                textBox1.Enabled = false;



        }

        private void button9_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog o = new OpenFileDialog())
            {

                if (o.ShowDialog() == DialogResult.OK)
                {


                    textBox2.Text = o.FileName;



                }

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog o = new OpenFileDialog())
            {

                if (o.ShowDialog() == DialogResult.OK)
                {

                    textBox3.Text = o.FileName;
                }

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //racunanje CRC
            //u textbox 2 i 3 su imena fajlova 
            //u textbox 4 upises rez


            FileCRC hash = new FileCRC();

            if (hash.CRC(this.textBox2.Text) == hash.CRC(this.textBox3.Text))
            {
                this.textBox4.Text = "Identicni fajlovi";
            }
            else
            {
                this.textBox4.Text = "Ima gubitaka";
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)//enigma
        {
            radioButton5.Enabled = false;
            textBox1.Enabled = false;

            groupBox1.Visible = true;
            groupBox5.Visible = true;
            groupBox3.Visible = false;
        }

        private void radioButton4_CheckedChanged_1(object sender, EventArgs e)//rc4
        {
            if (radioButton4.Checked)
            {
                parallelGroupBox.Visible = true;
            }

            textBox1.Enabled = true;
            groupBox3.Visible = false;
            groupBox5.Visible = false;
            groupBox1.Visible = true;
            radioButton5.Enabled = true;
            radioButton5.Checked = false;
        }


        //ucitaj bitmapu
        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "BMP Files (*.bmp)|*.bmp";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                try
                {

                    byte[] data;
                    data = ReadBitmap(file);

                    byte[] encrypt = new byte[data.Length];

                    if (radioButton4.Checked)
                    {
                        Rc4 cipher = new Rc4(keyTextBox.Text.ToString());
                        if (radioButton5.Checked)
                        {
                            //rc4+cbc
                            //Rc4 cipher = new Rc4(keyTextBox.Text.ToString());
                            string encriptedText = cipher.Encrypt(Encoding.Latin1.GetString(data), true, textBox1.Text.ToString(), true);
                            encrypt = Encoding.Latin1.GetBytes(encriptedText);
                        }
                        else
                        {
                            //Rc4 chiper = new Rc4(keyTextBox.Text);
                            string pom = cipher.Encrypt(Encoding.Latin1.GetString(data));
                            encrypt = Encoding.Latin1.GetBytes(pom);
                        }
                    }
                    if (radioButton2.Checked)
                    {
                        string pom = "";
                        if (radioButton5.Checked)
                        {
                            pom = TEA.Code(Encoding.Latin1.GetString(data), keyTextBox.Text.ToString(), true, textBox1.Text.ToString());
                        }
                        else  
                            pom = TEA.Code(Encoding.Latin1.GetString(data), keyTextBox.Text.ToString());

                        encrypt = Encoding.Latin1.GetBytes(pom);
                    }


                    WriteBitmap("encryption_result.bmp", encrypt);
                    System.Windows.Forms.MessageBox.Show("Result of encryption is new file in bin folder - encryption_result.bmp");

                }

                catch (IOException)
                {
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "BMP Files (*.bmp)|*.bmp";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                try
                {
                    byte[] data;
                    data = ReadBitmap(file);

                    byte[] encrypt = new byte[data.Length];
                    if (radioButton4.Checked)
                    {
                        Rc4 chiper = new Rc4(keyTextBox.Text);
                        string e1 = "";

                        if (radioButton5.Checked) 
                            e1 = chiper.Encrypt(Encoding.Latin1.GetString(data).Replace('�', '\0'), true, textBox1.Text.ToString(), false);
                        


                        else
                            e1 = chiper.Encrypt(Encoding.Latin1.GetString(data));
                        
                        encrypt = Encoding.Latin1.GetBytes(e1);

                    }
                    if (radioButton2.Checked)

                    {
                        string pom = "";
                        if (radioButton5.Checked)
                            pom = TEA.Decode(Encoding.Latin1.GetString(data), keyTextBox.Text.ToString(), true, textBox1.Text.ToString());
                        else
                            pom = TEA.Decode(Encoding.Latin1.GetString(data), keyTextBox.Text.ToString());

                        encrypt = Encoding.Latin1.GetBytes(pom);
                    }
                    WriteBitmap("decription_result.bmp", encrypt);
                    System.Windows.Forms.MessageBox.Show("Result of encryption is new file in bin folder - decription_result.bmp");

                }

                catch (IOException)
                {
                }
            }
        }


        private byte[] ReadBitmap(string name)
        {
            byte[] sadrzaj;
            FileStream stream;
            using (stream = new FileStream(name, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {

                    reader.ReadBytes(18);

                   
                 
                    this.bitmap_width = reader.ReadInt32();
                    this.bitmap_height = reader.ReadInt32();
                    sadrzaj = new byte[this.bitmap_width * this.bitmap_height * 3];

                    reader.ReadBytes(28);


                    sadrzaj = reader.ReadBytes((int)stream.Length);
                }
            }
            return sadrzaj;
           

        }

        private void WriteBitmap(string bitmapFile, byte[] bitmapBytes)
        {
            using (FileStream stream = new FileStream(bitmapFile, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {

                    writer.Write((short)0x4D42);
                    writer.Write((int)( bitmapBytes.Length+54));
                    writer.Write((short)0);
                    writer.Write((short)0);
                    writer.Write((int)54);


                    writer.Write((int)40);

                    writer.Write(this.bitmap_width);
                    writer.Write(this.bitmap_height);
                    writer.Write((short)1);
                    writer.Write((short)24);
                    writer.Write((int)0);
                    writer.Write((int)bitmapBytes.Length);
                    writer.Write((int)0);
                    writer.Write((int)0);
                    writer.Write((int)0);
                    writer.Write((int)0);


                    writer.Write(bitmapBytes);

                }

            }
          
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void ParallelCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ParallelCheck.Checked)
            {
                srcPathTextBox.Visible = true;
                destPathTextBox.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                button1.Visible = true;
            }
            else
            {
                srcPathTextBox.Visible = false;
                destPathTextBox.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                button1.Visible = false;
            }
        }
    }
}