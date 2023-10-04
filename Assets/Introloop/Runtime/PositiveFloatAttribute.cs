using UnityEngine;

namespace E7.Introloop
{
    internal class PositiveFloatAttribute : PropertyAttribute
    {
        public readonly string unit;

        public PositiveFloatAttribute()
        {
        }

        public PositiveFloatAttribute(string unit)
        {
            this.unit = unit;
        }
    }
}