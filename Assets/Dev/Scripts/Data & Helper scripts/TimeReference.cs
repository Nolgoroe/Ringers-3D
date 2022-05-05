using System;

public static class TimeReferenceDataScript //It is not inheriting from MonoBehavior!
{
    private static DateTime dt;
    private static bool isRunning = false;

    //Save current DateTime when user did the action
    public static void Start()
    {
        if (!isRunning)
        {
            dt = DateTime.Now;
            isRunning = true;
        }
    }

    public static void Reset()
    {
        isRunning = false;
    }

    // This gets the actual time in String value
    public static TimeSpan GetTimeElapsed()
    {
        if (!isRunning)
        {
            return TimeSpan.Zero; //Not running, return some default
        }

        TimeSpan elapsedTime = (DateTime.Now - dt);
        //return $"{elapsedTime:mm\\:ss}";
        return elapsedTime;
    }
}