# Tutorial

This section provides a hands-on tutorial on the basic usage of ALFE.

## Introduction

## Basics

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
            // counterclockwise
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



