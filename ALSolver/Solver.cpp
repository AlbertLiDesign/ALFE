#include "Solver.h"
#include <unsupported/Eigen/SparseExtra>

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
    SX_MAT A = sx_mat_create(dim, dim, rows_offset, cols, vals);

    SX_VEC b = SetSXVector(F, dim);

    SX_VEC x = sx_vec_create(dim);

    SX_AMG_PARS pars;
    SX_INT prob = 3;
    SX_INT nglobal = 0;

    sx_amg_pars_init(&pars);
    pars.maxit = 20;
    pars.verb = 2;

    sx_solver_amg(&A, &x, &b, &pars);

    for (int i = 0; i < dim; i++)
        X[i] = sx_vec_get_entry(&x, i);

    /* release memory */
    //sx_mat_destroy(&A);
    //sx_vec_destroy(&x);
    //sx_vec_destroy(&b);

    return 1;
}

int Solve_AMG_CG(int* rows_offset, int* cols, double* vals, double* F, int dim, int dof, int nnz, double* X)
{
    SX_MAT A = sx_mat_create(dim, dim, rows_offset, cols, vals);
    
    SX_VEC b = SetSXVector(F, dim);

    SX_VEC x = sx_vec_create(dim);

    /* solve Ax = b using CG with AMG preconditioner */
    {
        SX_INT maxits = 2000;  /* maximal iterations */
        SX_FLT tol = 1e-6;     /* stop tolerance */
        SX_FLT err, err0;
        SX_VEC r;     /* residual */
        SX_VEC p;
        SX_VEC z, q;
        SX_FLT rho1, rho0 = 0., alpha, beta;

        /* preconditioner */
        SX_AMG mg;
        SX_AMG_PARS pars;

        /* pars */
        sx_amg_pars_init(&pars);
        pars.maxit = 1;
        sx_amg_setup(&mg, &A, &pars);

        /* create vector */
        r = sx_vec_create(dim);
        z = sx_vec_create(dim);
        q = sx_vec_create(dim);
        p = sx_vec_create(dim);

        /* initial residual */
        sx_blas_mv_amxpbyz(-1., &A, &x, 1., &b, &r);
        err0 = sx_blas_vec_norm2(&r);

        sx_printf("\nsx: solver: CG, preconditioner: AMG\n");
        sx_printf("Convergence settings: relative residual: %f, maximal iterations: %d \n\n", tol, maxits);

        sx_printf("Initial residual: %f \n\n", err0);

        int i;
        for (i = 0; i < maxits; i++) {
            /* supposed to solve preconditioning system Mz = r */
#if 0       /* no pc */
            sx_blas_vec_copy(&r, &z);
#else       /* amg pc */
            sx_blas_vec_set(&z, 0.);
            sx_solver_amg_solve(&mg, &z, &r);
#endif

            /* rho = <r, z> */
            rho1 = sx_blas_vec_dot(&r, &z);

            if (i == 0) {
                /* p = z */
                sx_blas_vec_copy(&z, &p);
            }
            else {
                beta = rho1 / rho0;

                /* update p */
                sx_blas_vec_axpby(1, &z, beta, &p);
            }

            /* save rho */
            rho0 = rho1;

            /* update q */
            sx_blas_mv_mxy(&A, &p, &q);

            /* compute alpha */
            alpha = rho1 / sx_blas_vec_dot(&p, &q);

            /* update x */
            sx_blas_vec_axpy(alpha, &p, &x);

            /* update r */
            sx_blas_vec_axpy(-alpha, &q, &r);

            /* check convergence */
            err = sx_blas_vec_norm2(&r);

            sx_printf("itr: %d,     residual: %f, relative error: %f \n",
                i + 1, err, err / err0);

            if (err / err0 <= tol) break;
        }


        sx_vec_destroy(&r);
        sx_vec_destroy(&p);
        sx_vec_destroy(&z);
        sx_vec_destroy(&q);

        sx_amg_data_destroy(&mg);
    }

    for (int i = 0; i < dim; i++)
        X[i] = sx_vec_get_entry(&x, i);

    /* release memory */
    //sx_mat_destroy(&A);
    //sx_vec_destroy(&x);
    //sx_vec_destroy(&b);

    return 1;
}
