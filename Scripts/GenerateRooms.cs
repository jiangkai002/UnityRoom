using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using Sebastian.Geometry;

public class GenerateRooms 
{
    private int[] lines;
    private string json;
    public LineRenderer line;
    public List<int> indexes;
    public Vector3[] normals;
    public Shape shape;
    public static Material material;


    public static List<RoomBoundary> roomBoundarys = new List<RoomBoundary>();
    public List<string> Points = new List<string>();
    // Start is called before the first frame update
    void Start()
    {

        json = File.ReadAllText(Application.dataPath + "\\Resources\\RoomBoundary.json");

        roomBoundarys = JsonConvert.DeserializeObject<List<RoomBoundary>>(json);

        //for(int i=0;i<1;i++)
        //{
        for (int i = 0; i < roomBoundarys.Count(); i++)
        {
            List<Vector3> vectors = new List<Vector3>();
            var vectosRemoveRepeat = new List<Vector3>();
            vectors = GenerateVectors(roomBoundarys[i].Boundary);


            int count = 0;
            int count2 = 0;
            // count = RemoveRepeat(vectors).Count;
            //count2 = RemoveRepeat(vectors).Count;
            //while (count2 != count)
            //{
            //    count = RemoveRepeat(vectors).Count;
            //    count2 = RemoveRepeat(vectors).Count;
            //}

            vectosRemoveRepeat = RemoveRepeat(vectors);

            if ((vectors.Count() - vectosRemoveRepeat.Count()) < 10)
            {

                Debug.Log(vectors.Count() + "炸弹" + roomBoundarys[i].ElementId);

                CreateMesh(vectors);
                //GeneratePoints(vectors);
                //DrawLine(vectors.ToArray(), line);
                //GeneratePoints(vectors);
                //DrawLine(vectors.ToArray(), line);
            }

            //}

            else
            {
                Debug.Log(vectosRemoveRepeat.Count() + "除重炸弹" + roomBoundarys[i].ElementId);

                CreateMesh(vectosRemoveRepeat);
                //GeneratePoints(vectosRemoveRepeat);
                //DrawLine(vectors.ToArray(), line);

            }
        }
    }
    /// <summary>
    /// 生成房间体块顺带返回RoomModel类
    /// </summary>
    /// <param name="roomBoundaries"></param>
    public static void CreateRoom(List<RoomBoundary> roomBoundaries)
    {

        for (int i = 0; i < roomBoundarys.Count(); i++)
        {
            List<Vector3> vectors = new List<Vector3>();
            var vectosRemoveRepeat = new List<Vector3>();
            vectors = GenerateVectors(roomBoundarys[i].Boundary);

            vectosRemoveRepeat = RemoveRepeat(vectors);

            if ((vectors.Count() - vectosRemoveRepeat.Count()) < 10)
            {

                Debug.Log(vectors.Count() + "炸弹" + roomBoundarys[i].ElementId);

                CreateMesh(vectors);
                //GeneratePoints(vectors);
                //DrawLine(vectors.ToArray(), line);
                //GeneratePoints(vectors);
                //DrawLine(vectors.ToArray(), line);
            }
            else
            {
                Debug.Log(vectosRemoveRepeat.Count() + "除重炸弹" + roomBoundarys[i].ElementId);

                CreateMesh(vectosRemoveRepeat);
                //GeneratePoints(vectosRemoveRepeat);
                //DrawLine(vectors.ToArray(), line);

            }
        }

    }


    public static List<Vector3> RemoveRepeat(List<Vector3> points)
    {
        List<Vector3> Result = new List<Vector3>();
        List<Vector3> RepeatPoints = new List<Vector3>();
        List<Vector3> fPoints = points;
        for (int i = 1; i < points.Count; i++)
        {

            if (CalculateDistance(points[i - 1], points[i]))
            {
                RepeatPoints.Add(points[i]);
            }
        }


        foreach (var a in RepeatPoints)
        {
            fPoints.Remove(a);
        }
        return fPoints;
    }

