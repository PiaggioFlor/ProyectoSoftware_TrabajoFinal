// main.js - Inicialización y configuración principal 

// Inicialización cuando el DOM está listo
window.onload = () => {
  inicializarApp();
};

async function inicializarApp() {
  
  // Verificar sesión y cargar vistas correspondientes
  await verificarSesion();
  
  // Obtener estados de aprobación
  obtenerEstadosAprobacion();
}

// Manejo global de errores
window.addEventListener('error', (event) => {
  console.error('Error no capturado:', event.error);
});

window.addEventListener('unhandledrejection', (event) => {
  console.error('Promise rechazada no manejada:', event.reason);
});

// Prevenir pérdida de datos al salir
window.addEventListener('beforeunload', (event) => {
  // Verificar si hay formularios con datos sin guardar
  const formCrear = document.getElementById('formCrearProyecto');
  const formEditar = document.getElementById('formEditarProyecto');
  
  if (formCrear) {
    const titulo = document.getElementById('tituloProyecto');
    if (titulo && titulo.value.trim() !== '') {
      event.preventDefault();
      event.returnValue = '';
    }
  }
  
  if (formEditar) {
    const titulo = document.getElementById('inputTitulo');
    if (titulo && titulo.value.trim() !== '') {
      event.preventDefault();
      event.returnValue = '';
    }
  }
});

// Event listener para cuando se carga una vista
window.addEventListener('vistasCargada', (event) => {
  
  // Acciones específicas según la vista cargada
  switch(event.detail.vista) {
    case 'bienvenida':
      // Actualizar saludo
      const userName = localStorage.getItem('userName');
      const saludo = document.getElementById('saludoUsuario');
      if (saludo && userName) {
        saludo.textContent = `Hola, ${userName}`;
      }
      break;
  }
});