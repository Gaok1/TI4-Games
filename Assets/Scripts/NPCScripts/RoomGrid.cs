using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGrid : MonoBehaviour
{
    public LayerMask unwalkableMask; // Máscara para identificar obstáculos
    public float nodeRadius = 0.5f; // Raio de cada nó (pode ser ajustado conforme necessário)

    [HideInInspector]
    public Node[,] grid;

    Bounds combinedBounds; // Bounds combinados dos colliders

    float nodeDiameter;
    public int gridSizeX = 90; // Número de colunas
    public int gridSizeY = 90; // Número de linhas
    Vector2 gridWorldSize;

    // Variáveis para reposicionar a grade
    public Vector3 gridOffset = Vector3.zero; // Deslocamento da grade
    public bool useCustomGridOrigin = false; // Usar uma origem personalizada para a grade
    public Vector3 customGridOrigin = Vector3.zero; // Origem personalizada da grade

    void Start()
    {
        GenerateGrid();
        StartCoroutine(UpdateGridObstaclesRoutine());
    }

    // Método chamado quando uma variável é alterada no editor
    void OnValidate()
    {
        // Evita erro de execução no editor quando o jogo está em execução
        if (!Application.isPlaying)
        {
            GenerateGrid();
        }
    }

    public void GenerateGrid()
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

        // Calcula o diâmetro do nó
        nodeDiameter = nodeRadius * 2f;

        if (nodeDiameter == 0)
        {
            Debug.LogError("nodeDiameter é zero. Defina nodeRadius para um valor maior que zero.");
            return;
        }

        // Definimos o tamanho do mundo da grade com base no tamanho do nó e no tamanho da grade
        gridWorldSize = new Vector2(nodeDiameter * gridSizeX, nodeDiameter * gridSizeY);

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
        if (gridSizeX <= 0 || gridSizeY <= 0)
        {
            Debug.LogError("gridSizeX ou gridSizeY é menor ou igual a zero. Defina valores maiores que zero.");
            return;
        }

        grid = new Node[gridSizeX, gridSizeY];

        // Posição do canto inferior esquerdo da grade em espaço local
        Vector3 localBottomLeft;

        if (useCustomGridOrigin)
        {
            // Usa a origem personalizada fornecida (em espaço local)
            localBottomLeft = customGridOrigin;
        }
        else
        {
            // Usa os bounds combinados e aplica o gridOffset (em espaço local)
            localBottomLeft = combinedBounds.min + gridOffset - transform.position;
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calcular a posição local para este nó
                float localX = localBottomLeft.x + x * nodeDiameter + nodeRadius;
                float localY = localBottomLeft.y + y * nodeDiameter + nodeRadius;
                Vector3 localPoint = new Vector3(localX, localY, 0);

                // Converter a posição local para posição global
                Vector3 worldPoint = transform.TransformPoint(localPoint);

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

        // Converter a posição do mundo para espaço local do room
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        float percentX, percentY;

        if (useCustomGridOrigin)
        {
            percentX = (localPosition.x - customGridOrigin.x) / (nodeDiameter * gridSizeX);
            percentY = (localPosition.y - customGridOrigin.y) / (nodeDiameter * gridSizeY);
        }
        else
        {
            percentX = (localPosition.x - (combinedBounds.min.x + gridOffset.x - transform.position.x)) / (nodeDiameter * gridSizeX);
            percentY = (localPosition.y - (combinedBounds.min.y + gridOffset.y - transform.position.y)) / (nodeDiameter * gridSizeY);
        }

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt(gridSizeX * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(gridSizeY * percentY), 0, gridSizeY - 1);

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
        // Desenha a grade
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? new Color(1, 1, 1, 0.3f) : new Color(1, 0, 0, 0.3f);
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    // Coroutine para atualizar os obstáculos na grade a cada 1 segundo
    IEnumerator UpdateGridObstaclesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Espera 1 segundo

            UpdateObstacles();
        }
    }

    // Método para atualizar a walkability dos nós com base nos obstáculos atuais
    void UpdateObstacles()
    {
        if (grid == null)
            return;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node currentNode = grid[x, y];
                if (currentNode == null)
                    continue;

                // Determinar se este nó é caminhável (não há obstáculos)
                bool walkable = !Physics2D.OverlapCircle(currentNode.worldPosition, nodeRadius, unwalkableMask);

                // Atualizar a propriedade walkable somente se houve mudança
                if (currentNode.walkable != walkable)
                {
                    currentNode.walkable = walkable;
                }
            }
        }

        // Opcional: Forçar a atualização dos Gizmos no editor
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
