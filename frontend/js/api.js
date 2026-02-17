// api.js - Comunicación con la API 

// ==================== PROYECTOS ====================

async function fetchProyecto(id) {
  try {
    const response = await fetch(`${CONFIG.API_BASE_URL}/Project/${id}`);
    if (!response.ok) throw new Error("No se pudo obtener el proyecto");
    const proyecto = await response.json();
    return proyecto;
  } catch (error) {
    console.error("Error al obtener proyecto:", error);
    throw error;
  }
}

async function crearProyecto(proyecto) {
cargarComboAreas('areaProyecto');
cargarComboTiposProyecto('tipoProyecto');
  try {
    
    const response = await fetch(`${CONFIG.API_BASE_URL}/Project`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(proyecto)
    });
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error("Error del servidor:", errorText);
      throw new Error("Error al crear proyecto");
    }
    
    const resultado = await response.json();
    return resultado;
  } catch (error) {
    console.error("Error al crear proyecto:", error);
    throw error;
  }
}

async function getProyectosConFiltros(filtros) {
  try {
    const params = new URLSearchParams();
    
    if (filtros.title) params.append('title', filtros.title);
    if (filtros.statusId !== null && filtros.statusId !== undefined)
      params.append('statusId', filtros.statusId);
    if (filtros.createdByUserId !== null && filtros.createdByUserId !== undefined)
      params.append('createdByUserId', filtros.createdByUserId);
    if (filtros.approverUserId !== null && filtros.approverUserId !== undefined)
      params.append('approverUserId', filtros.approverUserId);
    
    const url = `${CONFIG.API_BASE_URL}/Project?${params.toString()}`;
    
    const response = await fetch(url);
    
    if (!response.ok) {
      console.error("Error al obtener proyectos:", response.status);
      return [];
    }
    
    const proyectos = await response.json();
    
    return proyectos.map(mapearProyectoAPI);

  } catch (error) {
    console.error("Error al obtener proyectos:", error);
    return [];
  }
}


// ==================== COMBOS ====================

async function cargarComboAreas(selectId = "areaProyecto") {
  try {
    
    const response = await fetch(`${CONFIG.API_BASE_URL}/Area`);
    
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }

    const areas = await response.json();

    const select = document.getElementById(selectId);
    
    if (!select) {
      console.error("ERROR: Select no encontrado con ID:", selectId);
      return;
    }

    select.innerHTML = '<option value="">Seleccionar área</option>';

    areas.forEach((area, index) => {
      const option = document.createElement("option");
      option.value = area.id;
      option.textContent = area.name;
      select.appendChild(option);
    });


  } catch (error) {
    console.error("ERROR al cargar áreas:", error);
    console.error("Stack:", error.stack);
    alert(`Error al cargar áreas: ${error.message}\n\nVerifica:\n1. La API está corriendo\n2. La URL es correcta\n3. No hay problemas de CORS`);
  }
}

async function cargarComboTiposProyecto(selectId = "tipoProyecto") {
  try {
    
    const response = await fetch(`${CONFIG.API_BASE_URL}/ProjectType`);
    
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }

    const tipos = await response.json();

    const select = document.getElementById(selectId);
    
    if (!select) {
      console.error(" ERROR: Select no encontrado con ID:", selectId);
      return;
    }

    select.innerHTML = '<option value="">Seleccionar tipo</option>';

    tipos.forEach((tipo, index) => {
      const option = document.createElement("option");
      option.value = tipo.id;
      option.textContent = tipo.name;
      select.appendChild(option);
    });


  } catch (error) {
    console.error("ERROR al cargar tipos:", error);
    console.error("Stack:", error.stack);
    alert(`Error al cargar tipos de proyecto: ${error.message}\n\nVerifica:\n1. La API está corriendo\n2. La URL es correcta\n3. No hay problemas de CORS`);
  }
}

async function obtenerEstadosAprobacion() {
  try {
    
    const response = await fetch(`${CONFIG.API_BASE_URL}/ApprovalStatus`);

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }

    const estados = await response.json();
    
    // Guardar en variable global
    estadosAprobacion = estados;

    // Intentar cargar en el filtro si existe
    const filtroEstado = document.getElementById("filtroEstado");
    
    if (filtroEstado) {
      filtroEstado.innerHTML = '<option value="">Todos los estados</option>';

      estados.forEach((estado, index) => {
        const option = document.createElement("option");
        option.value = estado.id;
        option.textContent = estado.name;
        filtroEstado.appendChild(option);

      });
    } 

  } catch (error) {
    console.error("ERROR al cargar estados:", error);
    console.error("Stack:", error.stack);
  }
}