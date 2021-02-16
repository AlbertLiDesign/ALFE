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