# Сделано на 21 версии Unigine SDK на C#

using System.Collections;
using System.Collections.Generic;
using Unigine;

public partial class SimplifiedCadViewManager : Component
{
	[ShowInEditor] public Node AssemblyRoot;
	private bool simplifiedEnabled = false;
	
	// Порог угла между соседними гранями, начиная с которого ребро считается "жестким"
	[ShowInEditor] public float HardEdgeAngleDeg = 30.0f;
	// Ширина черной полосы, которая строится вдоль найденного ребра
	[ShowInEditor] public float EdgeThickness = 0.01f;
	// Материал, которым будут отрисовываться черные ребра
	[ShowInEditor] public Material EdgeMaterial;
	// Небольшое смещение полосы над поверхностью, чтобы избежать z-fighting
	[ShowInEditor] public float EdgeOffset = 0.002f;
	
	// Все найденные детали сборки
    private readonly List<ObjectMeshStatic> parts = new();
	
	// Исходные материалы деталей, чтобы можно было восстановить обычный режим
	private readonly Dictionary<ObjectMeshStatic, Material[]> originalMaterials = new();
	// Копии материалов деталей, приведенные к белому матовому виду
	private readonly Dictionary<ObjectMeshStatic, Material[]> whiteMaterials = new();
	// Построенные overlay-объекты с черными ребрами для каждой детали
	private readonly Dictionary<ObjectMeshStatic, Node> edgeOverlays = new();

	void Init()
	{
		Log.Message("=== SimplifiedCadViewManager.Init ===\n");

		if (AssemblyRoot == null)
		{
			Log.Error("AssemblyRoot не назначен!\n");
			return;
		}

		CollectParts(AssemblyRoot);
		CacheOriginalMaterials();
		BuildWhiteMaterials();
		BuildAllEdgeOverlays();

		Log.Message($"Найдено деталей: {parts.Count}\n");
		foreach (var part in parts)
			Log.Message($" - {part.Name}\n");
	}

	void Update()
	{
		if (Input.IsKeyDown(Input.KEY.F))
		{
			Log.Message("F нажата\n");

			if (!simplifiedEnabled)
				EnableSimplifiedMode();
			else
				DisableSimplifiedMode();
		}
	}
	
	// Сохраняет исходные материалы всех поверхностей каждой детали
	private void CacheOriginalMaterials()
	{
		originalMaterials.Clear();

		foreach (var part in parts)
		{
			int surfaceCount = part.NumSurfaces;
			Material[] mats = new Material[surfaceCount];

			for (int s = 0; s < surfaceCount; s++)
			{
				mats[s] = part.GetMaterial(s);
			}

			originalMaterials[part] = mats;
		}

		Log.Message("Исходные материалы сохранены.\n");
	}
	
	// Создает для каждой детали набор "белых" материалов на основе исходных
	private void BuildWhiteMaterials()
	{
		whiteMaterials.Clear();

		foreach (var part in parts)
		{
			int surfaceCount = part.NumSurfaces;
			Material[] mats = new Material[surfaceCount];

			for (int s = 0; s < surfaceCount; s++)
			{
				Material src = originalMaterials[part][s];
				if (src == null)
					continue;

				Material inst = src.Inherit();

				if (inst.FindParameter("albedo_color") != -1)
					inst.SetParameterFloat4("albedo_color", new vec4(1.0f, 1.0f, 1.0f, 1.0f));

				if (inst.FindParameter("metalness") != -1)
					inst.SetParameterFloat("metalness", 0.0f);

				if (inst.FindParameter("roughness") != -1)
					inst.SetParameterFloat("roughness", 1.0f);

				mats[s] = inst;
			}

			whiteMaterials[part] = mats;
		}

		Log.Message("Белые материалы созданы.\n");
	}
	
