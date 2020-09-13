using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Abduction.Systems.TileMaps
{
    [System.Serializable]
    public struct TileMapLayer
    {
        [SerializeField]
        private string name;
        public string Name { get { return name; } }

        [SerializeField]
        private TileBase topTile;
        public TileBase TopTile { get { return topTile; } }

        [SerializeField]
        private TileBase fillTile;
        public TileBase FillTile { get { return fillTile; } }

        [SerializeField]
        private TileBase blendInTile;
        public TileBase BlendInTile { get { return blendInTile; } }

        [SerializeField]
        private int startDepth;
        public int StartDepth { get { return startDepth; } }

        [SerializeField]
        private int endDepth;
        public int EndDepth { get { return endDepth; } }

        [SerializeField]
        private float blendPercentage;
        public float BlendPercentage { get { return blendPercentage; } }

        [SerializeField]
        private float blendChance;
        public float BlendChance { get { return blendChance; } }

        [SerializeField]
        private AnimationCurve curve;
        public AnimationCurve Curve { get { return curve; } }

        [SerializeField]
        private AnimationCurve blendCurve;
        public AnimationCurve BlendCurve { get { return blendCurve; } }

        [SerializeField]
        private Vector2Int modifierRange;

        public int Height { get { return endDepth - startDepth; } }

        public int MinModifier { get { return Mathf.Min(modifierRange.x, modifierRange.y); } } 
        public int MaxModifier { get { return Mathf.Max(modifierRange.x, modifierRange.y); } }

        public int ModifierDelta { get { return MaxModifier - MinModifier; } }
    }

    [CreateAssetMenu(fileName = "TileMapSettings", menuName = "Abduction/Tile Map/Tile Map Settings", order = 1)]
    public class TileMapSettings : ScriptableObject
    {
        [SerializeField]
        private Vector2Int mapSize;
        public Vector2Int MapSize { get { return mapSize; } }

        [SerializeField]
        private int smoothingIterations;
        public int SmoothingIterations { get { return smoothingIterations; } }

        [SerializeField]
        private List<TileMapLayer> layers;
        public TileMapLayer[] Layers { get { return layers.ToArray(); } }
    }
}
