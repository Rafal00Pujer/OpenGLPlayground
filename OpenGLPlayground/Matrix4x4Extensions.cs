using Silk.NET.Maths;

namespace OpenGLPlayground;

internal static class Matrix4x4Extensions
{
    public static Span<float> AsSpan(this Matrix4X4<float> matrix)
    {
        var result = new float[16];

        PopulateColumn(matrix.Column1, result);
        PopulateColumn(matrix.Column2, result, 4);
        PopulateColumn(matrix.Column3, result, 8);
        PopulateColumn(matrix.Column4, result, 12);

        return result.AsSpan();

        static void PopulateColumn(in Vector4D<float> column, float[] result, int startIndex = 0)
        {
            var i = startIndex;
            var j = 0;

            while (i < startIndex + 4)
            {
                result[i] = column[j];

                i++;
                j++;
            }
        }
    }
}