	private void EnableSimplifiedMode()
	{
		Log.Message("EnableSimplifiedMode START\n");

		foreach (var part in parts)
		{
			Material[] mats = whiteMaterials[part];
			for (int s = 0; s < mats.Length; s++)
			{
				if (mats[s] != null)
				{
					Log.Message($"Set white material: {part.Name}, surface {s}\n");
					part.SetMaterial(mats[s], s);
				}
			}
		}

		ClearEdgeOverlays();
		BuildAllEdgeOverlays();

		foreach (var part in parts)
		{
			if (edgeOverlays.TryGetValue(part, out Node overlay) && overlay != null)
				overlay.Enabled = true;
		}

		simplifiedEnabled = true;
		Log.Message("Упрощенный CAD-режим включен.\n");
	}

	private void DisableSimplifiedMode()
	{
		foreach (var part in parts)
		{
			Material[] mats = originalMaterials[part];
			for (int s = 0; s < mats.Length; s++)
			{
				if (mats[s] != null)
				{
					Log.Message($"Restore material: {part.Name}, surface {s}\n");
					part.SetMaterial(mats[s], s);
				}
			}
		}

		ClearEdgeOverlays();

		simplifiedEnabled = false;
		Log.Message("Упрощенный CAD-режим выключен.\n");
	}
	
	// Рекурсивно проходит по узлам сцены, начиная с AssemblyRoot,
	// и собирает все объекты типа ObjectMeshStatic в список деталей
	private void CollectParts(Node root)
    {
        if (root == null)
            return;

        ObjectMeshStatic meshStatic = root as ObjectMeshStatic;
        if (meshStatic != null)
            parts.Add(meshStatic);

        int childrenCount = root.NumChildren;
        for (int i = 0; i < childrenCount; i++)
        {
            Node child = root.GetChild(i);
            CollectParts(child);
        }
    }
	
	// Анализирует меш детали и ищет "жесткие" ребра
	// Ребро считается жестким, если:
	// - у него только одна соседняя грань (граничное ребро),
	// - или угол между двумя соседними гранями больше либо равен HardEdgeAngleDeg
	// Для каждого такого ребра сохраняются:
	// - две вершины ребра,
	// - нормали соседних граней,
	// - третьи вершины этих граней
	// Эта информация нужна для правильного построения полосы вдоль ребра на поверхности
	private List<HardEdge> ExtractHardEdgesFromMesh(Mesh mesh, float hardAngleDeg)
	{
		var result = new List<HardEdge>();

		if (mesh == null)
			return result;

		int surfaceCount = mesh.NumSurfaces;

		for (int s = 0; s < surfaceCount; s++)
		{
			int vertexCount = mesh.GetNumCVertex(s);
			int indexCount = mesh.GetNumCIndices(s);

			if (vertexCount <= 0 || indexCount < 3)
				continue;

			vec3[] vertices = new vec3[vertexCount];
			for (int i = 0; i < vertexCount; i++)
				vertices[i] = mesh.GetVertex(i, s);

			int safeIndexCount = indexCount - (indexCount % 3);
			int triCount = safeIndexCount / 3;

			var triangles = new List<TriangleData>();
			var edges = new Dictionary<EdgeKey, EdgeInfo>();

			for (int t = 0; t < triCount; t++)
			{
				int i0 = mesh.GetCIndex(t * 3 + 0, s);
				int i1 = mesh.GetCIndex(t * 3 + 1, s);
				int i2 = mesh.GetCIndex(t * 3 + 2, s);

				if (i0 < 0 || i0 >= vertexCount ||
					i1 < 0 || i1 >= vertexCount ||
					i2 < 0 || i2 >= vertexCount)
					continue;

				vec3 v0 = vertices[i0];
				vec3 v1 = vertices[i1];
				vec3 v2 = vertices[i2];

				vec3 cross = MathLib.Cross(v1 - v0, v2 - v0);
				if (cross.Length < 0.000001f)
					continue;

				vec3 n = MathLib.Normalize(cross);

				int triId = triangles.Count;
				triangles.Add(new TriangleData
				{
					I0 = i0,
					I1 = i1,
					I2 = i2,
					Normal = n
				});

				AddEdge(edges, new EdgeKey(i0, i1), triId);
				AddEdge(edges, new EdgeKey(i1, i2), triId);
				AddEdge(edges, new EdgeKey(i2, i0), triId);
			}

			foreach (var pair in edges)
			{
				EdgeKey key = pair.Key;
				EdgeInfo info = pair.Value;

				if (key.A < 0 || key.A >= vertices.Length || key.B < 0 || key.B >= vertices.Length)
					continue;

				bool keep = false;

				HardEdge edge = new HardEdge
				{
					P0 = vertices[key.A],
					P1 = vertices[key.B],
					N1 = new vec3(0.0f),
					Third1 = new vec3(0.0f),
					N2 = new vec3(0.0f),
					Third2 = new vec3(0.0f),
					HasN2 = false
				};

				if (info.Tri1 != -1 && info.Tri2 == -1)
				{
					TriangleData tri1 = triangles[info.Tri1];
					keep = true;
					edge.N1 = tri1.Normal;
					edge.Third1 = GetThirdVertex(vertices, tri1, key.A, key.B);
				}
				else if (info.Tri1 != -1 && info.Tri2 != -1)
				{
					TriangleData tri1 = triangles[info.Tri1];
					TriangleData tri2 = triangles[info.Tri2];

					float d = MathLib.Dot(tri1.Normal, tri2.Normal);
					d = MathLib.Clamp(d, -1.0f, 1.0f);

					float angle = MathLib.Acos(d) * MathLib.RAD2DEG;
					if (angle >= hardAngleDeg)
					{
						keep = true;
						edge.N1 = tri1.Normal;
						edge.Third1 = GetThirdVertex(vertices, tri1, key.A, key.B);

						edge.N2 = tri2.Normal;
						edge.Third2 = GetThirdVertex(vertices, tri2, key.A, key.B);
						edge.HasN2 = true;
					}
				}

				if (keep)
					result.Add(edge);
			}
		}

		return result;
	}

