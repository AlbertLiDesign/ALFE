#include "Solver.h"

#include <unsupported/Eigen/SparseExtra>

bool SolveFE(int* rowA, int* colA, float* valA, float* F, int dim, int dof, int nnzA, float* X)
{
    auto A = SetCOOMatrix(rowA, colA, valA, dim, dim, nnzA);
    auto B = SetVector(F, dim);

    //Eigen::saveMarket(A, "C:/Users/alber/Desktop/mat.mtx", Eigen::UpLoType::Symmetric);
    //Eigen::saveMarket(B, "C:/Users/alber/Desktop/B.mtx");

    Eigen::VectorXf result;

    //Eigen::SimplicialLDLT<Eigen::SparseMatrix<float>> ldlt(A);
    //result = ldlt.solve(B);

    Eigen::PardisoLLT<Eigen::SparseMatrix<float>, 2> pardiso(A);
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
        X[i] = result(i);
    }

    return true;
}

