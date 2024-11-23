using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject thisScreen;
    public GameObject mainmenuScreen;

    public void BacktoMenu()
    {
        thisScreen.SetActive(false);
        mainmenuScreen.SetActive(true);
    }
}
