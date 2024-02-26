using Silk.NET.Maths;
using System.Globalization;

namespace OpenGLPlayground.Factories;

internal class ObjectMeshFactory(string filePath)
{
    private const string VertexId = "v";
    private const string TextcoodId = "vt";
    private const string NormalId = "vn";
    private const string FaceId = "f";

    /// <summary>
    /// Each triangle has 3 corners and each corner has 8 floats:
    /// 3 for position, 3 for normal and 2 for texture (U, V)
    /// </summary>
    private const int VerticesPerTriangle = 3 * 8;

    private readonly string[] _objectMeshTextLines = File.ReadAllLines(filePath);

    private bool _alreadyLoaded = false;

    private int _vertexCount = 0;
    private int _texcoodCount = 0;
    private int _normalCount = 0;
    private int _triangleCount = 0;

    private List<Vector3D<float>> _v = null!;
    private List<Vector2D<float>> _vt = null!;
    private List<Vector3D<float>> _vn = null!;
    private List<float> _vertices = null!;

    private Matrix4X4<float> _preTransform;

    public float[] Get(Matrix4X4<float> preTransform)
    {
        if (_alreadyLoaded)
        {
            return [.. _vertices];
        }

        _preTransform = preTransform;

        SetCounts();

        _v = new(_vertexCount);
        _vt = new(_texcoodCount);
        _vn = new(_normalCount);

        _vertices = new(_triangleCount * VerticesPerTriangle);

        SetLists();

        _alreadyLoaded = true;

        return [.. _vertices];
    }

    private void SetCounts()
    {
        foreach (var line in _objectMeshTextLines)
        {
            var words = line.Split(' ');

            if (words[0].StartsWith(TextcoodId))
            {
                _texcoodCount++;
                continue;
            }

            if (words[0].StartsWith(NormalId))
            {
                _normalCount++;
                continue;
            }

            if (words[0].StartsWith(VertexId))
            {
                _vertexCount++;
                continue;
            }

            if (words[0].StartsWith(FaceId))
            {
                // First word is Id
                _triangleCount += words.Length - 3;
                continue;
            }
        }
    }

    private void SetLists()
    {
        foreach (var line in _objectMeshTextLines)
        {
            var words = line.Split(' ');

            if (words[0].StartsWith(TextcoodId))
            {
                _vt.Add(ReadVec2(words));
                continue;
            }

            if (words[0].StartsWith(NormalId))
            {
                _vn.Add(ReadVec3(words));
                continue;
            }

            if (words[0].StartsWith(VertexId))
            {
                _v.Add(ReadVec3(words, 1.0f));
                continue;
            }

            if (words[0].StartsWith(FaceId))
            {
                ReadFace(words);
                continue;
            }
        }
    }

    private void ReadFace(string[] words)
    {
        var triangleCount = words.Length - 3;

        for (var i = 0; i < triangleCount; i++)
        {
            ReadCorner(words[1]);
            ReadCorner(words[2 + i]);
            ReadCorner(words[3 + i]);
        }
    }

    private void ReadCorner(string description)
    {
        const int FormatOffset = 1;

        var v_vt_vn = description.Split('/');

        int i;

        // Position
        i = Convert.ToInt32(v_vt_vn[0], CultureInfo.InvariantCulture) - FormatOffset;
        var pos = _v[i];

        _vertices.Add(pos.X);
        _vertices.Add(pos.Y);
        _vertices.Add(pos.Z);

        // Tex coord
        i = Convert.ToInt32(v_vt_vn[1], CultureInfo.InvariantCulture) - FormatOffset;
        var tex = _vt[i];

        _vertices.Add(tex.X);
        _vertices.Add(tex.Y);

        // Normal
        i = Convert.ToInt32(v_vt_vn[2], CultureInfo.InvariantCulture) - FormatOffset;
        var nor = _vn[i];

        _vertices.Add(nor.X);
        _vertices.Add(nor.Y);
        _vertices.Add(nor.Z);
    }

    private Vector2D<float> ReadVec2(string[] words)
    {
        //Index zero is Id of row
        var x = Convert.ToSingle(words[1], CultureInfo.InvariantCulture);
        var y = Convert.ToSingle(words[2], CultureInfo.InvariantCulture);

        return new Vector2D<float>(x, y);
    }

    private Vector3D<float> ReadVec3(string[] words, float w = 0.0f)
    {
        //Index zero is Id of row
        var x = Convert.ToSingle(words[1], CultureInfo.InvariantCulture);
        var y = Convert.ToSingle(words[2], CultureInfo.InvariantCulture);
        var z = Convert.ToSingle(words[3], CultureInfo.InvariantCulture);

        var vec = new Vector4D<float>(x, y, z, w);

        vec *= _preTransform;

        return new Vector3D<float>(vec.X, vec.Y, vec.Z);
    }
}
