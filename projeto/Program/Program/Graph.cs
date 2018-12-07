using System;
using System.Collections.Generic;

namespace Program
{
    //[Serializable]
    public class Graph
    {
        public List<Vertice> vertices = new List<Vertice>();
        public List<Edge> edges = new List<Edge>(); 
	    public Graph()
	    {

	    }

        //public Graph Clone()
        //{
        //    var clone = (Graph)this.MemberwiseClone();
        //    for (int i = 0; i < clone.edges.Count; i++)
        //    {
        //        clone.edges[i] = (Edge)clone.edges[i].Clone();
        //    }
        //    for (int j = 0; j < clone.vertices.Count; j++)
        //    {
        //        clone.vertices[j] = (Vertice)clone.vertices[j].Clone();
        //    }
        //    return clone;
        //}
        
    }
}
