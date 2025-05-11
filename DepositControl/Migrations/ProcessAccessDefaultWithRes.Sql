PRINT 'CREATE RES [New]'
IF NOT EXISTS(SELECT TOP 1 * FROM Resources WHERE Name = 'New')
BEGIN
    INSERT [dbo].[Resources] ([Culture], [Name], [IsEntity], [EntityName], [ColumName], [RowCodeId], [Value], [Type]) VALUES (N'en', N'New', 0, NULL, NULL, NULL, N'New', N'String')
    INSERT [dbo].[Resources] ([Culture], [Name], [IsEntity], [EntityName], [ColumName], [RowCodeId], [Value], [Type]) VALUES (N'es', N'New', 0, NULL, NULL, NULL, N'Nuevo', N'String')
END

PRINT 'CREATE RES [Configuration]'
IF NOT EXISTS(SELECT TOP 1 * FROM Resources WHERE Name = 'Configuration')
BEGIN
    INSERT [dbo].[Resources] ([Culture], [Name], [IsEntity], [EntityName], [ColumName], [RowCodeId], [Value], [Type]) VALUES (N'en', N'Configuration', 0, NULL, NULL, NULL, N'Configuration', N'String')
    INSERT [dbo].[Resources] ([Culture], [Name], [IsEntity], [EntityName], [ColumName], [RowCodeId], [Value], [Type]) VALUES (N'es', N'Configuration', 0, NULL, NULL, NULL, N'Configuración', N'String')
END

PRINT 'CREATE RES [Change State]'
IF NOT EXISTS(SELECT TOP 1 * FROM Resources WHERE Name = 'changestate')
BEGIN
    INSERT [dbo].[Resources] ([Culture], [Name], [IsEntity], [EntityName], [ColumName], [RowCodeId], [Value], [Type]) VALUES (N'en', N'changestate', 0, NULL, NULL, NULL, N'Change State', N'String')
    INSERT [dbo].[Resources] ([Culture], [Name], [IsEntity], [EntityName], [ColumName], [RowCodeId], [Value], [Type]) VALUES (N'es', N'changestate', 0, NULL, NULL, NULL, N'Cambiar Estado de Instancia de Proceso', N'String')
END

PRINT 'CREATE RES [Comment]'
IF NOT EXISTS(SELECT TOP 1 * FROM Resources WHERE Name = 'Comment')
BEGIN
    INSERT [dbo].[Resources] ([Culture], [Name], [IsEntity], [EntityName], [ColumName], [RowCodeId], [Value], [Type]) VALUES (N'en', N'Comment', 0, NULL, NULL, NULL, N'Comment', N'String')
    INSERT [dbo].[Resources] ([Culture], [Name], [IsEntity], [EntityName], [ColumName], [RowCodeId], [Value], [Type]) VALUES (N'es', N'Comment', 0, NULL, NULL, NULL, N'Comentario', N'String')
END

