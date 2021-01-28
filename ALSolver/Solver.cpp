#include "Solver.h"

#include <unsupported/Eigen/SparseExtra>

int SolveFE(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);

    //Eigen::saveMarket(A, "C:/Users/alber/Desktop/mat.mtx", Eigen::UpLoType::Symmetric);
    //Eigen::saveMarket(B, "C:/Users/alber/Desktop/B.mtx");

    Eigen::VectorXd result;

    if (dim < 20000)
    {
        Eigen::SimplicialLLT<Eigen::SparseMatrix<double>> llt(A);

        llt.analyzePattern(A);
        llt.factorize(A);

        result = llt.solve(B);
    }
    else
    {
        Eigen::PardisoLLT<Eigen::SparseMatrix<double>, 1> pardiso;
        //pardiso.pardisoParameterArray()[3] = 32; // PCG method
        pardiso.pardisoParameterArray()[59] = 0;
        pardiso.pardisoParameterArray()[1] =3; // 	The parallel (OpenMP) version of the nested dissection algorithm.

        pardiso.analyzePattern(A);
        pardiso.factorize(A);
        
        result = pardiso.solve(B);
    }

   //std::cout << 5000 * 2 + 1 << ": " << result(5000 * 2 + 1) << std::endl;

    for (int i = 0; i < dim; i++)
    {
        X[i] = result(i);
    }

    return 1;
}