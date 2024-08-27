using UnityEngine;
public class LoginInfo
{
    public enum Status
    {
        Success,
        Incorrect,
        Failed
    }
    string username { get; set; }
    string userId { get; set; }
    Status currentStatus;

    public LoginInfo() { }
    public LoginInfo(string newUsername, string newId)
    {
        username = newUsername;
        userId = newId;
        currentStatus = Status.Success;
    }

    public LoginInfo(Status newStatus)
    {
        currentStatus = newStatus;
    }

    public string GetUserId()
    {
        if (userId != null)
        {
            return userId;
        }
        Debug.Log("User ID is missing.");
        return "Fail";
    }

}

