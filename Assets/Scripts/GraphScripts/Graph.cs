using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

public enum RoomType
{
    Sala1, // 1 porta
    Sala2, // 2 portas
    Sala3, // 3 portas
    BossRoom // Sala do Boss
}

public class Graph
{
    public int Vertices;
    public List<Edge> Edges;
    public Dictionary<int, List<int>> adjacencyList;
    public List<int> roomMaxDegrees;
    private System.Random random;
    public int[] roomRotations;
    public RoomType[] roomTypes;
    private List<DoorDirection>[] roomUsedDoors;
    private GameObject[] rooms;

    public Graph(int vertices)
    {
        Vertices = vertices;
        Edges = new List<Edge>();
        adjacencyList = new Dictionary<int, List<int>>();
        roomMaxDegrees = new List<int>();
        random = new System.Random();
        roomRotations = new int[vertices];
        roomTypes = new RoomType[vertices];
        roomUsedDoors = new List<DoorDirection>[vertices];
        rooms = new GameObject[vertices]; // Inicializa o array com o tamanho correto

        for (int i = 0; i < vertices; i++)
        {
            adjacencyList[i] = new List<int>();
            roomUsedDoors[i] = new List<DoorDirection>();
            roomRotations[i] = -1;
            roomMaxDegrees.Add(3); // Ajuste conforme necessário
        }
    }


    public void AssignRoomTypes()
    {
        for (int i = 0; i < Vertices; i++)
        {
            int degree = adjacencyList[i].Count;

            if (i == Vertices - 1)
            {
                // Último índice é a sala do Boss
                roomTypes[i] = RoomType.BossRoom;
                roomMaxDegrees[i] = 1;
            }
            else
            {
                if (degree == 1)
                {
                    roomTypes[i] = RoomType.Sala1;
                    roomMaxDegrees[i] = 1;
                }
                else if (degree == 2)
                {
                    roomTypes[i] = RoomType.Sala2;
                    roomMaxDegrees[i] = 2;
                }
                else if (degree == 3)
                {
                    roomTypes[i] = RoomType.Sala3;
                    roomMaxDegrees[i] = 3;
                }
                else
                {
                    Debug.LogError($"Vertex {i} has an unexpected degree of {degree}");
                }
            }

            Debug.Log($"Assigned RoomType {roomTypes[i]} to vertex {i} with degree {degree}");
        }
    }
    public void UpdateRoomInstances(GameObject[] roomInstances)
    {
        rooms = roomInstances;
    }

   public void GenerateConnectedGraph()
    {
        // Atualize o número total de vértices para incluir a sala do Boss
        int bossRoomIndex = Vertices - 1; // O último índice será a sala do Boss
        int regularRoomCount = Vertices - 1; // Número de salas regulares

        int maxRetries = 100;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            Edges.Clear();
            foreach (var key in adjacencyList.Keys)
            {
                adjacencyList[key].Clear();
            }

            List<Edge> allPossibleEdges = new List<Edge>();
            for (int i = 0; i < regularRoomCount; i++)
            {
                for (int j = i + 1; j < regularRoomCount; j++)
                {
                    allPossibleEdges.Add(new Edge(i, j));
                }
            }

            Shuffle(allPossibleEdges);

            UnionFind uf = new UnionFind(regularRoomCount);
            List<Edge> spanningTree = new List<Edge>();
            int attemptsCount = 0;
            int maxAttempts = 1000;

            foreach (var edge in allPossibleEdges)
            {
                if (spanningTree.Count == regularRoomCount - 1)
                    break;

                if (attemptsCount >= maxAttempts)
                {
                    Debug.LogWarning($"Attempt {attempt + 1}: Maximum attempts reached during spanning tree construction.");
                    break;
                }

                int src = edge.Source;
                int dest = edge.Destination;

                if (uf.Find(src) != uf.Find(dest) &&
                    adjacencyList[src].Count < roomMaxDegrees[src] &&
                    adjacencyList[dest].Count < roomMaxDegrees[dest])
                {
                    uf.Union(src, dest);
                    AddEdge(src, dest);
                    spanningTree.Add(edge);
                }

                attemptsCount++;
            }

            if (spanningTree.Count == regularRoomCount - 1 && IsConnected(regularRoomCount))
            {
                Debug.Log($"Connected spanning tree generated successfully on attempt {attempt + 1}.");

                // Após gerar o grafo conectado, adicione a sala do Boss
                int nodeA = FindSuitableNodeForBossRoom();

                if (nodeA == -1)
                {
                    Debug.LogWarning("No suitable node found to connect the Boss Room. Retrying...");
                    continue;
                }

                // Adiciona uma aresta entre nodeA e a sala do Boss
                AddEdge(nodeA, bossRoomIndex);
                // adjacencyList[nodeA].Add(bossRoomIndex);
                // adjacencyList[bossRoomIndex].Add(nodeA);

                // Atualiza roomMaxDegrees e roomTypes
                roomMaxDegrees[nodeA] = adjacencyList[nodeA].Count; // Atualiza com o grau atual
                roomMaxDegrees[bossRoomIndex] = 1; // A sala do Boss tem apenas uma porta

                roomTypes[nodeA] = RoomType.Sala2; // Atualiza o tipo de sala para 2 portas
                roomTypes[bossRoomIndex] = RoomType.BossRoom;

                // Verifica o grau do nó após a atualização
                Debug.Log($"Nó {nodeA} conectado à sala do Boss. Novo grau: {adjacencyList[nodeA].Count}");

                // Atribui os tipos de sala
                AssignRoomTypes();

                return;
            }
            else
            {
                Debug.LogWarning($"Attempt {attempt + 1}: Failed to generate a valid connected graph. Trying again...");
            }
        }

