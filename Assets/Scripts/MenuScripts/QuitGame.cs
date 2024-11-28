using UnityEngine;
using UnityEngine.SceneManagement; // Se você quiser carregar uma nova cena ao invés de sair

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else

        Application.Quit();
#endif
    }

    // Função que é chamada quando o aplicativo é fechado
    private void OnApplicationQuit()
    {
        // Remove as chaves específicas para os itens salvos nos PlayerPrefs
        PlayerPrefs.DeleteKey("CyanCount");
        PlayerPrefs.DeleteKey("GreenCount");
        PlayerPrefs.DeleteKey("PurpleCount");
        PlayerPrefs.Save(); // Salva as alterações
    }
}