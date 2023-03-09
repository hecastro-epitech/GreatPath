using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) BackToMenu();
    }
}