        Debug.LogError("Failed to generate a connected graph after multiple attempts. Check degree constraints or the number of vertices.");
    }

    public int FindSuitableNodeForBossRoom()
    {
        int[] distances;
        BFS(0, out distances);

        int maxDistance = -1;
        int nodeA = -1;
        for (int i = 0; i < Vertices - 1; i++) // Exclui a sala do Boss
        {
            if (adjacencyList[i].Count == 1) // Procurar nós com grau 1 (extremidades)
            {
                if (distances[i] > maxDistance)
                {
                    maxDistance = distances[i];
                    nodeA = i;
                }
            }
        }
        return nodeA;
    }

    public void AlignAndConnectDoors(int startRoomIndex)
    {
        bool[] visited = new bool[Vertices];
        roomRotations[startRoomIndex] = 0; // Define a rotação inicial para a sala de início
        AlignAndConnectDoorsRecursive(startRoomIndex, visited);
    }


    private void BFS(int startNode, out int[] distances)
    {
        distances = new int[Vertices];
        for (int i = 0; i < Vertices; i++)
        {
            distances[i] = -1;
        }
        distances[startNode] = 0;

        Queue<int> queue = new Queue<int>();
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            foreach (int neighbor in adjacencyList[current])
            {
                if (distances[neighbor] == -1)
                {
                    distances[neighbor] = distances[current] + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }



    public int FindNodeFurthestFrom(int startNode)
    {
        int[] distances;
        BFS(startNode, out distances);

        int maxDistance = -1;
        int furthestNode = -1;
        for (int i = 0; i < Vertices; i++)
        {
            if (distances[i] > maxDistance)
            {
                maxDistance = distances[i];
                furthestNode = i;
            }
        }
        return furthestNode;
    }


    private void AlignAndConnectDoorsRecursive(int currentRoom, bool[] visited)
    {
        visited[currentRoom] = true;
        Debug.Log($"Aligning Room {currentRoom}, Rotation: {roomRotations[currentRoom] * 90} degrees");

        foreach (int neighbor in adjacencyList[currentRoom])
        {
            if (visited[neighbor])
                continue;

            Debug.Log($"Processing neighbor {neighbor} of Room {currentRoom}");

            // Obtenha uma porta não usada em currentRoom
            DoorDirection doorInCurrent = GetUnusedDoor(currentRoom);
            if (doorInCurrent == DoorDirection.None)
            {
                Debug.LogError($"Room {currentRoom} has no unused doors to connect to Room {neighbor}");
                continue;
            }

            // Direção oposta à porta usada em currentRoom
            DoorDirection requiredDoorInNeighbor = DirectionHelper.GetOppositeDirection(doorInCurrent);

            // Tente encontrar uma rotação para neighbor que alinhe a porta necessária
            int rotation = GetRotationForDoor(neighbor, requiredDoorInNeighbor);
            if (rotation == -1)
            {
                Debug.LogError($"Cannot rotate Room {neighbor} to align door {requiredDoorInNeighbor}");
                continue;
            }

            // Aplique a rotação lógica à sala neighbor
            roomRotations[neighbor] = rotation;

            // Atualize as direções das portas na sala neighbor
            UpdateDoorDirections(rooms[neighbor], rotation);

            // Rotaciona graficamente a sala neighbor
            RotateRoomGameObject(rooms[neighbor], rotation);

            Debug.Log($"Rotated Room {neighbor} to {rotation * 90} degrees to align door {requiredDoorInNeighbor}");

            // Marque as portas como usadas
            MarkDoorAsUsed(currentRoom, doorInCurrent);
            MarkDoorAsUsed(neighbor, requiredDoorInNeighbor);
            Debug.Log($"Marked doors as used: Room {currentRoom} - {doorInCurrent}, Room {neighbor} - {requiredDoorInNeighbor}");

            // Atualize os DoorTriggers para conectar as salas
            ConnectDoors(currentRoom, doorInCurrent, neighbor, requiredDoorInNeighbor);

            // Chame a função recursivamente para neighbor
            AlignAndConnectDoorsRecursive(neighbor, visited);
        }
    }



    private void RotateRoomGameObject(GameObject roomGO, int rotationIndex)
{
    // Calcula o ângulo de rotação em graus
    float rotationAngle = rotationIndex * 90f * -1;

    // Aplica a rotação ao transform da sala
    roomGO.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

    Debug.Log($"Rotated GameObject {roomGO.name} by {rotationAngle} degrees");
}



    private DoorDirection GetUnusedDoor(int roomIndex)
    {
        List<DoorDirection> availableDoors = GetRotatedDoors(roomIndex);
        foreach (var door in availableDoors)
        {
            if (!roomUsedDoors[roomIndex].Contains(door))
            {
                Debug.Log($"Room {roomIndex}: Unused door found - {door}");
                return door;
            }
        }
        Debug.LogError($"Room {roomIndex} has no unused doors");
        return DoorDirection.None;
    }


    private int GetRotationForDoor(int roomIndex, DoorDirection requiredDoor)
    {
        List<DoorDirection> originalDoors = GetOriginalDoors(roomIndex);

        for (int rotation = 0; rotation < 4; rotation++)
        {
            foreach (var door in originalDoors)
            {
                DoorDirection rotatedDoor = DirectionHelper.RotateDirection(door, rotation);
                if (rotatedDoor == requiredDoor)
                {
                    return rotation;
                }
            }
        }
        return -1; // Nenhuma rotação encontrada
    }

    private void UpdateDoorDirections(GameObject roomGO, int rotationIndex)
    {
        DoorTrigger[] doorTriggers = roomGO.GetComponentsInChildren<DoorTrigger>(true);
        foreach (var doorTrigger in doorTriggers)
        {
            DoorDirection originalDirection = doorTrigger.doorDirection;
            doorTrigger.currentDirection = DirectionHelper.RotateDirection(doorTrigger.doorDirection, rotationIndex);
            Debug.Log($"Room {roomGO.name}: Door {originalDirection} rotated by {rotationIndex * 90} degrees to {doorTrigger.currentDirection}");
        }
    }


    private void MarkDoorAsUsed(int roomIndex, DoorDirection door)
    {
        if (!roomUsedDoors[roomIndex].Contains(door))
        {
            roomUsedDoors[roomIndex].Add(door);
            Debug.Log($"Marked door {door} as used in Room {roomIndex}");
        }
    }

    // Graph.cs (Dentro da classe Graph)
    public void ConnectDoors(int roomAIndex, DoorDirection doorA, int roomBIndex, DoorDirection doorB)
    {
        GameObject roomA = rooms[roomAIndex];
        GameObject roomB = rooms[roomBIndex];

        DoorTrigger doorTriggerA = GetDoorTrigger(roomA, doorA);
        DoorTrigger doorTriggerB = GetDoorTrigger(roomB, doorB);

        if (doorTriggerA != null && doorTriggerB != null)
        {
            doorTriggerA.connectedRoomIndex = roomBIndex;
            doorTriggerB.connectedRoomIndex = roomAIndex;

            // Estabelece a referência mútua entre os DoorTriggers
            doorTriggerA.connectedDoorTrigger = doorTriggerB;
            doorTriggerB.connectedDoorTrigger = doorTriggerA;

            Debug.Log($"Conectou Sala {roomAIndex} (Porta {doorA}) com Sala {roomBIndex} (Porta {doorB})");
        }
        else
        {
            Debug.LogError($"Falha ao conectar as portas entre Sala {roomAIndex} e Sala {roomBIndex}");
        }
    }

    private DoorTrigger GetDoorTrigger(GameObject room, DoorDirection doorDirection)
    {
        DoorTrigger[] doorTriggers = room.GetComponentsInChildren<DoorTrigger>(true);
        foreach (var doorTrigger in doorTriggers)
        {
            if (doorTrigger.currentDirection == doorDirection)
            {
                Debug.Log($"Found DoorTrigger in {room.name} with direction {doorDirection}");
                return doorTrigger;
            }
        }
        Debug.LogError($"DoorTrigger with direction {doorDirection} not found in {room.name}");
        return null;
    }


    private List<DoorDirection> GetRotatedDoors(int roomIndex)
    {
        List<DoorDirection> originalDoors = GetOriginalDoors(roomIndex);
        List<DoorDirection> rotatedDoors = new List<DoorDirection>();
        int rotation = roomRotations[roomIndex];

        foreach (var door in originalDoors)
        {
            DoorDirection rotatedDoor = DirectionHelper.RotateDirection(door, rotation);
            rotatedDoors.Add(rotatedDoor);
        }

        return rotatedDoors;
    }

    private List<DoorDirection> GetOriginalDoors(int roomIndex)
    {
        if (roomTypes[roomIndex] == RoomType.Sala1)
        {
            return new List<DoorDirection> { DoorDirection.East };
        }
        else if (roomTypes[roomIndex] == RoomType.Sala2)
        {
            return new List<DoorDirection> { DoorDirection.East, DoorDirection.West };
        }
        else if (roomTypes[roomIndex] == RoomType.Sala3)
        {
            return new List<DoorDirection> { DoorDirection.East, DoorDirection.West, DoorDirection.South };
        }
        else if (roomTypes[roomIndex] == RoomType.BossRoom)
        {
            return new List<DoorDirection> { DoorDirection.South };
        }
        else
        {
            return new List<DoorDirection>();
        }
    }


    private void AddEdge(int source, int destination)
    {
        Edge newEdge = new Edge(source, destination);
        Edges.Add(newEdge);
        adjacencyList[source].Add(destination);
        adjacencyList[destination].Add(source);
        Debug.Log($"Added edge: {source} -- {destination}");
    }

    public bool IsConnected(int vertexCount)
    {
        bool[] visited = new bool[Vertices];
        DFS(0, visited);

        for (int i = 0; i < vertexCount; i++)
        {
            if (!visited[i])
                return false;
        }

        return true;
    }


    private void DFS(int vertex, bool[] visited)
    {
        visited[vertex] = true;

        foreach (int neighbor in adjacencyList[vertex])
        {
            if (!visited[neighbor])
                DFS(neighbor, visited);
        }
    }

    private void Shuffle(List<Edge> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Edge value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public string ToDotFormat()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("graph G {");
        sb.AppendLine("    node [shape=circle];");

        foreach (var edge in Edges)
        {
            sb.AppendLine($"    {edge.Source} -- {edge.Destination};");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    public void ExportToDotFile(string filePath)
    {
        string dotFormat = ToDotFormat();
        File.WriteAllText(filePath, dotFormat);
        Debug.Log($"Graph exported to DOT file: {filePath}");
    }

        public DoorDirection GetConnectingDoor(int roomAIndex, int roomBIndex)
    {
        foreach (var door in roomUsedDoors[roomAIndex])
        {
            DoorDirection oppositeDoor = DirectionHelper.GetOppositeDirection(door);
            if (roomUsedDoors[roomBIndex].Contains(oppositeDoor))
            {
                return door;
            }
        }
        return DoorDirection.None;
    }
    public bool VerifyAllDoorsAligned()
    {
        bool allAligned = true;
        foreach (var edge in Edges)
        {
            int roomA = edge.Source;
            int roomB = edge.Destination;

            DoorDirection doorA = GetConnectingDoor(roomA, roomB);
            DoorDirection doorB = DirectionHelper.GetOppositeDirection(doorA);

            if (!roomUsedDoors[roomA].Contains(doorA) || !roomUsedDoors[roomB].Contains(doorB))
            {
                Debug.LogError($"As portas entre a Sala {roomA} e a Sala {roomB} não estão alinhadas.");
                allAligned = false;
            }
            else
            {
                Debug.Log($"Portas entre a Sala {roomA} e a Sala {roomB} estão alinhadas: Sala {roomA} (Porta {doorA}), Sala {roomB} (Porta {doorB}).");
            }
        }
        return allAligned;
    }

}
