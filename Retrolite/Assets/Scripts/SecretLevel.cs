using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretLevel : MonoBehaviour
{
    public int LevelNumber;
    private void Awake()
    {
        switch (LevelNumber){
            case 1:
                switch(Random.Range(0,5))
                {
                    case 0:
                        SceneManager.LoadScene(8);
                        break;
                    case 1:
                        SceneManager.LoadScene(9);
                        break;
                    case 2:
                        SceneManager.LoadScene(9);
                        break;
                }
                break;
            case 2:
                
                break;
            case 3:
                
                break;
        }

    }
}
