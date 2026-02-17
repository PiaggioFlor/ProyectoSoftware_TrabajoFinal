// ui.js 

function renderizarProyectos(proyectos) {
  const contenedor = document.getElementById("proyectosContainer");
  
  if (!contenedor) {
    console.error("Contenedor de proyectos no encontrado");
    return;
  }
  
  if (!proyectos || proyectos.length === 0) {
    contenedor.innerHTML = `
      <div class="col-12">
        <div class="alert alert-info">
          <i class="bi bi-info-circle"></i> No se encontraron proyectos.
        </div>
      </div>
    `;
    return;
  }

  contenedor.innerHTML = "";
  const currentUserId = parseInt(localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ID));
  const currentUserRole = localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ROLE);
  
//organizamos informacion del usuario para las card

  proyectos.forEach((p, index) => {
    
    const estadoClass = obtenerClaseEstado(p.statusName);
    
    // BOTÓN EDITAR: proyecto Observed (statusId === 4) Y es  creador
    const esObservado = p.statusId === 4 || p.statusName === "Observed";
    const esCreador = p.createBy === currentUserId;
    const puedeEditar = esObservado && esCreador;


    // BOTÓN DECIDIR: Solo si todos los pasos anteriores están aprobados
    // Y el primer paso Pending/Observed es de tu rol
    let puedeTomarDecision = false;

    if (p.projectApprovalSteps && p.projectApprovalSteps.length > 0) {
      // Ordenar los pasos por stepOrder
      const stepsOrdenados = [...p.projectApprovalSteps].sort((a, b) => a.stepOrder - b.stepOrder);
      
      //evaluando los pasos ordenados
      
      let hayRechazado = false;
      let primerPendienteOObservado = null;
      
      for (const step of stepsOrdenados) {
        const esPending = step.statusId === 1 || step.statusName === "Pending";
        const esObservedStep = step.statusId === 4 || step.statusName === "Observed";
        const esAprobado = step.statusId === 2 || step.statusName === "Approved";
        const esRechazado = step.statusId === 3 || step.statusName === "Rejected";
        
        // Si encontramos un paso rechazado, nadie puede decidir nada
        if (esRechazado) {
          hayRechazado = true;
          break;
        }
        
        // Buscar el primer paso pendiente/observado
        if (!primerPendienteOObservado && (esPending || esObservedStep)) {
          primerPendienteOObservado = step;
          
          // Verificar que todos los anteriores estén aprobados
          const stepsAnteriores = stepsOrdenados.filter(s => s.stepOrder < step.stepOrder);
          const todosAprobados = stepsAnteriores.every(s => 
            s.statusId === 2 || s.statusName === "Approved"
          );
          
          if (!todosAprobados) {
            primerPendienteOObservado = null;
          }
          
          break;
        }
      }
      
      // Solo puede decidir si:
      // 1. NO hay ningún paso rechazado
      // 2. Hay un paso pendiente/observado
      // 3. Todos los pasos anteriores están aprobados
      // 4. Ese paso es de su rol
      if (!hayRechazado && primerPendienteOObservado) {
        const esDelRol = primerPendienteOObservado.approverRoleName === currentUserRole;
        puedeTomarDecision = esDelRol;
        
      } 
    }

    const card = document.createElement("div");
    card.className = "col-md-6 col-lg-4";

    card.innerHTML = `
      <div class="card project-card card-fixed-height">
        <div class="card-body">
          <!-- Header -->
          <div class="project-card-header">
            <h5 class="project-card-title">${p.title}</h5>
            <span class="badge ${estadoClass}">${p.statusName}</span>
          </div>

          <!-- Descripción -->
          <p class="project-description">${p.description}</p>

          <!-- Info Grid Compacto -->
          <div class="project-info-grid">
            <div class="project-info-item">
              <span class="project-info-label">Creador</span>
              <span class="project-info-value">${p.createByName}</span>
            </div>
            <div class="project-info-item">
              <span class="project-info-label">Área</span>
              <span class="project-info-value">${p.areaName}</span>
            </div>
            <div class="project-info-item">
              <span class="project-info-label">Tipo</span>
              <span class="project-info-value">${p.typeName}</span>
            </div>
            <div class="project-info-item">
              <span class="project-info-label">Monto</span>
              <span class="project-info-value">$${formatearNumero(p.estimatedAmount)}</span>
            </div>
          </div>

          <!-- Aprobaciones Compactas -->
          <div class="mt-2 mb-2">
            <small class="info-label">APROBACIONES</small>
          </div>
          <div class="approval-steps">
            ${p.projectApprovalSteps && p.projectApprovalSteps.length > 0 ? 
              p.projectApprovalSteps.map(s => `
                <div class="d-flex justify-content-between align-items-center mb-1">
                  <small>${s.approverRoleName}</small>
                  <span class="badge ${obtenerClaseEstado(s.statusName)} badge-sm">${s.statusName}</span>
                </div>
              `).join('') 
              : '<small class="text-muted">Sin pasos</small>'
            }
          </div>

          <!-- Botones de Acción -->
          <div class="card-actions">
            <button class="btn btn-sm btn-naranja flex-fill" onclick="verDetalleProyecto('${p.id}')">
              <i class="bi bi-eye"></i> Ver
            </button>
            
            ${puedeEditar ? `
              <button class="btn btn-sm btn-editar" onclick="verDetalleProyecto('${p.id}')">
                <i class="bi bi-pencil"></i> Editar
              </button>
            ` : ''}
            
            ${puedeTomarDecision ? `
              <button class="btn btn-sm btn-decidir" onclick="verDetalleProyecto('${p.id}')">
                <i class="bi bi-check"></i> Decidir
              </button>
            ` : ''}
          </div>
        </div>
      </div>
    `;
    contenedor.appendChild(card);
  });
}

