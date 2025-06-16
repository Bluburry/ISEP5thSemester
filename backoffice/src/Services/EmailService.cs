using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc.Routing;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace AppServices
{
    static class EmailService
    {

        private static readonly string activationUrl = "https://localhost:5001/api/PasswordActivation";
        private static readonly string emailVerificationUrl = "https://localhost:5001/api/Users/ActivatePatientAccount";
        private static readonly string clientId = "no";
        private static readonly string clientSecret = "nope";
        private static readonly string refreshToken = "nopety";
        private static readonly string tokenEndpoint = "replaced";

        

        public static async void sendActivationEmail(string emailAddress, TokenDto pathAuthToken)
        {

            // Refresh access token
            string oAuthToken = await RefreshAccessTokenAsync(refreshToken, clientId, clientSecret);

            var request = new HttpRequestMessage(HttpMethod.Post, activationUrl);

        // Add headers
            request.Headers.Add("token", pathAuthToken.TokenId);

            // Create the email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hospital SEM5PI", "no@nope.com"));
            message.To.Add(new MailboxAddress(emailAddress, emailAddress));
            message.Subject = "NO-REPLY: Please Verify Your Password";

            // Convert the request to a string
            var requestString = $"{request.Method} {request.RequestUri}\nVersion: {request.Version}\nHeaders:\n";
            foreach (var header in request.Headers)
            {
                requestString += $"{header.Key}: {string.Join(", ", header.Value)}\n";
            }

            // Include the request details in the email body
            message.Body = new TextPart("plain") { Text = requestString + "\n\nTest Email --//3" };


            using (var client = new SmtpClient())
            {
                client.Connect("smtp.nope.com", 587, SecureSocketOptions.StartTls);

                // Authenticate using OAuth2
                var oauth2 = new SaslMechanismOAuth2("hospitalsem5pi@nope.com", oAuthToken);
                client.Authenticate(oauth2);

                client.Send(message);
                client.Disconnect(true);
            }
        }
        
        public static async void sendEmailVerificationEmail(string emailAddress, TokenDto pathAuthToken)
        {

            // Refresh access token
            string oAuthToken = await RefreshAccessTokenAsync(refreshToken, clientId, clientSecret);

            var request = new HttpRequestMessage(HttpMethod.Post, emailVerificationUrl);

        // Add headers
            request.Headers.Add("token", pathAuthToken.TokenId);

            // Create the email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hospital SEM5PI", "hospitalsem5pi@nope.com"));
            message.To.Add(new MailboxAddress(emailAddress, emailAddress));
            message.Subject = "NO-REPLY: Please Verify Your Email";

            // Include the request details in the email body
             message.Body = new TextPart("plain") { Text = "Thank you for registering in our platform. To verify your email, please use this code:\n" + pathAuthToken.TokenId};

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.nope.com", 587, SecureSocketOptions.StartTls);

                // Authenticate using OAuth2
                var oauth2 = new SaslMechanismOAuth2("hospitalsem5pi@nope.com", oAuthToken);
                client.Authenticate(oauth2);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public static async void sendContactConfirmation(string emailAddress, TokenDto pathAuthToken, string logID)
        {

            // Refresh access token
            string oAuthToken = await RefreshAccessTokenAsync(refreshToken, clientId, clientSecret);

            // Send email using the refreshed access token

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hospital SEM5PI", "hospitalsem5pi@nope.com"));
            message.To.Add(new MailboxAddress(emailAddress, emailAddress));
            message.Subject = "NO-REPLY: Contact Information Changed";
            message.Body = new TextPart("plain") { Text = "\n\nYour contact information has been changed. These changes needs to be accepted." };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.nope.com", 587, SecureSocketOptions.StartTls);

                // Authenticate using OAuth2
                var oauth2 = new SaslMechanismOAuth2("hospitalsem5pi@nope.com", oAuthToken);
                client.Authenticate(oauth2);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public static async void SendEmail(string emailAddress, string body)
        {

            // Refresh access token
            string oAuthToken = await RefreshAccessTokenAsync(refreshToken, clientId, clientSecret);

            // Send email using the refreshed access token

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hospital SEM5PI", "hospitalsem5pi@nope.com"));
            message.To.Add(new MailboxAddress(emailAddress, emailAddress));
            message.Subject = "NO-REPLY: Please Verify Your Password";
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.nope.com", 587, SecureSocketOptions.StartTls);

                // Authenticate using OAuth2
                var oauth2 = new SaslMechanismOAuth2("hospitalsem5pi@nope.com", oAuthToken);
                client.Authenticate(oauth2);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        private static async Task<string> RefreshAccessTokenAsync(string refreshToken, string clientId, string clientSecret)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var body = $"client_id={clientId}&client_secret={clientSecret}&refresh_token={refreshToken}&grant_type=refresh_token";
                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to refresh token: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var tokenData = JObject.Parse(content);

                return tokenData["access_token"].ToString();
            }
        }

        public static async void sendPatientUpdateNotification(string emailAddress, TokenDto token, PatientDto patientDto)
        {
            // Refresh access token
            string oAuthToken = await RefreshAccessTokenAsync(refreshToken, clientId, clientSecret);

            // Send email using the refreshed access token

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hospital SEM5PI", "hospitalsem5pi@nope.com"));
            message.To.Add(new MailboxAddress(emailAddress, emailAddress));
            message.Subject = "NO-REPLY: Please Confirm Patient File's Changes";
            message.Body = new TextPart("plain") { Text = "Someone is trying to change your patient file information to the following:" + "\n" + patientDto.ToString() + "\n\nTo confirm this changes, use this code: \n" + token.TokenId};

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.nope.com", 587, SecureSocketOptions.StartTls);

                // Authenticate using OAuth2
                var oauth2 = new SaslMechanismOAuth2("hospitalsem5pi@nope.com", oAuthToken);
                client.Authenticate(oauth2);

                client.Send(message);
                client.Disconnect(true);
            }            

        }

        public static async void sendDeletionConfirmation(string emailAddress, TokenDto token)
        {
            // Refresh access token
            string oAuthToken = await RefreshAccessTokenAsync(refreshToken, clientId, clientSecret);

            // Send email using the refreshed access token

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hospital SEM5PI", "hospitalsem5pi@nope.com"));
            message.To.Add(new MailboxAddress(emailAddress, emailAddress));
            message.Subject = "NO-REPLY: Please Confirm Patient File's Deletion";
            message.Body = new TextPart("plain") { Text = "Someone is trying to delete your patient file information.\nIf you wish to proceed, confirm with this code: \n" + token.TokenId };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.nope.com", 587, SecureSocketOptions.StartTls);

                // Authenticate using OAuth2
                var oauth2 = new SaslMechanismOAuth2("hospitalsem5pi@nope.com", oAuthToken);
                client.Authenticate(oauth2);

                client.Send(message);
                client.Disconnect(true);
            }            

        }
        
        
    }
}
