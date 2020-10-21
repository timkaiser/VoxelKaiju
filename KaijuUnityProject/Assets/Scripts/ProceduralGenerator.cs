using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using Management;

// This script generates the map. It starts by generating a graph in form of a grid, and then removes some edges. The graph is then transformed into a streetmap.
public class ProceduralGenerator : MonoBehaviour {
    /// <summary>
    ///These are graph nodes. They represent intersections. Connections (neighborhood) between two nodes means that there is a street between them.
    /// </summary>
    public struct Node {
        public int x, y;               //the nodes position on the grid
        public List<Node> neighbors;   //adjacent nodes
        public bool isError;           //true if node does not exist in street graph (needed since node cannot be null)

        /* Constructor:
         * IN: int x: x position on the grid 
         *     int y: y position on the grid    */
        public Node(int x, int y) {
            this.x = x;
            this.y = y;
            neighbors = new List<Node>(); //initialize list of adjacent nodes
            isError = false;
        }

        /* Adds connection between two nodes in both directions
         * IN: Node n: neighboring node    */
        public void addNeighbor(Node n) {
            neighbors.Add(n);
            n.neighbors.Add(this);
        }

        /* Removes connection between two nodes in both directions
         * IN: Node n: node whichs connection should be removed  */
        public void removeNeighbor(Node n) {
            neighbors.Remove(n);
            n.neighbors.Remove(this);
        }

        public Node getCopy()
        {
            Node tmp = new Node(this.x, this.y);
            tmp.isError = true;
            tmp.neighbors = new List<Node>(this.neighbors);
            return tmp;
        }

        /* This returns the name of the node. 
         * OUT: string: name         */
        public string getName() { return "" + x + y; }

        /* This checks if a path between this node and a target node exisits. This just calls existPathTo(Node target, List<Node> visited).
         * IN:  Node target: node we want to find a path to  
         * OUT: bool: true if path exists, false otherwise */
        public bool existPathTo(Node target) {
            List<Node> visited = new List<Node>();  //initiates list for already visited nodes of deep search algorithm
            return existPathTo(target, visited);
        }

        /* This checks if a path between this node and a target node exists using a recursive deep search algorithm
         * IN:  Node target: node we want to find a path to  
         *      List<Node> visited: list of nodes the algorithm has already visited
         * OUT: bool: true if path exisits, false otherwise */
        public bool existPathTo(Node target, List<Node> visited) {
            visited.Add(this);                                      // add this node to the visited list
            if (target.x == x && target.y == y) { return true; }    // return true if this is the node we are looking for
            foreach (Node n in neighbors) {                          // iterate neighbors
                if (!visited.Contains(n) && n.existPathTo(target, visited)) { return true; }   //if neighbor n is not visited, check if part from n to target exists
            }
            return false;   // if no path was found, return false
        }

        /* This generate a descriptor of this node in the DOT Language for visualizing graphs
         * IN: StringBuilder sb: Stringbuilder that contains the rest of the DOT graph descriptor. Helps speeding up the string concatination process */
        public void generateDotDescriptor(StringBuilder sb)
        {
            sb.Append("   ").Append(getName()).Append("[pos = \"").Append(x).Append(",").Append(y).AppendLine("!\"]");  // adds node name and position to descriptor
            foreach (Node n in neighbors)
            {                                                     // iterate neighbors
                if ((n.x >= x && n.y >= y) || (n.x < x && n.y > y))
                {                           // if neighbors position is smaller than own position
                    sb.Append("   ").Append(getName()).Append(" -- ").AppendLine(n.getName());  // add edge to descripter
                }
            }
        }

        /// <summary>
        /// String representation of the node. Includes x and y values.
        /// </summary>
        /// <returns>"Node" + x + y</returns>
        override
        public string ToString() {
            return "Node" + x + y;
        }

        public STREETCODE getCode() {
            switch (neighbors.Count) {
                case 1: return STREETCODE.DEAD_END;
                case 2: return (neighbors[0].x == neighbors[1].x || neighbors[0].y == neighbors[1].y) ? STREETCODE.STRAIGHT : STREETCODE.CURVE;
                case 3: return STREETCODE.T_INTERSECT;
                case 4: return STREETCODE.INTERSECT;
                default: return STREETCODE.PLAIN;
            }
        }

