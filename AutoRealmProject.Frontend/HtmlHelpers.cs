using System.Globalization;

namespace AutoRealmProject.Frontend
{
    public static class HtmlHelpers
    {
        public static string FormatPrice(decimal price)
        {
            var usCulture = new CultureInfo("en-US");
            return string.Format(usCulture, "{0:C}", price);
        }
        public static string FormatMileage(int mileage)
        {
            return $"{mileage:N0} km";
        }
    }
}
