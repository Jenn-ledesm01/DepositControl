----------------------------------------------------------------------------
-- TYPE CONFIG
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[TypeConfig] ON 

INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (1, N'Process', N'ProcessConfig', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (2, N'Repository', N'RepositoryConfig', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (3, N'User states', N'UserState', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (4, N'Frequency', N'Frequency', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (5, N'Holydays behavior', N'HolyDaysBehavior', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (6, N'Database provider', N'DbProvider', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (8, N'Visibility', N'Visibility', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (9, N'Access', N'Access', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (10, N'Actions'' Log categories', N'AccionCategory', N'Categories for actions Log', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (11, N'DynamicDataSet', N'DynamicDataSet', N'', 0, 0, 0)
INSERT [dbo].[TypeConfig] ([Id], [Name], [Code], [Description], [Order], [HasIcon], [HasUniqueCode]) VALUES (12, N'DynamicDataSetRowAccionL', N'DynamicDataSetRowAccion', N'', 0, 0, 0)

SET IDENTITY_INSERT [dbo].[TypeConfig] OFF

----------------------------------------------------------------------------
-- TYPE
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Type] ON 

INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (2, N'Import', N'Import', 1, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (3, N'Presentation', N'Presentation', 1, 1, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (4, N'Report', N'Report', 1, 2, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (5, N'Periodic', N'Periodic', 2, 0, N'', NULL, 1)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (6, N'Parametric', N'Parametric', 2, 1, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (7, N'Parametric Config', N'ParametricConfig', 2, 2, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (8, N'Enable', N'Enable', 3, 0, N'', NULL, 1)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (9, N'Disable', N'Disable', 3, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (10, N'Delete', N'Delete', 3, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (12, N'New', N'New', 3, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (13, N'Daily', N'Daily', 4, 1, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (14, N'Weekly', N'Weekly', 4, 2, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (15, N'Biweekly', N'Biweekly', 4, 3, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (16, N'Monthly', N'Monthly', 4, 4, N'', NULL, 1)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (17, N'Quarterly', N'Quarterly', 4, 5, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (18, N'Annual', N'Annual', 4, 6, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (19, N'Custom', N'Custom', 4, 7, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (20, N'Run', N'Run', 5, 1, N'', NULL, 1)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (21, N'Do not run', N'NoRun', 5, 2, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (22, N'Previous Business Day', N'RunPrev', 5, 3, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (23, N'Next Business Day', N'RunNext', 5, 4, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (24, N't-Sql', N'System.Data.SqlClient', 6, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (28, N'Default', N'Default', 8, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (29, N'Normal', N'Normal', 8, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (30, N'Hidden', N'Hide', 8, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (31, N'Folder', N'Folder', 9, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (33, N'Menu', N'Menu', 9, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (34, N'Permission', N'Permission', 9, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (35, N'Security', N'Security', 10, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (36, N'Navigation', N'Navigation', 10, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (37, N'ChangeSet', N'ChangeSet', 11, 0, N'Changes in the entity or list', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (39, N'Delete', N'Delete', 12, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (40, N'Insert', N'Insert', 12, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (41, N'Edit', N'Edit', 12, 0, N'', NULL, 0)
INSERT [dbo].[Type] ([Id], [Name], [Code], [TypeConfig_Id], [Order], [Description], [Icon], [IsDefault]) VALUES (42, N'Process', N'Process', 10, 0, N'', NULL, 0)

SET IDENTITY_INSERT [dbo].[Type] OFF

----------------------------------------------------------------------------
-- ACCESS
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Access] ON 

INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (1, 31, 'Security', '', 'Security', '', '', 'fa fa-lock', 1, null)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (2, 33, 'Users', '', 'Users', '', '~/Security/Users', 'fa fa-user', 2, 1)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (3, 33, 'Profiles', '', 'Profiles', '', '~/Security/Profiles', 'fa fa-users', 1, 1)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (4, 34, 'UserEdit', '', 'UserEdit', '', '', 'glyphicon glyphicon-edit', 0, 2)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (5, 34, 'Developer', '', 'Developer', '', '', 'fa fa-wrench', 20, null)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (6, 31, 'Process', '', 'Process', '', '~/Process', 'fa fa-gears', 3, null)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (7, 34, 'ProfileEdit', '', 'ProfileEdit', '', '', 'glyphicon glyphicon-edit', 0, 3)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (8, 31, 'ReportDesing', '', 'ReportDesing', '', '~/ReportDesign', 'fa fa-gears', 10, null)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (9, 34, 'GridCustomView', '', 'GridCustomView', null, null, null, 0, null)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (10, 34, 'GridCustomViewSetDefaultFor', '', 'GridCustomViewSetDefaultFor', null, null, null, 0, 9)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (11, 34, 'GridCustomViewSetDefaultMe', '', 'GridCustomViewSetDefaultMe', null, null, null, 0, 9)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (12, 34, 'GridCustomViewSave', '', 'GridCustomViewSave', null, null, null, 0, 9)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (13, 34, 'GridCustomViewShare', '', 'GridCustomViewShare', null, null, null, 0, 9)
INSERT INTO Access(Id, [Type_Id], [Name], Description, Code, Data, Url, Icon, Posicion, Parent_Access_id) VALUES (14, 33, 'Audit', '', 'Audit', '', '~/Security/Audit', 'glyphicon glyphicon-search ', 4, 1)
GO

SET IDENTITY_INSERT [dbo].[Access] OFF

----------------------------------------------------------------------------
-- DATA_TYPE
----------------------------------------------------------------------------
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (1, 2, N'', N'List`1', N'', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (2, 2, N'', N'', N'', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (11, 1, N'nchar', N'string', N'DbType.AnsiStringFixedLength', 1)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (12, 1, N'char', N'string', N'DbType.StringFixedLength', 1)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (13, 1, N'ntext', N'string', N'DbType.AnsiString', 1)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (14, 1, N'text', N'string', N'DbType.String', 1)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (15, 1, N'nvarchar', N'string', N'DbType.AnsiString', 1)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (16, 1, N'varchar', N'string', N'DbType.String', 1)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (17, 1, N'image', N'byte[]', N'DbType.Binary', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (18, 1, N'binary', N'byte[]', N'DbType.Binary', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (19, 1, N'varbinary', N'byte[]', N'DbType.Binary', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (20, 1, N'rowversion', N'byte[]', N'DbType.Binary', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (21, 2, N'money', N'decimal', N'DbType.Currency', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (22, 2, N'smallmoney', N'decimal', N'SqlDbType.SmallMoney', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (23, 2, N'numeric', N'decimal', N'DbType.Decimal', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (24, 2, N'decimal', N'decimal', N'DbType.Decimal', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (25, 3, N'date', N'DateTime', N'DbType.Date', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (26, 3, N'datetime', N'DateTime', N'DbType.DateTime', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (27, 3, N'datetime2', N'DateTime', N'DbType.DateTime2', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (28, 2, N'tinyint', N'byte', N'DbType.Byte', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (29, 2, N'smallint', N'int', N'DbType.Int16', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (30, 2, N'int', N'int', N'DbType.Int32', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (31, 2, N'bigint', N'long', N'DbType.Int64', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (32, 2, N'float', N'double', N'DbType.Double', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (33, 2, N'real', N'single', N'DbType.Single', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (34, 1, N'uniqueidentifier', N'string', N'DbType.Guid', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (35, 3, N'timestamp', N'timespan', N'DbType.Time', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (36, 3, N'time', N'timespan', N'DbType.Time', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (37, 3, N'datetimeoffset', N'DateTimeOffset', N'DbType.DateTimeOffset', 0)
INSERT [dbo].[DataType] ([Id], [Behavior], [SQL], [C], [Parameter], [Specified_Length]) VALUES (38, 4, N'bit', N'bool', N'DbType.Boolean', 0)

----------------------------------------------------------------------------
-- PROFILE
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Profile] ON 

INSERT [dbo].[Profile] ([Id], [Name], [State_Type_Id]) VALUES (1, N'Administrator', 8)

SET IDENTITY_INSERT [dbo].[Profile] OFF

----------------------------------------------------------------------------
-- STATUS
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Status] ON 

INSERT [dbo].[Status] ([Id], [Name], [Code], [Description], [StatusConfig_Id], [Order], [Icon], [Active]) VALUES (1, N'Pending', N'Pending', N'', 1, 0, N'', 0)
INSERT [dbo].[Status] ([Id], [Name], [Code], [Description], [StatusConfig_Id], [Order], [Icon], [Active]) VALUES (2, N'Queue', N'Queue', N'', 1, 0, N'', 1)
INSERT [dbo].[Status] ([Id], [Name], [Code], [Description], [StatusConfig_Id], [Order], [Icon], [Active]) VALUES (3, N'Executing', N'Executing', N'', 1, 0, N'', 1)
INSERT [dbo].[Status] ([Id], [Name], [Code], [Description], [StatusConfig_Id], [Order], [Icon], [Active]) VALUES (4, N'Executed', N'Executed', N'', 1, 0, N'', 1)
INSERT [dbo].[Status] ([Id], [Name], [Code], [Description], [StatusConfig_Id], [Order], [Icon], [Active]) VALUES (6, N'Approve', N'Approve', N'', 1, 0, N'', 1)
INSERT [dbo].[Status] ([Id], [Name], [Code], [Description], [StatusConfig_Id], [Order], [Icon], [Active]) VALUES (7, N'Rejected', N'Rejected', N'', 1, 0, N'', 0)
INSERT [dbo].[Status] ([Id], [Name], [Code], [Description], [StatusConfig_Id], [Order], [Icon], [Active]) VALUES (8, N'Error', N'Error', N'', 1, 0, N'', 0)

SET IDENTITY_INSERT [dbo].[Status] OFF

----------------------------------------------------------------------------
-- STATUSCONFIG
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[StatusConfig] ON

INSERT [dbo].[StatusConfig] ([Id], [Name], [Code], [Description], [Order]) VALUES (1, N'Process', N'Process', N'', 0)

SET IDENTITY_INSERT [dbo].[StatusConfig] OFF

----------------------------------------------------------------------------
-- USER
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[User] ON 

GO
INSERT [dbo].[User] ([Id], [Name], [FullName], [LastLogin], [Email], [SessionOpen], [State_Type_Id], [Password], [SecretQuestion_Type_Id], [SecretAnswer], [ActivationKey], [Language_Type_Id], [FailLoginCount], [Company_Id]) VALUES (1, N'admin', N'Administrator', GETDATE(), N'admin@consensusgroup.net', 899, 8, N'ujJTh2rta8ItSm/1PYQGxq2GQZXtFEq1yHYhtsIztUi66uaVbfNG7IwX9eoQ817jy8UUeX7X3dMUVGTioLq0Ew==', NULL, NULL, NULL, NULL, 0, 0)
INSERT [dbo].[User] ([Id], [Name], [FullName], [LastLogin], [Email], [SessionOpen], [State_Type_Id], [Password], [SecretQuestion_Type_Id], [SecretAnswer], [ActivationKey], [Language_Type_Id], [FailLoginCount], [Company_Id]) VALUES (2, N'test', N'Tester', GETDATE(), N'test@consensusgroup.net', 899, 8, N'ujJTh2rta8ItSm/1PYQGxq2GQZXtFEq1yHYhtsIztUi66uaVbfNG7IwX9eoQ817jy8UUeX7X3dMUVGTioLq0Ew==', NULL, NULL, NULL, NULL, 0, 0)
GO

SET IDENTITY_INSERT [dbo].[User] OFF

----------------------------------------------------------------------------
-- REPOSITORY_CONFIG
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[RepositoryConfig] ON

INSERT [dbo].[RepositoryConfig] ([Id], [Name], [Code], [Type_Id], [ViewPage], [EditPage], [Entity_Id], [Deleted]) VALUES (2, N'BcpImport', N'BcpImport', 7, NULL, NULL, 4, 0)
INSERT [dbo].[RepositoryConfig] ([Id], [Name], [Code], [Type_Id], [ViewPage], [EditPage], [Entity_Id], [Deleted]) VALUES (3, N'Conexión', N'Conexión', 6, NULL, NULL, 3, 0)

SET IDENTITY_INSERT [dbo].[RepositoryConfig] OFF

----------------------------------------------------------------------------
-- USER_PROFILE
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[UserProfile] ON

INSERT [dbo].[UserProfile] ([Id], [User_Id], [Profile_Id]) VALUES (54, 1, 1)
INSERT [dbo].[UserProfile] ([Id], [User_Id], [Profile_Id]) VALUES (32, 2, 1)

SET IDENTITY_INSERT [dbo].[UserProfile] OFF

----------------------------------------------------------------------------
-- PROFILE_ACCESS
----------------------------------------------------------------------------
INSERT INTO [dbo].[ProfileAccess] ([Profile_Id], [Access_Id])
	SELECT 1, Id FROM Access
INSERT INTO [dbo].[ProfileAccess] ([Profile_Id], [Access_Id])
	SELECT 2, Id FROM Access

----------------------------------------------------------------------------
-- ACCTION
----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Accion] ON 

INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (25, N'UserNew', N'USUARIO - Nuevo', 0, 45, N'El usuario ;#; ha sido creado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (26, N'UserEdit', N'USUARIO - Editar', 0, 45, N'El usuario ;#; ha sido modificado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (27, N'UserChangeState', N'USUARIO - Cambiar Estado', 0, 45, N'El usuario ;#; ha sido modificado al estado ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (28, N'UserProfilesEdit', N'PERFIL DE USUARIO - Editar', 0, 45, N'El perfil del usuario ;#; ha sido modificado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (29, N'ProfileNew', N'PERFIL DE USUARIO - Nuevo', 0, 45, N'El perfil ;#; ha sido creado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (30, N'ProfileEdit', N'PERFIL DE USUARIO - Editar', 0, 45, N'El perfil ;#; ha sido modificado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (31, N'ProfileAccessEdit', N'PERFIL DE USUARIO - Editar Accesos', 0, 45, N'El acceso al perfil ;#; ha sido modificado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (32, N'ProfileChangeState', N'PERFIL DE USUARIO - Cambiar Estado', 0, 45, N'El perfil ;#; ha sido modificado al estado ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (33, N'Navigation', N'Navegación', 0, 35, N'El usuario ;#; ha navegado a: ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (34, N'LogIn', N'SESIÓN - Inicio', 0, 45, N'El usuario ;#; ha iniciado sesión.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (35, N'Timeout', N'SESIÓN - Tiempo de Espera caducado', 0, 45, N'La sesión del usuario ;#; ha expirado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (36, N'LogOut', N'SESIÓN - Cierre', 0, 45, N'El usuario ;#; se ha desconectado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (37, N'ProcessInstanceChangeState', N'PROCESO - Cambiar Estado Instancia', 0, 46, N'El estado de la instancia del proceso  ;#; ha sido modificado al estado ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (38, N'ProcessConfigSave', N'PROCESO - Cambiar Configuración', 0, 46, N'El usuario ha modificado la configuración:  ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (39, N'ProcessInstanceDelete', N'PROCESO - Eliminar Instancia', 0, 46, N'La instancia del proceso ;#; ha sido eliminada.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (40, N'ProcessInstanceViewDetail', N'PROCESO - Ver Detalles Instancia', 0, 46, N'Ver Detalles de instancia  ;#;')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (41, N'UserProfilesNew', N'USUARIO - Asignación de Perfiles', 0, 45, N'El perfil del usuario ;#; ha sido establecido.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (42, N'ProfileAccessNew', N'PERFIL DE USUARIO - Asignación de Accesos', 0, 45, N'El acceso al perfil ;#; ha sido establecido.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (43, N'UserDoesNotExist', N'SESIÓN - Ingreso de Usuario no válido', 0, 45, N'El usuario ;#; no se encuentra registrado en el sistema')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (44, N'ProfilesExportExcel', N'PERFIL DE USUARIO - Exportación en Excel', 0, 45, N'El usuario ;#; ha exportado el listado de Perfiles en Excel.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (45, N'ProfilesExportPDF', N'PERFIL DE USUARIO - Exportación en PDF', 0, 45, N'El usuario ;#; ha exportado el listado de Perfiles en PDF.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (46, N'UsersExportExcel', N'USUARIO - Exportar en Excel', 0, 45, N'El usuario ;#; ha exportado el listado de Usuarios en Excel.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (47, N'UsersExportPDF', N'USUARIO - Exportar en PDF', 0, 45, N'El usuario ;#; ha exportado el listado de Usuarios en PDF.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (48, N'UsersExportTXT', N'USUARIO - Exportar en TXT', 0, 45, N'El usuario ;#; ha exportado el listado de Usuarios en TXT.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (49, N'ProfilesExportTXT', N'PERFIL DE USUARIO - Exportar en TXT', 0, 45, N'El usuario ;#; ha exportado el listado de Perfiles en TXT.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (50, N'ProccessAccess', N'PROCESO - Acceso', 0, 46, N'El usuario ;#; ha accedido al Proceso: ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (51, N'ParametricAccess', N'PARAMÉTRICA - Acceso', 0, 46, N'El usuario ;#; ha accedido a la Paramétrica: ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (52, N'SecurityAccess', N'SEGURIDAD - Acceso', 0, 46, N'El usuario ;#; ha accedido a Seguridad: ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (53, N'ReportAccess', N'REPORTE - Acceso', 0, 46, N'El usuario ;#; ha accedido al Reporte: ;#;.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (54, N'AuditExportPDF', N'AUDITORÍA - Exportar en PDF', 0, 45, N'El usuario ;#; ha exportado el listado de Auditoria a PDF.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (55, N'UserDelete', N'USUARIO - Eliminar', 0, 45, N'El usuario ;#; ha sido eliminado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (56, N'ProfileDelete', N'PERFIL DE USUARIO - Eliminar', 0, 45, N'El perfil ;#; ha sido eliminado.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (57, N'ProcessInstanceNew', N'PROCESO - Nueva Instancia', 0, 46, N'La instancia del proceso ;#; ha sido creada.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (58, N'CRUDSave', N'PARAMÉTRICA - Edición', 0, 46, NULL)
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (59, N'AuditExportExcel', N'AUDITORÍA - Exportar en Excel', 0, 45, N'El usuario ;#; ha exportado el listado de Auditoria a Excel.')
INSERT [dbo].[Accion] ([Id], [Code], [Name], [Order], [Category_Type_Id], [Message]) VALUES (60, N'ProcessNew', N'PROCESO - Nuevo', 0, 46, N'El usuario ha creado el Proceso: ;#;.')

SET IDENTITY_INSERT [dbo].[Accion] OFF