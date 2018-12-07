using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
//DEAD
namespace Program
{
    public struct subset
    {
        public int parent;
        public int rank;
    };

    public struct Converter
    {
        public int id;
        public int original;
    };
    class Program
    {
        public static Graph g = new Graph();
        public static List<Vertice> visit = new List<Vertice>();

        public static List<Converter> converter = new List<Converter>();

        static void Main(string[] args)
        {
            try
            {   // Open the text file using a stream reader.
                //exemplorai4
                //C:/Users/Alex Rese/Dropbox/Mestrado/Dissertação/projeto/instance/estudocaso3.net
                using (TextReader reader = File.OpenText("C:/Users/Alex Rese/Dropbox/Mestrado - IA/Dissertação/projeto/instance/estudocaso2_v2.net"))
                {
                    string text = reader.ReadToEnd();
                    string[] lines = text.Split('\n');

                    string control = "";
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i] != "")
                        {
                            //string[] config;
                            //string aux = lines[i];
                            //config = aux.Split(';');
                            //control = config[0];
                            //if (converter.Exists(c => c.original.Equals(control)))
                            //{

                            //}
                                string[] config;
                                string data = lines[i];
                                char d = data[0];
                                if (d == '*')
                                {
                                    string aux = lines[i];
                                    config = aux.Split(' ');
                                    control = config[0];
                                }
                                switch (control)
                                {
                                    case "*vertices":
                                        if (d != '*')
                                        {
                                            insertVertice(lines[i]);
                                        }
                                        break;
                                    case "*edges":
                                        if (d != '*')
                                        {
                                            insertEdges(lines[i], false);
                                        }
                                        break;
                                    case "*arcs":
                                        if (d != '*')
                                        {
                                            insertArcs(lines[i], true);
                                        }
                                        break;
                                }
                        }
                    }
                    Graph s = new Graph();
                    List<Vertice> vertices = new List<Vertice>();
                    g.vertices[0].feeder = 1;
                    g.vertices[15].feeder = 1;
                    g.vertices[48].feeder = 1;
                    vertices.Add(g.vertices[0]);
                    vertices.Add(g.vertices[15]);
                    vertices.Add(g.vertices[48]);
                    //g.vertices[0].feeder = 1;
                    //g.vertices[1].feeder = 1;
                    //g.vertices[2].feeder = 1;
                    //vertices.Add(g.vertices[0]);
                    //vertices.Add(g.vertices[1]);
                    //vertices.Add(g.vertices[2]);
                    //ESTUDO DE CASO 2 NORMAL
                    //FEEDER 0, 15, 48
                    //kruskal(s);
                    //multipleKruskal(s, vertices);
                    //multipleReverseDelete(s, vertices);
                    //reverseDelete(s);
                    //boruvka(s);
                    //prim(s, g.vertices[0]);
                    mutiplePrim(s, vertices);
                    //mutiplePrimSwitchable(s, vertices);

