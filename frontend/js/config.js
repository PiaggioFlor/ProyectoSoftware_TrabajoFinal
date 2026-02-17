const CONFIG = {
  API_BASE_URL: "https://localhost:61773/api",
  ENDPOINTS: {
    USERS: "/User",
    PROJECTS: "/Project",
    AREAS: "/Area",
    PROJECT_TYPES: "/ProjectType",
    STATUS: "/ApprovalStatus",
    ROLE: "/Role",
    DECISION: (id) => `/project/${id}/decision`
  },
  STORAGE_KEYS: {
    USER_ID: "userId",
    USER_NAME: "userName",
    USER_EMAIL: "userEmail",
    USER_ROLE: "rolName",
    PROJECT_ID: "proyectoId"
  }
};

// Exportar para uso en otros archivos
if (typeof module !== 'undefined' && module.exports) {
  module.exports = CONFIG;
}