        public int getOrientation() {
            //  | ^ |   |   |   | ^ |_    _| ^ |_
            //  |___|   | ^ |   |_____    _______
            //          |   |   

            switch (getCode()) {
                case STREETCODE.DEAD_END:
                    //Debug.Log($"({x},{y}): {neighbors[0].x - x}|{neighbors[0].y - y}");
                    return diffToDeg(neighbors[0].x - x, neighbors[0].y - y);
                case STREETCODE.STRAIGHT: return diffToDeg(neighbors[0].x - x, neighbors[0].y - y);
                case STREETCODE.CURVE:
                    int a = diffToDeg(neighbors[0].x - x, neighbors[0].y - y);
                    int b = diffToDeg(neighbors[1].x - x, neighbors[1].y - y);
                    return (Mathf.Min(a, b) == 0 && Mathf.Max(a, b) == 270) ? 270 : Mathf.Min(a, b);
                case STREETCODE.T_INTERSECT:
                    //Debug.Log($"({x},{y}): {(neighbors[0].x + neighbors[1].x + neighbors[2].x) - 3 * x}|{((neighbors[0].y + neighbors[1].y + neighbors[2].y) - 3 * y)} = {diffToDeg((neighbors[0].x + neighbors[1].x + neighbors[2].x) - 3 * x, (neighbors[0].y + neighbors[1].y + neighbors[2].y) - 3 * y)}");
                    return diffToDeg((neighbors[0].x + neighbors[1].x + neighbors[2].x) - 3 * x, (neighbors[0].y + neighbors[1].y + neighbors[2].y) - 3 * y);
                case STREETCODE.INTERSECT:
                default: return 0;
            }
        }

        /// <summary>
        /// This methode takes a relative x,y position ((0,+1),(+1,0),(0,-1),(-1,0)) and transforms into an orientation (0, 90, 180, 270)
        /// </summary>
        public static int diffToDeg(int x, int z)
        {
            return (x == 0) ? (z > 0 ? 0 : 180) : (x > 0 ? 90 : 270);
        }

        /// <summary>
        /// Returns the direction in degrees (0, 90, 180, 270) where the neighbor node is located relative to the origin node.
        /// </summary>
        /// <param name="neighbor">The neighboring node you want to know the direction of.</param>
        /// <returns>Direction in degrees (left = 270, right = 90, up = 0, down = 180).</returns>
        public int getDirectionOfNeighbor(Node neighbor)
        {
            int x_rel = x - neighbor.x;
            int y_rel = y - neighbor.y;
            return (x_rel == 0) ? (y_rel < 0 ? 0 : 180) : (x_rel < 0 ? 90 : 270);
        }

        public Vector3 getOrientationToNeighbor(Node neighbor)
        {
            return new Vector3(neighbor.x - x, 0, neighbor.y - y);
        }

        /// <summary>
        /// Returns the neighbor of the node according to the direction given.
        /// </summary>
        /// <param name="dir">The direction (left = 270, right = 90, up = 0, down = 180) the neighbor should be located relative to the origin node.</param>
        /// <returns>The neighbor node of the node if found, a new node(-1,-1) otherwise.</returns>
        public Node getNeighborInDirection(int dir)
        {
            foreach (Node n in this.neighbors)
            {
                if (diffToDeg(n.x -x, n.y -y) == dir)
                    return n;
            }
            return new Node(-1, -1);
        }
        //0 0 ist 4,8 4,8 
        /// <summary>
        /// Calculates the Node center as world pos on a 2D plane.
        /// </summary>
        /// <returns>A 2D Vector where x is the Unity x direction and y is the unity.z direction. y is neglected as the tiles should always be centered at y=0.</returns>
        public Vector3 GetWorldPos()
        {
            float x_world = x * section_size * 3 + section_size;// + 0.5f * section_size - 0.5f; //3 * section_size + section_size due to the 2 straight sections in between the nodes
            float y_world = y * section_size * 3 + section_size;// + 0.5f * section_size - 0.5f; //0.5 * section size is the offset from left lower corner to middle
            return new Vector3(x_world, 0, y_world);
        }
    }

