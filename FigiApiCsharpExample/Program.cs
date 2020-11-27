using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Console = Colorful.Console;


namespace FigiApiCsharpExample
{
    class Program
    {
        [DllImport("user32.dll")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Get BB ID, by Kiril Spiridonov", Color.LightGreen);
            string identifier;
            string value;
            while (true)
            {
                Intro();
                string input = Console.ReadLine();
                input = input.ToLower();
                switch (input)
                {
                    case "1":
                        identifier = "TICKER";
                        System.Console.Write("Enter ticker value: ");
                        value = Console.ReadLine();
                        System.Console.WriteLine("Processing ...");
                        Request(identifier, value);

                        break;
                    case "2":
                        identifier = "ID_ISIN";
                        System.Console.Write("Enter ISIN value: ");
                        value = Console.ReadLine();
                        Request(identifier, value);



                        break;
                    case "0":
                    case "exit":
                        goto Foo;
                    case "browser":
                        System.Diagnostics.Process.Start("https://www.openfigi.com/");




                        break;




                    default: Console.WriteLine("Unexpected  command", Color.Red); break;
                }
            }


            Foo:
            System.Console.WriteLine("Exiting ...");

        }


        static void Request(string identifier, string value)
        {
            var sb = new StringBuilder();


            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var client = new RestClient("https://api.openfigi.com/v1/mapping");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("X-OPENFIGI-APIKEY", "");
            request.AddHeader("Content-Type", "text/json");
            var list = new List<OpenFIGIRequest>()
            {

                new OpenFIGIRequest(identifier, value)

            };

            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(list);

            var response = client.Post<List<OpenFIGIArrayResponse>>(request);

            foreach (var dataInstrument in response.Data)
                if (dataInstrument.Data != null && dataInstrument.Data.Any())
                    foreach (var instrument in dataInstrument.Data)
                    {
                        Console.WriteLine(instrument.figi + " " + instrument.Name + " " + instrument.SecurityDescription + " " + instrument.SecurityType + " " + instrument.TickerComplete + " " + instrument.Ticker);
                        sb.AppendLine(instrument.figi + " " + instrument.Name + " " + instrument.SecurityDescription + " " + instrument.SecurityType + " " + instrument.TickerComplete + " " + instrument.Ticker);
                    }


            
                            
            Console.WriteLine("------------------------------------------------------------", Color.Orange);
            //  System.Console.WriteLine("Where would you like to save the data?");
            try
            {
                Copy2Clipboard(sb.ToString());
                Console.WriteLine("Data copied to clipboard. Press CTR+V to paste anywhere.", Color.Green);

            }
            catch (Exception)
            {

                System.Console.WriteLine("Data did not successfully copied to clipboard.");
            }
            

           




        }


        static void Intro()
        {
            Console.WriteLine("If you wish to enter \"Browser Mode\" type \"browser\"");
            Console.WriteLine("If you wish to exit type \"exit\"");
            System.Console.WriteLine("Type of identifier you are going to search:");
            System.Console.WriteLine("1. Ticker");
            System.Console.WriteLine("2. ISIN");
            System.Console.WriteLine("0. Exit");
            System.Console.Write("Enter command: ");

        }

        static void Copy2Clipboard(string data)
        {
            OpenClipboard(IntPtr.Zero);
            var yourString = data;
            var ptr = Marshal.StringToHGlobalUni(yourString);
            SetClipboardData(13, ptr);
            CloseClipboard();
            Marshal.FreeHGlobal(ptr);

        }

       

    }
}
