using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace TCPChat.Server
{
    internal class Program
    {
        // открытие любого интернет подключения
        static TcpListener listener = new TcpListener(IPAddress.Any, 5050);
        // создает коллекцию коннект клиент
        static List<ConnectedClient> clients = new List<ConnectedClient>();
        static void Main(string[] args)
        {
            // запускаем слушание
            listener.Start();
            

            while (true)
            {
                 //ожидание клиента
                var client = listener.AcceptTcpClient();

                Task.Factory.StartNew(() =>
                {
                    //получаем стрим
                    var sr = new StreamReader(client.GetStream());
                    while (client.Connected)
                    {
                        //если ключент подключен
                        var line = sr.ReadLine();
                        var nick = line.Replace("Login: ", "");
                        //если удалим ологин и пробел, то значит там есть ник 
                        if (line.Contains("Login: ") && !string.IsNullOrWhiteSpace(nick))
                        {   //проверка на то, что есть ли у нас ник такой
                            if (clients.FirstOrDefault(s => s.Name == nick) is null)
                            {
                                clients.Add(new ConnectedClient(client, nick));
                                Console.WriteLine($"new connection: {nick}");
                                break;
                            }
                            else
                            { 
                                var sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;
                                sw.WriteLine("User with this nickname already exists");
                                client.Client.Disconnect(false);
                            }
                        }
                    }
                    
                    while(client.Connected)
                    {
                        try
                        {
                            var line = sr.ReadLine();
                            //рассылка сообщения, которое было отправленно
                            SendToAllClients(line);

                            Console.WriteLine(line);
                        }
                        catch (Exception ex)
                        { 
                            Console.WriteLine(ex.Message);
                        }
                     }
                });
            }
        }

        private static void SendToAllClients(string message)
        {
            Task.Run(() =>
            {
                for(int i = 0; i < clients.Count; i++)
                {
                    //проверка на отключение
                    try
                    {
                        if (clients[i].Client.Connected)
                        {
                            var sw = new StreamWriter(clients[i].Client.GetStream());
                            sw.AutoFlush = true;
                            sw.WriteLine(message);
                        }
                        else
                        {
                            Console.WriteLine($"{clients[i].Name} disconnected");
                            clients.RemoveAt(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });
        }
    }
}
