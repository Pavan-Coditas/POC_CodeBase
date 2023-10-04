using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace EmployeeManagment.Services.SmsSending
{
    public class SmsSender : ISmsSender
    {
        public string SendSms(string recipentNumber, string messageBody)
        {
            const string accountSid = "ACd88c73462fb3aef826f10fad3c27eb13";
            const string authToken = "edaddf56f7badb3b479b137c7718233a";
            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: messageBody,
                from: new Twilio.Types.PhoneNumber("(607) 317-7308"),
                to: new Twilio.Types.PhoneNumber(recipentNumber)
            );
            Console.WriteLine(message.Sid);
            return message.Status.ToString();
        }
    }
}
