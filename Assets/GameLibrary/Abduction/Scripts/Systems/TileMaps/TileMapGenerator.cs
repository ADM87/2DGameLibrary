using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Abduction.Systems.TileMaps
{
    public class TileLayerSorter : IComparer<TileMapLayer>
    {
        public int Compare(TileMapLayer x, TileMapLayer y) => x.StartDepth - y.StartDepth;
    }

    public class TileMapGenerator
    {
        public void GenerateMap(Tilemap tilemap, TileMapSettings settings)
        {
            TileMapLayer[] layers = settings.Layers;
            Array.Sort(layers, new TileLayerSorter());

            int width = settings.MapSize.x;
            int height = settings.MapSize.y;

            // Generate map layout
            int[,] map = GenerateNoise(width, height, layers);
            for (int i = 0; i < settings.SmoothingIterations; i++)
                map = SmoothNoise(map, width, height);

            int size = width * height;

            Vector3Int[] positions = new Vector3Int[size];
            TileBase[] tiles = new TileBase[size];

            foreach (TileMapLayer layer in layers)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = layer.StartDepth; y < layer.EndDepth && y < height; y++)
                    {
                        int i = x + (y * width);

                        int posX = x - Mathf.FloorToInt(width * 0.5f);
                        int posY = (height - y) - Mathf.FloorToInt(height * 0.5f);

                        positions[i] = new Vector3Int(posX, posY, 0);

                        if (map[x, y] > 0)
                        {
                            if (y - 1 > 0)
                                tiles[i] = map[x, y - 1] == 0 ? layer.TopTile : layer.FilleTile;
                            else
                                tiles[i] = layer.FilleTile;
                        }
                    }
                }
            }

            tilemap.SetTiles(positions, tiles);
        }

        private int[,] GenerateNoise(int width, int height, TileMapLayer[] layers)
        {
            int[,] map = new int[width, height];

            foreach (TileMapLayer layer in layers)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = layer.StartDepth; y < layer.EndDepth && y < height; y++)
                    {
                        if (x == 0 || x == width - 1 || y == height - 1)
                        {
                            map[x, y] = 1;
                            continue;
                        }

                        if (layer.ModifierDelta > 0)
                        {
                            int i = y - layer.StartDepth;
                            float step = -1 + (layer.Curve.Evaluate(i / (float)layer.Height) * 2f);
                            float percentage = layer.MinModifier + (Mathf.Abs(step) * layer.ModifierDelta);

                            map[x, y] = UnityEngine.Random.value * 100 < percentage ? 1 : 0;
                        }
                        else
                        {
                            map[x, y] = UnityEngine.Random.value * 100 < layer.MaxModifier ? 1 : 0;
                        }
                    }
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
                    if (InMap(nx, ny, width, height))
                        neighbors += map[nx, ny];
                    else
                        neighbors++;
                }
            }
            return neighbors;
        }

        private bool InMap(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }
    }
}
