using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{

    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public Button seePasswordButton;
    public Button loginButton;
    public TMP_Text errorText;

    private bool encryptPassword = true;
    private bool logged = false;

    private Client client = new Client();
    private Player player = Player.instance;

    // Start is called before the first frame update
    void Start()
    {
        seePasswordButton.onClick.AddListener(TogglePasswordVisibility);
        loginButton.onClick.AddListener(ProcessLogin);
    }

    public void TogglePasswordVisibility()
    {
        if (encryptPassword == true)
        {
            encryptPassword = false;
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        } else
        {
            encryptPassword = true;
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
        }
    }

    public async void ProcessLogin()
    {
        var username = usernameInputField.text;
        var password = passwordInputField.text;

        LoginInfo result = await client.PostLogin(username, password);

        logged = player.PlayerLoginUpdate(result.GetUserId());

    }

    // Update is called once per frame
    void Update()
    {
        if (logged == true)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
