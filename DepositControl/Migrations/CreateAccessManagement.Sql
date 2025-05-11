DECLARE @Type_Menu int
DECLARE @Type_Permission int

select @Type_Menu = Id from Type where Code like 'Menu';
select @Type_Permission = Id from Type where Code like 'Permission';

INSERT [dbo].[Access] ([Type_Id], [Name], [Description], [Code], [Data], [Url], [Icon], [Posicion], [Parent_Access_Id]) VALUES (@Type_Menu, N'AccessManagement', N'AccessManagement', N'AccessManagement', N'', N'~/Security/AccessManagement', N'fa fa-shield', 0, 1)
DECLARE @Access_AccessManagement int;
SELECT @Access_AccessManagement = Access.Id FROM Access WHERE Access.Code = 'AccessManagement' 

INSERT [dbo].[Access] ([Type_Id], [Name], [Description], [Code], [Data], [Url], [Icon], [Posicion], [Parent_Access_Id]) VALUES (@Type_Permission, N'AccessEdit', N'AccessEdit', N'AccessEdit', N'', N'', N'glyphicon glyphicon-edit', 0, @Access_AccessManagement)
DECLARE @Access_AccessEdit int;
SELECT @Access_AccessEdit = Access.Id FROM Access WHERE Access.Code = 'AccessEdit' 


DECLARE @Profile_Administrator int
SELECT @Profile_Administrator = ID from Profile WHERE Name = 'Administrator'

insert into ProfileAccess(Profile_Id, Access_Id) VALUES 
(@Profile_Administrator, @Access_AccessEdit),
(@Profile_Administrator, @Access_AccessManagement)