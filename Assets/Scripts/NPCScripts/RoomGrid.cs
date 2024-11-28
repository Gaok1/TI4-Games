using System.Collections.Generic;
using UnityEngine;

public class RoomGrid : MonoBehaviour
{
    public LayerMask unwalkableMask; // Máscara para identificar obstáculos
    public float nodeRadius = 0.5f; // Raio de cada nó (pode ser ajustado conforme necessário)

    Node[,] grid;

    Bounds combinedBounds; // Bounds combinados dos colliders

    float nodeDiameter;
    int gridSizeX = 90, gridSizeY = 90;
    Vector2 gridWorldSize;

    // Variáveis para reposicionar a grade
    public Vector3 gridOffset = Vector3.zero; // Deslocamento da grade
    public bool useCustomGridOrigin = false; // Usar uma origem personalizada para a grade
    public Vector3 customGridOrigin = Vector3.zero; // Origem personalizada da grade

    void Awake()
    {
        // Calcula os bounds combinados de todos os Collider2D nos filhos da sala
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        if (colliders.Length > 0)
        {
            combinedBounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
            {
                combinedBounds.Encapsulate(colliders[i].bounds);
            }
        }
        else
        {
            Debug.LogError("Nenhum Collider2D encontrado nos filhos da sala. Certifique-se de que os Tilemaps têm Colliders.");
            return;
        }

        // Definimos o tamanho do mundo da grade como os bounds combinados
        gridWorldSize = new Vector2(combinedBounds.size.x, combinedBounds.size.y);

        // Calcula o diâmetro do nó com base no tamanho do mundo e no tamanho da grade
        nodeDiameter = gridWorldSize.x / gridSizeX;

        // Ajusta o nodeRadius de acordo com o nodeDiameter
        nodeRadius = nodeDiameter / 2f;

        CreateGrid();
    }

    public void ResetNodes()
    {
        if (grid == null)
            return;

        foreach (Node node in grid)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }
    }

    void CreateGrid()
    {
        if (nodeDiameter == 0)
        {
            Debug.LogError("nodeDiameter é zero. Verifique o cálculo do nodeDiameter.");
            return;
        }

        grid = new Node[gridSizeX, gridSizeY];

        // Posição do canto inferior esquerdo da grade
        Vector3 worldBottomLeft;

        if (useCustomGridOrigin)
        {
            // Usa a origem personalizada fornecida
            worldBottomLeft = customGridOrigin;
        }
        else
        {
            // Usa os bounds combinados e aplica o gridOffset
            worldBottomLeft = combinedBounds.min + gridOffset;
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calcular a posição no mundo para este nó
                float worldX = worldBottomLeft.x + x * nodeDiameter + nodeRadius;
                float worldY = worldBottomLeft.y + y * nodeDiameter + nodeRadius;
                Vector3 worldPoint = new Vector3(worldX, worldY, 0);

                // Determinar se este nó é caminhável (não há obstáculos)
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);

                // Criar o nó e adicioná-lo à grade
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        if (grid == null)
        {
            Debug.LogError("Grid não está inicializada.");
            return null;
        }

        float percentX, percentY;

        if (useCustomGridOrigin)
        {
            percentX = (worldPosition.x - customGridOrigin.x) / (nodeDiameter * gridSizeX);
            percentY = (worldPosition.y - customGridOrigin.y) / (nodeDiameter * gridSizeY);
        }
        else
        {
            percentX = (worldPosition.x - (combinedBounds.min.x + gridOffset.x)) / (nodeDiameter * gridSizeX);
            percentY = (worldPosition.y - (combinedBounds.min.y + gridOffset.y)) / (nodeDiameter * gridSizeY);
        }

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.RoundToInt((gridSizeX - 1) * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((gridSizeY - 1) * percentY), 0, gridSizeY - 1);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Define os deslocamentos para cima, baixo, esquerda e direita
        int[,] directions = new int[,]
        {
            { 0, 1 },  // Cima
            { 1, 0 },  // Direita
            { 0, -1 }, // Baixo
            { -1, 0 }  // Esquerda
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int checkX = node.gridPosition.x + directions[i, 0];
            int checkY = node.gridPosition.y + directions[i, 1];

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                neighbours.Add(grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
        else if (Application.isPlaying && combinedBounds.size != Vector3.zero)
        {
            // Desenha um quadrado representando a área da grade
            Vector3 gridCenter;

            if (useCustomGridOrigin)
            {
                gridCenter = customGridOrigin + new Vector3(nodeDiameter * gridSizeX / 2f, nodeDiameter * gridSizeY / 2f, 0);
            }
            else
            {
                gridCenter = (combinedBounds.min + gridOffset) + new Vector3(nodeDiameter * gridSizeX / 2f, nodeDiameter * gridSizeY / 2f, 0);
            }

            Vector3 gridSize = new Vector3(nodeDiameter * gridSizeX, nodeDiameter * gridSizeY, 1);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(gridCenter, gridSize);
        }
    }
}
