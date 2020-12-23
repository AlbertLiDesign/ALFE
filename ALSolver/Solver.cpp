#include "Solver.h"

void SolveFE(int* rowA, int* colA, double* valA, double* F, int dim, int dof, int nnzA)
{
    auto A = SetSparseMatrix(rowA, colA, valA, dim, dim, nnzA, true);
    auto B = SetMatrix(F, dim, dof);
    

    Eigen::SimplicialLLT< Eigen::SparseMatrix<double>> llt(A);
    Eigen::MatrixXd result = llt.solve(B);
    //Eigen::PardisoLDLT<Eigen::SparseMatrix<double>> pardiso(A);
    //auto X = pardiso.solve(B);
    std::cout << A << std::endl;
}

Eigen::SparseMatrix<double> SetSparseMatrix(int* row, int* col, double* val, int rows, int cols, int nnz, bool isSymmetry)
{
    std::vector< Eigen::Triplet<double>> tripletsA;
    tripletsA.reserve(nnz);

    for (int i = 0; i < nnz; i++)
    {
        if (isSymmetry)
        {
            tripletsA.push_back(Eigen::Triplet<double>(row[i], col[i], val[i]));
            if (row[i] != col[i])
                tripletsA.push_back(Eigen::Triplet<double>(col[i], row[i], val[i]));
        }
        else
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