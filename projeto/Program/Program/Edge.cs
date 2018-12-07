using System;

namespace Program
{
    //[Serializable]
    public class Edge
    {

        public int switchable;
        public float value;
        public Vertice origin;
        public Vertice destiny;
        public bool directed;
        public Edge()
        {

        }

        //public Edge Clone()
        //{
        //    var clone = (Edge)this.MemberwiseClone();
        //    return clone;
        //}
    }
}
