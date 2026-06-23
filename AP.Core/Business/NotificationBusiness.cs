using AP.Data;
using AP.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace AP.Core.Business
{
    // PARTE 1
    // 1. Implementar Notification Repository mediante una instancia de la clase RepositoryNotification.
    // 2. Crear Notification Repository
    // 3. Debe crear las siguientes funcionalidades PRICIPALES:
    //  3.1 GetNotifications(int id = 0): Devuelve una lista de notificaciones. Si se proporciona un ID, devuelve la notificación específica.
    //  3.2 SaveOrUpdate(Notification notification): Guarda una nueva notificación o actualiza una existente.
    //  3.3 Delete(int id): Elimina una notificación por su ID.
    //  3.4 SearchNotifications(string criteria, string field): Busca notificaciones basadas en un criterio específico y un campo determinado (por ejemplo, "Message", "CreatedBy", etc.).
    // 4. Debe crear las siguientes funcionalidades SECUNDARIAS:
    //  3.5 Funcion que traiga las ultimas 10 por usuario
    //  3.6 Funcion que traiga las primeras 10 por usuario
    //  3.7 Funcion que traiga solo las PARES
    //  3.8 Funcion que traiga solo las IMPARES
    //  3.9 Funcion que filtre por ACTIVOS e INACTIVOS
    // 5. Asegúrese de manejar las excepciones adecuadamente, especialmente al guardar o actualizar notificaciones. (o sea usar try catch en el CONTROLLER)

    // PARTE 2
    // 1. Crear Controller de Notifications: NotificationsController
    // 2. Crear las Vistas correspondientes para cada una de las funcionalidades implementadas en el NotificationBusiness.
    // 3. Debe asegurarse de que utilice el mismo "look & feel" (o sea estilo)
    // 4. La Pantalla de Index debe tener un componente de busqueda igual al de "Products"
    // 5. En la pantalla de Index, cada notificación debe tener opciones para Editar, Detalles y Eliminar, tal cual el ejemplo de products
    // 6. Para las funcionalidades secundarias, puede crear botones en la pantalla de Index para acceder a ellas...
    //      ...o bien, integrarlas como filtros dentro del mismo componente de búsqueda.

    // ENTREGABLE:
    // 1. Clase NotificationsRepository
    // 2. Clase NotificationsBusiness
    // 3. View de Index de Notifications
    // TODO EL PROYECTO NO, solo esas clases
    // Enviarlo como un *.zip

    public class NotificationBusiness
    {
        private readonly IRepositoryNotification _repositoryNotification;

        public NotificationBusiness()
        {
            _repositoryNotification = new RepositoryNotification();
        }

        public IEnumerable<Notification> GetNotifications(int id = 0)
        {
            if (id == 0)
                return _repositoryNotification.GetAll();
            else
                return new List<Notification> { _repositoryNotification.GetById(id) };
        }

        public bool SaveOrUpdate(Notification notification)
        {
            if (notification.id == 0)
                _repositoryNotification.Add(notification);
            else
                _repositoryNotification.Update(notification);
            return true;
        }

        public void Delete(int id)
        {
            _repositoryNotification.Delete(id);
        }

        public IEnumerable<Notification> SearchNotifications(string criteria, string field)
        {
            var notifications = GetNotifications(id: 0);
            switch (field)
            {
                case "message":
                    return notifications.Where(n => n.message != null && n.message.ToLower().Contains(criteria.ToLower()));
                case "user_id":
                    if (int.TryParse(criteria, out int userId))
                        return notifications.Where(n => n.user_id == userId);
                    return new List<Notification>();
                case "created_at":
                    return notifications.Where(n => n.created_at != null && n.created_at.ToString().Contains(criteria));
                default:
                    return new List<Notification>();
            }
        }

        // Últimas 10 notificaciones de un usuario (por ID descendente)
        public IEnumerable<Notification> GetLast10ByUser(int userId)
        {
            return GetNotifications(id: 0)
                .Where(n => n.user_id == userId)
                .OrderByDescending(n => n.id)
                .Take(10);
        }

        // Primeras 10 notificaciones de un usuario (por ID ascendente)
        public IEnumerable<Notification> GetFirst10ByUser(int userId)
        {
            return GetNotifications(id: 0)
                .Where(n => n.user_id == userId)
                .OrderBy(n => n.id)
                .Take(10);
        }

        // Solo notificaciones con ID par
        public IEnumerable<Notification> GetEven()
        {
            return GetNotifications(id: 0).Where(n => n.id % 2 == 0);
        }

        // Solo notificaciones con ID impar
        public IEnumerable<Notification> GetOdd()
        {
            return GetNotifications(id: 0).Where(n => n.id % 2 != 0);
        }

        // Filtrar por estado: activo (is_read = false) o inactivo (is_read = true)
        public IEnumerable<Notification> GetByStatus(bool isRead)
        {
            return GetNotifications(id: 0).Where(n => n.is_read == isRead);
        }
    }
}