                    //mutiplePrimRand(s, vertices);
                    Console.Read();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                Console.Read();
            }

        }

        public static void insertArcs(string line, bool d)
        {
            string[] edge;
            edge = line.Split(' ');

            Edge e = new Edge();
            e.origin = g.vertices.First(x => x.id == int.Parse(edge[0]));
            e.destiny = g.vertices.First(x => x.id == int.Parse(edge[1]));
            e.value = float.Parse(edge[2]);
            e.directed = d;
            g.edges.Add(e);
            return;
        }

        public static void insertEdges(string line, bool d)
        {
            string[] edge;
            edge = line.Split(' ');

            Edge e = new Edge();
            e.origin = g.vertices.First(x => x.id == int.Parse(edge[0]));
            e.destiny = g.vertices.First(x => x.id == int.Parse(edge[1]));
            e.value = float.Parse(edge[2]);
            e.switchable = 0;// int.Parse(edge[3]);
            e.directed = d;
            g.edges.Add(e);
            if (d == false)
            {
                e = new Edge();
                e.origin = g.vertices.First(x => x.id == int.Parse(edge[1]));
                e.destiny = g.vertices.First(x => x.id == int.Parse(edge[0]));
                e.value = float.Parse(edge[2]);
                e.switchable = 0; // int.Parse(edge[3]);
                e.directed = d;
                g.edges.Add(e);
            }

            return;
        }

        public static void insertVertice(string line)
        {
            string[] vertice;
            vertice = line.Split(' ');

            Vertice v = new Vertice();
            v.id = int.Parse(vertice[0]);
            v.name = vertice[1];
            v.index = g.vertices.Count();
            g.vertices.Add(v);
            return;

        }

        // A utility function to find set of an element i
        // (uses path compression technique)
        public static int findBoruvka(subset[] subsets, int i)
        {
            // find root and make root as parent of i
            // (path compression)
            if (subsets[i].parent != i)
              subsets[i].parent = findBoruvka(subsets, subsets[i].parent);
 
            return subsets[i].parent;
        }

            // A function that does union of two sets of x and y
            // (uses union by rank)
        public static bool unionBoruvka(subset[] subsets, int x, int y, bool multiple = false)
        {
            int xroot = findBoruvka(subsets, x);
            int yroot = findBoruvka(subsets, y);

            if (xroot == yroot || (multiple && (g.vertices[xroot].feeder == 1 && g.vertices[yroot].feeder == 1)))
            {
                return false;
            }

            if (multiple)
            {
                if (g.vertices[yroot].feeder == 1) {
                    subsets[xroot].parent = yroot;
                } else {
                    subsets[yroot].parent = xroot;
                }

                return true;
            }

            // Attach smaller rank tree under root of high
            // rank tree (Union by Rank)
            if (subsets[xroot].rank < subsets[yroot].rank)
            {
                subsets[xroot].parent = yroot;
            }
            else if (subsets[xroot].rank > subsets[yroot].rank)
            {
                subsets[yroot].parent = xroot;
            }
            // If ranks are same, then make one as root and
            // increment its rank by one
            else
            {
                subsets[yroot].parent = xroot;
                subsets[xroot].rank++;
            }

            return true;
        }

        

        public static void boruvka(Graph s)
        {
            bool multiple = true;
            List<Edge> edges = new List<Edge>();
            subset[] subsets = new subset[g.vertices.Count()];
            for (int v = 0; v < g.vertices.Count(); v++)
            {
                subsets[v].parent = v;
                subsets[v].rank = 0;
            }
            //NUMERO DE RAMOS DO GRAFO
            int numTrees = g.vertices.Count();

            int feederCount = g.vertices.Where(v => v.feeder == 1).Count();

            while (edges.Count() < 1 * (g.vertices.Count() - feederCount))
            {
                //Dicionário de arestas selecionadas para cada componente
                Dictionary<int, Edge> candidates = new Dictionary<int, Edge>();
                for (int u = 0; u < g.vertices.Count(); u++)
                {
                    // Escolhe o vizinho de menor custo aplicável
                    List<Edge> zs = g.edges
                        .Where(e => 
                        {
                            int a = findBoruvka(subsets, e.origin.index);
                            int b = findBoruvka(subsets, e.destiny.index);
                            bool bothFeeders = g.vertices[a].feeder == 1 && g.vertices[b].feeder == 1;
                            bool ret = e.origin.index.Equals(u) && a != b;
                            if (multiple)
                            {
                                return ret && !bothFeeders;
                            }

                            return ret;
                        })
                        .OrderBy(e => e.value)
                        .ToList();

                    if (zs.Count() == 0)
                    {
                        //Console.WriteLine(g.vertices[u].id + " não tem para onde ir");
                    }
                    else
                    {
                        Edge z = zs.First();
                        Console.WriteLine("Candidato para " + g.vertices[u].id + ": " + z.origin.id + "->" + z.destiny.id);
                        int uSet = findBoruvka(subsets, u);
                        //Se não há chave associada ou se a aresta tem custo menor
                        if (!candidates.ContainsKey(uSet) || z.value < candidates[uSet].value)
                        {
                            candidates[uSet] = z;
                        }
                    }
                }

                foreach (int set in candidates.Keys)
                {
                    Edge e = candidates[set];
                    numTrees--;
                    
                    if (unionBoruvka(subsets, e.origin.index, e.destiny.index, multiple))
                    {
                        edges.Add(e);
                        s.edges.Add(e);
                    }
                }
            }
            Console.WriteLine("IMPRIME");
            for (int i = 0; i < edges.Count(); i = i + 1) {
            
                //IMPRIME
                Console.WriteLine("o:" + edges[i].origin.id + " d:" + edges[i].destiny.id + " v:" + edges[i].value);
            }

            printSolutionFileB(s);
            return;
        }

        public static void prim(Graph s, Vertice initial)
        {
            //ADICIONA O VERTICE INICIAL A SOLUCAO
            s.vertices.Add(initial);
            List<Edge> edges;
            bool loop = true;
            while (loop)
            {
                //PEGA TODAS AS ARESTAS ADJACENTES AOS VERTICES DA SOLUCAO
                edges = g.edges
                        .Where(e => s.vertices.Contains(e.origin))
                        .OrderBy(e => e.value)
                        .ToList();

                //PARA CADA ARESTA NA SOLUCAO
                for (int i = 0; i < edges.Count(); i++)
                {
                    //SE NAO EXISTE A ARESTA NA SOLUCAO
                    if (!s.edges.Exists(x => x.Equals(edges[i])))
                    {
                        //SE NÃO EXISTE O VERTICE DE DESTINO NA SOLUCAO
                        if (!s.vertices.Exists(x => x.Equals(edges[i].destiny)))
                        {
                            s.edges.Add(edges[i]);
                            s.edges.Add(oppositeVertice(edges[i].destiny, edges[i].origin));
                            s.vertices.Add(edges[i].destiny);
                            break;
                        }
                    }
                }
                //SE TODOS O VERTICES FORAM ATIGINDOS FINALIZA LOOP
                if (g.vertices.Count() == s.vertices.Count())
                {
                    loop = false;
                }
            }
            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int i = 0; i < s.edges.Count(); i = i + 2)
            {
                //IMPRIME
                Console.WriteLine("o:" + s.edges[i].origin.id + " d:" + s.edges[i].destiny.id + " v:" + s.edges[i].value);
            }
            printSolutionFile(s);
        }

        public static bool isSwitchable(List<int> sw)
        {
            for (int i = 0; i < sw.Count(); i++)
            {
                if (sw[i] == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static void mutiplePrimSwitchable(Graph s, List<Vertice> feeders)
        {
            List<Graph> graphs = new List<Graph>();
            List<int> sw = new List<int>();

            for (int i = 0; i < feeders.Count(); i++)
            {
                s = new Graph();
                s.vertices.Add(feeders[i]);
                Console.WriteLine(feeders[i].name);
                graphs.Insert(i, s);
                sw.Add(0);
            }

            List<Edge> edges;
            bool loop = true;

            int nextFeeder = 0;
            while (loop)
            {
                if (isSwitchable(sw))
                {
                    bool v = false;
                    for (int j = 0; j < feeders.Count(); j++)
                    {
                        //PEGA TODAS AS ARESTAS ADJACENTES AOS VERTICES DA SOLUCAO
                        edges = g.edges
                            .Where(e => graphs[j].vertices.Contains(e.origin))
                            .Where(e => e.switchable == 0)
                            .OrderBy(e => e.value)
                            .ToList();

                        v = false;

                        if (edges.Count() != 0)
                        {
                            v = true;
                        }
                        //PARA CADA ARESTA NA SOLUCAO
                        for (int i = 0; i < edges.Count(); i++)
                        {

                            bool b = true;
                            for (int z = 0; z < feeders.Count(); z++)
                            {
                                //SE NAO EXISTE A ARESTA EM NENHUMA DAS SOLUCOES E SE NÃO EXISTE O VERTICE DE DESTINO EM NENHUMA DAS SOLUCOES
                                if (!graphs[z].edges.Exists(x => x.Equals(edges[i])) && !graphs[z].vertices.Exists(x => x.Equals(edges[i].destiny)))
                                {
                                    b = true;
                                }
                                else
                                {
                                    b = false;
                                    break;
                                }
                            }
                            if (b)
                            {
                                graphs[j].edges.Add(edges[i]);
                                graphs[j].edges.Add(oppositeVertice(edges[i].destiny, edges[i].origin));
                                graphs[j].vertices.Add(edges[i].destiny);
                                v = false;
                            }

                        }
                        if (v)
                        {
                            sw[j] = 1;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < feeders.Count(); i++)
                    {
                        sw[i] = 0;
                    }

                    int j = nextFeeder;
                    //PEGA TODAS AS ARESTAS ADJACENTES AOS VERTICES DA SOLUCAO
                    edges = g.edges
                        .Where(e => graphs[j].vertices.Contains(e.origin))
                        .OrderBy(e => e.value)
                        .ToList();

                    //PARA CADA ARESTA NA SOLUCAO
                    for (int i = 0; i < edges.Count(); i++)
                    {
                        bool b = true;
                        for (int z = 0; z < feeders.Count(); z++)
                        {
                            //SE NAO EXISTE A ARESTA EM NENHUMA DAS SOLUCOES E SE NÃO EXISTE O VERTICE DE DESTINO EM NENHUMA DAS SOLUCOES
                            if (!graphs[z].edges.Exists(x => x.Equals(edges[i])) && !graphs[z].vertices.Exists(x => x.Equals(edges[i].destiny)))
                            {
                                b = true;
                            }
                            else
                            {
                                b = false;
                                break;
                            }
                        }
                        if (b)
                        {
                            graphs[j].edges.Add(edges[i]);
                            graphs[j].edges.Add(oppositeVertice(edges[i].destiny, edges[i].origin));
                            graphs[j].vertices.Add(edges[i].destiny);
                            break;
                        }
                    }
                    if (nextFeeder == feeders.Count()-1)
                    {
                        nextFeeder = 0;
                    }
                    else
                    {
                        nextFeeder++;
                    }


                }
                int allVertices = 0;
                for (int x = 0; x < feeders.Count(); x++)
                {
                    allVertices = graphs[x].vertices.Count() + allVertices;
                }
                //SE TODOS O VERTICES FORAM ATIGINDOS FINALIZA LOOP SAO INATINGIVEIS
                if (g.vertices.Count() <= allVertices)
                {
                    loop = false;
                }

            }
            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int j = 0; j < feeders.Count(); j++)
            {
                Console.WriteLine("GRAFO" + j);
                for (int i = 0; i < graphs[j].edges.Count(); i = i + 2)
                {
                    //IMPRIME
                    Console.WriteLine("o:" + graphs[j].edges[i].origin.id + " d:" + graphs[j].edges[i].destiny.id + " v:" + graphs[j].edges[i].value + " =" + graphs[j].edges[i].switchable);
                }
            }
            Console.WriteLine("FIM");
        }

        public static void mutiplePrim(Graph s, List<Vertice> feeders)
        {
            List<Graph> graphs = new List<Graph>();
            for (int i = 0; i < feeders.Count(); i++)
            {
                s = new Graph();
                s.vertices.Add(feeders[i]);
                Console.WriteLine(feeders[i].name);
                graphs.Insert(i, s);
            }

            List<Edge> edges;
            bool loop = true;

            while (loop)
            {
                for (int j = 0; j < feeders.Count(); j++)
                {
                    //PEGA TODAS AS ARESTAS ADJACENTES AOS VERTICES DA SOLUCAO
                    edges = g.edges
                        .Where(e => graphs[j].vertices.Contains(e.origin))
                        .OrderBy(e => e.value)
                        .ToList();

                    //PARA CADA ARESTA NA SOLUCAO
                    for (int i = 0; i < edges.Count(); i++)
                    {
                        bool b = true;
                        for (int z = 0; z < feeders.Count(); z++)
                        {
                            //SE NAO EXISTE A ARESTA EM NENHUMA DAS SOLUCOES E SE NÃO EXISTE O VERTICE DE DESTINO EM NENHUMA DAS SOLUCOES
                            if (!graphs[z].edges.Exists(x => x.Equals(edges[i])) && !graphs[z].vertices.Exists(x => x.Equals(edges[i].destiny)))
                            {
                                b = true;
                            }
                            else
                            {
                                b = false;
                                break;
                            }
                        }
                        if (b)
                        {
                            graphs[j].edges.Add(oppositeVertice(edges[i].destiny, edges[i].origin));
                            graphs[j].edges.Add(edges[i]);
                            graphs[j].vertices.Add(edges[i].destiny);
                            break;

                        }
                    }

                }
                int allVertices = 0;
                for (int x = 0; x < feeders.Count(); x++)
                {
                    allVertices = graphs[x].vertices.Count() + allVertices;
                }
                //SE TODOS O VERTICES FORAM ATIGINDOS FINALIZA LOOP SAO INATINGIVEIS
                if (g.vertices.Count() <= allVertices)
                {
                    loop = false;
                }
            }
            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int j = 0; j < feeders.Count(); j++)
            {
                Console.WriteLine("GRAFO" + j);
                for (int i = 0; i < graphs[j].edges.Count(); i = i + 2)
                {
                    //IMPRIME
                    Console.WriteLine("o:" + graphs[j].edges[i].origin.id + " d:" + graphs[j].edges[i].destiny.id + " v:" + graphs[j].edges[i].value);
                }
            }
            printSolutionFilePrim(graphs, feeders);
        }

        public static void printSolutionFilePrim(List<Graph> graphs, List<Vertice> feeders)
        {
            for (int j = 0; j < feeders.Count(); j++)
            {
                for (int i = 0; i < graphs[j].edges.Count(); i = i + 2)
                {
                    string line = graphs[j].edges[i].origin.id + ";" + graphs[j].edges[i].destiny.id + ";" + graphs[j].edges[i].value;

                    string path = @"C:\Users\Alex Rese\Dropbox\Mestrado - IA\Dissertação\projeto\instance\results.txt";
                    if (!File.Exists(path))
                    {
                        File.Create(path).Close();
                        TextWriter tw = new StreamWriter(path);
                        tw.WriteLine(line);
                        tw.Close();
                    }
                    else if (File.Exists(path))
                    {
                        TextWriter tw = new StreamWriter(path, true);
                        tw.WriteLine(line);
                        tw.Close();
                    }
                }
            }
        }

        public static void mutiplePrimRand(Graph s, List<Vertice> feeders)
        {
            List<Graph> graphs = new List<Graph>();
            for (int i = 0; i < feeders.Count(); i++)
            {
                s = new Graph();
                s.vertices.Add(feeders[i]);
                Console.WriteLine(feeders[i].name);
                graphs.Insert(i, s);
            }

            List<Edge> edges;
            bool loop = true;

            while (loop)
            {

                Random rnd = new Random();
                for (int j = rnd.Next(0, 2); j < feeders.Count(); j++)
                {

                    //Console.WriteLine(j);
                    //PEGA TODAS AS ARESTAS ADJACENTES AOS VERTICES DA SOLUCAO
                    edges = g.edges
                        .Where(e => graphs[j].vertices.Contains(e.origin))
                        .OrderBy(e => e.value)
                        .ToList();

                    //PARA CADA ARESTA NA SOLUCAO
                    for (int i = 0; i < edges.Count(); i++)
                    {
                        bool b = true;
                        for (int z = 0; z < feeders.Count(); z++)
                        {
                            //SE NAO EXISTE A ARESTA EM NENHUMA DAS SOLUCOES E SE NÃO EXISTE O VERTICE DE DESTINO EM NENHUMA DAS SOLUCOES
                            if (!graphs[z].edges.Exists(x => x.Equals(edges[i])) && !graphs[z].vertices.Exists(x => x.Equals(edges[i].destiny)))
                            {
                                b = true;
                            }
                            else
                            {
                                b = false;
                                break;
                            }
                        }
                        if (b)
                        {
                            graphs[j].edges.Add(oppositeVertice(edges[i].destiny, edges[i].origin));
                            graphs[j].edges.Add(edges[i]);
                            graphs[j].vertices.Add(edges[i].destiny);
                            break;

                        }
                    }

                }
                int allVertices = 0;
                for (int x = 0; x < feeders.Count(); x++)
                {
                    allVertices = graphs[x].vertices.Count() + allVertices;
                }
                //SE TODOS O VERTICES FORAM ATIGINDOS FINALIZA LOOP SAO INATINGIVEIS
                if (g.vertices.Count() <= allVertices)
                {
                    loop = false;
                }
            }
            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int j = 0; j < feeders.Count(); j++)
            {
                Console.WriteLine("GRAFO" + j);
                for (int i = 0; i < graphs[j].edges.Count(); i = i + 2)
                {
                    //IMPRIME
                    Console.WriteLine("o:" + graphs[j].edges[i].origin.id + " d:" + graphs[j].edges[i].destiny.id + " v:" + graphs[j].edges[i].value);
                }
            }
        }

        public static Edge oppositeVertice(Vertice o, Vertice d)
        {
            //PEGA O ARESTA INVERTIDA ONDE ORIGEM = DESTINO E DESTINO = ORIGEM
            List<Edge> edges = g.edges
                        .Where(e => e.destiny == d)
                        .Where(e => e.origin == o)
                        .ToList();
            //RETORNA A ARESTA INVERTIDA
            return edges.First();
        }

        public static int find(int[] subsets, int i)
        {
            //PROCURA A RAIZ E RETORNA O i
            if (subsets[i] == -1)
            {
                return i;
            }
            return find(subsets, subsets[i]);
        }

        public static void union(int[] subsets, int x, int y)
        {
            int xroot = find(subsets, x);
            int yroot = find(subsets, y);
            subsets[xroot] = yroot;
        }

        public static void kruskal(Graph s)
        {
            int[] subsets = new int[g.vertices.Count()];
            //ORDENA AS ARESTA COM BASE EM SEU PESO
            g.edges = g.edges.OrderBy(e => e.value).ToList();
            //INICIALIZA OS SUBSTES COM -1
            for (int j = 0; j < g.vertices.Count(); j++)
            {
                subsets[j] = -1;
            }
            //PARA CADA ARESTA NO GRAFO
            for (int i = 0; i < g.edges.Count(); i = i + 2)
            {
                int x = find(subsets, g.edges[i].origin.id - 1);
                int y = find(subsets, g.edges[i].destiny.id - 1);
                //SE A LIGACAO NAO GERA CICLO
                if (x != y)
                {
                    s.edges.Add(g.edges[i]);
                    s.edges.Add(oppositeVertice(g.edges[i].origin, g.edges[i].destiny));
                    union(subsets, x, y);
                }
            }
            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int j = 0; j < s.edges.Count(); j = j + 2)
            {
                //IMPRIME
                Console.WriteLine("o:" + s.edges[j].origin.id + " d:" + s.edges[j].destiny.id + " v:" + s.edges[j].value);
            }
            printSolutionFile(s);
            return;
        }

        public static void printSolutionFile(Graph s)
        {
            for (int j = 0; j < s.edges.Count(); j = j + 2)
            {
                string line = s.edges[j].origin.id + ";" + s.edges[j].destiny.id + ";" + s.edges[j].value;

                string path = @"C:\Users\Alex Rese\Dropbox\Mestrado - IA\Dissertação\projeto\instance\results.txt";
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    TextWriter tw = new StreamWriter(path);
                    tw.WriteLine(line);
                    tw.Close();
                }
                else if (File.Exists(path))
                {
                    TextWriter tw = new StreamWriter(path, true);
                    tw.WriteLine(line);
                    tw.Close();
                }
            }
        }

        public static void printSolutionFileB(Graph s)
        {
            for (int j = 0; j < s.edges.Count(); j = j + 1)
            {
                string line = s.edges[j].origin.id + ";" + s.edges[j].destiny.id + ";" + s.edges[j].value;

                string path = @"C:\Users\Alex Rese\Dropbox\Mestrado - IA\Dissertação\projeto\instance\results.txt";
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    TextWriter tw = new StreamWriter(path);
                    tw.WriteLine(line);
                    tw.Close();
                }
                else if (File.Exists(path))
                {
                    TextWriter tw = new StreamWriter(path, true);
                    tw.WriteLine(line);
                    tw.Close();
                }
            }

        }

        public static int findMultiple(int[] subsets, int i)
        {
            //PROCURA A RAIZ E RETORNA O i
            if (subsets[i] == -1)
            {
                return i;
            }
            return find(subsets, subsets[i]);
        }

        public static void unionMultiple(int[] subsets, int x, int y)
        {
            int xroot = findMultiple(subsets, x);
            int yroot = findMultiple(subsets, y);
            if (g.vertices[xroot].feeder == 1)
            {
                subsets[yroot] = xroot;
            }
            else
            {
                subsets[xroot] = yroot;
            }
        }

        public static void multipleKruskal(Graph s, List<Vertice> vertices)
        {
            //ORDENA AS ARESTA COM BASE EM SEU PESO
            int[] subsets = new int[g.vertices.Count()];
            //List<Vertice> subsets = new List<Vertice>();
            g.edges = g.edges.OrderBy(e => e.value).ToList();
            for (int j = 0; j < g.vertices.Count(); j++)
            {
                subsets[j] = -1;
            }
            //PARA CADA ARESTA NO GRAFO
            for (int i = 0; i < g.edges.Count(); i = i + 2)
            {
                int x = find(subsets, g.edges[i].origin.id - 1);
                int y = find(subsets, g.edges[i].destiny.id - 1);
                //SE A LIGACAO NAO GERA CICLO
                if (x != y)
                {
                    if (g.vertices[x].feeder != 1 || g.vertices[y].feeder != 1)
                    {
                        s.edges.Add(g.edges[i]);
                        s.edges.Add(oppositeVertice(g.edges[i].origin, g.edges[i].destiny));
                        unionMultiple(subsets, x, y);
                    }
                }
            }

            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int j = 0; j < s.edges.Count(); j = j + 2)
            {
                //IMPRIME
                Console.WriteLine("o:" + s.edges[j].origin.id + " d:" + s.edges[j].destiny.id + " v:" + s.edges[j].value);
            }
            printSolutionFile(s);
            return;
        }

        public static int oppositeVerticeIndex(List<Edge> e, Vertice o, Vertice d)
        {
            for (int i = 0; i < e.Count(); i++)
            {
                if (e[i].destiny == d && e[i].origin == o)
                {
                    return i;
                }
            }
            return 0;
        }

        public static Graph cloneGraph(Graph graph)
        {
            //graph.cl
            Graph newGraph = new Graph();
            for (int i = 0; i < graph.vertices.Count(); i++)
            {
                Vertice newVertice = new Vertice();
                newVertice.id = graph.vertices[i].id;
                newVertice.name = graph.vertices[i].name;
                newVertice.feeder = graph.vertices[i].feeder;
                newGraph.vertices.Add(newVertice);
            }
            for (int i = 0; i < graph.edges.Count(); i++)
            {
                Edge newEdge = new Edge();
                Vertice vo = new Vertice();
                Vertice vd = new Vertice();
                newEdge.origin = newGraph.vertices.First(x => x.id == graph.edges[i].origin.id);
                newEdge.destiny = newGraph.vertices.First(x => x.id == graph.edges[i].destiny.id);
                newEdge.directed = graph.edges[i].directed;
                newEdge.value = graph.edges[i].value;
                newGraph.edges.Add(newEdge);
            }

            return newGraph;
        }

        public static bool isConnected(Graph s)
        {
            List<Vertice> visit = new List<Vertice>();
            int count = 0;
            visit.Add(s.edges.First().origin);
            List<Edge> edges;
            while (s.vertices.Count() > count)
            {
                edges = s.edges
                    .Where(e => visit.Contains(e.origin))
                    .ToList();
                for (int i = 0; i < edges.Count(); i++)
                {
                    if (!visit.Contains(edges[i].destiny))
                    {
                        visit.Add(edges[i].destiny);
                    }
                }
                count++;
            }
            if (visit.Count() == s.vertices.Count())
            {
                return true;
            }
            return false;
        }

        public static void reverseDelete(Graph s)
        {
            //ORDENA AS ARESTA COM BASE EM SEU PESO
            g.edges = g.edges.OrderByDescending(e => e.value).ToList();
            Graph aux = new Graph();

            s = cloneGraph(g);
            for (int i = 0; i < s.edges.Count(); i = i + 2)
            {
                aux = cloneGraph(s);
                var n = aux.edges.Where(x => x.destiny.id == g.edges[i].destiny.id && x.origin.id == g.edges[i].origin.id).First();
                var m = aux.edges.Where(x => x.destiny.id == g.edges[i].origin.id && x.origin.id == g.edges[i].destiny.id).First();
                aux.edges.Remove(m);
                aux.edges.Remove(n);
                if (isConnected(aux))
                {
                    Console.WriteLine("CLONE");
                    s = cloneGraph(aux);
                }
            }

            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int i = 0; i < s.edges.Count(); i = i + 2)
            {
                //IMPRIME
                Console.WriteLine("o:" + s.edges[i].origin.id + " d:" + s.edges[i].destiny.id + " v:" + s.edges[i].value);
            }
            printSolutionFile(s);
            return;
        }

        public static void multipleReverseDelete(Graph s, List<Vertice> vertices)
        {
            //ORDENA AS ARESTA COM BASE EM SEU PESO
            g.edges = g.edges.OrderByDescending(e => e.value).ToList();
            Graph aux = new Graph();

            s = cloneGraph(g);
            for (int i = 0; i < s.edges.Count(); i = i + 2)
            {
                aux = cloneGraph(s);
                var n = aux.edges.Where(x => x.destiny.id == g.edges[i].destiny.id && x.origin.id == g.edges[i].origin.id).First();
                var m = aux.edges.Where(x => x.destiny.id == g.edges[i].origin.id && x.origin.id == g.edges[i].destiny.id).First();
                aux.edges.Remove(n);
                aux.edges.Remove(m);
                if (isConnected(aux))
                {
                    s = cloneGraph(aux);
                }
            }

            List<Vertice> feeder = new List<Vertice>();
            feeder = s.vertices.Where(x => x.feeder == 1).ToList();
            List<List<Vertice>> visit = new List<List<Vertice>>();
            List<Edge> mostCost = new List<Edge>();
            for (int i = 0; i < feeder.Count(); i++)
            {
                Edge e = new Edge();
                mostCost.Add(e);
                visit.Add(new List<Vertice>());
                visit[i].Add(feeder[i]);
            }

            int sumVertice = 0;
            bool insert = true;
            while (sumVertice < s.vertices.Count())
            {
                //Console.WriteLine("teste");
                insert = true;
                sumVertice = 0;
                for (int i = 0; i < feeder.Count(); i++)
                {
                    List<Edge> edges;
                    edges = s.edges
                            .Where(e => visit[i].Contains(e.origin))
                            .OrderBy(e => e.value)
                            .ToList();
                    for (int j = 0; j < edges.Count(); j = j + 2)
                    {
                        for (int k = 0; k < feeder.Count(); k++)
                        {
                            if (visit[k].Contains(edges[j].destiny) && k != i)
                            {
                                insert = false;
                                var n = s.edges.Where(x => x.destiny.id == edges[j].destiny.id && x.origin.id == edges[j].origin.id).First();
                                var m = s.edges.Where(x => x.destiny.id == edges[j].origin.id && x.origin.id == edges[j].destiny.id).First();
                                s.edges.Remove(n);
                                s.edges.Remove(m);
                            }
                        }

                        if (!visit[i].Contains(edges[j].destiny) && insert == true)
                        {
                            visit[i].Add(edges[j].destiny);
                        }
                    }
                    sumVertice = sumVertice + visit[i].Count();
                }
            }
            for (int i = 0; i < feeder.Count(); i++)
            {
                Console.Write(i + "----");
                for (int j = 0; j < visit[i].Count(); j++)
                {
                    Console.Write(visit[i][j].id + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("total=" + s.vertices.Count() + "-" + sumVertice);

            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int i = 0; i < s.edges.Count(); i = i + 2)
            {
                //IMPRIME
                Console.WriteLine("o:" + s.edges[i].origin.id + " d:" + s.edges[i].destiny.id + " v:" + s.edges[i].value);
            }
            //printSolutionFile(s);
            return;
        }

        //NAO FOI IMPLEMENTADO
        public static void floydWarshall(Graph s)
        {
            //ORDENA AS ARESTA COM BASE EM SEU PESO
            g.edges = g.edges.OrderBy(e => e.value).ToList();

            //EXIBE A SOLUCAO
            Console.WriteLine("IMPRIME");
            for (int i = 0; i < g.edges.Count(); i++)
            {
                //IMPRIME
                Console.WriteLine("o:" + s.edges[i].origin.id + " d:" + s.edges[i].destiny.id + " v:" + s.edges[i].value);
            }
            return;
        }
    }
}
