using UnityEngine;
using UnityEngine.SceneManagement;  // Para carregar a cena

public class SceneChanger : MonoBehaviour
{
    public string sceneName = "NewScene";  // Nome da cena para a qual você deseja mudar (substitua "NewScene" pelo nome real da sua cena)
    public float timeToWait = 30f;        // Tempo em segundos antes de trocar a cena

    private float timer = 0f;

    void Update()
    {
        // Incrementa o timer de acordo com o tempo que passou
        timer += Time.deltaTime;

        // Quando o timer atingir o tempo definido, mude para a nova cena
        if (timer >= timeToWait)
        {
            ChangeScene();
        }
    }

    void ChangeScene()
    {
        // Carrega a cena especificada
        SceneManager.LoadScene(sceneName);
    }
}
