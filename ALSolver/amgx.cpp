#include "Solver.h"
#include <amgx_c.h>

#include <fstream>
#include <sstream>
#include <iostream>
#include <string>

std::string ReadConfigFile(const std::string& filepath) {
    std::ifstream fileStream(filepath);
    std::stringstream buffer;
    if (fileStream) {
        buffer << fileStream.rdbuf();
        fileStream.close();
        return buffer.str();
    }
    else {
        std::cerr << "Unable to open config file: " << filepath << std::endl;
        exit(EXIT_FAILURE);
    }
}

double* Solve_AMGX(int* rows_offset, int* cols, double* vals, double* F, int dim, int nnz)
{
    std::string configFilePath = "./config.json";
    std::string configStr = ReadConfigFile(configFilePath);

    // ��ʼ�� AMGX ��
    AMGX_initialize();

    // ���� AMGX ����
    AMGX_config_handle cfg;
    AMGX_config_create(&cfg, configStr.c_str());

    // ���� AMGX ���������
    AMGX_matrix_handle A;
    AMGX_vector_handle B, X;
    AMGX_resources_handle rsrc;
    AMGX_resources_create_simple(&rsrc, cfg);
    AMGX_matrix_create(&A, rsrc, AMGX_mode_dDDI);
    AMGX_vector_create(&B, rsrc, AMGX_mode_dDDI);
    AMGX_vector_create(&X, rsrc, AMGX_mode_dDDI);

    // ���� AMGX �����
    AMGX_solver_handle solver;
    AMGX_solver_create(&solver, rsrc, AMGX_mode_dDDI, cfg);

    // ���þ�������
    AMGX_matrix_upload_all(A, dim, nnz, 1, 1, rows_offset, cols, vals, NULL);

    // �����Ҳ���������
    AMGX_vector_upload(B, dim, 1, F);

    // ʹ��Ĭ��ֵ��ʼ��������
    double* x_init = new double[dim](); // ����ʹ�� 0 ��ʼ��
    AMGX_vector_upload(X, dim, 1, x_init);
    delete[] x_init;

    // �������ϵͳ
    AMGX_solver_setup(solver, A);
    AMGX_solver_solve(solver, B, X);

    // ��ȡ���
    double* result = new double[dim];
    AMGX_vector_download(X, result);

    // ����
    AMGX_solver_destroy(solver);
    AMGX_vector_destroy(B);
    AMGX_vector_destroy(X);
    AMGX_matrix_destroy(A);
    AMGX_resources_destroy(rsrc);
    AMGX_config_destroy(cfg);

    // �ر� AMGX ��
    AMGX_finalize();
    return result;
}
