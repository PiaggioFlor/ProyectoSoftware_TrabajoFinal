// auth.js - Gestión de autenticación y sesión 

async function iniciarSesion() {
  const inputId = parseInt(document.getElementById("inputLoginId").value);

  if (!inputId || inputId <= 0) {
    alert("Por favor, ingrese un ID de usuario válido.");
    return;
  }

  try {
    const response = await fetch(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.USERS}`);
    if (!response.ok) throw new Error("No se pudo obtener la lista de usuarios");

    const usuarios = await response.json();
    const usuario = usuarios.find(u => u.id === inputId);

    if (!usuario) {
      alert("ID de usuario no encontrado.");
      return;
    }

    // Guardar datos de usuario
    localStorage.setItem(CONFIG.STORAGE_KEYS.USER_ID, usuario.id);
    localStorage.setItem(CONFIG.STORAGE_KEYS.USER_NAME, usuario.name);
    localStorage.setItem(CONFIG.STORAGE_KEYS.USER_EMAIL, usuario.email);
    localStorage.setItem(CONFIG.STORAGE_KEYS.USER_ROLE, usuario.approverRole.name); 

    // Ocultar login y mostrar app
    document.getElementById("loginView").style.display = "none";
    document.getElementById("appView").style.display = "block";
    
    // Cargar navbar
    await cargarPartial('navbar', 'navbarView');
    
    // Cargar vista de bienvenida
    await cargarVista('bienvenida');
    
    // Cargar datos iniciales
    cargarComboAreas();
    cargarComboTiposProyecto();
    obtenerEstadosAprobacion();

  } catch (error) {
    console.error("Error al iniciar sesión:", error.message);
    alert("Error al iniciar sesión: " + error.message);
  }
}

function cerrarSesion() {
  if (confirm("¿Está seguro que desea cerrar sesión?")) {
    // Limpiar localStorage
    localStorage.removeItem(CONFIG.STORAGE_KEYS.USER_ID);
    localStorage.removeItem(CONFIG.STORAGE_KEYS.USER_NAME);
    localStorage.removeItem(CONFIG.STORAGE_KEYS.USER_EMAIL);
    localStorage.removeItem(CONFIG.STORAGE_KEYS.USER_ROLE);
    localStorage.removeItem(CONFIG.STORAGE_KEYS.PROJECT_ID);
    
    // Mostrar login y ocultar app
    document.getElementById("loginView").style.display = "block";
    document.getElementById("appView").style.display = "none";
    
    // Limpiar contenido
    document.getElementById('navbarView').innerHTML = '';
    document.getElementById('contentView').innerHTML = '';
    
    // Recargar vista de login
    cargarPartial('login', 'loginView');
  }
}

async function verificarSesion() {
  const userId = localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ID);

  if (userId) {
    // Usuario tiene sesión activa
    document.getElementById("loginView").style.display = "none";
    document.getElementById("appView").style.display = "block";
    
    // Cargar navbar y vista de bienvenida
    await cargarPartial('navbar', 'navbarView');
    await cargarVista('bienvenida');
  } else {
    // Usuario NO tiene sesión
    document.getElementById("loginView").style.display = "block";
    document.getElementById("appView").style.display = "none";
    
    // Cargar vista de login
    await cargarPartial('login', 'loginView');
  }
}

function obtenerUsuarioActual() {
  return {
    id: localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ID),
    name: localStorage.getItem(CONFIG.STORAGE_KEYS.USER_NAME),
    email: localStorage.getItem(CONFIG.STORAGE_KEYS.USER_EMAIL),
    role: localStorage.getItem(CONFIG.STORAGE_KEYS.USER_ROLE)
  };
}