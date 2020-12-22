# Albert's BESO program report

Author: Albert Li

Date: 08/11/2020

Programming language: Python, C#(for Grasshopper), C++(for Voxelization)

Platform: Rhinoceros / Grasshopper

Dependent libraries: Numpy, Scipy, Pypardiso, PyKdtree 

## Introduction

This program is developed on the Rhinoceros / Grasshopper platform. The finite element calculation currently only supports hexahedral elements. The structure of this program can be divided into six parts. They are voxelization, preprocessing, calculating element stiffness matrices,  assembling the global stiffness matrix, pre-filtering and the BESO loop. The BESO loop includes operations such as Finite Element Analysis (FEA), calculating sensitivity numbers, filtering and averaging sensitivity numbers, adding and removing elements, and updating the global stiffness matrix. A flowchart of the program is given in Figure 1.

<img src="https://i.loli.net/2020/11/05/nWGx2VshRFUXImC.png" alt="Figure 1. Flowchart of the high-performance BESO program." style="zoom: 25%;" />

<center>Figure 1. Flowchart of the high-performance BESO program</center>



##  Implementation

My program takes as input any closed solid NUBRS model and mesh model via the interactive interface offered by Rhinoceros / Grasshopper platform. Besides, the user can specify the boundary conditions to be considered in the optimization process using Grasshopper components developed by myself. Figure 2 shows a simple operation process to define the boundary conditions on Grasshopper.

<img src="https://i.loli.net/2020/11/05/GXTEj5AsPMF2UZJ.png" alt="Figure 2. a simple operation process to define the boundary conditions on Grasshopper." style="zoom: 33%;" />

<center>Figure 2. A simple operation process to define the boundary conditions on Grasshopper</center>

The remaining part will introduce some important implementation method and my performance tests. It is noted that My experiments were run on a standard laptop equipped with an Intel Core i7-9750H processor running at 2.60GHz, 32GB of RAM, and NVIDIA RTX2060 graphics card with 6GB memory.

### Voxelization

