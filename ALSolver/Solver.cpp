#include "Solver.h"

void SolveFE(int* rowA, int* colA, double* valA, double* F, int dim, int dof, int nnzA, double* X)
{
    auto A = SetSparseMatrix(rowA, colA, valA, dim, dim, nnzA);
    auto B = SetMatrix(F, dim, dof);

    Eigen::PardisoLDLT<Eigen::SparseMatrix<double>> pardiso(A);
    Eigen::MatrixXd result = pardiso.solve(B);
    //Eigen::SimplicialLDLT< Eigen::SparseMatrix<double>> ldlt(A);
    //Eigen::MatrixXd result = ldlt.solve(B);

    for (int i = 0; i < dim; i++)
    {
        for (int j = 0; j < dof; j++)
        {
            X[i * dof + j] = result(i, j);
        }
    }
}

