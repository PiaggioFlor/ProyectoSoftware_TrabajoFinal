using Aplication.Interfaces;
using Aplication.Interfaces.Commands;
using Aplication.Interfaces.Querys;
using Aplication.Interfaces.Services;
using Aplication.CustomExceptions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Dtos.Responses;
using Aplication.Dtos.Requests;

namespace Aplication.UseCase
{
    public class ProjectProposalService : IProjectProposalService
    {
        private readonly IUserQuery _userQuery;
        private readonly IAreaQuery _areaQuery;
        private readonly IProjectTypeQuery _projectType;
        private readonly IApprovalStatusQuery _approvalStatusQuery;
        private readonly IProjectProposalCommand _proposalCommand;
        private readonly IApprovalRuleQuery _approvalRuleQuery;
        private readonly IProjectApprovalStepCommand _stepCommand;
        private readonly IProjectApprovalStepQuery _stepQuery;
        private readonly IApproverRoleQuery _approverRoleQuery;
        private readonly IProjectProposalQuery _proposalQuery;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectProposalService(IUserQuery userQuery, IAreaQuery areaQuery, IProjectTypeQuery projectType,
            IApprovalStatusQuery approvalStatusQuery, IProjectProposalCommand proposalCommand, IApprovalRuleQuery approvalRuleQuery,
            IProjectApprovalStepCommand stepCommand, IProjectApprovalStepQuery stepQuery, IApproverRoleQuery approverRoleQuery,
            IProjectProposalQuery proposalQuery, IUnitOfWork unitOfWork)
        {
            _userQuery = userQuery;
            _areaQuery = areaQuery;
            _projectType = projectType;
            _approvalStatusQuery = approvalStatusQuery;
            _proposalCommand = proposalCommand;
            _approvalRuleQuery = approvalRuleQuery;
            _stepCommand = stepCommand;
            _stepQuery = stepQuery;
            _approverRoleQuery = approverRoleQuery;
            _proposalQuery = proposalQuery;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
        {
            // Verificar si el título ya existe
            var existingProposals = await _proposalQuery.GetProjectProposals(request.title, null, null, null);
            if (existingProposals.Any())
                throw new BadRequestException("El titulo ya se encuentra registrado");

            // Crear entidad principal
            var projectProposal = new ProjectProposal
            {
                Title = request.title,
                Description = request.description,
                Area = (int)request.area,
                Type = (int)request.type,
                EstimatedAmount = (int)request.amount,
                EstimatedDuration = (int)request.duration,
                Status = 1, // Estado pendiente
                CreateAt = DateTime.Today,
                CreateBy = (int)request.user,
                ProjectApprovalSteps = new List<ProjectApprovalStep>()
            };

            // Transacción
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Insertar el proyecto
                await _proposalCommand.InsertProjectProposal(projectProposal);

                // Calcular pasos de aprobación
                var steps = await _stepQuery.CalculateSteps(projectProposal);

                // Insertar cada step
                foreach (var step in steps)
                {
                    step.ProjectProposalId = projectProposal.Id;
                    await _stepCommand.InsertProjectApprovalStep(step);
                    //projectProposal.ProjectApprovalSteps.Add(step);
                }

                // Actualizar el proyecto con los steps generados
                await _proposalCommand.UpdateProjectProposal(projectProposal);

                // Confirmar la transacción
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            // Obtener las entidades relacionadas
            var area = await _areaQuery.GetAreaId(projectProposal.Area);
            var type = await _projectType.GetProjectTypeId(projectProposal.Type);
            var user = await _userQuery.GetUserId(projectProposal.CreateBy);
            var status = await _approvalStatusQuery.GetApprovalStatusId(projectProposal.Status);

            return new ProjectResponse
            {
                Id = projectProposal.Id,
                Title = projectProposal.Title,
                Description = projectProposal.Description,
                Amount = projectProposal.EstimatedAmount,
                Duration = projectProposal.EstimatedDuration,
                Area = projectProposal.Areas == null ? null : new Dtos.Responses.GenericResponse
                {
                    Id = projectProposal.Areas.Id,
                    Name = projectProposal.Areas.Name
                },
                Status = projectProposal.ApprovalStatus == null ? null : new Dtos.Responses.GenericResponse
                {
                    Id = projectProposal.ApprovalStatus.Id,
                    Name = projectProposal.ApprovalStatus.Name
                },
                Type = projectProposal.ProjectType == null ? null : new Dtos.Responses.GenericResponse
                {
                    Id = projectProposal.ProjectType.Id,
                    Name = projectProposal.ProjectType.Name
                },
                User = projectProposal.User == null ? null : new UsersResponse
                {
                    Id = projectProposal.User.Id,
                    Name = projectProposal.User.Name,
                    Email = projectProposal.User.Email,
                    ApproverRole = projectProposal.User.ApproverRole == null ? null : new Dtos.Responses.GenericResponse
                    {
                        Id = projectProposal.User.ApproverRole.Id,
                        Name = projectProposal.User.ApproverRole.Name
                    }
                },
                Steps = projectProposal.ProjectApprovalSteps?.Select(s => new ApprovalStepResponse
                {
                    Id = s.Id,
                    StepOrder = s.StepOrder,
                    DecisionDate = s.DecisionDate,
                    Observations = s.Observations,
                    ApproverUser = s.User == null ? null : new UsersResponse
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Email = s.User.Email,
                        ApproverRole = s.User.ApproverRole == null ? null : new Dtos.Responses.GenericResponse
                        {
                            Id = s.User.ApproverRole.Id,
                            Name = s.User.ApproverRole.Name
                        }
                    },
                    ApproverRole = s.ApproverRole == null ? null : new Dtos.Responses.GenericResponse
                    {
                        Id = s.ApproverRole.Id,
                        Name = s.ApproverRole.Name
                    },
                    Status = s.ApprovalStatus == null ? null : new Dtos.Responses.GenericResponse
                    {
                        Id = s.ApprovalStatus.Id,
                        Name = s.ApprovalStatus.Name
                    }
                }).ToList() ?? new List<ApprovalStepResponse>()
            };
        }

