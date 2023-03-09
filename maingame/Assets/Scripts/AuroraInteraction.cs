using System.IO;
using System.Net;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

[System.Serializable]
public class AuroraMsg
{
    public string MESSAGE;
}

public class AuroraInteraction : MonoBehaviour
{
    public GameObject textPromt;
    public GameObject actions;

    private int initialState = 0;
    private List<string> states = new List<string> { "introduction", "prefight", "postfight", "preboss", "win", "death" };
    
    public void generateNewInfo(string state)
    {
        actions.SetActive(false);
        textPromt.GetComponent<TMPro.TextMeshProUGUI>().text = "Let me think a little...";
        Task<string> getTask = GetAsync("http://127.0.0.1:5000/getmsg/?mode="+ state);
        getTask.GetAwaiter().OnCompleted(() =>
       {
           AuroraMsg result = UnityEngine.JsonUtility.FromJson<AuroraMsg>(getTask.Result);
           textPromt.GetComponent<TMPro.TextMeshProUGUI>().text = result.MESSAGE;
           textPromt.GetComponent<UITextTypeWriter>().OnEnable();
       });
    }

    public void PromptChoice() {
        textPromt.GetComponent<TMPro.TextMeshProUGUI>().text = "What do you want to learn about?";
        actions.SetActive(true);
    }

    public string Get(string uri)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
    public async Task<string> GetAsync(string uri)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            return await reader.ReadToEndAsync();
        }
    }
}
