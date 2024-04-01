//Eigen::VectorXd Solve_CholmodSupernodalLLT(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
//{
//    Eigen::Map<Eigen::SparseMatrix<double, Eigen::StorageOptions::RowMajor>> A(dim, dim, nnz, rows_offset, cols, vals);
//    auto B = SetVector(F, dim);
//    Eigen::CholmodSupernodalLLT<Eigen::SparseMatrix<double>> cholmodSupernodal(A);
//    cholmodSupernodal.analyzePattern(A);
//    cholmodSupernodal.factorize(A);
//    return cholmodSupernodal.solve(B);
//}