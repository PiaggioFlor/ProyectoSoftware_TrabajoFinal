using Aplication.Interfaces.Services;
using Aplication.CustomExceptions;
using Aplication.Dtos;
using Aplication.Dtos.Requests;
using Aplication.Dtos.Responses;
//using Aplication.Exceptions;0
//using Aplication.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace TP1_ORM_Piaggio_Florencia.Controllers
{
    [ApiController]
    [Route("api/")]
    public class InformationController : ControllerBase
    {
        //SERVICES
        private readonly IApprovalStatusService _approvalStatusService;
        private readonly IApproverRoleService _approverRoleService;
        private readonly IAreaService _areaService;
        private readonly IProjectTypeService _projectTypeService;
        private readonly IUserService _userService;
        public InformationController(IApprovalStatusService approvalStatusService,
            IApproverRoleService approverRoleService, IAreaService areaService, IProjectTypeService projectTypeService, IUserService userService)
        {
            _approvalStatusService = approvalStatusService;
            _approverRoleService = approverRoleService;
            _areaService = areaService;
            _projectTypeService = projectTypeService;
            _userService = userService;
        }

        [HttpGet("Area")]
        [ProducesResponseType(typeof(List<GenericResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> AreaGetAll()
        {
            try
            {
                //LLAMADA AL SERVICE
                var areas = await _areaService.GetAreas();
                //DEVUELVE EL RESULTADO
                return Ok(areas);
            }
            catch (Exception ex) { return NotFound(new ApiErrorResponse() { message = ex.Message }); }
        }

        [HttpGet("ProjectType")]
        [ProducesResponseType(typeof(List<GenericResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> TypeGetAll()
        {
            try
            {
                //LLAMADA AL SERVICE
                var types = await _projectTypeService.GetProjectTypes();
                //DEVUELVE EL RESULTADO
                return Ok(types);
            }
            catch (Exception ex) { return NotFound(new ApiErrorResponse() { message = ex.Message }); }
        }

        [HttpGet("Role")]
        [ProducesResponseType(typeof(List<GenericResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> RoleGetAll()
        {
            try
            {
                //LLAMADA AL SERVICE
                var roles = await _approverRoleService.GetApproverRoles();
                //DEVUELVE EL RESULTADO
                return Ok(roles);
            }
            catch (Exception ex) { return NotFound(new ApiErrorResponse() { message = ex.Message }); }
        }

        [HttpGet("ApprovalStatus")]
        [ProducesResponseType(typeof(List<GenericResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> StatusGetAll()
        {
            try
            {
                //LLAMADA AL SERVICE
                var status = await _approvalStatusService.GetApprovalStatus();
                //DEVUELVE EL RESULTADO
                return Ok(status);
            }
            catch (Exception ex) { return NotFound(new ApiErrorResponse() { message = ex.Message }); }
        }

        [HttpGet("User")]
        [ProducesResponseType(typeof(List<UsersResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> UserGetAll()
        {
            try
            {
                //LLAMADA AL SERVICE
                var users = await _userService.GetUsers();
                //DEVUELVE EL RESULTADO
                return Ok(users);
            }
            catch (Exception ex) { return NotFound(new ApiErrorResponse() { message = ex.Message }); }
        }

    }
}