	// Возвращает третью вершину треугольника, которая не принадлежит рассматриваемому ребру
	// Нужна для определения правильного направления полосы на грани
	private vec3 GetThirdVertex(vec3[] vertices, TriangleData tri, int edgeA, int edgeB)
	{
		if (tri.I0 != edgeA && tri.I0 != edgeB)
			return vertices[tri.I0];

		if (tri.I1 != edgeA && tri.I1 != edgeB)
			return vertices[tri.I1];

		return vertices[tri.I2];
	}

	// Добавляет информацию о том, какому треугольнику принадлежит ребро
	// Для каждого ребра сохраняются до двух соседних треугольников
	private void AddEdge(Dictionary<EdgeKey, EdgeInfo> edges, EdgeKey key, int triIndex)
	{
		if (!edges.TryGetValue(key, out EdgeInfo info))
		{
			info = new EdgeInfo();
			info.Tri1 = triIndex;
			edges[key] = info;
		}
		else
		{
			info.Tri2 = triIndex;
		}
	}
	
	// Для всех деталей сборки строит overlay-объекты с геометрией черных ребер
	// Для каждой детали:
	// 1) загружает меш,
	// 2) находит hard edges,
	// 3) создает объект overlay с полосами вдоль ребер
	private void BuildAllEdgeOverlays()
	{
		edgeOverlays.Clear();

		foreach (var part in parts)
		{
			string meshPath = part.MeshPath;
			if (string.IsNullOrEmpty(meshPath))
				continue;

			Mesh mesh = new Mesh();
			if (mesh.Load(meshPath) == 0)
				continue;

			var edges = ExtractHardEdgesFromMesh(mesh, HardEdgeAngleDeg);
			var overlay = CreateEdgeOverlayQuads(part, edges);

			if (overlay != null)
				edgeOverlays[part] = overlay;
		}

		Log.Message("Edge overlay построены.\n");
	}
	
	// Создает объект ObjectMeshDynamic, в который записывается вся геометрия полос,
	// построенных вдоль найденных hard edges одной детали
	private ObjectMeshDynamic CreateEdgeOverlayQuads(ObjectMeshStatic part, List<HardEdge> edges)
	{
		if (edges == null || edges.Count == 0)
			return null;

		ObjectMeshDynamic overlay = new ObjectMeshDynamic();
		part.AddChild(overlay);

		Mesh mesh = new Mesh();
		mesh.AddSurface("edges");

		foreach (var e in edges)
		{
			AddHardEdgeSurfaceStrips(mesh, e);
		}

		mesh.CreateBounds();
		overlay.SetMesh(mesh);

		if (EdgeMaterial != null)
			overlay.SetMaterial(EdgeMaterial, 0);

		overlay.Enabled = false;
		return overlay;
	}
	