    /// <summary>
    /// These tiles represent 1 black (48x48 voxel) on the map. They are used in the map array
    /// </summary>
    public struct Tile {
        public float x, y;
        public float orientation;

        public STREETCODE type;
        public AREA area;

        public Tile(float x, float y, float orientation, STREETCODE type) {
            this.x = x; 
            this.y = y;
            this.orientation = orientation;

            this.area = AREA.CITY;
            this.type = type;
        }

        public void set(float x, float y, float orientation, STREETCODE type) {
            this.x = x;
            this.y = y;
            this.orientation = orientation;

            this.type = type;
        }
    }

    List<Node> streetgraph;                // contains all nodes of the graph representing the street layout
    
    //map settings
    public int width = 7, height = 5;      // size of the map
    public float edge_probability = 0.86f; // probability that there is a connection between to adjacent nodes
    public int num_parks = 5;
    public int radius_centralpark = 2;

    Tile[,] tilemap;

    public enum STREETCODE { STRAIGHT, CURVE, T_INTERSECT, INTERSECT, PLAIN, DEAD_END, BUILDINGS, BUILDINGS_CORNER };   //codes for street segments in array, each enum corresponds to a number from 1 to n
    public GameObject[] street_prefabs;

    public enum AREA { CITY, SUBURBS, HIGHRISERS, PARK, GOAL, GOAL_EMPTY, BEACH};
    public GameObject[][] tile_prefabs; //prefabs for tile

    public static float section_size;     // Size of a street section
    public static float street_size;

    [SerializeField]
    private GameObject map;

    // Start is called before the first frame update
    void Start()
    {
        section_size = Config.GetFloat(Config.Type.Generation, "cityblock_size");
        street_size = Config.GetFloat(Config.Type.Generation, "street_size");

        //load streets
        street_prefabs = new GameObject[STREETCODE.GetNames(typeof(STREETCODE)).Length];
        for (int i = 0; i < street_prefabs.Length; i++)
        {
            street_prefabs[i] = Resources.Load<GameObject>("prefabs/Tiles/STREETS/" + STREETCODE.GetNames(typeof(STREETCODE))[i]);
        }

        //load tils with buildings
        tile_prefabs = new GameObject[AREA.GetNames(typeof(AREA)).Length][];

        for(int i = 0; i < tile_prefabs.Length; i++) {
            string area = "" + AREA.GetNames(typeof(AREA))[i];
            tile_prefabs[i] = new GameObject[3];
            tile_prefabs[i][0] = Resources.Load<GameObject>("prefabs/Tiles/" + area + "_0");
            tile_prefabs[i][1] = Resources.Load<GameObject>("prefabs/Tiles/" + area + "_1");
            tile_prefabs[i][2] = Resources.Load<GameObject>("prefabs/Tiles/" + area + "_2");
        }
        //Debug.Log(Resources.LoadAll("prefabs/Tiles/City/Corner", typeof(GameObject)).Cast<GameObject>().ToArray().Length);
        Debug.Log(AREA.GetNames(typeof(AREA))[0]);
    }

