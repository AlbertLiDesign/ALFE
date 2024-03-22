#include "Solver.h"

int SolveSystem(int solver, int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X)
{
    Eigen::VectorXd result;

    switch (solver)
    {
    case 0:
        result = Solve_SimplicialLLT(rows_offset, cols, vals, F, dim, nnz);
        break;
    //case 1:
    //    result = Solve_CholmodSimplicialLLT(rows_offset, cols, vals, F, dim, nnz);
    //    break;
    case 1:
        result = Solve_Serial_PARDISO(rows_offset, cols, vals, F, dim, nnz);
        break;
    case 2:
        result = Solve_CG(rows_offset, cols, vals, F, dim, nnz);
        break;
    case 3:
        result = Solve_PARDISO(rows_offset, cols, vals, F, dim, nnz);
        break;
    case 4:
        result = Solve_AMGX(rows_offset, cols, vals, F, dim, nnz);
        break;
    //case 5:
    //    result = Solve_CholmodSupernodalLLT(rows_offset, cols, vals, F, dim, nnz);
    //    break;
    //case 6:
    //    result = Solve_AMG_CG(rows_offset, cols, vals, F, dim, nnz);
    //        break;
    default:
        break;
    }

    for (int i = 0; i < dim; i++)
        X[i] = result(i);

    return 1;
}

Eigen::VectorXd Solve_SimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);
    //std::cout << B << std::endl;
    Eigen::SimplicialLLT<Eigen::SparseMatrix<double>> llt(A);
    llt.analyzePattern(A);
    llt.factorize(A);
    return llt.solve(B);
}
//Eigen::VectorXd Solve_CholmodSimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
//{
//    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
//    auto B = SetVector(F, dim);
//    Eigen::CholmodSimplicialLLT<Eigen::SparseMatrix<double>> cholmodLLT(A);
//    cholmodLLT.analyzePattern(A);
//    cholmodLLT.factorize(A);
//    return cholmodLLT.solve(B);
//}

Eigen::VectorXd Solve_Serial_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);
    Eigen::PardisoLLT<Eigen::SparseMatrix<double>, 1> pardiso;
    pardiso.pardisoParameterArray()[59] = 0;
    mkl_set_num_threads(1);
    pardiso.analyzePattern(A);
    pardiso.factorize(A);
    return pardiso.solve(B);
}
Eigen::VectorXd Solve_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);
    Eigen::ConjugateGradient<Eigen::SparseMatrix<double>> cg(A);
    return cg.solve(B);
}

Eigen::VectorXd Solve_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);
    Eigen::PardisoLLT<Eigen::SparseMatrix<double>, 1> pardiso;
    pardiso.pardisoParameterArray()[59] = 0;
    pardiso.pardisoParameterArray()[1] = 3; // 	The parallel (OpenMP) version of the nested dissection algorithm.

    pardiso.analyzePattern(A);
    pardiso.factorize(A);
    return pardiso.solve(B);
}
//Eigen::VectorXd Solve_CholmodSupernodalLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
//{
//    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
//    auto B = SetVector(F, dim);
//    Eigen::CholmodSupernodalLLT<Eigen::SparseMatrix<double>> cholmodSupernodal(A);
//    cholmodSupernodal.analyzePattern(A);
//    cholmodSupernodal.factorize(A);
//    return cholmodSupernodal.solve(B);
//}
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