	// Удаляет все ранее созданные overlay-объекты с ребрами
	// Используется при выключении режима и перед перестроением overlay
	private void ClearEdgeOverlays()
	{
		foreach (var pair in edgeOverlays)
		{
			if (pair.Value != null)
				pair.Value.DeleteLater();
		}

		edgeOverlays.Clear();
	}
	
	// Для одного найденного hard edge строит одну или две полосы:
	// - одну по первой грани,
	// - и вторую по второй грани, если ребро имеет две соседние грани
	private void AddHardEdgeSurfaceStrips(Mesh mesh, HardEdge e)
	{
		AddFaceEdgeStrip(mesh, e.P0, e.P1, e.N1, e.Third1);

		if (e.HasN2)
			AddFaceEdgeStrip(mesh, e.P0, e.P1, e.N2, e.Third2);
	}

	// Для одного найденного hard edge строит одну или две полосы:
	// - одну по первой грани,
	// - и вторую по второй грани, если ребро имеет две соседние грани
	private void AddFaceEdgeStrip(Mesh mesh, vec3 p0, vec3 p1, vec3 faceNormal, vec3 thirdVertex)
	{
		vec3 dir = p1 - p0;
		if (dir.Length < 0.000001f)
			return;

		dir = MathLib.Normalize(dir);

		vec3 n = faceNormal;
		if (n.Length < 0.000001f)
			return;

		n = MathLib.Normalize(n);

		vec3 side = MathLib.Cross(n, dir);
		if (side.Length < 0.000001f)
			return;

		side = MathLib.Normalize(side);

		vec3 mid = (p0 + p1) * 0.5f;
		vec3 toThird = thirdVertex - mid;

		if (MathLib.Dot(side, toThird) < 0.0f)
			side = -side;

		vec3 lift = n * EdgeOffset;
		vec3 width = side * EdgeThickness;

		vec3 v0 = p0 + lift;
		vec3 v1 = p1 + lift;
		vec3 v2 = p1 + lift + width;
		vec3 v3 = p0 + lift + width;

		int baseIndex = mesh.GetNumCVertex();

		mesh.AddVertex(v0);
		mesh.AddVertex(v1);
		mesh.AddVertex(v2);
		mesh.AddVertex(v3);

		mesh.AddIndex(baseIndex + 0);
		mesh.AddIndex(baseIndex + 1);
		mesh.AddIndex(baseIndex + 2);

		mesh.AddIndex(baseIndex + 0);
		mesh.AddIndex(baseIndex + 2);
		mesh.AddIndex(baseIndex + 3);
	}
}


// Ключ ребра, состоящий из двух индексов вершин
// Индексы упорядочиваются, чтобы ребро (A, B) и ребро (B, A) считались одинаковыми
public struct EdgeKey
{
    public int A;
    public int B;

    public EdgeKey(int i0, int i1)
    {
        if (i0 < i1)
        {
            A = i0;
            B = i1;
        }
        else
        {
            A = i1;
            B = i0;
        }
    }

    public override bool Equals(object obj)
    {
        if (!(obj is EdgeKey))
            return false;

        EdgeKey other = (EdgeKey)obj;
        return A == other.A && B == other.B;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (A * 397) ^ B;
        }
    }
}

// Хранит информацию о том, какие два треугольника используют данное ребро
public class EdgeInfo
{
    public int Tri1 = -1;
    public int Tri2 = -1;
}

// Описание "жесткого" ребра:
// координаты концов, нормали соседних граней,
// третьи вершины этих граней и флаг наличия второй грани
public struct HardEdge
{
    public vec3 P0;
    public vec3 P1;

    public vec3 N1;
    public vec3 Third1;

    public vec3 N2;
    public vec3 Third2;
    public bool HasN2;
}

// Данные одного треугольника меша:
// три индекса вершин и его нормаль
public struct TriangleData
{
    public int I0;
    public int I1;
    public int I2;
    public vec3 Normal;
}

