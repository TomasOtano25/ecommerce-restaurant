namespace EcommerceRestaurant.Web.Helpers
{
    public interface IMailHelper
    {
        void SendEmail(string to, string subject, string body);
    }
}
