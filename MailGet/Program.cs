using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailGet {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Google API Sample: List MyLibrary");
            Console.WriteLine("================================");
            try {
                new MailProvider().getEmails("me", "ea security code").Wait();
            }
            catch (AggregateException ex) {
                foreach (var e in ex.InnerExceptions) {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }

            Console.WriteLine("GetFolderPath: {0}", Environment.GetFolderPath(Environment.SpecialFolder.Personal));

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}