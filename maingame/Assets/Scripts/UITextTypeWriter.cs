using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// attach to UI Text component (with the full text already there)
public class UITextTypeWriter : MonoBehaviour
{
    public int MAX_TXT_LEN = 200;
    TMPro.TextMeshProUGUI txt;

    GameObject bobbingText;

    string story;
    
    public int textSpeed = 32;

    string mode = "none";

    void SetPlayerMoviment(bool active)
    {
        var entities =
            FindObjectsOfType<MonoBehaviour>().OfType<EntityController>();

        foreach (EntityController e in entities)
        {
            e.allowMoviment = active;
        }
    }

    void Awake()
    {
        try
        {
            bobbingText = GameObject.Find("BobbingText");
            bobbingText.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError("bobbingText not found");
        }
    }

    public void OnEnable()
    {
        bobbingText.SetActive(false);

        mode = "none";
        SetPlayerMoviment(false);

        txt = GetComponent<TMPro.TextMeshProUGUI>();

        // Debug.Log(txt);
        story = txt.text;
        txt.text = "";

        // TODO: add optional delay when to start
        StartCoroutine("PlayText");
    }

    IEnumerator PlayText()
    {
        mode = "write";
        bobbingText.SetActive(false);
        int substringEnd = story.IndexOf(" ", MAX_TXT_LEN > story.Length ? story.Length - 1 : MAX_TXT_LEN);
        
        foreach (char c in story.Substring(0, substringEnd != -1 ?  substringEnd : story.Length - 1))
        {
            txt.text += c;
            if (mode == "write")
            {
                yield return new WaitForSeconds(1f / textSpeed);
            }
        }

        bobbingText.SetActive(true);
        if (story.Length <= MAX_TXT_LEN && !story.Equals("What do you want to learn about?"))
            mode = "close";
        else
            mode = "show_more";
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (mode == "close")
            {
                GameObject.Find("AuroraTalkScene").SetActive(false);
                SetPlayerMoviment(true);
            }
            else if (mode == "write")
            {
                mode = "close";
            }else if (mode == "show_more")
            {
                int substringEnd = story.IndexOf(" ", MAX_TXT_LEN > story.Length ? story.Length - 1 : MAX_TXT_LEN);
                story = story.Remove(0, substringEnd != -1 ?  substringEnd : story.Length - 1);
                txt.text = "";
                StartCoroutine("PlayText");
            }
        }
    }
}
