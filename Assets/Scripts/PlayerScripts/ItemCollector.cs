using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int cyanCount = 0;
    private int greenCount = 0;
    private int purpleCount = 0;

    private void Start()
    {
        // Carrega valores salvos (se houver)
        cyanCount = PlayerPrefs.GetInt("CyanCount", 0);
        greenCount = PlayerPrefs.GetInt("GreenCount", 0);
        purpleCount = PlayerPrefs.GetInt("PurpleCount", 0);

        // Atualiza a UI inicial
        UIManager.Instance.cyanText.text = cyanCount.ToString();
        UIManager.Instance.greenText.text = greenCount.ToString();
        UIManager.Instance.purpleText.text = purpleCount.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();

            if (item != null)
            {
                if (item.color == ItemColor.Cyan)
                {
                    cyanCount++;
                    UIManager.Instance.cyanText.text = cyanCount.ToString();
                }
                else if (item.color == ItemColor.Green)
                {
                    greenCount++;
                    UIManager.Instance.greenText.text = greenCount.ToString();
                }
                else if (item.color == ItemColor.Purple)
                {
                    purpleCount++;
                    UIManager.Instance.purpleText.text = purpleCount.ToString();
                }

                // Salva os valores
                PlayerPrefs.SetInt("CyanCount", cyanCount);
                PlayerPrefs.SetInt("GreenCount", greenCount);
                PlayerPrefs.SetInt("PurpleCount", purpleCount);
                PlayerPrefs.Save();

                Destroy(other.gameObject);
            }
        }
    }
}