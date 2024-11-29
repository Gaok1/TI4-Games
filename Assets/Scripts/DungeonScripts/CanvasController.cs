using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject itemCollectedCanvas; // Canvas que será exibido quando o item for coletado
    private string itemKey = "ItemCollected"; // A mesma chave usada no ItemCollection

    void Start()
    {
        // Verifica o valor no PlayerPrefs ao iniciar o jogo
        if (PlayerPrefs.GetInt(itemKey, 0) == 1)  // 1 significa que o item foi coletado
        {
            ShowItemCollectedCanvas();
        }
    }

    // Função que mostra o canvas oculto
    void ShowItemCollectedCanvas()
    {
        if (itemCollectedCanvas != null)
        {
            itemCollectedCanvas.SetActive(true); // Ativa o canvas
        }
        else
        {
            Debug.LogWarning("O Canvas de item coletado não foi atribuído!");
        }
    }
}
