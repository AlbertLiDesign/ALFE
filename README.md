# ALFE
A simple Finite Element Library based on .NET (very young now)

```
------------------- Model Info -------------------
Nodes: 1000000
Elements: 998001
Type: UnitQuadElement
------------------- Matrix Info -------------------
Rows: 1998000
Cols: 1998000
NNZ: 35916040
------------------- Time Cost -------------------
Computing Ke: 1.5272 ms
Assembling KG: 3975.2869 ms
Solving: 6415.7871 ms
```

Test
```
Eigen::SimplicialLLT:
20, 000 dim = 82 ms
80, 000 dim = 800 ms
180,000 dim = 2970 ms

Eigen::PardisoLLT:
20000 dim = 157ms
80, 000 dim = 334 ms
180,000 dim = 748 ms
```