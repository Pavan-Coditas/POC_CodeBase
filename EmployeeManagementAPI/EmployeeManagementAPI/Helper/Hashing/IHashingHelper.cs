namespace EmployeeManagement.Api.Helper.Hashing
{
    public interface IHashingHelper
    {
        string OneWayHash(string password);
        bool VerifyHash(string password, string userPassword);
    }
}
