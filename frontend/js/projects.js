// projects.js - Lógica de negocio de proyectos 

async function cargarProyectos() {
  
  const userId = localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ID);
  const filtroTitulo = document.getElementById("filtroTitulo");
  const filtroEstado = document.getElementById("filtroEstado");
  
  const title = filtroTitulo ? filtroTitulo.value : '';
  const statusId = filtroEstado ? filtroEstado.value : '';

  const proyectos = await getProyectosPorUsuario(userId, title, statusId);
  renderizarProyectos(proyectos);
}

async function getProyectosConFiltros(filtros) {
  try {
    const params = new URLSearchParams();
    
    // Usar los nombres correctos de parámetros según la API
    if (filtros.title) params.append('title', filtros.title);
    if (filtros.statusId) params.append('status', filtros.statusId); 
    if (filtros.createdByUserId) params.append('applicant', filtros.createdByUserId); 
    if (filtros.approverUserId) params.append('approvalUser', filtros.approverUserId);
    
    const url = `${CONFIG.API_BASE_URL}/Project?${params.toString()}`;
    
    const response = await fetch(url);
    
    if (!response.ok) {
      console.error("Error al obtener proyectos:", response.status);
      return [];
    }
    
    const proyectos = await response.json();
    
    return proyectos;
  } catch (error) {
    console.error("Error al obtener proyectos:", error);
    return [];
  }
}

async function getProyectosPorUsuario(userId, title, statusId) {
  const proyectosCreados = await getProyectosConFiltros({ 
    title, 
    statusId, 
    createdByUserId: userId, 
    approverUserId: null 
  });
  
  const proyectosAprobados = await getProyectosConFiltros({ 
    title, 
    statusId, 
    createdByUserId: null, 
    approverUserId: userId 
  });

  // Unir ambos arrays sin duplicados usando Map
  const proyectosMap = new Map();
  proyectosCreados.forEach(p => proyectosMap.set(p.id, p));
  proyectosAprobados.forEach(p => proyectosMap.set(p.id, p));

  const proyectos = Array.from(proyectosMap.values());
  
  // Mapear a estructura esperada por UI
  return mapearProyectosAPI(proyectos);
}

async function manejarCrearProyecto() {
  const userId = parseInt(localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ID));

  // Estructura correcta que espera tu API - IDs directos
  const nuevoProyecto = {
    title: document.getElementById("tituloProyecto").value,
    description: document.getElementById("descripcionProyecto").value,
    amount: parseFloat(document.getElementById("montoEstimado").value),
    duration: parseInt(document.getElementById("duracionEstimada").value),
    area: parseInt(document.getElementById("areaProyecto").value),
    user: userId,
    type: parseInt(document.getElementById("tipoProyecto").value)
  };

  try {
    const response = await fetch(`${CONFIG.API_BASE_URL}/Project`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(nuevoProyecto)
    });
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error("Error del servidor:", errorText);
      throw new Error("Error al crear proyecto");
    }
    
    const resultado = await response.json();
    console.log("Proyecto creado:", resultado);
    
    alert("Proyecto creado exitosamente");
    
    // Navegar a mis proyectos
    await cargarVista('mis-proyectos');
    
  } catch (error) {
    console.error(error);
    alert("Error al crear el proyecto: " + error.message);
  }
}

async function guardarCambiosProyecto() {
  const id = localStorage.getItem(CONFIG.STORAGE_KEYS.PROJECT_ID);

  // Estructura PATCH que espera tu API
  const proyectoActualizado = {
    title: document.getElementById("inputTituloEdit").value,
    description: document.getElementById("inputDescripcionEdit").value,
    duration: parseInt(document.getElementById("inputDuracionEdit").value)
  };


  try {
    const response = await fetch(`${CONFIG.API_BASE_URL}/Project/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(proyectoActualizado)
    });
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error("Error del servidor:", errorText);
      throw new Error("Error al actualizar proyecto");
    }
    
    alert("Proyecto actualizado correctamente");
    
    // Ocultar el formulario
    ocultarSeccionEditar();
    
    // Recargar el detalle del proyecto
    await cargarDetalleProyecto(id);
    
  } catch (error) {
    console.error("Error al actualizar:", error);
    alert("Error al actualizar el proyecto: " + error.message);
  }}

async function tomarDecisionDesdeFormulario() {
  const proyectoId = localStorage.getItem(CONFIG.STORAGE_KEYS.PROJECT_ID);
  const userId = parseInt(localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ID));
  const userRole = localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ROLE);
  const estado = parseInt(document.getElementById("selectEstado").value);
  const observaciones = document.getElementById("inputObservaciones").value;

  try {
    // Primero obtener el proyecto para encontrar el step ID
    const responseProyecto = await fetch(`${CONFIG.API_BASE_URL}/Project/${proyectoId}`);
    if (!responseProyecto.ok) throw new Error("No se pudo obtener el proyecto");
    
    const proyecto = await responseProyecto.json();
    
    // Encontrar el step pendiente del usuario actual
    const stepPendiente = proyecto.steps?.find(step => {
      const esPendingOObserved = step.status?.name === "Pending" || step.status?.name === "Observed";
      const esDelRol = step.approverRole?.name === userRole;
      return esPendingOObserved && esDelRol;
    });
    
    if (!stepPendiente) {
      alert("No se encontró un paso pendiente para aprobar");
      throw new Error("No se encontró un paso pendiente para aprobar");
    }
    

    // Estructura que espera tu API para tomar decisión
    const decision = {
      id: stepPendiente.id,  
      user: userId,
      status: estado,
      observation: observaciones
    };


    const response = await fetch(`${CONFIG.API_BASE_URL}/Project/${proyectoId}/decision`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(decision)
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("Error del servidor:", errorText);
      throw new Error("Error al registrar decisión");
    }
    
    const resultado = await response.text();
    
    alert("Decisión registrada correctamente");
    
    // Recargar el detalle del proyecto
    await cargarDetalleProyecto(proyectoId);
    
    // Ocultar la sección de decisión
    ocultarSeccionDecision();
    
  } catch (error) {
    console.error("Error al registrar decisión:", error);
    alert("Error al registrar la decisión: " + error.message);
  }
    console.error("Error al registrar decisión:", error);
    alert("Error al registrar la decisión: " + error.message);
  }