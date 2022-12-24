using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TCPChat.Client
{
    class Coder
    {
               //Библеотека символов
        public static string[] Code_of_littere = {"0000000","0000001","0000010","0000011","0000100","0000101",
          "0000110","0000111","0001000","0001001","0001010","0001011","0001100","0001101","0001110",
          "0001111","0010000","0010001","0010010","0010011","0010100","0010101","0010110","0010111",
          "0011000","0011001","0011010","0011100","0011110","0011111","0100000","0100001","0100010",
          "0100011","0100100","0100101","0100110","0100111","0101000","0101001","0101010","0101011",
          "0101100","0101101","0101110","0101111","0110000","0110001","0110010","0110011","0110100",
          "0110101","0110110","0110111","0111000","0111001","0111010","0111011","0111100","0111101",
          "0111110","0111111","1000000","1000001","1000010","1000011","1000100","1000101","1000110"};
        public static string[] Number_of_lettere = {"А","Б","В","Г","Д","Е","Ж","З","И","Й","К","Л","М",
          "Н","О","П","Р","С","Т","У","Ф","Х","Ц","Ч","Ш","Щ","Ь","Ы","Ъ","Э","Ю","Я"," ",",",".","!","?",
          "а","б","в","г","д","е","ё","з","и","й","к","л","м",
          "н","о","п","р","с","т","у","ф","х","ц","ч","ш","щ","ь","ы","ъ","э","ю","я"};

        // static void Main(string[] args)
        // {
 
        //
        //     Console.WriteLine("Введите текст");
        //     string text = Console.ReadLine(); //Приходит от клиента 
        //     string binar_text = text_for_binar(text, Number_of_lettere, Code_of_littere);
        //     //создание кода
        //     string key_code = generate_key(binar_text.Length);
        //     string code_out = text_for_code(key_code, binar_text); //Эту переменную отправлять на сервес
        //     string text_binar_out = code_for_text(key_code, code_out);// Сюда приходит переменная с сервера 
        //     string text_out = binar_out_for_text(text_binar_out, Code_of_littere, Number_of_lettere);//отправляеться в клиент
        //
        //     Console.WriteLine(text_out);
        //
        //     Console.ReadLine();
        // }
        //
        // public static string set_key
        public static string generate_key(int length)
        {
            Random rnd = new Random(); string key_code = "";
            for (int i = 0; i < length; i++)
            {
                key_code += Convert.ToString(rnd.Next() & 1); //key_code тоже
            }

            Console.WriteLine($"Dumping key[{length}]: {key_code}");
            return key_code;
        }
        public static string text_for_code(string key_code, string binar_code_text)
        {
            string code_out = "";
            for (int i = 0; i < binar_code_text.Length; i++)
            {
                int a = binar_code_text[i];
                int b = key_code[i];
                if ((a == 48) && (b == 48))
                {
                    code_out += "0";
                }
                else if ((a == 48) && (b == 49))
                {
                    code_out += "1";
                }
                else if ((a == 49) && (b == 48))
                {
                    code_out += "1";
                }
                else
                {
                    code_out += "0";
                }
            }
            return code_out;
        }
        public static string code_for_text(string key_code, string code_out)
        {
            string binar_code_text = "";
            for (int i = 0; i < code_out.Length; i++)
            {
                int a = code_out[i];
                int b = key_code[i];
                if ((a == 48) && (b == 48))
                {
                    binar_code_text += 0;
                }
                else if ((a == 48) && (b == 49))
                {
                    binar_code_text += 1;
                }
                else if ((a == 49) && (b == 48))
                {
                    binar_code_text += 1;
                }
                else
                {
                    binar_code_text += 0;
                }
            }
            return binar_code_text;
        }
        public static string text_for_binar(string text, string[] Number_of_lettere, string[] Code_of_littere)
        {
            // Plain text -> Binary representation
            string binar_text = "";
            foreach (char n in text)
            {
                string Bukva = n.ToString();
                for (int i = 0; i < Number_of_lettere.Length; i++)
                {
                    if (Bukva == Number_of_lettere[i])
                    {
                        binar_text += Code_of_littere[i];
                    }
                }
            }
            return binar_text;
        }
        public static string binar_out_for_text(string text_binar_out, string[] Code_of_littere, string[] Number_of_lettere)
        {
            int Quantity_char = text_binar_out.Length / 7;
            int Counter_char_index = 0, char_index_leght = 7, n_index = 0;
            string text_out = "";
            for (int i = 0; i < Quantity_char; i++)
            {
                n_index = 0;
                foreach (string n in Code_of_littere)
                {
                    if (n == text_binar_out.Substring(Counter_char_index,char_index_leght))
                    {

                        text_out += Number_of_lettere[n_index];
                    }
                    n_index += 1;
                }
                Counter_char_index += 7;
            }
            return text_out;
        }
    }
    public class MainViewModel : ViewModelBase
    {
        //наши данные
        private string key = Coder.generate_key(1000);
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5050;
        public string Nick { get; set; } = "";
        public string Chat
        {
            get => GetValue<string>(); 
            set => SetValue(value);
        }
        public string Message { get => GetValue<string>(); set => SetValue(value); }
        //перевод
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;


        private void Listener()
        { //читаем стрим
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {      //сервер не будет отправлять пустое значение
                        if (_client?.Connected == true)
                        {
                            var line = _reader?.ReadLine();
                            if (line != null)
                            {
                                try
                                {
                                    var messageSplitted = line.Split(": "); //.Skip(1).ToArray();
                                    string nickname = messageSplitted[0];
                                    string encryptedMessage = String.Join("", messageSplitted.Skip(1));
                                    var decoded = Coder.code_for_text(key, encryptedMessage);
                                    var decrypted = Coder.binar_out_for_text(decoded, Coder.Code_of_littere,
                                        Coder.Number_of_lettere);
                                    Chat += nickname + ": " + decrypted + "\n";
                                    // Chat += line + "\n";
                                }
                                catch
                                {
                                    Chat += $"System: failed to decode message '{line}'\n";
                                }
                            }
                            else
                            { //если пришла пустота отключаем от сервера и отключаем от сервера
                                _client.Close();
                                Chat += "Connected error.\n";
                            }
                        }
                        //задержка чтобы комп не умер
                        Task.Delay(10).Wait();
                    }
                    catch (Exception ex)
                    { 
                        Chat += ex.Message + "\n";
                    }
                }
            });
        }
        public AsyncCommand ConnectCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Run(() =>
                    {
                        try
                        {   // инициализацмя
                            _client = new TcpClient();
                            // соединение
                            _client.Connect(IP, Port);
                            // объявление стримов (читать и писать стрим)
                            _reader = new StreamReader(_client.GetStream());
                            _writer = new StreamWriter(_client.GetStream());
                            Listener();
                            //автоотправка стрима
                            _writer.AutoFlush = true;
                            //логин пользрваться
                            _writer.WriteLine($"Login: {Nick}");
                            Chat += $"System: Your key: {key}\nSystem: You can change your key using command !setkey <key>\n";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                    //нельзя повторно подключиться
                }, () => _client is null || _client?.Connected == false);
            }
        }
        // кнопочка отпарвки
        public AsyncCommand SendCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Run(() =>
                    {
                        if (Message.StartsWith("!setkey"))
                        {
                            string[] splited = Message.Split(' ');
                            if (splited.Length == 2)
                            {
                                key = splited[1];
                                Chat += $"Set your key to {key}\n";
                            }
                            else
                            {
                                Chat += "System: Invalid command format\n";
                            }
                            Message = "";
                            return;
                        }
                        
                        string encryptedMessage = Coder.text_for_binar(Message, Coder.Number_of_lettere, Coder.Code_of_littere);
                        encryptedMessage = Coder.text_for_code(key, encryptedMessage);
                        _writer?.WriteLine($"{Nick}: {encryptedMessage}");
                        // _writer?.WriteLine($"{Nick}: {Message}");
                        // очищаем поле для вода
                        Message = "";
                    });
                    // работаем только при подключении и только когда сообщение не пусто
                }, () => _client?.Connected == true, !string.IsNullOrWhiteSpace(Message));
            }
        }
    }
}