    public static bool CalculateDistance(Vector3 point1, Vector3 point2)
    {
        var distance = Vector3.Distance(point1, point2);
        return distance < 0.1;
    }

    /// <summary>
    ///生成所有的点
    /// </summary>
    /// <param name="points"></param>
    public void GeneratePoints(List<Vector3> vectors)
    {
        for (int i = 0; i < vectors.Count; i++)
        //foreach (var singlePoint in vectors)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Cube);
            point.transform.position = new Vector3(vectors[i].x, 0, vectors[i].z);
            point.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            point.name = i.ToString();
        }
    }
    public static List<Vector3> GenerateVectors(string points)
    {
        List<Vector3> vectors = new List<Vector3>();
        List<string> singlePoints = points.Split('|').ToList();
        foreach (var singlePoint in singlePoints)
        {
            double x;
            double z;
            if (singlePoint.Split(',').Length == 2)
            {
                double.TryParse(singlePoint.Split(',')[0], out x);
                double.TryParse(singlePoint.Split(',')[1], out z);
                x = x * 0.001;
                z = z * 0.001;
                vectors.Add(new Vector3((float)x, 0, (float)z));
            }
        }
        return vectors;
    }
    public void DrawLine(Vector3[] points, LineRenderer line)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            LineRenderer singleline = GameObject.Instantiate(line);
            singleline.SetPosition(0, points[i]);
            singleline.SetPosition(1, points[i + 1]);
        }
        LineRenderer lastLine = GameObject.Instantiate(line);
        lastLine.SetPosition(0, points.Last());
        lastLine.SetPosition(1, points[0]);
    }
    public static void CreateMesh(List<Vector3> points)
    {
        List<Vector3> points1 = points;

        List<Vector3> points2 = points1.Select(p => p = new Vector3(p.x, p.y + 1000, p.z)).ToList();

        GameObject M = new GameObject("上面");
        GameObject M2 = new GameObject("下面");
        GameObject M3 = new GameObject("合体");
        List<Shape> shapes = new List<Shape>();
        List<Shape> shapes1 = new List<Shape>();
        Shape shape = new Shape();
        Shape shape1 = new Shape();
        shape.points = points1;
        shape1.points = points2;
        shapes.Add(shape);
        shapes1.Add(shape1);

        CompositeShape compositeShape = new CompositeShape(shapes);
        //CompositeShape compositeShape1 = new CompositeShape(shapes1);

        M.AddComponent<MeshFilter>();
        M.AddComponent<MeshRenderer>();
        M.GetComponent<MeshFilter>().mesh = compositeShape.GetMesh();

        var c = M2.AddComponent<MeshFilter>();
        var b = M2.AddComponent<MeshRenderer>();
        //M2.GetComponent<MeshFilter>().mesh = compositeShape1.GetMesh();

        M3.AddComponent<MeshFilter>();
        M3.AddComponent<MeshRenderer>();

        //M2 = Instantiate(M);
        c.mesh.vertices = M.GetComponent<MeshFilter>().mesh.vertices;
        c.mesh.triangles = M.GetComponent<MeshFilter>().mesh.triangles;

        int[] triangles = M2.GetComponent<MeshFilter>().mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = t;
        }
        M2.GetComponent<MeshFilter>().mesh.triangles = triangles;
        M2.transform.position = new Vector3(M.transform.position.x, M.transform.position.y - 50, M.transform.position.z);


        //Mesh newMesh = new Mesh();
        //newMesh.CombineMeshes();

        //int[] trianglesSum = MergerArray(M.GetComponent<MeshFilter>().mesh.triangles,M2.GetComponent<MeshFilter>().mesh.triangles);
        //Vector3[] ppp = MergerPoints(M.GetComponent<MeshFilter>().mesh.vertices,M2.GetComponent<MeshFilter>().mesh.vertices);
        var CeMian = MergeMesh(M.GetComponent<MeshFilter>().mesh, M2.GetComponent<MeshFilter>().mesh);

        MeshFilter[] meshFilters = { M.GetComponent<MeshFilter>(), M2.GetComponent<MeshFilter>(), CeMian.GetComponent<MeshFilter>() };

        CombineInstance[] combineInstances = new CombineInstance[3];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combineInstances);

        M3.GetComponent<MeshFilter>().sharedMesh = newMesh;
        M3.GetComponent<MeshRenderer>().material = material;

        //string filename =Application.dataPath+"/123.obj";

        //ObjExporter.MeshToFile(M3,Application.dataPath+"123.obj");

    }

    public int[] MergerArray(int[] First, int[] Second)
    {
        int[] result = new int[First.Length + Second.Length];
        First.CopyTo(result, 0);
        Second.CopyTo(result, First.Length);
        return result;
    }
    public Vector3[] MergerPoints(Vector3[] First, Vector3[] Second)
    {
        Vector3[] result = new Vector3[First.Length + Second.Length];
        First.CopyTo(result, 0);
        Second.CopyTo(result, First.Length);
        return result;
    }
    //Mesh ReverseMesh(Mesh mesh)
    //{
    //    Mesh reverseMesh = new Mesh();
    //    int[] triangles = mesh.triangles;
    //    for (int i = 0; i < triangles.Length; i += 3)
    //    {
    //        int t = triangles[i];
    //        triangles[i] = triangles[i + 2];
    //        triangles[i + 2] = t;
    //    }
    //    reverseMesh.triangles = triangles;
    //    return reverseMesh;
    //}

    public static GameObject MergeMesh(Mesh mesh1, Mesh mesh2)
    {
        Vector3[] points1 = mesh1.vertices;
        Vector3[] points2 = points1.Select(x => x = new Vector3(x.x, x.y - 50, x.z)).ToArray();
        List<MeshFilter> meshFilters1 = new List<MeshFilter>();
        GameObject M3 = new GameObject("侧面合体");

        M3.AddComponent<MeshFilter>();
        M3.AddComponent<MeshRenderer>();

        List<List<Vector3>> p = new List<List<Vector3>>();
        for (int i = 0; i < points1.Length - 1; i++)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(points1[i]);
            points.Add(points1[i + 1]);
            points.Add(points2[i]);
            points.Add(points2[i + 1]);
            p.Add(points);

        }
        List<Vector3> temple = new List<Vector3>() { points1[points1.Length - 1], points1[0], points2[points2.Length - 1], points2[0] };
        p.Add(temple);
        foreach (var a in p)
        {
            //GeneratePoints(a);
            GameObject M = new GameObject("侧面");
            GameObject M2 = new GameObject("侧面的另一面");

            var meshf = M.AddComponent<MeshFilter>();
            var meshf2 = M2.AddComponent<MeshFilter>();
            M2.AddComponent<MeshRenderer>();
            M.AddComponent<MeshRenderer>();
            meshf.mesh.vertices = a.ToArray();
            int[] triangles = new int[6];
            triangles[0] = 0; triangles[1] = 2; triangles[2] = 3; triangles[3] = 3; triangles[4] = 1; triangles[5] = 0;
            meshf.mesh.triangles = triangles;
            meshf2.mesh.vertices = a.ToArray();
            int[] triangles2 = triangles;
            for (int i = 0; i < triangles2.Length; i += 3)
            {
                int t = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = t;
            }
            meshf2.mesh.triangles = triangles2;
            meshFilters1.Add(meshf);
            meshFilters1.Add(meshf2);
        }

        CombineInstance[] combineInstances = new CombineInstance[meshFilters1.Count()];

        for (int i = 0; i < meshFilters1.Count(); i++)
        {
            combineInstances[i].mesh = meshFilters1[i].sharedMesh;
            combineInstances[i].transform = meshFilters1[i].transform.localToWorldMatrix;
        }
        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combineInstances);

        M3.GetComponent<MeshFilter>().sharedMesh = newMesh;

        return M3;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
