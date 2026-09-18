namespace GolBet.Web.Helpers;

public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo ColombiaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");

    public static DateTime ToColombiaTime(this DateTime utcDate)
        => TimeZoneInfo.ConvertTimeFromUtc(utcDate, ColombiaZone);
}