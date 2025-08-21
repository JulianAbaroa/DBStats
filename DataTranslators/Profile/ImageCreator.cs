using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace DBStats.DataTranslators.Profile
{
    public class ImageCreator
    {
        public static string CreateEmblemImage(int textureZeroID, int textureOneID, int colorZeroID, int colorOneID, int colorTwoID, string assetsPath, string playerName)
        {
            var emblemName = AssetsMapper.GetEmblemImageName(textureZeroID);
            string emblemFirstPath = Path.Combine(assetsPath, "Emblems", $"{emblemName.Primary}.png");
            string emblemSecondPath = Path.Combine(assetsPath, "Emblems", $"{emblemName.Secondary}.png");

            var backgroundName = AssetsMapper.GetEmblemImageName(textureOneID);
            string backgroundPath = Path.Combine(assetsPath, "Emblems", $"{backgroundName.Primary}.png");

            if (!File.Exists(emblemFirstPath) || !File.Exists(emblemSecondPath) || !File.Exists(backgroundPath))
                throw new InvalidOperationException($"Error: one or more emblem files not found. EmblemFirst: {emblemFirstPath}, EmblemSecond: {emblemSecondPath}, Background: {backgroundPath}");

            // tus colores vienen como Rgba32
            Rgba32 tint0 = EmblemColors.GetColor(colorZeroID); // para emblemFirstPart
            Rgba32 tint1 = EmblemColors.GetColor(colorOneID);  // para emblemSecondPart
            Rgba32 tint2 = EmblemColors.GetColor(colorTwoID);  // para background

            // convertimos a floats 0..1 (solo RGB usados para fidelidad)
            static (float r, float g, float b, float a) ToFloat(Rgba32 c) =>
                (c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);

            var vt0 = ToFloat(tint0);
            var vt1 = ToFloat(tint1);
            var vt2 = ToFloat(tint2);

            using var emblemFirstPart = Image.Load<Rgba32>(emblemFirstPath);
            using var emblemSecondPart = Image.Load<Rgba32>(emblemSecondPath);
            using var emblemBackground = Image.Load<Rgba32>(backgroundPath);

            // -------------------------------------------------------
            // Función: reemplazar RGB EXACTO por el color elegido
            // Si tint.a == 0 -> ELIMINAR TODOS LOS PIXELES (poner alpha = 0)
            // -------------------------------------------------------
            void ApplyExactColor(Image<Rgba32> img, (float r, float g, float b, float a) tint, string name, bool preserveAlpha = true, bool forceOpaque = false)
            {
                // Caso tonto pedido: si el tint tiene alpha 0, hacemos toda la imagen totalmente transparente
                if (tint.a <= 0f)
                {
                    img.Mutate(ctx => ctx.ProcessPixelRowsAsVector4((row, y) =>
                    {
                        for (int x = 0; x < row.Length; x++)
                        {
                            // Ponemos alpha = 0 y RGB pueden quedarse a 0 (no se dibujará)
                            row[x] = new Vector4(0f, 0f, 0f, 0f);
                        }
                    }));
                    // No lanzamos excepción en este caso; la imagen está borrada intencionalmente.
                    return;
                }

                int nonTransparentCount = 0;

                img.Mutate(ctx => ctx.ProcessPixelRowsAsVector4((row, y) =>
                {
                    for (int x = 0; x < row.Length; x++)
                    {
                        Vector4 p = row[x]; // X=R, Y=G, Z=B, W=A (0..1)

                        // Si el pixel es totalmente transparente, lo dejamos intacto
                        if (p.W <= 0f)
                            continue;

                        float na = p.W;
                        if (forceOpaque) na = 1f;

                        // Reemplazo directo del RGB (fidelidad total)
                        float nr = tint.r;
                        float ng = tint.g;
                        float nb = tint.b;

                        // Asegurar rango
                        nr = MathF.Min(MathF.Max(nr, 0f), 1f);
                        ng = MathF.Min(MathF.Max(ng, 0f), 1f);
                        nb = MathF.Min(MathF.Max(nb, 0f), 1f);

                        row[x] = new Vector4(nr, ng, nb, na);
                        nonTransparentCount++;
                    }
                }));

                if (nonTransparentCount == 0)
                    throw new InvalidOperationException($"Aplicación de color fallida: la imagen '{name}' no tiene píxeles no-transparentes después del pintado.");
            }

            // -------------------------------------------------------
            // Aplicar colores EXACTOS a cada parte
            // -------------------------------------------------------
            // Nota: por defecto preserveAlpha=true para conservar bordes suaves.
            ApplyExactColor(emblemFirstPart, vt0, "emblemFirstPart (primary)", preserveAlpha: true, forceOpaque: false);
            ApplyExactColor(emblemSecondPart, vt1, "emblemSecondPart (secondary)", preserveAlpha: true, forceOpaque: false);
            ApplyExactColor(emblemBackground, vt2, "emblemBackground (background)", preserveAlpha: true, forceOpaque: false);

            // -------------------------------------------------------
            // Asegurar tamaños
            // -------------------------------------------------------
            int targetW = emblemBackground.Width;
            int targetH = emblemBackground.Height;

            if (emblemFirstPart.Width != targetW || emblemFirstPart.Height != targetH)
                emblemFirstPart.Mutate(ctx => ctx.Resize(targetW, targetH));

            if (emblemSecondPart.Width != targetW || emblemSecondPart.Height != targetH)
                emblemSecondPart.Mutate(ctx => ctx.Resize(targetW, targetH));

            // -------------------------------------------------------
            // Composición final: background -> secondary -> primary
            // -------------------------------------------------------
            using var canvas = new Image<Rgba32>(targetW, targetH);
            canvas.Mutate(ctx =>
            {
                ctx.DrawImage(emblemBackground, 1f);
                ctx.DrawImage(emblemSecondPart, 1f);
                ctx.DrawImage(emblemFirstPart, 1f);
            });

            // Guardado y retorno
            string outputDirectory = Path.Combine(assetsPath, "GeneratedEmblems");
            if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

            string finalEmblemPath = Path.Combine(outputDirectory, $"emblem_{playerName}.png");
            canvas.Save(finalEmblemPath);

            return finalEmblemPath;
        }
    }
}