    ///<summary>
    /// This methode generates a graph for the game. First it generates a grid shaped graph, then it removes a few edges without disconnection the graph. 
    /// </summary>
    void generateStreetGraph()
    {
        streetgraph = new List<Node>();     // initialize the node list

        // initialize nodes
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                streetgraph.Add(new Node(x, y));
            }
        }

        // initialize edges between adjacent nodes
        foreach (Node n in streetgraph)
        {
            if (n.x < width - 1) { n.addNeighbor(findNode(n.x + 1, n.y)); }
            if (n.y < height - 1) { n.addNeighbor(findNode(n.x, n.y + 1)); }
        }

        // remove some edges at random
        foreach (Node n1 in streetgraph)
        {
            // add edges which should be removed to removal list
            List<Node> remove = new List<Node>();
            foreach (Node n2 in n1.neighbors)
            {
                if (Random.value > edge_probability)
                {
                    remove.Add(n2);
                }
            }
            // remove edges if this doesnt disconnect the graph (speration between both steps necessary because of for each)
            foreach (Node n2 in remove)
            {
                n1.removeNeighbor(n2);
                if (!n1.existPathTo(n2))
                {
                    n1.addNeighbor(n2);
                }
            }
        }
        // print the DOT descriptor of the street graph for debugging purposes
        //Debug.Log(generateDotGraph());
    }


   // public Texture2D debug_tex;
    /// <summary>
    /// This methode uses the previously generated Streetgraph and translates it into an array
    /// </summary>
    void generateArray()
    {
        tilemap = new Tile[width * 3, height * 3];

        foreach (Node n in streetgraph)
        {
            int array_x = n.x * 3 + 1;
            int array_y = n.y * 3 + 1;

            float world_x = array_x * section_size;
            float world_y = array_y * section_size;


            //place intersection onto map
            tilemap[array_x, array_y].set(world_x, world_y, n.getOrientation(), n.getCode());


            //place plains on the corners
            tilemap[array_x - 1, array_y + 1].set(world_x - section_size, world_y + section_size,  90, STREETCODE.PLAIN);
            tilemap[array_x - 1, array_y - 1].set(world_x - section_size, world_y - section_size,   0, STREETCODE.PLAIN);
            tilemap[array_x + 1, array_y - 1].set(world_x + section_size, world_y - section_size, 270, STREETCODE.PLAIN);
            tilemap[array_x + 1, array_y + 1].set(world_x + section_size, world_y + section_size, 180, STREETCODE.PLAIN);

            //place adjacent streets
            for (int i = 0; i < 360; i += 90)
            {
                int x_rel = (i == 90) ? 1 : (i == 270) ? -1 : 0;
                int y_rel = (i == 0) ? 1 : (i == 180) ? -1 : 0;
                if (isStreet(n.getCode(), n.getOrientation(), i))
                {
                    tilemap[array_x + x_rel, array_y + y_rel].set(world_x + section_size * x_rel, world_y + section_size * y_rel, i, STREETCODE.STRAIGHT);
                } else {
                    tilemap[array_x + x_rel, array_y + y_rel].set(world_x + section_size * x_rel, world_y + section_size * y_rel, (i+180)%360, STREETCODE.PLAIN);
                }
            }
        }

        //place buildings on plain areas
        /*string s = "";
        for (int i = 0; i < tilemap.GetLength(0); i++) {
            for (int j = 0; j < tilemap.GetLength(1); j++) {
                if (tilemap[i, j].type == STREETCODE.PLAIN) {
                    s += getNumberAdjStreets(tilemap, i, j);
                } else {
                    s += "_";
                }
            }
            s += "\n";
        }

        Debug.Log(s);*/

        for (int i = 0; i < tilemap.GetLength(0); i++) {
            for (int j = 0; j < tilemap.GetLength(1); j++) {
                //Debug.Log($"[({i}/{j})({tilemap[i,j].x}/{tilemap[i, j].y})] -> {tilemap[i, j].type}({(int)(tilemap[i, j].type)})");
                if (tilemap[i,j].type == STREETCODE.PLAIN) {
                    int adj_str = getNumberAdjStreets(tilemap, i, j);
                    if (adj_str == 1) {
                        tilemap[i, j].type = STREETCODE.BUILDINGS;
                        correctOrientationForSingleStreetPlain(tilemap, i, j);
                    }
                    if (adj_str == 2) { tilemap[i, j].type = STREETCODE.BUILDINGS_CORNER; }
                }  
            }
        }

        string s = "";
        for (int i = 0; i < tilemap.GetLength(0); i++) {
            for (int j = 0; j < tilemap.GetLength(1); j++) {
                if (!isCode4Street(tilemap[i, j].type)) {
                    s += (int)tilemap[i, j].type;
                } else {
                    s += "_";
                }
            }
            s += "\n";
        }

        Debug.Log(s);
        /*Color[] c = { Color.white, Color.grey, Color.black, Color.green };
        debug_tex = new Texture2D(width * section_size * 3, height * section_size * 3);
        for (int i = 0; i < tilemap.GetLength(0); i++) {
            for (int j = 0; j < tilemap.GetLength(1); j++) {
                debug_tex.SetPixel(i, j, tilemap[i,j]. != PLACEABLES.NONE ? Color.red : c[(int)(tilemap[i, j].underground)]);
            }
        }

        debug_tex.Apply();*/
    }

    public Texture2D debug_tex;

    /// <summary>
    /// This methode assigns a area (defined in the enum AREA) to every cell of the array
    /// </summary>
    void setAreas() {
        //set beaches on borders
        for (int i = 0; i < tilemap.GetLength(0); i++) {
            tilemap[i, 0].area = AREA.BEACH;
            tilemap[i, tilemap.GetLength(1) - 1].area = AREA.BEACH;
        }
        for (int j = 1; j < tilemap.GetLength(1) - 1; j++) {
            tilemap[0, j].area = AREA.BEACH;
            tilemap[tilemap.GetLength(0) - 1, j].area = AREA.BEACH;
        }

        //set suburbs
        for (int i = 0; i < tilemap.GetLength(0) / 3; i++) {
            for (int j = 0; j < tilemap.GetLength(1); j++) {
                if (tilemap[i, j].area == AREA.CITY) {
                    tilemap[i, j].area = AREA.SUBURBS;
                }
            }
        }

        //place highrisers
        for (int i = (int)(tilemap.GetLength(0) * 0.45); i < tilemap.GetLength(0) * 0.85; i++) {
            for (int j = (int)(tilemap.GetLength(1) * 0.2); j < tilemap.GetLength(1) * 0.85; j++) {
                if (tilemap[i, j].area == AREA.CITY) tilemap[i, j].area = AREA.HIGHRISERS;
            }
        }

        //place goal
        int goal_x, goal_y;        

        do {
            goal_x = Random.Range((int)(tilemap.GetLength(0)*0.5), (int)(tilemap.GetLength(0)*0.8));
            goal_y = Random.Range((int)(tilemap.GetLength(1) * 0.25), (int)(tilemap.GetLength(1) * 0.8));
        } while (isCode4Street(tilemap[goal_x, goal_y].type));

        //find upper left corner (since goal needs 2x2 tiles)
        if (isCode4Street(tilemap[goal_x + 1, goal_y].type)){
            goal_x--;
        }
        if (isCode4Street(tilemap[goal_x, goal_y + 1].type)){
            goal_y--;
        }

        for (int i = Mathf.Max(0, goal_x - radius_centralpark); i <= Mathf.Min(goal_x + radius_centralpark, tilemap.GetLength(0) - 1); i++) {
            for (int j = Mathf.Max(0, goal_y - radius_centralpark); j <= Mathf.Min(goal_y + radius_centralpark, tilemap.GetLength(1) - 1); j++) {
                if (tilemap[i, j].area != AREA.BEACH) tilemap[i, j].area = AREA.PARK;
            }
        }

        tilemap[goal_x, goal_y].area = AREA.GOAL;
        tilemap[goal_x+1, goal_y].area = AREA.GOAL_EMPTY;
        tilemap[goal_x, goal_y+1].area = AREA.GOAL_EMPTY;
        tilemap[goal_x+1, goal_y+1].area = AREA.GOAL_EMPTY;
        tilemap[goal_x, goal_y].orientation = 180;

        //place parks
        for (int p = 0; p < num_parks; p++) {
            int park_x, park_y;
            do {
                park_x = Random.Range(2, tilemap.GetLength(0) - 2);
                park_y = Random.Range(2, tilemap.GetLength(1) - 2);
            } while (isCode4Street(tilemap[park_x, park_y].type));

            for (int i = Mathf.Max(0, park_x - 1); i <= Mathf.Min(park_x + 1, tilemap.GetLength(0) - 1); i++) {
                for (int j = Mathf.Max(0, park_y - 1); j <= Mathf.Min(park_y + 1, tilemap.GetLength(1) - 1); j++) {
                    if (tilemap[i, j].area != AREA.BEACH && tilemap[i, j].area != AREA.GOAL) tilemap[i, j].area = AREA.PARK;
                }
            }
        }


        //debug output CITY, SUBURBS, HIGHRISERS, PARK, GOAL, BEACH
        /*Texture2D debug_tex_temp = new Texture2D(tilemap.GetLength(0), tilemap.GetLength(1));
        Color[] c = { Color.cyan, Color.white, Color.blue, Color.green, Color.red, Color.yellow };
        for (int i = 0; i < tilemap.GetLength(0); i++) {
            for (int j = 0; j < tilemap.GetLength(1); j++) {
                debug_tex_temp.SetPixel(i, j,c[(int)tilemap[i,j].area]);
                if(isCode4Street(tilemap[i, j].type)) {
                    debug_tex_temp.SetPixel(i, j, Color.Lerp(Color.gray,c[(int)tilemap[i, j].area],0.7f));
                }
            }
        }
        debug_tex_temp.Apply();
        debug_tex = new Texture2D(tilemap.GetLength(0)*10, tilemap.GetLength(1)*10);

        for (int i = 0; i < tilemap.GetLength(0)*10; i++) {
            for (int j = 0; j < tilemap.GetLength(1)*10; j++) {
                debug_tex.SetPixel(i, j,debug_tex_temp.GetPixel((int)(i/10),(int)(j/10)));
            }
        }
        debug_tex.Apply();*/
    }

    /// <summary>
    /// This methode uses a previously genarated Map Layout and instantiates the necessary prefabs.
    /// </summary>
    void instantiateMap() {
        if(map != null) { Destroy(map); }
        map = new GameObject("Map");
        List<CarSpawner> spawners = new List<CarSpawner>();
        for (int i = 0; i < tilemap.GetLength(0); i++) {
            for (int j = 0; j < tilemap.GetLength(1); j++) {
                //Debug.Log($"[({i}/{j})({tilemap[i,j].x}/{tilemap[i, j].y})] -> {tilemap[i, j].type}({(int)(tilemap[i, j].type)})");
                if (isCode4Street(tilemap[i, j].type)){
                    GameObject obj = Instantiate(street_prefabs[(int)(tilemap[i, j].type)], new Vector3(tilemap[i, j].x, 0, tilemap[i, j].y), Quaternion.Euler(0, tilemap[i, j].orientation, 0));
                    obj.transform.SetParent(map.transform);
                    if (tilemap[i, j].type.Equals(STREETCODE.DEAD_END))
                    {
                        spawners.Add(obj.transform.Find("Spawner").GetComponent<CarSpawner>());
                    }             
                }
                else
                {
                    int area = (int)(tilemap[i, j].area);
                    int adj_str = getNumberAdjStreets(tilemap, i, j);
                    //fix corners in beaches
                    if(tilemap[i, j].area == AREA.BEACH && !((i==0 || i==tilemap.GetLength(0)-1) && (j == 0 || j == tilemap.GetLength(1)-1))) {
                        adj_str = 1;
                        tilemap[i, j].orientation = i == 0 ? 90 : j == 0 ? 0 : i == tilemap.GetLength(0) - 1 ? 270 : 180;
                    }
                    Instantiate(tile_prefabs[area][adj_str], new Vector3(tilemap[i, j].x, 0, tilemap[i, j].y), Quaternion.Euler(0, tilemap[i, j].orientation, 0)).transform.SetParent(map.transform);
                }
            }
        }
        map.AddComponent<TrafficManager>().Init(streetgraph);
        map.AddComponent<CarSpawnManager>().Init(this, spawners);
    }


    /* This finds a node at a specific position
     * IN:  int x: x position of searched node
     *      int y: y position of searched node
     * OUT: Node the algorithm is looking for, new Node with position (-1,-1) and isError flag set to true if it cannot be found    */
    public Node findNode(int x, int y)
    {
        foreach (Node n in streetgraph) { if (n.x == x && n.y == y) { return n; } }
        Node n_new = new Node(-1, -1);
        streetgraph.Add(n_new);
        return n_new;
    }

    public Node AmIonNode(float x_world, float z_world)
    {
        int x_node = Mathf.FloorToInt(x_world / section_size / 3);
        int y_node = Mathf.FloorToInt(z_world / section_size / 3);
        Node possibleNode = findNode(x_node, y_node);
        Vector3 nodeWorld = possibleNode.GetWorldPos();
        if (Mathf.Abs(x_world - nodeWorld.x) <= section_size && Mathf.Abs(z_world - nodeWorld.z) <= section_size)
        {
            return possibleNode;
        }
        else return new Node(-1, -1);
    }

    public Node findNode(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x - section_size) / (3 * section_size));
        int y = Mathf.RoundToInt((pos.z - section_size) / (3 * section_size));
        return findNode(x, y);
    }

    /* This generates a descriptor for the graph in the DOT graph visualisation language. 
     * Graph can be visualized on https://dreampuf.github.io/GraphvizOnline/ (use engine setting 'neato')
     * OUT: string: graph descriptor    */
    public string generateDotGraph()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("graph Streets{");
        foreach (Node n in streetgraph)
        {
            n.generateDotDescriptor(sb);
        }
        sb.Append("}");

        return sb.ToString();
    }

    public bool isStreet(STREETCODE sc, int orientation, int dir)
    {
        switch (sc)
        {
            case STREETCODE.INTERSECT: return true;
            case STREETCODE.T_INTERSECT: return (Mathf.Abs(orientation - dir) != 180);
            case STREETCODE.STRAIGHT: return (Mathf.Abs(orientation - dir) == 0 || Mathf.Abs(orientation - dir) == 180);
            case STREETCODE.CURVE:
                //Debug.Log(sc + " | " + orientation + ":" + dir);
                return (orientation == dir || (orientation + 90) % 360 == dir);
            case STREETCODE.DEAD_END: return orientation == dir;
            default: return false;
        }
    }

    public bool isCode4Street(STREETCODE s) {
        return s == STREETCODE.DEAD_END || s == STREETCODE.STRAIGHT || s == STREETCODE.CURVE || s == STREETCODE.T_INTERSECT || s == STREETCODE.INTERSECT;
    }

    public int getNumberAdjStreets(Tile[,] t, int x, int y) {
        //if (x <= 0 || y <= 0 || x >= t.GetLength(0) - 1 || y >= t.GetLength(1) - 1) return 0;
        return (x > 0                  ? isCode4Street(t[x - 1, y].type) ? 1 : 0 : 0)
             + (x < t.GetLength(0) - 1 ? isCode4Street(t[x + 1, y].type) ? 1 : 0 : 0)
             + (y > 0                  ? isCode4Street(t[x, y - 1].type) ? 1 : 0 : 0)
             + (y < t.GetLength(1) - 1 ? isCode4Street(t[x, y + 1].type) ? 1 : 0 : 0);
    }

    public void correctOrientationForSingleStreetPlain(Tile[,] t, int x, int y) {
        if (x > 0              && isCode4Street(t[x - 1, y].type)) { t[x, y].orientation = 270; return; }
        if (x < t.GetLength(0) && isCode4Street(t[x + 1, y].type)) { t[x, y].orientation = 90;  return; }
        if (y > 0              && isCode4Street(t[x, y - 1].type)) { t[x, y].orientation = 180; return; }
        if (y < t.GetLength(1) && isCode4Street(t[x, y + 1].type)) { t[x, y].orientation = 0;   return; }
    }

    /// <summary>
    /// Generates the Level according to the current settings.
    /// </summary>
    public void Generate()
    {
        //generate map
        generateStreetGraph();
        generateArray();
        setAreas();
        instantiateMap();
    }

    /// <summary>
    /// Defines the settings for the map generator.
    /// </summary>

    public void DefineMapSettings(ActiveLevel activeLevel)
    {
        width = activeLevel.width;
        height = activeLevel.height;
    }
}
