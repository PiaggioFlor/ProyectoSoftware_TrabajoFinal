// mapper.js - Mapea la estructura de la API a la estructura esperada por la UI


function mapearProyectoAPI(proyecto) {

  
  const resultado = {
    id: proyecto.id,
    title: proyecto.title,
    description: proyecto.description,
    estimatedAmount: proyecto.amount,
    estimatedDuration: proyecto.duration,
    
    // Usuario creador
    createByName: proyecto.user?.name || proyecto.createByName || 'Desconocido',
    createBy: proyecto.user?.id || proyecto.createBy || 0,
    
    // Área
    areaName: proyecto.area?.name || 'Sin área',
    areaId: proyecto.area?.id || 0,
    
    // Estado
    statusName: proyecto.status?.name || 'Desconocido',
    statusId: proyecto.status?.id || 0,
    
    // Tipo
    typeName: proyecto.type?.name || 'Sin tipo',
    type: proyecto.type?.id || 0,
    
    // Pasos de aprobación 
    projectApprovalSteps: Array.isArray(proyecto.steps) ? proyecto.steps.map(step => {
      
      // Extraer el nombre del rol
      let roleName;
      if (step.approverRole && typeof step.approverRole === 'object') {
        roleName = step.approverRole.name;

      } else if (typeof step.approverRole === 'string') {
        roleName = step.approverRole;

      } else {
        roleName = 'Sin rol';

      }
      
      const stepMapeado = {
        id: step.id,
        stepOrder: step.stepOrder,
        decisionDate: step.decisionDate,
        observations: step.observations || '',
        
        approverUserName: step.approverUser?.name || 'Sin asignar',
        approverUserId: step.approverUser?.id || 0,
        
        // CRÍTICO: approverRole puede venir como objeto, extraer el name
        approverRoleName: roleName,
        approverRoleId: step.approverRole?.id || 0,
        
        // CRÍTICO: status puede venir como objeto, extraer el name y id
        statusName: step.status?.name || step.statusName || 'Pendiente',
        statusId: step.status?.id || step.statusId || 0
      };
      
      
      return stepMapeado;
    }) : []
  };
  
  
  return resultado;
}

/**
 * Convierte array de proyectos de la API
 */
function mapearProyectosAPI(proyectos) {
  if (!Array.isArray(proyectos)) {
    console.error("proyectos no es un array:", proyectos);
    return [];
  }
  
  const resultado = proyectos.map(mapearProyectoAPI);
  
  return resultado;
}