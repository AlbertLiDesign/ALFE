#include "Solver.h"

#include <unsupported/Eigen/SparseExtra>

void SolveFE(int* rowA, int* colA, float* valA, float* F, int dim, int dof, int nnzA, float* X)
{
    auto A = SetSparseMatrix(rowA, colA, valA, dim, dim, nnzA);
    auto B = SetMatrix(F, dim, dof);

    Eigen::PardisoLDLT<Eigen::SparseMatrix<float>> pardiso(A);
    auto result = pardiso.solve(B);

    for (int i = 0; i < dim; i++)
    {
        for (int j = 0; j < dof; j++)
        {
            X[i * dof + j] = result(i, j);
        }
    }
}

