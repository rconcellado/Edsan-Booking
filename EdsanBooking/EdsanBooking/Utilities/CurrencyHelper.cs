namespace EdsanBooking.Utilities
{
    public static class CurrencyHelper
    {
        public static string FormatAsPeso(decimal amount)
        {
            return string.Format(new System.Globalization.CultureInfo("en-PH"), "{0:C}", amount);
        }
    }
}
