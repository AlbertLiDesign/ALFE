//#include "Solver.h"
//
//#include <amgcl/backend/builtin.hpp>
//#include <amgcl/adapter/crs_tuple.hpp>
//#include <vector>
//#include <iostream>
//
//#include <amgcl/make_solver.hpp>
//#include <amgcl/amg.hpp>
//#include <amgcl/coarsening/smoothed_aggregation.hpp>
//#include <amgcl/relaxation/spai0.hpp>
//
//#include <amgcl/solver/cg.hpp>
//#include <amgcl/value_type/static_matrix.hpp>
//#include <amgcl/adapter/block_matrix.hpp>
//#include <amgcl/adapter/eigen.hpp>
//
//#include <amgcl/io/mm.hpp>
//#include <amgcl/profiler.hpp>
//
//Eigen::VectorXd Solve_AMG_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
//{
//    amgcl::profiler<> prof;
//
//    // Read sparse matrix from MatrixMarket format.
//    // In general this should come pre-assembled.
//    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> map(dim, dim, nnz, rows_offset, cols, vals);
//  
//    Eigen::SparseMatrix<double, Eigen::RowMajor> A(map);
//
//    // Use vector of ones as RHS for simplicity:
//    auto b = SetVector(F, dim);
//
//    // Zero initial approximation:
//    Eigen::VectorXd x = Eigen::VectorXd::Zero(dim);
//
//    // Compose the solver type
//    typedef amgcl::static_matrix<double, 2, 2> dmat_type; // matrix value type in double precision
//    typedef amgcl::static_matrix<double, 2, 1> dvec_type; // the corresponding vector value type
//    typedef amgcl::static_matrix<float, 2, 2> smat_type; // matrix value type in single precision
//
//    typedef amgcl::backend::builtin<dmat_type> SBackend; // the solver backend
//    typedef amgcl::backend::builtin<smat_type> PBackend; // the preconditioner backend
//
//    // Setup the solver:
//    typedef amgcl::make_solver<
//        amgcl::amg<
//        PBackend,
//        amgcl::coarsening::smoothed_aggregation,
//        amgcl::relaxation::spai0
//        >,
//        amgcl::solver::cg<SBackend>
//    > Solver;
//
//    // Solver parameters
//    Solver::params prm;
//    prm.solver.tol = 1e-8;
//    prm.solver.maxiter = 100;
//    prm.precond.max_levels = 20;
//    prm.precond.pre_cycles = 1;
//    prm.precond.ncycle = 1;
//    prm.precond.coarsening.aggr.block_size = 2;
//
//    prof.tic("setup");
//    auto Ab = amgcl::adapter::block_matrix<dmat_type>(A);
//    Solver solve(Ab, prm);
//    prof.toc("setup");
//    std::cout << solve << std::endl;
//
//    // Solve the system for the given RHS:
//    int    iters;
//    double error;
//    
//    // Reinterpret both the RHS and the solution vectors as block-valued:
//    auto b_ptr = reinterpret_cast<dvec_type*>(b.data());
//    auto x_ptr = reinterpret_cast<dvec_type*>(x.data());
//    auto b_block = amgcl::make_iterator_range(b_ptr, b_ptr + dim / 2);
//    auto x_block = amgcl::make_iterator_range(x_ptr, x_ptr + dim / 2);
//
//    prof.tic("solve");
//    std::tie(iters, error) = solve(b_block, x_block);
//    prof.toc("solve");
//
//    std::cout << iters << " " << error << std::endl
//       << prof << std::endl;
//
//    return x;
//}
