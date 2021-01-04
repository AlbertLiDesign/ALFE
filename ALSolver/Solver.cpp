#include "Solver.h"

#include <unsupported/Eigen/SparseExtra>

int SolveFE(int* rows_offset, int* cols, float* vals, float* F, int dim, int dof, int nnz, float* X)
{
    Eigen::Map<Eigen::SparseMatrix<float, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);

    //Eigen::saveMarket(A, "C:/Users/alber/Desktop/mat.mtx", Eigen::UpLoType::Symmetric);
    //Eigen::saveMarket(B, "C:/Users/alber/Desktop/B.mtx");

    Eigen::VectorXf result;

    if (dim < 20000)
    {
        Eigen::SimplicialLLT<Eigen::SparseMatrix<float>> llt(A);
        result = llt.solve(B);
    }
    else
    {
        Eigen::PardisoLLT<Eigen::SparseMatrix<float>,1> pardiso(A);
        //pardiso.pardisoParameterArray()[3] = 32; // PCG method
        pardiso.pardisoParameterArray()[59] = 1;
        result = pardiso.solve(B);
    }

    //std::cout << 5000 * 2 + 1 << ": " << result(5000 * 2 + 1) << std::endl;

    for (int i = 0; i < dim; i++)
    {
        X[i] = result(i);
    }

    return 1;
}