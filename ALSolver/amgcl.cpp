#include "Solver.h"
#include <vector>
#include <iostream>

#include <amgcl/backend/cuda.hpp>
#include <amgcl/backend/eigen.hpp>
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

// Include CUDA runtime
//#include <cuda_runtime.h>

//Eigen::VectorXd Solve_AMGCL(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
//{
//    // Show the name of the GPU we are using:
//    int device;
//    cudaDeviceProp prop;
//    cudaGetDevice(&device);
//    cudaGetDeviceProperties(&prop, device);
//    std::cout << prop.name << std::endl;
//
//    amgcl::profiler<> prof("AMGCL");
//
//    // Read sparse matrix from inputs
//    Eigen::Map<Eigen::SparseMatrix<double, Eigen::RowMajor>> map(dim, dim, nnz, rows_offset, cols, vals);
//    Eigen::SparseMatrix<double, Eigen::RowMajor> A(map);
//
//    // Use input vector as RHS:
//    Eigen::Map<Eigen::VectorXd> b(F, dim);
//
//    // Zero initial approximation:
//    Eigen::VectorXd x = Eigen::VectorXd::Zero(dim);
//
//    // Define the solver and preconditioner types using CUDA backend
//    typedef amgcl::backend::cuda<double> Backend;
//
//    typedef amgcl::make_solver<
//        amgcl::amg<
//        Backend,
//        amgcl::coarsening::smoothed_aggregation,
//        amgcl::relaxation::spai0
//        >,
//        amgcl::solver::cg<Backend>
//    > Solver;
//
//    Backend::params backend_params;
//    Solver::params solver_params;
//    solver_params.solver.tol = 1e-6;
//    solver_params.solver.maxiter = 300;
//    solver_params.precond.max_levels = 10;
//    solver_params.precond.pre_cycles = 1;
//    solver_params.precond.ncycle = 1;
//    solver_params.precond.coarsening.aggr.block_size = 1;
//
//    prof.tic("setup");
//    Solver solve(A, solver_params);
//    prof.toc("setup");
//    std::cout << solve << std::endl;

    //int iters;
    //double error;

    //prof.tic("solve");
    //std::tie(iters, error) = solve(b, x);
    //prof.toc("solve");

    //std::cout << "Iters: " << iters << std::endl
    //    << "Error: " << error << std::endl
    //    << prof << std::endl;

//    return x;
//}

Eigen::VectorXd Solve_AMGCL(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{

    amgcl::profiler<> prof;

    // Read sparse matrix from MatrixMarket format.
    // In general this should come pre-assembled.
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> map(dim, dim, nnz, rows_offset, cols, vals);
  
    Eigen::SparseMatrix<double, Eigen::RowMajor> A(map);

    // Use vector of ones as RHS for simplicity:
    Eigen::Map<Eigen::VectorXd> b(F, dim);

    // Zero initial approximation:
    Eigen::VectorXd x = Eigen::VectorXd::Zero(dim);

    // Compose the solver type
    typedef amgcl::static_matrix<double, 2, 2> dmat_type;
    typedef amgcl::static_matrix<double, 2, 1> dvec_type;

    typedef amgcl::backend::builtin<dmat_type> SBackend; // the solver backend
    typedef amgcl::backend::builtin<dmat_type> PBackend; // the preconditioner backend

    // Setup the solver:
    typedef amgcl::make_solver<
        amgcl::amg<
        PBackend,
        amgcl::coarsening::smoothed_aggregation,
        amgcl::relaxation::spai0
        >,
        amgcl::solver::cg<SBackend>
    > Solver;

    // Solver parameters
    Solver::params prm;
    prm.solver.tol = 1e-6;
    prm.solver.maxiter = 300;
    prm.precond.max_levels = 10;
    prm.precond.pre_cycles = 1;
    prm.precond.ncycle = 1;
    prm.precond.coarsening.aggr.block_size = 1;

    prof.tic("setup");
    auto Ab = amgcl::adapter::block_matrix<dmat_type>(A);
    Solver solve(Ab, prm);
    prof.toc("setup");
    std::cout << solve << std::endl;

    // Solve the system for the given RHS:
    int    iters;
    double error;

    // Reinterpret both the RHS and the solution vectors as block-valued:
    auto b_ptr = reinterpret_cast<dvec_type*>(b.data());
    auto x_ptr = reinterpret_cast<dvec_type*>(x.data());
    auto b_block = amgcl::make_iterator_range(b_ptr, b_ptr + dim / 2);
    auto x_block = amgcl::make_iterator_range(x_ptr, x_ptr + dim / 2);

    prof.tic("solve");
    std::tie(iters, error) = solve(b_block, x_block);
    prof.toc("solve");

    // Output the number of iterations, the relative error,
    // and the profiling data:
    std::cout << "Iters: " << iters << std::endl
        << "Error: " << error << std::endl
        << prof << std::endl;

    return x;
}

//Eigen::VectorXd Solve_AMGCL(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
//{
//    amgcl::profiler<> prof;
//
//    // 直接映射Eigen稀疏矩阵
//    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> map(dim, dim, nnz, rows_offset, cols, vals);
//    Eigen::SparseMatrix<double, Eigen::RowMajor> A(map);
//
//    // 直接使用Eigen向量
//    Eigen::Map<Eigen::VectorXd> b(F, dim);
//
//    // 零初值近似解向量
//    Eigen::VectorXd x = Eigen::VectorXd::Zero(dim);
//
//    // 定义求解器后端和预处理器后端
//    typedef amgcl::backend::eigen<double> Backend; // 同时作为求解器和预处理器后端
//
//    // 设置求解器：AMG作为预处理器，共轭梯度法作为求解器
//    typedef amgcl::make_solver<
//        amgcl::amg<
//        Backend,
//        amgcl::coarsening::smoothed_aggregation,
//        amgcl::relaxation::spai0
//        >,
//        amgcl::solver::cg<Backend>
//    > Solver;
//
//    // 求解器参数
//    Solver::params prm;
//    prm.solver.tol = 1e-6;
//    prm.solver.maxiter = 300;
//    prm.precond.max_levels = 10;
//    prm.precond.pre_cycles = 1;
//    prm.precond.ncycle = 1;
//    prm.precond.coarsening.aggr.block_size = 1;
//
//    prof.tic("setup");
//    Solver solve(A, prm);
//    prof.toc("setup");
//    std::cout << solve << std::endl;
//
//    // 解决给定右手侧的系统
//    int    iters;
//    double error;
//
//    prof.tic("solve");
//    std::tie(iters, error) = solve(b, x);
//    prof.toc("solve");
//
//    // 输出迭代次数、相对误差和性能数据
//    std::cout << "Iters: " << iters << std::endl
//        << "Error: " << error << std::endl
//        << prof << std::endl;
//
//    return x;
//}
