using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class Board{

    private static Color gridColor = new Color(0.69f,0.91f,0.922f);
    public const int ROW_OF_NODES = 16;
    public const int COLUMN_OF_NODES = 21;
    public List<Port> ports = new List<Port>();
    private List<Node> portNodes = new List<Node>();

    private Node[,] node;

    public Board() {
        node = new Node[ROW_OF_NODES,COLUMN_OF_NODES];
        initialize();
        setIsland();
        setAdjacentNodes();
    }

    public List<Port> getAllPorts() {
        return ports;
    }

    public List<Node> getAllPortNodes() {
        return portNodes;
    }

    public List<Node> getAllNodes() {
        List<Node> nodes = new List<Node>();
        for (int row = 0; row < ROW_OF_NODES; row++) {
            for (int col = 0; col < COLUMN_OF_NODES; col++) {
                Node toAdd = getNodeAt(row,col);
                if (toAdd != null) {
                    nodes.Add(getNodeAt(row,col));
                }
            }
        }
        return nodes;
    }

    public Node getNodeAt(int row,int col) {
        return node[row,col];
    }

    public Node getNodeAt(Vector2Int v) {
        return node[v.x,v.y];
    }

    private void initialize() {
        for (int row = 0; row < ROW_OF_NODES; row++) {
            for (int col = 0; col < COLUMN_OF_NODES; col++) {
                node[row,col] = new Node(row,col);
            }
        }
    }    

    private void setIsland() {

        TextAsset islandsFile = Resources.Load<TextAsset>("worldData/island");

        string islandsText = islandsFile.text;
        string[] islandsLines = Regex.Split(islandsText,"\n");
        foreach(string s in islandsLines){
            string[] coordinates = s.Split(',');
            int x = int.Parse(coordinates[0]);
            int y = int.Parse(coordinates[1]);
            getNodeAt(x,y).setIsland(true);
        }
    }


    private void setAdjacentNodes() {

        TextAsset adjacentFile = Resources.Load<TextAsset>("worldData/adjacentNodes");
        string adjacentText = adjacentFile.text;
        string[] adjacentLines = Regex.Split(adjacentText,"\n");
        foreach(string s in adjacentLines) {
            string[] delim = s.Split(':');
            string[] coordinates = delim[0].Split(',');
            string[] adjacentNodes = delim[1].Split(',');
            int x = int.Parse(coordinates[0]);
            int y = int.Parse(coordinates[1]);
            Node node = getNodeAt(x,y);
            for (int i = 0; i < adjacentNodes.Length; i++) {
                if (int.Parse(adjacentNodes[i]) == 1) {
                    int xDiff = Direction.DIRECTIONS[i][1];
                    int yDiff = Direction.DIRECTIONS[i][2];
                    Node adjacentNode = getNodeAt(node.getX() + xDiff,node.getY() + yDiff);
                    node.setAdjacentNode(i,adjacentNode);
                }
            }
        }
    }

    public void CreateGridVisuals() {
        GameObject parent;
        HashSet<EdgePair> renderedEdges = new HashSet<EdgePair>();
        if((parent = GameObject.Find("Nodes")) == null) {
            parent = GameObject.Instantiate(new GameObject());
            parent.name = "Nodes";
        }
        for (int x = 0; x < ROW_OF_NODES; x++) {
            for (int y = 0; y < COLUMN_OF_NODES; y++) {
                Node n = getNodeAt(x,y);
                if (!n.isIsland()) {
                    
                    GameObject node = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Node"));
                    node.name = (x + "," + y);
                    node.transform.parent = parent.transform;
                    node.transform.position = n.getRealPos();
                    node.GetComponent<SpriteRenderer>().color = gridColor;
                    n.setGameObject(node);

                    Node[] adjacents = n.getAdjacentNodes();
                    for (int i = 0; i < adjacents.Length; i++) {
                        EdgePair ep;
                        if (adjacents[i] != null && !renderedEdges.Contains(ep = new EdgePair(adjacents[i].getPosition(),n.getPosition()))) {
                            renderedEdges.Add(ep);
                            LineRenderer lr = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/NodeLine")).GetComponent<LineRenderer>();                         
                            Vector3 adjPos = new Vector3(adjacents[i].getY(),Board.ROW_OF_NODES - 1 - adjacents[i].getX(),0);
                            Vector3 pos = new Vector3(n.getY(),Board.ROW_OF_NODES - 1 - n.getX(),0);
                            lr.SetPosition(0,adjPos);
                            lr.SetPosition(1,pos);
                            lr.transform.SetParent(node.transform);
                            lr.startWidth = 0.02f;
                            lr.endWidth = 0.02f;
                            lr.startColor = gridColor;
                            lr.endColor = gridColor;
                        }
                    }
                }
            }
        }
    }

    // used to compare for duplicate edges for creating line renderers for grid lines
    private class EdgePair : IEquatable<EdgePair> {

        public Vector2 a;
        public Vector2 b;

        public EdgePair(Vector2 _a,Vector2 _b) {
            AssignValues(_a,_b);
        }

        public override bool Equals(object other) {
            if (other.GetType() != this.GetType()) { return false; }
            return Equals(other as EdgePair);
        }

        public bool Equals(EdgePair other) {
            return (other.a == a && other.b == b);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = 17;
                hashCode = (hashCode * 397) + a.GetHashCode();
                hashCode = (hashCode * 397) + b.GetHashCode();
                return hashCode;
            }
        }

        public void AssignValues(Vector2 v1,Vector2 v2) {
            if (v1.x < v2.x) {
                a = v1;
                b = v2;
            } else if (v1.x > v2.x) {
                a = v2;
                b = v1;
            } else if (v1.y < v2.y) {
                a = v1;
                b = v2;
            } else {
                a = v2;
                b = v1;
            }
        }
    }
}
