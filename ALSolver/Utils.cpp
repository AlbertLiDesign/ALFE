#include "Solver.h"

Eigen::SparseMatrix<float> SetCOOMatrix(int* row, int* col, float* val, int rows, int cols, int nnz)
{
    std::vector< Eigen::Triplet<float>> tripletsA;
    tripletsA.reserve(nnz);

    for (int i = 0; i < nnz; i++)
    {
            tripletsA.push_back(Eigen::Triplet<float>(row[i], col[i], val[i]));
    }
    Eigen::SparseMatrix<float> sparseA(rows, cols);
    sparseA.setFromTriplets(tripletsA.begin(), tripletsA.end());
    return sparseA;
}


Eigen::VectorXf SetVector(float* matrix, int dim)
{
    Eigen::VectorXf vec(dim);
    for (int i = 0; i < dim; i++)
    {
        vec(i) = matrix[i];
    }
    return vec;
}