        public async Task<ProjectResponse> UpdateProjectAsync(Guid proposalId, UpdateProjectRequest request)
        {
            var proposal = await _proposalQuery.GetProjectProposalId(proposalId);
            if (proposal == null)
                throw new NotFoundException("El proyecto no se ha encontrado");

            if (proposal.Status != 4)
                throw new ConflictException("El proyecto no se encuentra en estado observado");

            var duplicated = await _proposalQuery.GetProjectProposals(request.Title, null, null, null);
            if (duplicated.Any(p => p.Id != proposalId))
                throw new BadRequestException("El titulo ya se encuentra en la basee de datos");

            // Actualizar datos simples
            proposal.Title = request.Title;
            proposal.Description = request.Description;
            proposal.EstimatedDuration = (int)request.Duration;
            proposal.Status = 4;//esto sigue como observed, hasta que alguien apruebe??

            await _proposalCommand.UpdateProjectProposal(proposal);

            // Armar respuesta manualmente
            return new ProjectResponse
            {
                Id = proposal.Id,
                Title = proposal.Title,
                Description = proposal.Description,
                Duration = proposal.EstimatedDuration,
                Amount = proposal.EstimatedAmount,
                Area = new Dtos.Responses.GenericResponse
                {
                    Id = proposal.Areas.Id,
                    Name = proposal.Areas.Name
                },
                Status = new Dtos.Responses.GenericResponse
                {
                    Id = proposal.ApprovalStatus.Id,
                    Name = proposal.ApprovalStatus.Name
                },
                Type = new Dtos.Responses.GenericResponse
                {
                    Id = proposal.ProjectType.Id,
                    Name = proposal.ProjectType.Name
                },
                User = new UsersResponse
                {
                    Id = proposal.User.Id,
                    Name = proposal.User.Name,
                    Email = proposal.User.Email,
                    ApproverRole = new Dtos.Responses.GenericResponse
                    {
                        Id = proposal.User.ApproverRole.Id,
                        Name = proposal.User.ApproverRole.Name
                    }
                },
                Steps = proposal.ProjectApprovalSteps?.Select(step => new ApprovalStepResponse
                {
                    Id = step.Id,
                    StepOrder = step.StepOrder,
                    DecisionDate = step.DecisionDate,
                    Observations = step.Observations,
                    ApproverUser = step.User != null ? new UsersResponse
                    {
                        Id = step.User.Id,
                        Name = step.User.Name,
                        Email = step.User.Email,
                        ApproverRole = step.User.ApproverRole != null ? new Dtos.Responses.GenericResponse
                        {
                            Id = step.User.ApproverRole.Id,
                            Name = step.User.ApproverRole.Name
                        } : null!
                    } : null!,
                    ApproverRole = step.ApproverRole != null ? new Dtos.Responses.GenericResponse
                    {
                        Id = step.ApproverRole.Id,
                        Name = step.ApproverRole.Name
                    } : null!,
                    Status = step.ApprovalStatus != null ? new Dtos.Responses.GenericResponse
                    {
                        Id = step.ApprovalStatus.Id,
                        Name = step.ApprovalStatus.Name
                    } : null!
                }).ToList() ?? new List<ApprovalStepResponse>()
            };
        }


