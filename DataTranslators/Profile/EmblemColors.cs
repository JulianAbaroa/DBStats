using SixLabors.ImageSharp.PixelFormats;

namespace DBStats.DataTranslators.Profile;

public class EmblemColors
{
    private static readonly Dictionary<int, Rgba32> _colorMap = new()
    {
        { 1, new Rgba32(83, 83, 83, 100) },     // #535353
        { 2, new Rgba32(140, 140, 140, 100) },  // #8c8c8c
        { 3, new Rgba32(199, 199, 199, 100) },  // #c7c7c7
        { 4, new Rgba32(99, 75, 55, 100) },     // #634b37
        { 5, new Rgba32(143, 109, 90, 100) },   // #8f6d5a
        { 6, new Rgba32(188, 153, 131, 100) },  // #bc9983
        { 7, new Rgba32(101, 107, 72, 100) },   // #656b48
        { 8, new Rgba32(134, 150, 93, 100) },   // #86965d
        { 9, new Rgba32(167, 185, 123, 100) },  // #a7b97b
        { 10, new Rgba32(40, 105, 54, 100) },   // #286936
        { 11, new Rgba32(74, 156, 95, 100) },   // #4a9c5f
        { 12, new Rgba32(119, 189, 129, 100) }, // #77bd81
        { 13, new Rgba32(35, 119, 116, 100) },  // #237774
        { 14, new Rgba32(58, 161, 159, 100) },  // #3aa19f
        { 15, new Rgba32(110, 204, 199, 100) }, // #6eccc7
        { 16, new Rgba32(46, 79, 115, 100) },   // #2e4f73
        { 17, new Rgba32(84, 113, 148, 100) },  // #547194
        { 18, new Rgba32(125, 159, 201, 100) }, // #7d9fc9
        { 19, new Rgba32(79, 77, 125, 100) },   // #4f4d7d
        { 20, new Rgba32(109, 105, 167, 100) }, // #6d69a7
        { 21, new Rgba32(150, 146, 205, 100) }, // #9692cd
        { 22, new Rgba32(145, 51, 51, 100) },   // #913333
        { 23, new Rgba32(194, 54, 54, 100) },   // #c23636
        { 24, new Rgba32(218, 131, 131, 100) }, // #da8383
        { 25, new Rgba32(164, 71, 27, 100) },   // #a4471b
        { 26, new Rgba32(219, 108, 50, 100) },  // #db6c32
        { 27, new Rgba32(219, 156, 120, 100) }, // #db9c78
        { 28, new Rgba32(161, 116, 31, 100) },  // #a1741f
        { 29, new Rgba32(206, 180, 63, 100) },  // #ceb43f
        { 30, new Rgba32(224, 219, 106, 100) }, // #e0db6a
        { 31, new Rgba32(0, 0, 0, 0) },         // Alpha
    };

    public static Rgba32 GetColor(int colorID)
    {
        if (_colorMap.TryGetValue(colorID, out Rgba32 value))
        {
            return value;
        }
        else
        {
            throw new InvalidOperationException($"Error: there's no color to the '{colorID}' id.");
        }
    }
}