#include "Solver.h"

#include <unsupported/Eigen/SparseExtra>

void SolveFE(int* rowA, int* colA, float* valA, float* F, int dim, int dof, int nnzA, float* X)
{
    auto A = SetCOOMatrix(rowA, colA, valA, dim, dim, nnzA);
    auto B = SetMatrix(F, dim, dof);

    //Eigen::saveMarket(A, "C:/Users/alber/Desktop/mat.mtx", Eigen::UpLoType::Symmetric);
    //Eigen::saveMarket(B, "C:/Users/alber/Desktop/B.mtx");

    Eigen::MatrixXf result;

    Eigen::PardisoLLT<Eigen::SparseMatrix<float>, 1> pardiso(A);
    //pardiso.pardisoParameterArray()[59] = 1;
    result = pardiso.solve(B);

    //if (dim <= 1000)
    //{
    //    Eigen::SimplicialLDLT<Eigen::SparseMatrix<float>> ldlt(A);
    //    result = ldlt.solve(B);
    //}
    //else
    //{
    //    Eigen::PardisoLDLT<Eigen::SparseMatrix<float>> pardiso(A);
    //    result = pardiso.solve(B);
    //}

    for (int i = 0; i < dim; i++)
    {
        for (int j = 0; j < dof; j++)
        {
            X[i * dof + j] = result(i, j);
        }
    }
}

