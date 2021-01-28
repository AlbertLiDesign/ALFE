#pragma once

#include <iostream>
#include <string>

#define EIGEN_USE_MKL_ALL
#include <Eigen/Eigen>
#include <Eigen/PardisoSupport>

Eigen::VectorXd SetVector(double* matrix, int dim);
Eigen::SparseMatrix<double> SetCOOMatrix(int* row, int* col, double* val, int rows, int cols, int nnz);
extern "C" __declspec(dllexport) int SolveFE(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X);