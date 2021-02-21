# ALFE
A simple Finite Element Library based on .NET (very young now)

```
------------------- Device Info -------------------
CPU: Intel(R) Core(TM) i7-9750H CPU @ 2.60GHz
Number Of Cores: 6

------------------- Model Info -------------------
Nodes: 1002001
Elements: 1000000
Type: PixelElement

------------------- Matrix Info -------------------
Rows: 2002000
Cols: 2002000
NNZ: 35987992

------------------- Time Cost -------------------
Computing Ke: 16.8572 ms
Initializing KG: 4049.2476 ms
Prefiltering: 19861.231 ms

################### Step: 0 #####################
Compliance: 45.8276778560443
Volume: 1

------------------- Time Cost -------------------
Assembling KG: 1029.0903 ms
Solving: 11649.5673 ms
Computing Sensitivity: 1154.2919 ms
Fltering Sensitivity: 799.1802 ms
Marking Elements: 680.3271 ms
Checking Convergence: 127.7134 ms

################### Step: 1 #####################
Compliance: 45.8550761268051
Volume: 0.960000040001902

------------------- Time Cost -------------------
Assembling KG: 1205.1218 ms
Solving: 11749.0695 ms
Computing Sensitivity: 1191.3517 ms
Fltering Sensitivity: 894.1928 ms
Marking Elements: 640.4304 ms
Checking Convergence: 114.3624 ms

################### Step: 2 #####################
Compliance: 46.1766870912667
Volume: 0.921600478003727

------------------- Time Cost -------------------
Assembling KG: 1210.1942 ms
Solving: 12040.7222 ms
Computing Sensitivity: 1101.9441 ms
Fltering Sensitivity: 881.3594 ms
Marking Elements: 626.9642 ms
Checking Convergence: 136.8982 ms
```
```
------------------- Model Info -------------------
Nodes: 2500
Degree-of-freedom: 5000
Elements: 2401
Type: PixelElement
------------------- Matrix Info -------------------
Rows: 4900
Cols: 4900
NNZ: 85840
------------------- Time Cost -------------------
Solver: SimplicialLLT
Solving: 28.1793 ms

Solver: CholmodSimplicialLLT
Solving: 47.6843 ms

Solver: CholmodSuperNodalLLT
Solving: 49.8226 ms

Solver: PARDISO
Solving: 115.6076 ms
```
```
------------------- Model Info -------------------
Nodes: 8800
Degree-of-freedom: 17600
Elements: 8611
Type: PixelElement
------------------- Matrix Info -------------------
Rows: 17440
Cols: 17440
NNZ: 309400
------------------- Time Cost -------------------
Solver: SimplicialLLT
Solving: 269.4793 ms

Solver: CholmodSimplicialLLT
Solving: 311.7806 ms

Solver: CholmodSuperNodalLLT
Solving: 152.5887 ms

Solver: PARDISO
Solving: 151.8107 ms
```
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
Solving: 2007.5914 ms

Solver: CholmodSimplicialLLT
Solving: 839.5229 ms

Solver: CholmodSuperNodalLLT
Solving: 703.2 ms

Solver: PARDISO
Solving: 432.2006 ms
```
```
------------------- Model Info -------------------
Nodes: 250000
Degree-of-freedom: 500000
Elements: 249001
Type: PixelElement
------------------- Matrix Info -------------------
Rows: 499000
Cols: 499000
NNZ: 8958040
------------------- Time Cost -------------------
Solver: SimplicialLLT
Solving: 31814.1352 ms

Solver: CholmodSimplicialLLT
Solving: 5759.2955 ms

Solver: CholmodSuperNodalLLT
Solving: 5532.607 ms

Solver: PARDISO
Solving: 1935.2917 ms
```

```
------------------- Model Info -------------------
Nodes: 90000
Degree-of-freedom: 180000
Elements: 89401
Type: PixelElement
------------------- Matrix Info -------------------
Rows: 179400
Cols: 179400
NNZ: 3214840
------------------- Time Cost -------------------
Solver: SimplicialLLT
Solving: 7435.2329 ms

Solver: CholmodSimplicialLLT
Solving: 1715.392 ms

Solver: CholmodSuperNodalLLT
Solving: 1787.637 ms

Solver: PARDISO
Solving: 678.029 ms

Solver: SXAMG
Solving: 24377.3846 ms

Solver: AMG_CG
Solving: 1325.4935 ms
```