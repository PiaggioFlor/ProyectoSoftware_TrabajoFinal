// utils.js - Funciones auxiliares y utilidades

let estadosAprobacion = [];

function estadoToTexto(id) {
  const estado = estadosAprobacion.find(e => e.id === id);
  return estado ? estado.name : "Desconocido";
}

function formatearFecha(fecha) {
  if (!fecha) return "N/A";
  return new Date(fecha).toLocaleDateString();
}

function formatearFechaHora(fecha) {
  if (!fecha) return "N/A";
  return new Date(fecha).toLocaleString();
}

function formatearMoneda(monto) {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS'
  }).format(monto);
}

function validarFormularioProyecto(datos) {
  const errores = [];

  if (!datos.title || datos.title.trim() === '') {
    errores.push('El título es obligatorio');
  }

  if (!datos.description || datos.description.trim() === '') {
    errores.push('La descripción es obligatoria');
  }

  if (!datos.areaId || datos.areaId === '') {
    errores.push('Debe seleccionar un área');
  }

  if (!datos.type || datos.type === '') {
    errores.push('Debe seleccionar un tipo de proyecto');
  }

  if (!datos.estimatedAmount || datos.estimatedAmount <= 0) {
    errores.push('El monto estimado debe ser mayor a 0');
  }

  if (!datos.estimatedDuration || datos.estimatedDuration <= 0) {
    errores.push('La duración estimada debe ser mayor a 0');
  }

  return errores;
}

function mostrarErrores(errores) {
  const mensaje = errores.join('\n');
  alert(mensaje);
}

function debounce(func, wait) {
  let timeout;
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
}

// Función para manejar errores de forma centralizada
function manejarError(error, mensajeUsuario = "Ha ocurrido un error") {
  console.error("Error:", error);
  alert(`${mensajeUsuario}: ${error.message}`);
}