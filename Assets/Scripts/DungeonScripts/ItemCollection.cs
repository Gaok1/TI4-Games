using UnityEngine;
using UnityEngine.SceneManagement; // Adiciona o namespace para o gerenciamento de cenas

public class ItemCollection : MonoBehaviour
{
    public string playerPrefsKey = "ItemCollected"; // A chave que será usada no PlayerPrefs
    public string sceneToLoad = "NextScene"; // O nome da cena que será carregada após a coleta

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto colidido tem a tag "Player"
        if (collision.CompareTag("Player"))
        {
            CollectItem(); // Chama a função que coleta o item
        }
    }

    void CollectItem()
    {
        // Salva no PlayerPrefs que o item foi coletado
        PlayerPrefs.SetInt(playerPrefsKey, 1); // 1 significa que o item foi coletado
        PlayerPrefs.Save(); // Salva a informação no PlayerPrefs

        // Carrega a cena após a coleta do item
        LoadScene();
    }

    void LoadScene()
    {
        // Carrega a cena especificada pelo nome
        SceneManager.LoadScene(sceneToLoad);
    }
}
