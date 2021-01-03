# ALFE
A simple Finite Element Library based on .NET (very young now)

```
------------------- Device Info -------------------
CPU:Intel(R) Core(TM) i7-9750H CPU @ 2.60GHz
------------------- Model Info -------------------
Nodes: 1002001
Elements: 1000000
Type: PixelElement
------------------- Matrix Info -------------------
Rows: 2002000
Cols: 2002000
NNZ: 35987992
------------------- Time Cost -------------------
Computing Ke: 3.0401 ms
Assembling KG: 3900.3937 ms
Solving: 8701.911 ms
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