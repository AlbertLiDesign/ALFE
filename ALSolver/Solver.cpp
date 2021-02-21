#include "Solver.h"

int Solve_SimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X)
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

int Solve_CholmodSimplicialLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);

    Eigen::VectorXd result;

    Eigen::CholmodSupernodalLLT<Eigen::SparseMatrix<double>> llt(A);

    llt.analyzePattern(A);
    llt.factorize(A);

    result = llt.solve(B);

    for (int i = 0; i < dim; i++)
        X[i] = result(i);

    return 1;
}

int Solve_CholmodSuperNodalLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);

    Eigen::VectorXd result;

    Eigen::CholmodSupernodalLLT<Eigen::SparseMatrix<double>> llt(A);

    llt.analyzePattern(A);
    llt.factorize(A);

    result = llt.solve(B);

    for (int i = 0; i < dim; i++)
        X[i] = result(i);

    return 1;
}


int Solve_PARDISO(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X)
{
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
    auto B = SetVector(F, dim);

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

int Solve_AMG(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X)
{
    SX_MAT A;
    SX_AMG_PARS pars;
    SX_RTN rtn;
    SX_VEC b, x;

    A = sx_mat_create(dim, dim, rows_offset, cols, vals);
    b = sx_vec_create(dim);
    for (int i = 0; i < dim; i++)
        sx_vec_set_entry(&b, i, F[i]);
    x = sx_vec_create(dim);

    sx_amg_pars_init(&pars);
    pars.verb = 3;

    //sx_printf("A: m = %d, n = %d, nnz = %d\n", A.num_rows, A.num_cols, A.num_nnzs);
    //sx_amg_pars_print(&pars);

    rtn = sx_solver_amg(&A, &x, &b, &pars);

    sx_printf("AMG residual: %f\n", rtn.ares); /* absolute residual */
    sx_printf("AMG relative residual: %f\n", rtn.rres); /* relative residual */
    sx_printf("AMG iterations: %f\n", rtn.nits);  /* number of iterations */

    for (int i = 0; i < dim; i++)
        X[i] = sx_vec_get_entry(&x, i);

    sx_mat_destroy(&A);
    sx_vec_destroy(&x);
    sx_vec_destroy(&b);

    return 1;
}



int Solve_AMG_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz, double* X)
{
    amgcl::profiler<> prof;

    // Read sparse matrix from MatrixMarket format.
    // In general this should come pre-assembled.
    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> map(dim, dim, nnz, rows_offset, cols, vals);
  
    Eigen::SparseMatrix<double, Eigen::RowMajor> A(map);

    // Use vector of ones as RHS for simplicity:
    auto b = SetVector(F, dim);

    // Zero initial approximation:
    Eigen::VectorXd x = Eigen::VectorXd::Zero(dim);


    // Compose the solver type
    typedef amgcl::static_matrix<double, 2, 2> dmat_type; // matrix value type in double precision
    typedef amgcl::static_matrix<double, 2, 1> dvec_type; // the corresponding vector value type
    typedef amgcl::static_matrix<float, 2, 2> smat_type; // matrix value type in single precision

    typedef amgcl::backend::builtin<dmat_type> SBackend; // the solver backend
    typedef amgcl::backend::builtin<smat_type> PBackend; // the preconditioner backend

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
    prm.solver.tol = 1e-8;
    prm.solver.maxiter = 100;
    prm.precond.max_levels = 20;
    prm.precond.pre_cycles = 1;
    prm.precond.ncycle = 1;

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

    std::cout << iters << " " << error << std::endl
       << prof << std::endl;

    for (int i = 0; i < dim; i++)
        X[i] = x[i];

    return 1;
}
