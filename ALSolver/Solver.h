#pragma once

#include <iostream>
#include <string>
#include "sxamg.h"

#define EIGEN_USE_MKL_ALL
#include <Eigen/Eigen>
#include <Eigen/PardisoSupport>

Eigen::VectorXd SetVector(double* matrix, int dim);
SX_VEC SetSXVector(double* matrix, int dim);
Eigen::SparseMatrix<double> SetCOOMatrix(int* row, int* col, double* val, int rows, int cols, int nnz);
SX_MAT SetSXMatrix(int* row, int* col, double* val, int rows, int cols, int nnz);
extern "C" __declspec(dllexport) int SolveSimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X);
extern "C" __declspec(dllexport) int SolvePARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X);
extern "C" __declspec(dllexport) int SolveSXAMG(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X);