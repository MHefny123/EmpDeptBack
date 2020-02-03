using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmpDeptBack.Data;
using EmpDeptBack.Interfaces;
using EmpDeptBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EmpDeptBack.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public EmployeeController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<EmployeeModel> Get()
        {
            var result = _unitOfWork.EmployeeRepository.Get(includeProperties: "Department");
            List<EmployeeModel> resultModel = new List<EmployeeModel>();
            foreach (var item in result)
            {
                resultModel.Add(new EmployeeModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    DepartmentId = item.DepartmentId,
                    DepartmentName = item.Department.Name
                });
            }
            return resultModel;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public EmployeeModel Get(int id)
        {
            var result = _unitOfWork.EmployeeRepository
                .Get(filter: x => x.Id == id, includeProperties: "Department")
                .FirstOrDefault();
            if (result == null) return null;

            return new EmployeeModel
            {
                Id = result.Id,
                Name = result.Name,
                DepartmentId = result.DepartmentId,
                DepartmentName = result.Department.Name
            };
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]EmployeeModel employee)
        {
            CreateDepartmentIfNew(employee);
            _unitOfWork.EmployeeRepository.Insert(new Employee
            {
                Name = employee.Name,
                DepartmentId = employee.DepartmentId
            });
            _unitOfWork.Save();
        }

        private void CreateDepartmentIfNew(EmployeeModel employee)
        {
            if (employee.DepartmentId == 0)
            {
                _unitOfWork.DepartmentRepository.Insert(new Department
                {
                    Name = employee.DepartmentName
                });
                
                _unitOfWork.Save();
                employee.DepartmentId = _unitOfWork.DepartmentRepository
                    .Get(x=>x.Name == employee.DepartmentName).FirstOrDefault()?.Id ?? 0;
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]EmployeeModel employee)
        {
            CreateDepartmentIfNew(employee);
            _unitOfWork.EmployeeRepository.Update(new Employee
            {
                Id = employee.Id,
                Name = employee.Name,
                DepartmentId = employee.DepartmentId
            });
            _unitOfWork.Save();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _unitOfWork.EmployeeRepository.Delete(id);
            _unitOfWork.Save();
        }
    }
}
