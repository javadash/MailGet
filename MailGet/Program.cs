using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MailGet {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Google API Sample: List MyLibrary");
            Console.WriteLine("================================");
            try {
                var mordo = new MailProvider().GetMail("me", "ea security code");
                mordo.Wait();
                var sparta = mordo.Result;
                var regex = new Regex(@"(\d{6})");
                var securityCode = regex.Match(sparta);
                Console.WriteLine("This is the code we want: {0}", securityCode);
            }
            catch (AggregateException ex) {
                foreach (var e in ex.InnerExceptions) {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}