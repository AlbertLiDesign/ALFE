#pragma once

#include <iostream>
#include <string>
#include <unsupported/Eigen/SparseExtra>
#include<iostream>

#define EIGEN_USE_MKL_ALL
#include <Eigen/Eigen>
#include <Eigen/PardisoSupport>

Eigen::MatrixXd SetMatrix(double* matrix, int dim, int dof);
Eigen::SparseMatrix<double> SetSparseMatrix(int* row, int* col, double* val, int rows, int cols, int nnz);
extern "C" __declspec(dllexport) void SolveFE(int* rowA, int* colA, double* valA, double*F, int dim, int dof, int nnzA, double* X);