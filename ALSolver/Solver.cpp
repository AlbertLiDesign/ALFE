#include "Solver.h"

#include <unsupported/Eigen/SparseExtra>

void SolveFE(int* rowA, int* colA, float* valA, float* F, int dim, int dof, int nnzA, float* X)
{
    auto A = SetSparseMatrix(rowA, colA, valA, dim, dim, nnzA);
    auto B = SetMatrix(F, dim, dof);

    Eigen::saveMarket(A, "C:/Users/alber/Desktop/mat.mtx", Eigen::UpLoType::Symmetric);
    Eigen::saveMarket(B, "C:/Users/alber/Desktop/B.mtx");

    Eigen::PardisoLDLT<Eigen::SparseMatrix<float>> pardiso(A);
    auto result = pardiso.solve(B);
    //Eigen::SimplicialLDLT< Eigen::SparseMatrix<float>> ldlt(A);
    //auto result = ldlt.solve(B);

    for (int i = 0; i < dim; i++)
    {
        for (int j = 0; j < dof; j++)
        {
            X[i * dof + j] = result(i, j);
        }
    }
}

