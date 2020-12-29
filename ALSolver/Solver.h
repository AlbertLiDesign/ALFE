#pragma once

#include <iostream>
#include <string>

#define EIGEN_USE_MKL_ALL
#include <Eigen/Eigen>
#include <Eigen/PardisoSupport>

Eigen::VectorXf SetVector(float* matrix, int dim);
Eigen::SparseMatrix<float> SetCOOMatrix(int* row, int* col, float* val, int rows, int cols, int nnz);
extern "C" __declspec(dllexport) bool SolveFE(int* rows_offset, int* cols, float* vals, float* F, int dim, int dof, int nnz, float* X);