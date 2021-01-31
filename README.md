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