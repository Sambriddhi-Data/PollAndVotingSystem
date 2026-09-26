using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollAndVotingSystem.DTOs.Department;
using PollAndVotingSystem.Repositories;

namespace PollAndVotingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentsController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _departmentRepository.GetAllAsync();
            var result = departments.Select(d => new DepartmentResponseDto { Id = d.Id, Name = d.Name });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return NotFound();

            return Ok(new DepartmentResponseDto { Id = department.Id, Name = department.Name });
        }
    }
}