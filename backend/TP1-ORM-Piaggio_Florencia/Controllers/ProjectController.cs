using Aplication.Interfaces.Services;
using Aplication.CustomExceptions;
using Aplication.Dtos;
using Aplication.Dtos.Requests;
using Aplication.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace TP1_ORM_Piaggio_Florencia.Controllers
{
    [ApiController]
    [Route("api/project")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectProposalService _projectProposalService;

        public ProjectController(IProjectProposalService projectProposalService)
        {
            _projectProposalService = projectProposalService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectResponse), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Error de validación.";

                return BadRequest(new ApiErrorResponse { message = firstError });
            }

            try
            {
                var createdProject = await _projectProposalService.CreateAsync(request);
                return Created(string.Empty, createdProject);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiErrorResponse { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiErrorResponse { message = "El ID del proyecto no puede estar vacío." });
            }

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Error de validación.";

                return BadRequest(new ApiErrorResponse { message = firstError });
            }

            try
            {
                var result = await _projectProposalService.GetProjectProposalByIdAsync(id);

                if (result == null)
                {
                    return NotFound(new ApiErrorResponse { message = "No se ha encontrado el proyecto solicitado" });
                }

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorResponse { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiErrorResponse { message = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<ProjectResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<IActionResult> GetProjects(
            [FromQuery] string? title,
            [FromQuery] int? status,
            [FromQuery] int? applicant,
            [FromQuery] int? approvalUser)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Error de validación.";

                return BadRequest(new ApiErrorResponse { message = firstError });
            }

            if (title != null && title.Length > 255)
            {
                return BadRequest(new ApiErrorResponse { message = "El título no puede superar 255 caracteres." });
            }

            try
            {
                var projects = await _projectProposalService.GetProjectProposals(title, status, applicant, approvalUser);
                return Ok(projects);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiErrorResponse { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ProjectResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 409)]
        public async Task<IActionResult> UpdateProjectInformation(
            Guid id,
            [FromBody] UpdateProjectRequest request)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiErrorResponse { message = "El ID del proyecto no puede estar vacío." });
            }

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Error de validación.";

                return BadRequest(new ApiErrorResponse { message = firstError });
            }

            try
            {
                var result = await _projectProposalService.UpdateProjectAsync(id, request);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorResponse { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new ApiErrorResponse { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiErrorResponse { message = ex.Message });
            }
        }

        [HttpPatch("{id}/decision")]
        [ProducesResponseType(typeof(ProjectResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 409)]
        public async Task<IActionResult> UpdateStepStatus(
            Guid id,
            [FromBody] UpdateStepRequest request)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiErrorResponse { message = "El ID del proyecto no puede estar vacío." });
            }

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Error de validación.";

                return BadRequest(new ApiErrorResponse { message = firstError });
            }

            try
            {
                var result = await _projectProposalService.ApprovalAction(id, request);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorResponse { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new ApiErrorResponse { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiErrorResponse { message = ex.Message });
            }
        }
    }
}