        public async Task<ProjectResponse?> GetProjectProposalByIdAsync(Guid id)
        {
            var project = await _proposalQuery.GetProjectProposalId(id);

            if (project == null)
                throw new NotFoundException("No se ha encontrado el proyecto solicitado");

            return new ProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                Amount = project.EstimatedAmount,
                Duration = project.EstimatedDuration,
                Area = new GenericResponse
                {
                    Id = project.Areas.Id,
                    Name = project.Areas.Name
                },
                Status = new GenericResponse
                {
                    Id = project.ApprovalStatus.Id,
                    Name = project.ApprovalStatus.Name
                },
                Type = new GenericResponse
                {
                    Id = project.ProjectType.Id,
                    Name = project.ProjectType.Name
                },
                User = new UsersResponse
                {
                    Id = project.User.Id,
                    Name = project.User.Name,
                    Email = project.User.Email,
                    ApproverRole = new GenericResponse
                    {
                        Id = project.User.ApproverRole.Id,
                        Name = project.User.ApproverRole.Name
                    }
                },
                Steps = (project.ProjectApprovalSteps ?? new List<ProjectApprovalStep>())
                .Select(s => new ApprovalStepResponse
                {
                    Id = s.Id,
                    StepOrder = s.StepOrder,
                    DecisionDate = s.DecisionDate,
                    Observations = s.Observations,
                    ApproverUser = s.User != null ? new UsersResponse
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Email = s.User.Email,
                        ApproverRole = s.User.ApproverRole != null ? new GenericResponse
                        {
                            Id = s.User.ApproverRole.Id,
                            Name = s.User.ApproverRole.Name
                        } : null!
                    } : null!,
                    ApproverRole = s.ApproverRole != null ? new GenericResponse
                    {
                        Id = s.ApproverRole.Id,
                        Name = s.ApproverRole.Name
                    } : null!,
                    Status = s.ApprovalStatus != null ? new GenericResponse
                    {
                        Id = s.ApprovalStatus.Id,
                        Name = s.ApprovalStatus.Name
                    } : null!
                }).ToList()
            };
        }

        public async Task<ICollection<ProjectResponse>> GetProjectProposals(string? title, int? status, int? applicant, int? approvalUser)
        {
            var proposals = await _proposalQuery.GetProjectProposals(title, status, applicant, approvalUser);


            return proposals.Select(p => new ProjectResponse
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,

                User = p.User == null ? null : new UsersResponse
                {
                    Id = p.User.Id,
                    Name = p.User.Name,
                    Email = p.User.Email,
                    ApproverRole = p.User.ApproverRole == null ? null : new GenericResponse
                    {
                        Id = p.User.ApproverRole.Id,
                        Name = p.User.ApproverRole.Name
                    }
                },
                Area = new GenericResponse
                {
                    Id = p.Areas.Id,
                    Name = p.Areas.Name
                },

                Status = p.ApprovalStatus == null ? null : new GenericResponse
                {
                    Id = p.ApprovalStatus.Id,
                    Name = p.ApprovalStatus.Name
                },

                Type = new GenericResponse
                {
                    Id = p.ProjectType.Id,
                    Name = p.ProjectType.Name
                },

                Amount = p.EstimatedAmount,
                Duration = p.EstimatedDuration,

                Steps = p.ProjectApprovalSteps?.Select(s => new ApprovalStepResponse
                {
                    Id = s.Id,
                    StepOrder = s.StepOrder,
                    DecisionDate = s.DecisionDate,
                    Observations = s.Observations,
                    ApproverUser = s.User == null ? null : new UsersResponse
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Email = s.User.Email,
                        ApproverRole = s.User.ApproverRole == null ? null : new GenericResponse
                        {
                            Id = s.User.ApproverRole.Id,
                            Name = s.User.ApproverRole.Name
                        }
                    },
                    ApproverRole = s.ApproverRole == null ? null : new GenericResponse
                    {
                        Id = s.ApproverRole.Id,
                        Name = s.ApproverRole.Name
                    },
                    Status = s.ApprovalStatus == null ? null : new GenericResponse
                    {
                        Id = s.ApprovalStatus.Id,
                        Name = s.ApprovalStatus.Name
                    }
                }).ToList() ?? new List<ApprovalStepResponse>()

            }).ToList();
        } 

        public async Task<ProjectResponse> ApprovalAction(Guid projectProposalId, UpdateStepRequest request)
        {
            // Buscar la propuesta
            var proposal = await _proposalQuery.GetProjectProposalId(projectProposalId);
            if (proposal == null)
                throw new NotFoundException("No se ha encontrado el proyecto solicitado");

            // Obtener todos los pasos de esa propuesta
            var allSteps = await _stepQuery.GetStepsProposalId(projectProposalId);

            // Buscar el primer paso pendiente u observado
            var step = allSteps
                .Where(s => s.Status == 1 || s.Status == 4)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefault();

            if (step == null)
                throw new NotFoundException("No se ha encontrado el paso de aprobación");

            if (request.Status < 1 || request.Status > 4)
                throw new BadRequestException("Estado inválido");

            if (step.Status != 1 && step.Status != 4)
                throw new ConflictException("El paso ya fue procesado y no puede modificarse");

            if (step.Status == 4 && request.Status == 4)
                throw new ConflictException("El paso ya se encuentra observado");

            if (await _userQuery.GetRoleUser(request.User) != step.ApproverRoleId)
                throw new BadRequestException("El usuario no tiene el rol correspondiente para aprobar este paso");

            // Actualizar el paso
            step.ApproverUserId = request.User;
            step.DecisionDate = DateTime.UtcNow;
            step.Status = request.Status;
            step.Observations = request.Observation;

            await _stepCommand.UpdateProjectApprovalStep(step);

            // Recalcular estado de la propuesta
            if (allSteps.Any(s => s.Status == 3))
                proposal.Status = 3; // Rechazado
            else if (allSteps.All(s => s.Status == 2))
                proposal.Status = 2; // Aprobado
            else if (allSteps.Any(s => s.Status == 4))
                proposal.Status = 4; // Observado
            else
                proposal.Status = 1; // Pendiente

            await _proposalCommand.UpdateProjectProposal(proposal);

            // Recargar el proyecto desde la base de datos con los Includes necesarios
            var updatedProposal = await _proposalQuery.GetProjectProposalId(proposal.Id);
            if (updatedProposal == null)
                throw new NotFoundException("No se pudo recuperar el proyecto actualizado");

            // Mapear el proyecto actualizado
            return new ProjectResponse
            {
                Id = updatedProposal.Id,
                Title = updatedProposal.Title,
                Description = updatedProposal.Description,
                Amount = updatedProposal.EstimatedAmount,
                Duration = updatedProposal.EstimatedDuration,
                Area = updatedProposal.Areas == null ? null : new Dtos.Responses.GenericResponse
                {
                    Id = updatedProposal.Areas.Id,
                    Name = updatedProposal.Areas.Name
                },
                Status = updatedProposal.ApprovalStatus == null ? null : new Dtos.Responses.GenericResponse
                {
                    Id = updatedProposal.ApprovalStatus.Id,
                    Name = updatedProposal.ApprovalStatus.Name
                },
                Type = updatedProposal.ProjectType == null ? null : new Dtos.Responses.GenericResponse
                {
                    Id = updatedProposal.ProjectType.Id,
                    Name = updatedProposal.ProjectType.Name
                },
                User = updatedProposal.User == null ? null : new UsersResponse
                {
                    Id = updatedProposal.User.Id,
                    Name = updatedProposal.User.Name,
                    Email = updatedProposal.User.Email,
                    ApproverRole = updatedProposal.User.ApproverRole == null ? null : new Dtos.Responses.GenericResponse
                    {
                        Id = updatedProposal.User.ApproverRole.Id,
                        Name = updatedProposal.User.ApproverRole.Name
                    }
                },
                Steps = updatedProposal.ProjectApprovalSteps?.Select(s => new ApprovalStepResponse
                {
                    Id = s.Id,
                    StepOrder = s.StepOrder,
                    DecisionDate = s.DecisionDate,
                    Observations = s.Observations,
                    ApproverUser = s.User == null ? null : new UsersResponse
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Email = s.User.Email,
                        ApproverRole = s.User.ApproverRole == null ? null : new Dtos.Responses.GenericResponse
                        {
                            Id = s.User.ApproverRole.Id,
                            Name = s.User.ApproverRole.Name
                        }
                    },
                    ApproverRole = s.ApproverRole == null ? null : new Dtos.Responses.GenericResponse
                    {
                        Id = s.ApproverRole.Id,
                        Name = s.ApproverRole.Name
                    },
                    Status = s.ApprovalStatus == null ? null : new Dtos.Responses.GenericResponse
                    {
                        Id = s.ApprovalStatus.Id,
                        Name = s.ApprovalStatus.Name
                    }
                }).ToList() ?? new List<ApprovalStepResponse>()
            };
        }
        }
    }