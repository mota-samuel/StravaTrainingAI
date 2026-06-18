namespace Domain.Extension;
public static class TimeSpanExtension
{
    public static string ToTimeString(this TimeSpan ts) 
    {
        if(ts.TotalHours >= 1)
        {
            return $"{(int)ts.TotalHours}h{ts.Minutes:D2}m";
        }
        else
        {
            return $"{ts.Minutes}m{ts.Seconds:D2}s";
        }
    }
}
