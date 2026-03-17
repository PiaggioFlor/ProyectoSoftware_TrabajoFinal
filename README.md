# Sistema de Aprobación de Proyectos

Sistema web para la gestión y aprobación de proyectos internos de una empresa de soluciones tecnológicas. Permite registrar propuestas, generar flujos de aprobación dinamicos y realizar seguimiento del estado de cada solicitud.

## Funcionalidades

- Crear y editar solicitudes de proyectos
- Flujo de aprobación, observación y rechazo según criterios como tipo de proyecto, monto estimado y área solicitante
- Búsqueda y filtrado por título, estado, usuario creador y aprobador
- Visualización del estado completo de cada solicitud y su historial de aprobaciones
- Listado de entidades: Área, Tipo de Proyecto, Estado, Usuario y Rol de Aprobador

## Tecnologías

- **Backend:** C# · ASP.NET · API REST
- **Frontend:** JavaScript · HTML · CSS
- **Base de datos:** SQL Server

## Arquitectura

- Separación en capas (backend / frontend)
- API REST para comunicación entre cliente y servidor
- Implementación de lógica de negocio en el backend

```
/backend    → API REST y lógica de negocio
/frontend   → Interfaz web
```

## Contexto

Proyecto académico desarrollado de forma individual, simulando un entorno empresarial donde se requiere gestionar y automatizar el proceso de aprobación de proyectos.
