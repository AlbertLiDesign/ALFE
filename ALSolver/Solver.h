#pragma once

#include <iostream>
#include <string>
#include <vector>

#define EIGEN_USE_MKL_ALL
#include <Eigen/Eigen>
#include <Eigen/PardisoSupport>

#include <unsupported/Eigen/SparseExtra>
//#include <Eigen/CholmodSupport>

//#include<cholmod.h>

//AMGCL_USE_EIGEN_VECTORS_WITH_BUILTIN_BACKEND()
extern "C" __declspec(dllexport) int SolveSystem(int solver, int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X);

Eigen::VectorXd SetVector(double* matrix, int dim);
Eigen::SparseMatrix<double> SetCOOMatrix(int* row, int* col, double* val, int rows, int cols, int nnz);

Eigen::VectorXd Solve_SimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz);
Eigen::VectorXd Solve_Serial_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz);
Eigen::VectorXd Solve_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz);
Eigen::VectorXd Solve_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz);
double* Solve_AMGX(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz);
//double* Solve_CholmodSupernodalLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz);
Eigen::VectorXd Solve_AMGCL(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz);