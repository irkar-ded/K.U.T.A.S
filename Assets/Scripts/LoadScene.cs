using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public void onLoad(string name) => LoadingScreen.LoadScene(name);
    public void exitGame() => Application.Quit();
}
