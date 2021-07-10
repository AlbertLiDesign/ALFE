# Tutorial

This section provides a hands-on tutorial on the basic usage of ALFE.

## Introduction

## 2D Cantilever

Here is a very basic example. We can start from creating a simple 2D cantilever with 6 mm x 4 mm. It is meshed with 24 four-node plane-stress elements. The material is assumed with Young's modulus of 1 MPa and Poisson's ratio of 0.3.

### 1. Define a cantilever

Define nodes and elements. The number of nodes in the two directions can be represented by `xnum`and `ynum`.

```csharp
List<Node> nodes = new List<Node>(xnum * ynum);
List<Element> elems = new List<Element>((xnum - 1) * (ynum - 1));
```

Then, all nodes can be found through two for loops.

```csharp
for (int i = 0; i < xnum; i++)
{
    for (int j = 0; j < ynum; j++)
    {
        nodes.Add(new Node(i, j));
    }
}
```

Constructing a quadrilateral element \(a pixel element\) requires a nodal set and a specified material. The material can be created by the code below.

```csharp
var materal = new Material(1.0, 0.3); // E = 1.0, u = 0.3
```

Next, elements can be defined easily through inputting the **counterclockwise** vertex indices.

```csharp
for (int i = 0; i < xnum - 1; i++)
{
    for (int j = 0; j < ynum - 1; j++)
    {
        List<Node> nodalSet = new List<Node>(4)
        {
            // Counterclockwise
            nodes[i * ynum + j],
            nodes[i * ynum + (j + 1)],
            nodes[(i + 1) * ynum+ (j + 1)],
            nodes[(i + 1) * ynum + j],
        };
        
        // Construct a pixel element
        elems.Add(new Pixel(nodalSet, material));
    }
}
```

### 2. Define loads and boundary conditions

Now, let's set loads and boundary conditions. So, we need to define two lists for loads and supports. The number of loads is 1 because we just apply a load on the center of the one side. Another side of the cantilever should be fixed, so the number of supports should be `ynum`.

```csharp
List<Load> loads = new List<Load>(1);
List<Support> supports = new List<Support>(ynum);
```

The easiest way to define supports is adding a `if` structure when defining nodes.

```csharp
for (int i = 0; i < xnum; i++)
{
    for (int j = 0; j < ynum; j++)
    {
        nodes.Add(new Node(i, j));
        if (i == 0) supports.Add(new Support(j, SupportType.Fixed));
    }
}
```

Also, we can compute the index of the loaded node according to `ynum`. A 2D vector is used to present a force.

```csharp
loads.Add(new Load(nodes.Count - (int)Math.Ceiling(ynum / 2.0), new Vector2D(0.0, -1.0)));
```

### 3. Do FEA

Before doing FEA, we need to create a model and a FE system through assembling above lists.

```csharp
// Create a model
Model model = new Model(2, nodes, elems, loads, supports);
​
// Create a FE system
FESystem system = new FESystem(model);
```

After that, we can initialize the system and solve it.

```csharp
// Initialize the system and solve it.
system.Initialize();
system.Solve();
```

Finally, let's print all displacements

```csharp
// Print the displacement information
Console.WriteLine(system.DisplacementInfo());
Console.ReadKey();
```

### 4. Complete Code

```csharp
static void Main(string[] args)
{
    int xnum = 7;
    int ynum = 5;
    List<Node> nodes = new List<Node>(xnum * ynum);
    List<Element> elems = new List<Element>((xnum - 1) * (ynum - 1));
    List<Load> loads = new List<Load>(1);
    List<Support> supports = new List<Support>(ynum);
    // Define nodes
    for (int i = 0; i < xnum; i++)
    {
        for (int j = 0; j < ynum; j++)
        {
            nodes.Add(new Node(i, j));
            if (i == 0)
                supports.Add(new Support(j, SupportType.Fixed));
        }
    }
    // Define Material
    Material material = new Material(1.0, 0.3);
    // Define elements
    for (int i = 0; i < xnum - 1; i++)
    {
        for (int j = 0; j < ynum - 1; j++)
        {
            List<Node> nodalSet = new List<Node>(4)
            {
                // Counterclockwise
                nodes[i * ynum + j],
                nodes[i * ynum + (j + 1)],
                nodes[(i + 1) * ynum+ (j + 1)],
                nodes[(i + 1) * ynum + j],
            };
            // Construct a pixel element
            elems.Add(new Pixel(nodalSet, material));
        }
    }
    // Apply the load
    loads.Add(new Load(nodes.Count - (int)Math.Ceiling(ynum / 2.0), new Vector2D(0.0, -1.0)));
    // Create a model
    Model model = new Model(2, nodes, elems, loads, supports);
    // Create a FE system
    FESystem system = new FESystem(model);
    // Initialize the system and solve it.
    system.Initialize();
    system.Solve();
    // Print the displacement information
    Console.WriteLine(system.DisplacementInfo());
    Console.ReadKey();
}
```

**Output:**

```text
------------------- Displacement Info -------------------
0       0
0       0
0       0
0       0
0       0
-0.122919399521718      -0.267920998495105
-0.0500423086666319     -0.227484064738565
1.36218031146597E-16    -0.223986691884282
0.0500423086666321      -0.227484064738564
0.122919399521718       -0.267920998495105
-0.162856935787937      -0.507809155006323
-0.086802959069945      -0.480393755317766
1.70459255392177E-16    -0.47401205480756
0.0868029590699452      -0.480393755317765
0.162856935787938       -0.507809155006322
-0.202833069455686      -0.735390869945012
-0.120585272382518      -0.735603485210006
8.67636025787084E-17    -0.755717881657478
0.120585272382518       -0.735603485210005
0.202833069455686       -0.735390869945011
-0.251617425739746      -0.900771587783815
-0.164473424850421      -1.00547944641779
-2.12119789659344E-16   -1.09692038996175
0.164473424850421       -1.00547944641778
0.251617425739745       -0.900771587783813
-0.145756152264201      -0.902954826576372
-0.18641612221033       -1.26756653910868
-6.15018065870458E-16   -1.59977503918858
0.186416122210329       -1.26756653910868
0.1457561522642 -0.90295482657637
0.176305974828536       -0.907973273096894
0.1868720640062 -1.23653764484316
-1.21662657260202E-15   -2.5576494344298
-0.186872064006202      -1.23653764484316
-0.176305974828538      -0.907973273096892
```



