#include "Solver.h"


Eigen::SparseMatrix<double> SetSparseMatrix(int* row, int* col, double* val, int rows, int cols, int nnz)
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


Eigen::MatrixXd SetMatrix(double* matrix, int dim, int dof)
{
    Eigen::MatrixXd mat(dim, dof);
    for (int i = 0; i < dim; i++)
    {
        for (int j = 0; j < dof; j++)
        {
            mat(i, j) = matrix[i * dof + j];
        }
    }
    return mat;
}