// Función auxiliar para ir a editar
async function irAEditar(proyectoId) {
  localStorage.setItem(CONFIG.STORAGE_KEYS.PROJECT_ID, proyectoId);
  
  // Cargar el proyecto para guardar sus datos
  try {
    const response = await fetch(`${CONFIG.API_BASE_URL}/Project/${proyectoId}`);
    if (response.ok) {
      const proyecto = await response.json();
      const datosEdicion = {
        titulo: proyecto.title,
        descripcion: proyecto.description,
        duracion: proyecto.duration
      };
      localStorage.setItem('proyectoEditando', JSON.stringify(datosEdicion));
    }
  } catch (error) {
    console.error("Error al cargar proyecto:", error);
  }
  
  await cargarVista('editar-proyecto');
}

// Función para ir a tomar decisión
async function irATomarDecision(proyectoId) {
  localStorage.setItem(CONFIG.STORAGE_KEYS.PROJECT_ID, proyectoId);
  await cargarVista('detalle-proyecto');
  
  // Esperar a que se cargue la vista y luego mostrar la sección de decisión
  setTimeout(() => {
    mostrarSeccionDecision();
  }, 500);
}

async function cargarDetalleProyecto(proyectoId) {
  try {
    const response = await fetch(`${CONFIG.API_BASE_URL}/Project/${proyectoId}`);
    
    if (!response.ok) throw new Error("No se pudo obtener el proyecto");

    const proyectoAPI = await response.json();
    
    // Mapear a estructura esperada
    const proyecto = mapearProyectoAPI(proyectoAPI);
    
    // Guardar en localStorage para la vista
    localStorage.setItem(CONFIG.STORAGE_KEYS.PROJECT_ID, proyectoId);
    
    // Renderizar en la vista actual
    renderizarDetalleEnVista(proyecto);
    
  } catch (error) {
    console.error("Error al cargar detalle:", error);
    alert("Error al cargar el detalle: " + error.message);
  }
}

function renderizarDetalleEnVista(proyecto) {
  // Actualizar información básica
  const titulo = document.getElementById("detalleTitulo");
  const descripcion = document.getElementById("detalleDescripcion");
  const area = document.getElementById("detalleArea");
  const tipo = document.getElementById("detalleTipo");
  const monto = document.getElementById("detalleMonto");
  const duracion = document.getElementById("detalleDuracion");
  const creadoPor = document.getElementById("detalleCreadoPor");
  const estado = document.getElementById("detalleEstado");
  const proyectoId = document.getElementById("detalleProyectoId");

  if (titulo) titulo.textContent = proyecto.title;
  if (descripcion) descripcion.textContent = proyecto.description;
  
  if (area) {
    area.textContent = proyecto.areaName;
    area.dataset.areaId = proyecto.areaId;
  }
  
  if (tipo) {
    tipo.textContent = proyecto.typeName;
    tipo.dataset.tipoId = proyecto.type;
  }
  
  if (proyectoId) proyectoId.dataset.id = proyecto.id;
  if (monto) monto.textContent = formatearNumero(proyecto.estimatedAmount);
  if (duracion) duracion.textContent = proyecto.estimatedDuration;
  if (creadoPor) creadoPor.textContent = proyecto.createByName;
  
  if (estado) {
    estado.textContent = proyecto.statusName;
    estado.className = `badge ${obtenerClaseEstado(proyecto.statusName)}`;
  }

  // Guardar datos para edición
  guardarDatosParaEdicion(proyecto);

  // Mostrar pasos de aprobación
const pasosContainer = document.getElementById("detallePasos");
if (pasosContainer) {
  pasosContainer.innerHTML = "";

  if (proyecto.projectApprovalSteps && proyecto.projectApprovalSteps.length > 0) {
    proyecto.projectApprovalSteps.forEach((step) => {
      const estadoStepClass = obtenerClaseEstado(step.statusName);
      
      // Solo mostrar aprobador si existe
      const aprobadorHTML = step.approverUserName && step.approverUserName !== 'Sin asignar' 
        ? `<div><span class="info-label">Aprobador:</span> ${step.approverUserName}</div>` 
        : '';
      
      // Solo mostrar observaciones si existen
      const observacionesHTML = step.observations 
        ? `<div class="mt-2"><span class="info-label">Observaciones:</span><br><small>${step.observations}</small></div>` 
        : '';
      
      // Solo mostrar fecha si existe y no es null
      const fechaHTML = step.decisionDate && step.decisionDate !== 'null' 
        ? `<div><small class="text-muted">${formatearFechaHora(step.decisionDate)}</small></div>` 
        : '';
      
      const stepCard = document.createElement("div");
      stepCard.className = "approval-step-card";
      stepCard.innerHTML = `
        <div class="approval-step-header">
          <h6 class="approval-step-title">Paso ${step.stepOrder} - ${step.approverRoleName}</h6>
          <span class="badge ${estadoStepClass}">${step.statusName}</span>
        </div>
        <div class="approval-step-info">
          <div>
            ${aprobadorHTML}
            ${fechaHTML}
          </div>
          ${observacionesHTML ? `
            <div>
              <span class="info-label">Observaciones:</span>
              <div style="color: white; margin-top: 4px;">${step.observations}</div>
            </div>
          ` : ''}
        </div>
      `;
      pasosContainer.appendChild(stepCard);
    });
  } else {
    pasosContainer.innerHTML = '<p class="text-muted">No hay pasos de aprobación registrados.</p>';
  }
}

  // Configurar botones
  configurarBotonesDetalle(proyecto);
}

