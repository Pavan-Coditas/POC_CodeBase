namespace EmployeeManagment.Services.SmsSending
{
    public interface ISmsSender
    {
        string SendSms(string recipentNumber, string message);
    }
}
