#include "Solver.h"

int SolveSystem(int solver, int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X)
{
    double* result = new double[dim];

    switch (solver)
    {
    case 0:
        memcpy(X, Solve_SimplicialLLT(rows_offset, cols, vals, F, dim, nnz).data(), dim * sizeof(double));
        break;
    case 1:
        memcpy(X, Solve_Serial_PARDISO(rows_offset, cols, vals, F, dim, nnz).data(), dim * sizeof(double));
        break;
    case 2:
        memcpy(X, Solve_CG(rows_offset, cols, vals, F, dim, nnz).data(), dim * sizeof(double));
        break;
    case 3:
        memcpy(X, Solve_PARDISO(rows_offset, cols, vals, F, dim, nnz).data(), dim * sizeof(double));
        break;
    //case 4:
    //    memcpy(X, Solve_CholmodSupernodalLLT(rows_offset, cols, vals, F, dim, nnz), dim * sizeof(double));
    //    break;
    case 5:
        memcpy(X, Solve_AMGCL(rows_offset, cols, vals, F, dim, nnz).data(), dim * sizeof(double));
        break;
    case 6:
        memcpy(X, Solve_AMGX(rows_offset, cols, vals, F, dim, nnz), dim * sizeof(double));
        break;
    default:
        break;
    }
    return 1;
}

Eigen::VectorXd Solve_SimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);
    Eigen::SimplicialLLT<Eigen::SparseMatrix<double>> llt(A);
    llt.analyzePattern(A);
    llt.factorize(A);
    return llt.solve(B);
}

Eigen::VectorXd Solve_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);
    Eigen::ConjugateGradient<Eigen::SparseMatrix<double>> cg(A);
    return cg.solve(B);
}