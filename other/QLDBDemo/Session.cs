public class Session
{
    public string SessionName { get; set; }

    public string Venue { get; set; }

    public int Duration { get; set; }

    public override string ToString()
    {
        return $"{SessionName} - {Venue} : {Duration} minutes.";
    }
}