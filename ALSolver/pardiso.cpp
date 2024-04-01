#include "Solver.h"

Eigen::VectorXd Solve_Serial_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);
    Eigen::PardisoLLT<Eigen::SparseMatrix<double>, 1> pardiso;
    pardiso.pardisoParameterArray()[59] = 0;
    mkl_set_num_threads(1);
    pardiso.analyzePattern(A);
    pardiso.factorize(A);
    return pardiso.solve(B);
}

Eigen::VectorXd Solve_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);
    Eigen::PardisoLLT<Eigen::SparseMatrix<double>, 1> pardiso;
    pardiso.pardisoParameterArray()[59] = 0;
    pardiso.pardisoParameterArray()[1] = 3; // 	The parallel (OpenMP) version of the nested dissection algorithm.

    pardiso.analyzePattern(A);
    pardiso.factorize(A);

    return pardiso.solve(B);
}

