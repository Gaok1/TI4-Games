using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform target; // Referência ao jogador
    private List<Node> path;
    private int currentPathIndex;
    public RoomGrid roomGrid; // Variável pública para receber o RoomGrid
    public float speed = 5f;
    public float pathUpdateInterval = 0.2f; // Intervalo para atualizar o caminho
    
    private float pathUpdateTimer = 0f;

    // Variáveis para animação
    private Animator animator;
    private Vector2 movement;
    private int direction;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (roomGrid == null)
        {
            Debug.LogError("EnemyAI não tem uma referência ao RoomGrid.");
            enabled = false;
            return;
        }

        // Inicializa o Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator não encontrado no inimigo.");
        }

        FindPath();
    }

    void Update()
    {
        if (target == null || roomGrid == null)
            return;

        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            pathUpdateTimer = 0f;
            FindPath();
        }

        MoveAlongPath();
    }

    void FindPath()
    {
        if (roomGrid == null)
            return;

        roomGrid.ResetNodes();

        Node startNode = roomGrid.NodeFromWorldPoint(transform.position);
        Node targetNode = roomGrid.NodeFromWorldPoint(target.position);

        if (!startNode.walkable || !targetNode.walkable)
        {
            if (!startNode.walkable)
            {
                Debug.LogError("Nó inicial não é caminhável.");
            }

            if (!targetNode.walkable)
            {
                Debug.LogError("Nó de destino não é caminhável.");
            }
            return;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Ordena o openSet com base no fCost
            openSet.Sort((nodeA, nodeB) => nodeA.fCost.CompareTo(nodeB.fCost));

            Node currentNode = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in roomGrid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int tentativeGCost = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (tentativeGCost < neighbour.gCost)
                {
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        currentPathIndex = 0;
    }

    void MoveAlongPath()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            // Se não houver caminho, definir IsMoving para false
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
            return;
        }

        Vector3 targetPosition = path[currentPathIndex].worldPosition;

        // Calcula o vetor de movimento
        Vector3 movementVector = (targetPosition - transform.position).normalized;

        // Move o inimigo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Atualiza a animação
        UpdateAnimation(movementVector);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }
    }

    void UpdateAnimation(Vector3 movementVector)
    {
        if (animator == null)
            return;

        // Determina a direção com base no vetor de movimento
        if (movementVector.x < 0)
            direction = 3; // Esquerda
        else if (movementVector.x > 0)
            direction = 2; // Direita
        else if (movementVector.y > 0)
            direction = 1; // Cima
        else if (movementVector.y < 0)
            direction = 0; // Baixo

        // Atualiza os parâmetros do Animator
        animator.SetInteger("Direction", direction);
        animator.SetBool("IsMoving", movementVector.magnitude > 0);
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        return 10 * (dstX + dstY); // Usando distância Manhattan
    }
}
