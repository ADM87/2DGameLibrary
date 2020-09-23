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
        public void GenerateMap(Tilemap tilemap, TileMapSettings settings, bool castsShadows)
        {
            TileMapLayer[] layers = settings.Layers;
            Array.Sort(layers, new TileLayerSorter());

            int width = settings.MapSize.x;
            int height = settings.MapSize.y;

            // Generate map layout
            int[,] map = FillMap(width, height, layers);
            for (int i = 0; i < settings.SmoothingIterations; i++)
                map = SmoothMap(map, width, height);

            CleanRegions(map, width, height);

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
                            // TODO - This isn't great. Revisit.
                            if (y - 1 >= 0 && map[x, y - 1] == 0 && layer.TopTile != null)
                            {
                                tiles[i] = layer.TopTile;
                            }
                            else
                            {
                                int current = y - layer.StartDepth;
                                float progress = current / (float)layer.Height;

                                if (progress < layer.BlendPercentage && layer.BlendInTile != null)
                                {
                                    float blendInHeight = layer.Height * layer.BlendPercentage;
                                    float step = layer.BlendCurve.Evaluate(current / blendInHeight);
                                    float blendInChance = UnityEngine.Random.value * 100;

                                    tiles[i] = blendInChance > layer.BlendChance * step ? layer.BlendInTile : layer.FillTile;
                                    continue;
                                }

                                tiles[i] = layer.FillTile;
                            }
                        }
                    }
                }
            }

            tilemap.SetTiles(positions, tiles);
        }

        private int[,] FillMap(int width, int height, TileMapLayer[] layers)
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

        private int[,] SmoothMap(int[,] map, int width, int height)
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
                        neighbors += map[nx, ny] > 0 ? 1 : 0;
                    else
                        neighbors++;
                }
            }
            return neighbors;
        }

        private void CleanRegions(int[,] map, int width, int height)
        {
            List<List<Vector2Int>> filledRegions = GetMapRegions(map, 1, width, height);
            foreach (List<Vector2Int> filledRegion in filledRegions)
            {
                if (filledRegion.Count < 30)
                {
                    foreach (Vector2Int tile in filledRegion)
                        map[tile.x, tile.y] = 0;
                }
            }

            List<List<Vector2Int>> emptyRegions = GetMapRegions(map, 0, width, height);
            foreach (List<Vector2Int> emptyRegion in emptyRegions)
            {
                if (emptyRegion.Count < 30)
                {
                    foreach (Vector2Int tile in emptyRegion)
                        map[tile.x, tile.y] = 1;
                }
            }
        }

        private List<List<Vector2Int>> GetMapRegions(int[,] map, int matchingType, int width, int height)
        {
            List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
            bool[,] checkedTiles = new bool[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!checkedTiles[x, y] && map[x, y] == matchingType)
                    {
                        checkedTiles[x, y] = true;

                        List<Vector2Int> region = new List<Vector2Int>();
                        Queue<Vector2Int> tiles = new Queue<Vector2Int>();

                        tiles.Enqueue(new Vector2Int(x, y));

                        while (tiles.Count > 0)
                        {
                            Vector2Int tile = tiles.Dequeue();
                            region.Add(tile);

                            for (int checkX = tile.x - 1; checkX <= tile.x + 1; checkX++)
                            {
                                for (int checkY = tile.y - 1; checkY <= tile.y + 1; checkY++)
                                {
                                    if (InMap(checkX, checkY, width, height) && !checkedTiles[checkX, checkY])
                                    {
                                        if (checkX == tile.x || checkY == tile.y)
                                        {
                                            checkedTiles[checkX, checkY] = true;
                                            if (map[checkX, checkY] == matchingType)
                                                tiles.Enqueue(new Vector2Int(checkX, checkY));
                                        }
                                    }
                                }
                            }
                        }
                        if (region.Count > 0)
                            regions.Add(region);
                    }
                }
            }
            return regions;
        }

        private bool InMap(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }
    }
}
