public class StatusJson
{
    public int ar_id { get; set; }
    public string status { get; set; }

    public StatusJson(int newId, string newStatus)
    {
        ar_id = newId;
        status = newStatus;
    }

}
