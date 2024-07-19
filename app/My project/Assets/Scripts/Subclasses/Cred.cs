namespace Assets.Scripts.Subclasses
{
    class Cred
    {
        string username { get; set; }
        string password { get; set; }

        public Cred (string newUsername, string newPassword)
        {
            username = newUsername;
            password = newPassword;
        }
    }
}