The voxelization part can be executed using `cuda_voxelizer`, which implements an optimized version of the method described in M. Schwarz and HP Seidel's 2010 paper [*Fast Parallel Surface and Solid Voxelization on GPU's*](http://research.michael-schwarz.com/publ/2010/vox/). 

Figure 3 shows that a mesh model (Stanford Bunny) is voxelized into **3, 387, 579 voxels** by calculating $256×256×256$ voxels in **2.2 seconds.** The calculation speed of this program has met the needs of most users in voxelization functions.

<img src="https://i.loli.net/2020/11/05/dG6uWyFpg74aMC1.jpg" alt="微信图片_20201105114050" style="zoom:33%;" />

<center>Figure 3. Stanford Bunny model is voxelized into 3, 387, 579 voxels in 2.2 seconds.</center>

### Assembling the global stiffness matrix

The global stiffness matrix is a sparse symmetric matrix. This means that only the elements above the diagonal need to be assembled, while the other half of the elements can be quickly assigned by switching the row and column index. Besides, in order to solve the finite element system, we need to delete the rows and columns corresponding to the degrees of freedom with known node displacements in the global stiffness matrix. It is worth noting that the processed global stiffness matrix is a sparse positive definite matrix, and this property plays an important role in the solution part. It will be mentioned below.

The deleting operator is straightforward for a dense matrix, but it is difficult for a sparse matrix. This is because the sparse matrix is stored in a data structure similar to `(row index, column index, value)` , and if a row in the sparse matrix is to be deleted, then the index of the row of all following elements need to be reduced by one. 

To solve this problem, a necessary data structure for recording deleted information needs to be designed. Let us consider that there is a vector with 13 rows whose row index can be represented by $r \in [0,13)$. The index of the row to be deleted is assumed to be a list like $[4, 6, 7, 9]$.  In order to calculate the new row index after deleting these four rows, we need to record the difference of each row index and their existence. Table 1 shows the data structure in this simple case.

|   Index    |  0   |  1   |  2   |  3   |   4   |  5   |  6   |   7   |  8   |   9   |  10  |  11  |  12  |
| :--------: | :--: | :--: | :--: | :--: | :---: | :--: | :--: | :---: | :--: | :---: | :--: | :--: | :--: |
| Difference |  0   |  0   |  0   |  0   |   1   |  1   |  1   |   2   |  2   |   3   |  3   |  3   |  3   |
| Existence  | True | True | True | True | False | True | True | False | True | False | True | True | True |

<center>Table 1. A data structure for recording difference and existence</center>

The implementation of this data structure is very easy. It can be quickly created in the form of a dictionary with the following simple Python code. The global stiffness matrix is positive definite, so its rows and columns can be removed by this dictionary. Eventually, this data structure will effectively delete the specified rows and columns from a sparse matrix.

```Python
def Scan(num, list):
    list.sort()
    t = 0
    var = 0
    scan = {}
    for i in range(num):
        for j in range(var, len(list)):
            if i >= list[j]:
                t = j
                var += 1
                break
        if i == list[t]:
            scan[i] = [var, False]
        else:
            scan[i] = [var, True]
    return scan
```

### Pre-filtering

The essence of the pre-filtering is to calculate the influence weight of points in a given range on a specified point. This method is particularly widely used in computer graphics, especially in the field of point cloud processing and 3D reconstruction. In order to save time for pre-filtering, [KD-Tree data structure](https://en.wikipedia.org/wiki/K-d_tree) is used to speed up. A simple implementation in Python can be found [here](https://github.com/storpipfugl/pykdtree).

In order to verify the acceleration effect brought by KD-Tree data structure, I tested a model with 8550 nodes and 7308 elements. Table 2 shows the detailed information about the result.

|  Method  |  Time   | Speedup Ratio |
| :------: | :-----: | :-----------: |
| Original | 429.93s |    914.74     |
| KD-Tree  |  0.47s  |       1       |

<center>Table 2. The acceleration effect of KD-Tree data structure</center>

### FEA Solver

The large-scale and high-speed finite element solver has always been the goal pursued by commercial FEA software. I compared various sparse matrix calculation methods to find the fastest method for accelerating my FEA part. Firstly, I used a low-resolution model (8550 nodes and 7308 elements) for testing to filter out those solvers that cannot be used for large-scale calculations. Table 3 shows the detailed information of the matrix (named Test Matrix) generated by the model and Table 4 shows the detailed information about the result.

|    Name     | Num Rows | Num Cols |  Non-zeros  | Positive Definite | Type |
| :---------: | :------: | :------: | :---------: | :---------------: | :--: |
| Test Matrix | 24, 795  | 24, 795  | 1, 792, 553 |        Yes        | Real |

<center>Table 3. The detailed information of Test Matrix</center>

|                            Solver                            | Language |        Math        | Processor | Serial/Parallel |   Time    |
| :----------------------------------------------------------: | :------: | :----------------: | :-------: | :-------------: | :-------: |
| [PyPardiso (MKL)](https://github.com/haasad/PyPardisoProject) |  Python  |      Cholesky      |    CPU    |    Parallel     | **0.52s** |
| [Scipy.lsqr](https://docs.scipy.org/doc/scipy/reference/generated/scipy.sparse.linalg.lsqr.html) |  Python  |    Least Square    |    CPU    |     Serial      |  16.40s   |
| [Scipy.spsolve](https://docs.scipy.org/doc/scipy/reference/generated/scipy.sparse.linalg.spsolve.html) |  Python  |      Unknown       |    CPU    |     Serial      |  37.93s   |
| [SimplicialLDLT (Eigen)](https://eigen.tuxfamily.org/dox/classEigen_1_1SimplicialLDLT.html) |   C++    |      Cholesky      |    CPU    |     Serial      |  25.87s   |
| [PardisoLDLT (Eigen / MKL)](https://eigen.tuxfamily.org/dox/classEigen_1_1PardisoLDLT.html) |   C++    |      Cholesky      |    CPU    |    Parallel     | **0.34s** |
| [CuSparse.Cholesky (CUDA)](https://docs.nvidia.com/cuda/cusparse/index.html) |   C++    |      Cholesky      |    GPU    |    Parallel     | **1.97s** |
|    [FEniCS (Ameba's Solver)](https://fenicsproject.org/)     |  Python  | Conjugate Gradient |    CPU    |     Serial      |   4.20s   |
| [cupyx.lsqr (CUDA)](https://docs.cupy.dev/en/stable/reference/generated/cupyx.scipy.sparse.linalg.lsqr.html) |  Python  |    Least Square    |    GPU    |    Parallel     |  15.23s   |
| [sksparse.cholmod(CHOLMOD)](https://scikit-sparse.readthedocs.io/en/latest/cholmod.html) |  Python  |      Cholesky      |    CPU    |     Serial      |  29.54s   |

<center>Table 4. Comparison of sparse solver performance in solving low-resolution model problems</center>

The least square method can be ruled out because it needs to construct a positive definite matrix through $A^TA$ first, but the processed global stiffness matrix itself is a sparse positive definite matrix. Therefore, the speed of least square method is obviously much slower than using Cholesky directly. Furthermore, according to the result shows on table 3, PyPardiso, PardisoLDLT and CuSparse are promising method for large-scale matrix solving.

And then, I used a large-scale sparse positive definite matrix [TEM152078](https://sparse.tamu.edu/Guettel/TEM152078) from [Suite Sparse Matrix Collection](https://sparse.tamu.edu/) to test these solvers. The specific information of this matrix is shown in Table 5 and the comparison result  can be check in Table 6.

|   Name    | Num Rows | Num Cols |  Non-zeros  | Positive Definite | Type |
| :-------: | :------: | :------: | :---------: | :---------------: | :--: |
| TEM152078 | 152, 078 | 152, 078 | 6, 459, 326 |        Yes        | Real |

<center>Table 5. The detailed information of TEM152078 </center>

|                            Solver                            | Language |   Math   | Processor | Serial/Parallel |     Time      |
| :----------------------------------------------------------: | :------: | :------: | :-------: | :-------------: | :-----------: |
| [PyPardiso (MKL)](https://github.com/haasad/PyPardisoProject) |  Python  | Cholesky |    CPU    |    Parallel     |     3.73s     |
| [PardisoLDLT (Eigen / MKL)](https://eigen.tuxfamily.org/dox/classEigen_1_1PardisoLDLT.html) |   C++    | Cholesky |    CPU    |    Parallel     |   **0.40s**   |
| [CuSparse.Cholesky (CUDA)](https://docs.nvidia.com/cuda/cusparse/index.html) [RTX 2060 6G] |   C++    | Cholesky |    GPU    |    Parallel     | Out of memory |
| [CuSparse.Cholesky (CUDA)](https://docs.nvidia.com/cuda/cusparse/index.html) [RTX 8000 48G] |   C++    | Cholesky |    GPU    |    Parallel     |    89.65s     |

<center>Table 6. Comparison of sparse solver performance in large-scale matrix solving problems</center>

Therefore, PyPardiso and PardisoLDLT can be selected as the high-performance FEA solver. Next, I designed a high-resolution model (107, 010 nodes and 99, 680 elements) for testing them. The global stiffness matrix (Test Matrix 2) has 317, 463 rows and columns and 24, 457, 708 non-zeros (showed in Table 7).

|     Name      | Num Rows | Num Cols |  Non-zeros   | Positive Definite | Type |
| :-----------: | :------: | :------: | :----------: | :---------------: | :--: |
| Test Matrix 2 | 317, 463 | 317, 463 | 24, 457, 708 |        Yes        | Real |

<center>Table 7. The detailed information of Test Matrix 2 </center>

|                            Solver                            | Language |   Math   | Processor | Serial/Parallel |  Time  |
| :----------------------------------------------------------: | :------: | :------: | :-------: | :-------------: | :----: |
| [PyPardiso (MKL)](https://github.com/haasad/PyPardisoProject) |  Python  | Cholesky |    CPU    |    Parallel     | 33.06s |
| [PardisoLDLT (Eigen / MKL)](https://eigen.tuxfamily.org/dox/classEigen_1_1PardisoLDLT.html) |   C++    | Cholesky |    CPU    |    Parallel     | 0.55s  |

<center>Table 8. Performance comparison between PyPardiso and PardisoLDLT  </center>

Compared with the Python version of Pardiso, the C++ Pardiso solver has a huge improvement (Table 8). Thus,  it can be predicted that the development of the C++ version of BESO will greatly improve the performance.

## Future Work

After implementing a high-performance finite element solver (PyPardiso), my BESO program has been dramatically improved, but there are still many problems that need to be improved. Figure 4 shows the time spent in various parts of the current BESO program.

<img src="C:\Users\alber\AppData\Roaming\Typora\typora-user-images\image-20201108182659426.png" alt="image-20201108182659426"  />

<center>Figure 4. The time spent in various parts of the current BESO program</center>

Here `calKes` represents the time of calculating the element stiffness matrix of each element. The main time-consumer in this part is to calculate the triple integral of each element. Besides,  assembly and update process (`Construct KG` & `Update KG`) of the global stiffness matrix are still slow. This may depend on the running speed of the Python program. I will continue to think about how to optimize the performance of these three parts in the further work.

