using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmpDeptBack.Data;
using EmpDeptBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmpDeptBack.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public DepartmentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Department> Get()
        {
            return _unitOfWork.DepartmentRepository.Get();
        }
    }
}
