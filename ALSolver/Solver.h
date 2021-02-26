#pragma once

#include <iostream>
#include <string>
#include <vector>

#define EIGEN_USE_MKL_ALL
#include <Eigen/Eigen>
#include <Eigen/PardisoSupport>
#include <Eigen/CholmodSupport>

#include <unsupported/Eigen/SparseExtra>

#include <amgcl/backend/builtin.hpp>
#include <amgcl/adapter/crs_tuple.hpp>

#include <amgcl/make_solver.hpp>
#include <amgcl/amg.hpp>
#include <amgcl/coarsening/smoothed_aggregation.hpp>
#include <amgcl/relaxation/spai0.hpp>

#include <amgcl/solver/cg.hpp>
#include <amgcl/value_type/static_matrix.hpp>
#include <amgcl/adapter/block_matrix.hpp>
#include <amgcl/adapter/eigen.hpp>

#include <amgcl/io/mm.hpp>
#include <amgcl/profiler.hpp>

#include <sxamg.h>

AMGCL_USE_EIGEN_VECTORS_WITH_BUILTIN_BACKEND()

Eigen::VectorXd SetVector(double* matrix, int dim);
Eigen::SparseMatrix<double> SetCOOMatrix(int* row, int* col, double* val, int rows, int cols, int nnz);
extern "C" __declspec(dllexport) int Solve_SimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X);
extern "C" __declspec(dllexport) int Solve_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X);
extern "C" __declspec(dllexport) int Solve_CholmodSimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X);
extern "C" __declspec(dllexport) int Solve_CholmodSuperNodalLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X);
extern "C" __declspec(dllexport) int Solve_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X);
extern "C" __declspec(dllexport) int Solve_AMG(int* rows_offset, int* cols, double* vals, double* F, int dim,int nnz, double* X);
extern "C" __declspec(dllexport) int Solve_AMG_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X);