# ALFE
ALFE is a free, lightweight and efficient C# Library for linear elastic finite element methods.

## Installation

## Usage

``` C#
static void Main(string[] args)
{
    // Create a cantilever model with 200 * 200 pixel elements
    Model model2d = new Cantilever2D(ElementType.PixelElement, 200, 200).Model;

    // Create a finite element system
    FESystem sys0 = new FESystem(model2d, Solver.SimplicialLLT);

    // Initialize the system
    sys0.Initialize();

    // Solve
    sys0.Solve();

    // Print model information
    Console.Write(sys0.Model.ModelInfo());

    // Print matrix inforamtin
    Console.Write(sys0.MatrixInfo());

    // Print solving information
    Console.Write(sys0.SolvingInfo());

    // Print displacement
    Console.Write(sys0.DisplacementInfo());

    Console.ReadKey();
}
```

Output:
```
------------------- Model Info -------------------
Nodes: 40000
Degree-of-freedom: 80000
Elements: 39601
Type: PixelElement
------------------- Matrix Info -------------------
Rows: 79600
Cols: 79600
NNZ: 1423240
------------------- Time Cost -------------------
Solver: SimplicialLLT
Computing Ke: 22.875 ms
Initializing KG: 220.9646 ms
Assembling KG: 45.5011 ms
Solving: 2311.3483 ms
```

## Documentation
Please click [here](https://albertlidesign.gitbook.io/alfe/)
## License
The code of ALFE itself is licensed under [LGPL-2.1](https://github.com/AlbertLiDesign/ALFE/blob/master/LICENSE).