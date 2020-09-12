using UnityEngine;
using UnityEngine.Tilemaps;

namespace Abduction.Systems.TileMaps
{
    public class TileMapGenerator
    {
        public void GenerateMap(Tilemap tilemap, TileMapSettings settings)
        {
            int width = settings.MapSize.x;
            int height = settings.MapSize.y;
            int smoothingIterations = settings.SmoothingIterations;

            // Generate map layout
            int[,] map = GenerateNoise(width, height, settings.FillPercentage);
            for (int i = 0; i < smoothingIterations; i++)
                map = SmoothNoise(map, width, height);

            int size = width * height;

            Vector3Int[] positions = new Vector3Int[size];
            TileBase[] tiles = new TileBase[size];

            // Generate tilemap data.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = x + (y * width);

                    positions[i] = new Vector3Int(x - Mathf.FloorToInt(width * 0.5f), y - Mathf.FloorToInt(height * 0.5f), 0);

                    if (map[x, y] == 1)
                        tiles[i] = settings.TileSet[2].Tile;
                }
            }

            tilemap.SetTiles(positions, tiles);
        }

        private int[,] GenerateNoise(int width, int height, float fillPercentage)
        {
            int[,] map = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        map[x, y] = 1;
                    else
                        map[x, y] = Random.value * 100 < fillPercentage ? 1 : 0;
                }
            }
            return map;
        }

        private int[,] SmoothNoise(int[,] map, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighbors = CountNeighbors(map, x, y, width, height);

                    if (neighbors > 4)
                        map[x, y] = 1;
                    else if (neighbors < 4)
                        map[x, y] = 0;
                }
            }
            return map;
        }

        private int CountNeighbors(int[,] map, int x, int y, int width, int height)
        {
            int neighbors = 0;
            for (int nx = x - 1; nx <= x + 1; nx++)
            {
                for (int ny = y - 1; ny <= y + 1; ny++)
                {
                    if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        neighbors++;
                    else if (nx != x || ny != y)
                        neighbors += map[nx, ny];
                }
            }
            return neighbors;
        }
    }
}
