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
//    // Setup the solver:
//    amgcl::profiler<> prof;
//
//    auto A = std::tie(dim, rows_offset, cols, vals);
//
//    // The RHS is filled with ones:
//    std::vector<double> f(dim);
//    for (int i = 0; i < dim; i++)
//    {
//        f.push_back(F[i]);
//    }
//    std::vector<double> x(dim, 0.0);
//
//    // Scale the matrix so that it has the unit diagonal.
//// First, find the diagonal values:
//    std::vector<double> D(dim, 1.0);
//    for (ptrdiff_t i = 0; i < dim; ++i) {
//        for (ptrdiff_t j = rows_offset[i], e = rows_offset[i + 1]; j < e; ++j) {
//            if (cols[j] == i) {
//                D[i] = 1 / sqrt(vals[j]);
//                break;
//            }
//        }
//    }
//
//    // Then, apply the scaling in-place:
//    for (ptrdiff_t i = 0; i < dim; ++i) {
//        for (ptrdiff_t j = rows_offset[i], e = rows_offset[i + 1]; j < e; ++j) {
//            vals[j] *= D[i] * D[cols[j]];
//        }
//        f[i] *= D[i];
//    }
//
//    // Compose the solver type
//    typedef amgcl::static_matrix<double, 2, 2> dmat_type; // matrix value type in double precision
//    typedef amgcl::static_matrix<double, 2, 1> dvec_type; // the corresponding vector value type
//    typedef amgcl::static_matrix<float, 2, 2> smat_type; // matrix value type in single precision
//
//    typedef amgcl::backend::builtin<dmat_type> SBackend; // the solver backend
//    typedef amgcl::backend::builtin<smat_type> PBackend; // the preconditioner backend
//
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
//    prm.solver.maxiter = 500;
//    // Initialize the solver with the system matrix.
//    // Use the block_matrix adapter to convert the matrix into
//    // the block format on the fly:
//    prof.tic("setup");
//    auto Ab = amgcl::adapter::block_matrix<dmat_type>(A);
//    Solver solve(Ab, prm);
//    prof.toc("setup");
//
//    // Show the mini-report on the constructed solver:
//    std::cout << solve << std::endl;
//
//    // Solve the system with the zero initial approximation:
//    int iters;
//    double error;
//
//    // Reinterpret both the RHS and the solution vectors as block-valued:
//    auto f_ptr = reinterpret_cast<dvec_type*>(f.data());
//    auto x_ptr = reinterpret_cast<dvec_type*>(x.data());
//    auto vecF = amgcl::make_iterator_range(f_ptr, f_ptr + dim / 2);
//    auto vecX = amgcl::make_iterator_range(x_ptr, x_ptr + dim / 2);
//
//    prof.tic("solve");
//    std::tie(iters, error) = solve(Ab, vecF, vecX);
//    prof.toc("solve");
//
//    // Output the number of iterations, the relative error,
//    // and the profiling data:
//    std::cout << "Iters: " << iters << std::endl
//        << "Error: " << error << std::endl
//        << prof << std::endl;
//
//    for (int i = 0; i < dim; i++)
//        X[i] = x[i];
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
