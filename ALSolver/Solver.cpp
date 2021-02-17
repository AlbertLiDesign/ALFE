#include "Solver.h"

int Solve_SimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);

    Eigen::VectorXd result;

    Eigen::SimplicialLLT<Eigen::SparseMatrix<double>> llt(A);

    llt.analyzePattern(A);
    llt.factorize(A);

    result = llt.solve(B);

    for (int i = 0; i < dim; i++)
        X[i] = result(i);

    return 1;
}

int Solve_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);

    //Eigen::saveMarket(A, "C:/Users/alber/Desktop/mat.mtx", Eigen::UpLoType::Symmetric);
    //Eigen::saveMarket(B, "C:/Users/alber/Desktop/B.mtx");

    Eigen::VectorXd result;

    Eigen::PardisoLLT<Eigen::SparseMatrix<double>, 1> pardiso;
    pardiso.pardisoParameterArray()[59] = 0;
    pardiso.pardisoParameterArray()[1] = 3; // 	The parallel (OpenMP) version of the nested dissection algorithm.

    pardiso.analyzePattern(A);
    pardiso.factorize(A);

    result = pardiso.solve(B);

    for (int i = 0; i < dim; i++)
        X[i] = result(i);

    return 1;
}

int Solve_AMG(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X)
{
    amgcl::profiler<> prof;

    // Read sparse matrix from MatrixMarket format.
    // In general this should come pre-assembled.
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> map(dim, dim, nnz, rows_offset, cols, vals);

    Eigen::SparseMatrix<double, Eigen::RowMajor> A(map);

    // Use vector of ones as RHS for simplicity:
    auto B = SetVector(F, dim);

    // Zero initial approximation:
    Eigen::VectorXd x = Eigen::VectorXd::Zero(dim);

    // Setup the solver:
    typedef amgcl::make_solver<
        amgcl::amg<
        amgcl::backend::builtin<double>,
        amgcl::coarsening::smoothed_aggregation,
        amgcl::relaxation::spai0
        >,
        amgcl::solver::cg<amgcl::backend::builtin<double> >
    > Solver;

    // Solver parameters
    Solver::params prm;
    prm.solver.maxiter = 500;

    prof.tic("setup");
    Solver solve(A);
    prof.toc("setup");
    std::cout << solve << std::endl;

    // Solve the system for the given RHS:
    int    iters;
    double error;
    prof.tic("solve");
    std::tie(iters, error) = solve(B, x);
    prof.toc("solve");

    std::cout << iters << " " << error << std::endl
        << prof << std::endl;

    for (int i = 0; i < dim; i++)
        X[i] = x[i];

    return 1;
}

int Solve_AMG_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X)
{
    amgcl::profiler<> prof;

    // Read sparse matrix from MatrixMarket format.
    // In general this should come pre-assembled.
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> map(dim, dim, nnz, rows_offset, cols, vals);
  
    Eigen::SparseMatrix<double, Eigen::RowMajor> A(map);

    // Use vector of ones as RHS for simplicity:
    auto B = SetVector(F, dim);

    // Zero initial approximation:
    Eigen::VectorXd x = Eigen::VectorXd::Zero(dim);

    // Setup the solver:
    typedef amgcl::make_solver<
        amgcl::amg<
        amgcl::backend::builtin<double>,
        amgcl::coarsening::smoothed_aggregation,
        amgcl::relaxation::spai0
        >,
        amgcl::solver::cg<amgcl::backend::builtin<double> >
    > Solver;

    // Solver parameters
    Solver::params prm;
    prm.solver.maxiter = 500;

    prof.tic("setup");
    Solver solve(A);
    prof.toc("setup");
    std::cout << solve << std::endl;

    // Solve the system for the given RHS:
    int    iters;
    double error;
    prof.tic("solve");
    std::tie(iters, error) = solve(B, x);
    prof.toc("solve");

    std::cout << iters << " " << error << std::endl
       << prof << std::endl;

    for (int i = 0; i < dim; i++)
        X[i] = x[i];

    return 1;
}
