using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MailGet {
    class MailProvider {
        public async Task GetMail(String userId, String query) {

            // If modifying these scopes, delete your previously saved credentials at ~/.credentials/gmail-dotnet-quickstart.json

            string[] Scopes = {
                GmailService.Scope.GmailReadonly,
                //GmailService.Scope.MailGoogleCom,
                //GmailService.Scope.GmailModify
            };
            UserCredential credential;
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read)) {
                string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                    // user's account, but not other types of account access.
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }
            // Create Gmail API service
            var gmailService = new GmailService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            var emailListRequest = gmailService.Users.Messages.List(userId);
            emailListRequest.LabelIds = "INBOX";
            emailListRequest.Q = query; //only get unread;
            emailListRequest.MaxResults = 2; // only get 2 results

            //get our emails
            var emailListResponse = await emailListRequest.ExecuteAsync();

            if (emailListResponse != null && emailListResponse.Messages != null) {
                Console.WriteLine("there are {0} emails. press any key to continue!", emailListResponse.Messages.Count);
                    
                //then loop through each email and get what fields you want...
                foreach (var email in emailListResponse.Messages) {

                    var emailInfoRequest = gmailService.Users.Messages.Get("me", email.Id);
                    //make another request for that email id...
                    var emailInfoResponse = await emailInfoRequest.ExecuteAsync();

                    if (emailInfoResponse != null) {
                        String from = "";
                        String date = "";
                        String subject = "";
                        String body = "";
                        //loop through the headers and get the fields we need...
                        foreach (var mParts in emailInfoResponse.Payload.Headers) {
                            if (mParts.Name == "Date") {
                                date = mParts.Value;
                            } else if (mParts.Name == "From") {
                                from = mParts.Value;
                            } else if (mParts.Name == "Subject") {
                                subject = mParts.Value;
                            }

                            if (date != "" && from != "") {
                                if (emailInfoResponse.Payload.Parts == null && emailInfoResponse.Payload.Body != null) {
                                    body = DecodeBase64String(emailInfoResponse.Payload.Body.Data);
                                } else {
                                    body = GetNestedBodyParts(emailInfoResponse.Payload.Parts, "");
                                }

                                //now you have the data you want....
                            }

                        } 
                        //Console.Write(body);
                        Console.WriteLine("{0}  --  {1}  -- {2}", subject, date, email.Id);
                            
                    }

                }
            }

        }

        public async Task DeleteMail(GmailService service, String userId, String messageId) {
            await Task.Delay(100);
            try {
                service.Users.Messages.Delete(userId, messageId).Execute();
            }
            catch (Exception e) {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        static String DecodeBase64String(string s) {
            var ts = s.Replace("-", "+");
            ts = ts.Replace("_", "/");
            var bc = Convert.FromBase64String(ts);
            var tts = Encoding.UTF8.GetString(bc);

            return tts;
        }

        static String GetNestedBodyParts(IList<MessagePart> part, string curr) {
            string str = curr;
            if (part == null) {
                return str;
            } else {
                foreach (var parts in part) {
                    if (parts.Parts == null) {
                        if (parts.Body != null && parts.Body.Data != null) {
                            var ts = DecodeBase64String(parts.Body.Data);
                            str += ts;
                        }
                    } else {
                        return GetNestedBodyParts(parts.Parts, str);
                    }
                }

                return str;
            }

        }
    }
}