#include "Solver.h"

Eigen::SparseMatrix<double> SetCOOMatrix(int* row, int* col, double* val, int rows, int cols, int nnz)
{
    std::vector< Eigen::Triplet<double>> tripletsA;
    tripletsA.reserve(nnz);

    for (int i = 0; i < nnz; i++)
    {
            tripletsA.push_back(Eigen::Triplet<double>(row[i], col[i], val[i]));
    }
    Eigen::SparseMatrix<double> sparseA(rows, cols);
    sparseA.setFromTriplets(tripletsA.begin(), tripletsA.end());
    return sparseA;
}

Eigen::VectorXd SetVector(double* matrix, int dim)
{
    Eigen::VectorXd vec(dim);
    for (int i = 0; i < dim; i++)
    {
        vec(i) = matrix[i];
    }
    return vec;
}

SX_MAT SetSXMatrix(int* row, int* col, double* val, int rows, int cols, int nnz)
{
    SX_MAT A;
    SX_INT nz = 0;
    SX_INT i, j;

    A.num_rows = rows;
    A.num_cols = cols;
    A.num_nnzs = nnz;

    A.Ap = row;
    A.Aj = col;
    A.Ax = val;

    return A;
}

SX_VEC SetSXVector(double* matrix, int dim)
{
    SX_VEC vec = sx_vec_create(dim);
    for (int i = 0; i < dim; i++)
    {
        sx_vec_set_entry(&vec, i, matrix[i]);
    }
    return vec;
}