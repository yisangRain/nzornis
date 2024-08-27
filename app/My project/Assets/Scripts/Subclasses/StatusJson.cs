public class StatusJson
{

    public int ar_id { get; set; }
    public string status { get; set; }

    public StatusJson(int newId, string newStatus)
    {
        ar_id = newId;
        status = TranslateStatus(newStatus);
    }

    private string TranslateStatus(string stat)
    {
        string target = "Ready";
        switch (stat)
        {
            case "0":
                target = "Raw Image";
                break;
            case "1":
                target = "Raw Video";
                break;
            case "2":
                target = "Converting";
                break;
            default:
                break;
        }

        return target;
            
    }

}
