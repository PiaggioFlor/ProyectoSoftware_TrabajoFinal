// htmlLoader.js - Cargador de vistas HTML

/**
 * Carga una vista HTML en el contenedor especificado
 * @param {string} vista - Nombre de la vista a cargar (sin .html)
 * @param {string} contenedor - ID del contenedor donde cargar la vista
 */
async function cargarVista(vista, contenedor = 'contentView') {
  
  try {
    const response = await fetch(`views/${vista}.html`);
    
    if (!response.ok) {
      throw new Error(`Error al cargar la vista: ${vista}`);
    }
    
    const html = await response.text();
    document.getElementById(contenedor).innerHTML = html;
    
    // Ejecutar scripts inline del HTML cargado
    ejecutarScriptsDeVista(contenedor);
    
    // Disparar evento personalizado para que otros módulos sepan que se cargó la vista
    window.dispatchEvent(new CustomEvent('vistasCargada', { detail: { vista } }));
    
  } catch (error) {
    console.error('Error al cargar vista:', error);
    document.getElementById(contenedor).innerHTML = `
      <div class="container mt-5">
        <div class="alert alert-danger">
          <h4>Error al cargar la vista</h4>
          <p>${error.message}</p>
        </div>
      </div>
    `;
  }
}

/**
 * Ejecuta los scripts inline que están en el HTML cargado
 * @param {string} contenedorId - ID del contenedor
 */
function ejecutarScriptsDeVista(contenedorId) {
  const contenedor = document.getElementById(contenedorId);
  const scripts = contenedor.querySelectorAll('script');
  
  scripts.forEach(oldScript => {
    const newScript = document.createElement('script');
    
    // Copiar atributos
    Array.from(oldScript.attributes).forEach(attr => {
      newScript.setAttribute(attr.name, attr.value);
    });
    
    // Copiar contenido
    newScript.appendChild(document.createTextNode(oldScript.innerHTML));
    
    // Reemplazar el script viejo con el nuevo
    oldScript.parentNode.replaceChild(newScript, oldScript);
  });
}

/**
 * Carga un partial HTML (navbar, footer, etc.)
 * @param {string} partial - Nombre del partial a cargar
 * @param {string} contenedor - ID del contenedor
 */
async function cargarPartial(partial, contenedor) {
  try {
    const response = await fetch(`views/${partial}.html`);
    
    if (!response.ok) {
      throw new Error(`Error al cargar partial: ${partial}`);
    }
    
    const html = await response.text();
    document.getElementById(contenedor).innerHTML = html;
    
    // Actualizar nombre de usuario en navbar si corresponde
    if (partial === 'navbar') {
      actualizarNombreUsuarioEnNavbar();
    }
    
  } catch (error) {
    console.error('Error al cargar partial:', error);
  }
}

/**
 * Actualiza el nombre de usuario en la navbar
 */
function actualizarNombreUsuarioEnNavbar() {
  const userName = localStorage.getItem('userName');
  const nombreUsuarioElement = document.getElementById('nombreUsuario');
  if (nombreUsuarioElement && userName) {
    nombreUsuarioElement.textContent = userName;
  }
}

/**
 * Navega a una vista específica con animación
 * @param {string} vista - Nombre de la vista
 */
function navegarA(vista) {
  const contentView = document.getElementById('contentView');
  
  // Fade out
  contentView.style.opacity = '0';
  contentView.style.transition = 'opacity 0.2s';
  
  setTimeout(async () => {
    await cargarVista(vista);
    // Fade in
    contentView.style.opacity = '1';
  }, 200);
}