function guardarDatosParaEdicion(proyecto) {
  // Solo guardar los campos que se pueden editar (PATCH)
  const datosEdicion = {
    titulo: proyecto.title,
    descripcion: proyecto.description,
    duracion: proyecto.estimatedDuration
  };
  localStorage.setItem('proyectoEditando', JSON.stringify(datosEdicion));
}

function configurarBotonesDetalle(proyecto) {
  const btnEditar = document.getElementById("btnEditarProyecto");
  const btnDecision = document.getElementById("btnTomarDecision");
  
  const currentUserId = parseInt(localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ID));
  const currentUserRole = localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ROLE);
  
  
  if (!btnEditar) {
    console.error(" Botón #btnEditarProyecto NO EXISTE");
  } else {
    
    const esObservado = proyecto.statusId === 4 || proyecto.statusName === "Observed";
    const esCreador = proyecto.createBy === currentUserId;
    
    const puedeEditar = esObservado && esCreador;
    
    if (puedeEditar) {
      btnEditar.style.display = "inline-block";

    } else {
      btnEditar.style.display = "none";

    }
  }

  // ========== BOTÓN DECIDIR ==========

  if (!btnDecision) {
    console.error("Botón #btnTomarDecision NO EXISTE");
  } else {

    let puedeDecidirDetalle = false;
    
    if (proyecto.projectApprovalSteps && proyecto.projectApprovalSteps.length > 0) {
      const stepsOrdenados = [...proyecto.projectApprovalSteps].sort((a, b) => a.stepOrder - b.stepOrder);
      
      let hayRechazado = false;
      let primerPendienteOObservado = null;
      
      for (const step of stepsOrdenados) {
        const esPending = step.statusId === 1 || step.statusName === "Pending";
        const esObservedStep = step.statusId === 4 || step.statusName === "Observed";
        const esRechazado = step.statusId === 3 || step.statusName === "Rejected";
        
        if (esRechazado) {
          hayRechazado = true;
          break;
        }
        
        if (!primerPendienteOObservado && (esPending || esObservedStep)) {
          primerPendienteOObservado = step;
          
          const stepsAnteriores = stepsOrdenados.filter(s => s.stepOrder < step.stepOrder);
          const todosAprobados = stepsAnteriores.every(s => 
            s.statusId === 2 || s.statusName === "Approved"
          );
          
          if (!todosAprobados) {
            primerPendienteOObservado = null;
          }
          
          break;
        }
      }
      
      if (!hayRechazado && primerPendienteOObservado) {
        const esDelRol = primerPendienteOObservado.approverRoleName === currentUserRole;
        puedeDecidirDetalle = esDelRol;
    }
    
    if (puedeDecidirDetalle) {
      btnDecision.style.display = "inline-block";

    } else {
      btnDecision.style.display = "none";
    }
  }

}}

async function verDetalleProyecto(proyectoId) {
  localStorage.setItem(CONFIG.STORAGE_KEYS.PROJECT_ID, proyectoId);
  await cargarVista('detalle-proyecto');
}

// Función auxiliar para obtener clase CSS según estado
function obtenerClaseEstado(estado) {
  const clases = {
    'Pending': 'bg-warning',
    'Approved': 'bg-success',
    'Rejected': 'bg-danger',
    'Observed': 'bg-info',
    'In Progress': 'bg-primary',
    'Completed': 'bg-dark'
  };
  return clases[estado] || 'bg-secondary';
}

// Función auxiliar para formatear números
function formatearNumero(num) {
  return new Intl.NumberFormat('es-AR').format(num);
}