USE [ProductDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/*
Las versiones anteriores de UserRoles y UserActions no tenian llave primaria.
Si todavia existen asi, se recrean para que Entity Framework pueda importarlas.
*/
DECLARE @RecreateUserRoles bit = 0;
DECLARE @RecreateUserActions bit = 0;

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NOT NULL
   AND OBJECTPROPERTY(OBJECT_ID(N'dbo.UserRoles'), 'TableHasPrimaryKey') = 0
    SET @RecreateUserRoles = 1;

IF OBJECT_ID(N'dbo.UserActions', N'U') IS NOT NULL
   AND OBJECTPROPERTY(OBJECT_ID(N'dbo.UserActions'), 'TableHasPrimaryKey') = 0
    SET @RecreateUserActions = 1;

IF (@RecreateUserRoles = 1 OR @RecreateUserActions = 1)
   AND OBJECT_ID(N'dbo.Permissions', N'U') IS NOT NULL
    DROP TABLE dbo.Permissions;

IF @RecreateUserRoles = 1
    DROP TABLE dbo.UserRoles;

IF @RecreateUserActions = 1
    DROP TABLE dbo.UserActions;
GO

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserRoles
    (
        ID int IDENTITY(1,1) NOT NULL,
        UserID int NOT NULL,
        RoleID int NOT NULL,
        CONSTRAINT PK_UserRoles PRIMARY KEY (ID),
        CONSTRAINT UQ_UserRoles_User_Role UNIQUE (UserID, RoleID),
        CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserID)
            REFERENCES dbo.Users(UserID),
        CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleID)
            REFERENCES dbo.Roles(RoleID)
    );
END;
GO

IF OBJECT_ID(N'dbo.UserActions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserActions
    (
        ID int IDENTITY(1,1) NOT NULL,
        Name nvarchar(100) NOT NULL,
        Description nvarchar(255) NULL,
        CONSTRAINT PK_UserActions PRIMARY KEY (ID),
        CONSTRAINT UQ_UserActions_Name UNIQUE (Name)
    );
END;
GO

IF OBJECT_ID(N'dbo.Permissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permissions
    (
        ID int IDENTITY(1,1) NOT NULL,
        UserRole int NOT NULL,
        UserAction int NOT NULL,
        CONSTRAINT PK_Permissions PRIMARY KEY (ID),
        CONSTRAINT UQ_Permissions_Role_Action UNIQUE (UserRole, UserAction),
        CONSTRAINT FK_Permissions_UserRoles FOREIGN KEY (UserRole)
            REFERENCES dbo.UserRoles(ID),
        CONSTRAINT FK_Permissions_UserActions FOREIGN KEY (UserAction)
            REFERENCES dbo.UserActions(ID)
    );
END;
GO

/* Cambiar estos dos valores antes de ejecutar esta seccion. */
DECLARE @UserEmail nvarchar(256) = N'prueba1@gmail.com';
DECLARE @RoleName nvarchar(255) = N'Admin';

IF @UserEmail = N'CAMBIAR_CORREO@ejemplo.com'
    THROW 50001, 'Debe indicar el correo del usuario registrado en @UserEmail.', 1;

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE Email = @UserEmail)
    THROW 50002, 'El usuario no existe en AspNetUsers. Primero debe registrarlo desde la aplicacion.', 1;

/* Identity registra en AspNetUsers; el modelo ProductDB usa dbo.Users. */
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @UserEmail)
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash, CreatedAt, IsActive)
    SELECT LEFT(UserName, 255), LEFT(Email, 255), LEFT(PasswordHash, 255), GETDATE(), 1
    FROM dbo.AspNetUsers
    WHERE Email = @UserEmail;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = @RoleName)
    INSERT INTO dbo.Roles (RoleName) VALUES (@RoleName);

DECLARE @UserID int = (SELECT TOP 1 UserID FROM dbo.Users WHERE Email = @UserEmail);
DECLARE @RoleID int = (SELECT TOP 1 RoleID FROM dbo.Roles WHERE RoleName = @RoleName);

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.UserRoles
    WHERE UserID = @UserID AND RoleID = @RoleID
)
    INSERT INTO dbo.UserRoles (UserID, RoleID) VALUES (@UserID, @RoleID);

/* Mantiene tambien el campo RoleID que ya existe en dbo.Users. */
UPDATE dbo.Users
SET RoleID = @RoleID
WHERE UserID = @UserID;

IF NOT EXISTS (SELECT 1 FROM dbo.UserActions WHERE Name = N'NotificationsView')
    INSERT INTO dbo.UserActions (Name, Description)
    VALUES (N'NotificationsView', N'Permite consultar notificaciones');

IF NOT EXISTS (SELECT 1 FROM dbo.UserActions WHERE Name = N'NotificationsEdit')
    INSERT INTO dbo.UserActions (Name, Description)
    VALUES (N'NotificationsEdit', N'Permite editar notificaciones');

DECLARE @UserRoleID int =
(
    SELECT ID
    FROM dbo.UserRoles
    WHERE UserID = @UserID AND RoleID = @RoleID
);

INSERT INTO dbo.Permissions (UserRole, UserAction)
SELECT @UserRoleID, UA.ID
FROM dbo.UserActions AS UA
WHERE UA.Name IN (N'NotificationsView', N'NotificationsEdit')
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.Permissions AS P
      WHERE P.UserRole = @UserRoleID
        AND P.UserAction = UA.ID
  );

SELECT U.Email, R.RoleName, UA.Name AS PermissionName
FROM dbo.Users AS U
INNER JOIN dbo.UserRoles AS UR ON UR.UserID = U.UserID
INNER JOIN dbo.Roles AS R ON R.RoleID = UR.RoleID
INNER JOIN dbo.Permissions AS P ON P.UserRole = UR.ID
INNER JOIN dbo.UserActions AS UA ON UA.ID = P.UserAction
WHERE U.UserID = @UserID
ORDER BY UA.Name;
GO
