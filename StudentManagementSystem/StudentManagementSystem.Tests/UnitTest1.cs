using System;
using Microsoft.Data.SqlClient;
using Xunit;

namespace StudentManagementSystem.Tests
{
    public class StudentManagementSystemTest
    {
        private readonly string connectionString = 
            "Data Source=Almeida\\SQLEXPRESS;Initial Catalog=StudentManagementSystem;Integrated Security=True;TrustServerCertificate=True";

        // 🔹 Caso 1 — Criar Aluno
        [Fact]
        public void Caso1_CriarAluno()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            // Limpa registro anterior, se existir
            new SqlCommand("DELETE FROM Student WHERE StudentID = 1", connection).ExecuteNonQuery();

            var cmd = new SqlCommand(
                "INSERT INTO Student (StudentID, FirstName, LastName, DateOfBirth, DepartmentID) " +
                "VALUES (1, 'Maria', 'Silva', '2000-01-01', 1)", connection);

            int rows = cmd.ExecuteNonQuery();
            Assert.Equal(1, rows);

            var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Student WHERE StudentID = 1", connection);
            int count = (int)checkCmd.ExecuteScalar();
            Assert.Equal(1, count);
        }

        // 🔹 Caso 2 — Atualizar Aluno
        [Fact]
        public void Caso2_AtualizarAluno()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            // Garante que o aluno exista
            new SqlCommand(
                "IF NOT EXISTS (SELECT * FROM Student WHERE StudentID = 1) " +
                "INSERT INTO Student (StudentID, FirstName, LastName, DateOfBirth, DepartmentID) " +
                "VALUES (1, 'Maria', 'Silva', '2000-01-01', 1)", connection).ExecuteNonQuery();

            var updateCmd = new SqlCommand(
                "UPDATE Student SET LastName = 'Souza' WHERE StudentID = 1", connection);
            int rows = updateCmd.ExecuteNonQuery();
            Assert.Equal(1, rows);

            var checkCmd = new SqlCommand("SELECT LastName FROM Student WHERE StudentID = 1", connection);
            string lastName = checkCmd.ExecuteScalar()?.ToString();
            Assert.Equal("Souza", lastName);
        }

        // 🔹 Caso 3 — Deletar Aluno
        [Fact]
        public void Caso3_DeletarAluno()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            new SqlCommand(
                "IF NOT EXISTS (SELECT * FROM Student WHERE StudentID = 1) " +
                "INSERT INTO Student (StudentID, FirstName, LastName, DateOfBirth, DepartmentID) " +
                "VALUES (1, 'Maria', 'Souza', '2000-01-01', 1)", connection).ExecuteNonQuery();

            var deleteCmd = new SqlCommand("DELETE FROM Student WHERE StudentID = 1", connection);
            int rows = deleteCmd.ExecuteNonQuery();
            Assert.Equal(1, rows);

            var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Student WHERE StudentID = 1", connection);
            int count = (int)checkCmd.ExecuteScalar();
            Assert.Equal(0, count);
        }

        //  Caso 4 — Adicionar Curso a Aluno ..
        [Fact]
        public void Caso4_AdicionarCursoAAluno()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            // Cria aluno e curso se não existirem
            new SqlCommand("IF NOT EXISTS (SELECT * FROM Student WHERE StudentID = 1) " +
                           "INSERT INTO Student VALUES (1, 'Maria', 'Souza', '2000-01-01', 1)", connection).ExecuteNonQuery();

            new SqlCommand("IF NOT EXISTS (SELECT * FROM Course WHERE CourseID = 10) " +
                           "INSERT INTO Course VALUES (10, 'Lógica de Programação', 1)", connection).ExecuteNonQuery();

            var cmd = new SqlCommand(
                "INSERT INTO Enrollment (StudentID, CourseID) VALUES (1, 10)", connection);
            int rows = cmd.ExecuteNonQuery();
            Assert.Equal(1, rows);
        }

        //  Caso 5 — Inserir Notas e Calcular GPA
        [Fact]
        public void Caso5_InserirNotasECalcularGPA()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            // Garante relação aluno-curso
            new SqlCommand("IF NOT EXISTS (SELECT * FROM Enrollment WHERE StudentID = 1 AND CourseID = 10) " +
                           "INSERT INTO Enrollment (StudentID, CourseID, Grade) VALUES (1, 10, 0)", connection).ExecuteNonQuery();

            // Insere nota 85
            var cmd = new SqlCommand(
                "UPDATE Enrollment SET Grade = 85 WHERE StudentID = 1 AND CourseID = 10", connection);
            int rows = cmd.ExecuteNonQuery();
            Assert.Equal(1, rows);

            // Converte nota para GPA
            new SqlCommand(
                "UPDATE Enrollment SET Grade = 4.0 WHERE Grade >= 80 AND StudentID = 1 AND CourseID = 10", connection).ExecuteNonQuery();

            var checkCmd = new SqlCommand("SELECT Grade FROM Enrollment WHERE StudentID = 1 AND CourseID = 10", connection);
            decimal grade = Convert.ToDecimal(checkCmd.ExecuteScalar());
            Assert.Equal(4.0m, grade);
        }

        //  Caso 6 — Buscar Aluno
        [Fact]
        public void Caso6_BuscarAluno()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            new SqlCommand(
                "IF NOT EXISTS (SELECT * FROM Student WHERE StudentID = 1) " +
                "INSERT INTO Student VALUES (1, 'Maria', 'Souza', '2000-01-01', 1)", connection).ExecuteNonQuery();

            var cmd = new SqlCommand(
                "SELECT s.FirstName, s.LastName, c.CourseName, e.Grade " +
                "FROM Student s " +
                "JOIN Enrollment e ON s.StudentID = e.StudentID " +
                "JOIN Course c ON e.CourseID = c.CourseID " +
                "WHERE s.StudentID = 1", connection);

            using SqlDataReader reader = cmd.ExecuteReader();
            Assert.True(reader.HasRows, "Nenhum dado retornado para o aluno pesquisado.");
        }

        // 🔹 Caso 7 — Inserir Novo Curso
        [Fact]
        public void Caso7_InserirNovoCurso()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Course (CourseID, CourseName, DepartmentID) VALUES (20, 'Banco de Dados', 1)", connection);
            int rows = cmd.ExecuteNonQuery();
            Assert.Equal(1, rows);

            var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Course WHERE CourseID = 20", connection);
            int count = (int)checkCmd.ExecuteScalar();
            Assert.Equal(1, count);
        }

        // 🔹 Caso 8 — Inserir Novo Departamento
        [Fact]
        public void Caso8_InserirNovoDepartamento()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Department (DepartmentID, DepartmentName) VALUES (5, 'Engenharia de Software')", connection);
            int rows = cmd.ExecuteNonQuery();
            Assert.Equal(1, rows);

            var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Department WHERE DepartmentID = 5", connection);
            int count = (int)checkCmd.ExecuteScalar();
            Assert.Equal(1, count);
        }
    }
}
