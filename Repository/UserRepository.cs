using CaixaWEB.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CaixaWEB.Repository
{
    public interface IUserRepository<T> where T : class
    {
        void SaveLog(LogModel log);
        List<LogModel> UserLog(int Id);
        void Save(User Login);
        User FindUser(string Login);
        void Update(int Id, double AccountBalance);
    }

    public class UserRepository : IUserRepository<User>
    {

        private readonly string ConnectionString;
        protected IConfiguration _config;

        public UserRepository(IConfiguration config)
        {
            _config = config;
            ConnectionString = _config.GetConnectionString("DefaultConnection");
        }

        public void SaveLog(LogModel log)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    conn.Execute(@"INSERT INTO [dbo].[LogInfo]
											   ([Details]
											   ,[Date]
                                               ,[UserId])
									
										 VALUES
											   (@Details
											   ,@Date
                                               ,@UserId)", log, transaction: t);
                    t.Commit();
                }
            }

        }

        public List<LogModel> UserLog(int Id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    List<LogModel> result = new List<LogModel>();
                    result = conn.Query<LogModel>(@"SELECT * FROM [dbo].[LogInfo] WHERE [UserId] = @Id", new { Id }, transaction: t).ToList();
                              
                    return result;
                }
            }
        }
        
        public void Save(User Login)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    conn.Execute(@"INSERT INTO [dbo].[User]
											   ([Login]
											   ,[Password]
                                               ,[AccountBalance])
									
										 VALUES
											   (@Login
											   ,@Password
                                               ,@AccountBalance)", Login, transaction: t);
                    t.Commit();
                }
            }
        }

        public User FindUser(string Login)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {

                    User result = conn.Query<User>(@"SELECT * FROM [dbo].[User] WHERE [Login] = @Login", new { Login }, transaction: t).SingleOrDefault();
                    return result;
                }
            }
        }

        public void Update(int Id, double AccountBalance)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    conn.Execute(@"UPDATE [dbo].[User] SET [AccountBalance] = @AccountBalance WHERE Id = @Id", new { AccountBalance, Id }, transaction: t);
                    t.Commit();
                }
            }
        }

    }
}
