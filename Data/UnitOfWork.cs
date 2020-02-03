using EmpDeptBack.Interfaces;
using EmpDeptBack.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpDeptBack.Data
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext Context;


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }

        public GenericRepository<T> Repository<T>() where T : class, new()
        {
            return new GenericRepository<T>(Context);
        }

        #region Employee repository
        private GenericRepository<Employee> employeeRepository;
        public GenericRepository<Employee> EmployeeRepository
        {
            get
            {

                if (employeeRepository == null)
                {
                    employeeRepository = new GenericRepository<Employee>(Context);
                }
                return employeeRepository;
            }
        }
        #endregion

        #region Department repository
        private GenericRepository<Department> departmentRepository;
        public GenericRepository<Department> DepartmentRepository
        {
            get
            {

                if (departmentRepository == null)
                {
                    departmentRepository = new GenericRepository<Department>(Context);
                }
                return departmentRepository;
            }
        }
        #endregion
        public virtual int Save()
        {
            int returnValue = 200;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
                //  {
                try
                {
                    Context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null)
                    {
                        var number = sqlException.Number;

                        if (number == 547)
                        {
                            returnValue = 501;

                        }
                        else
                            returnValue = 500;
                    }
                }
                catch (Exception ex)
                {
                    //Log Exception Handling message                      
                    returnValue = 500;
                    dbContextTransaction.Rollback();
                }
            //    }

            return returnValue;
        }

        public virtual async Task<int> SaveAsync()
        {
            int returnValue = 200;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null)
                    {
                        var number = sqlException.Number;

                        if (number == 547)
                        {
                            returnValue = 501;

                        }
                        else
                            returnValue = 500;
                    }
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    returnValue = 500;
                    dbContextTransaction.Rollback();
                }
            }

            return returnValue;
        }

    }
}
