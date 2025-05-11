Alter Table [Status] add Color varchar(100) 

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
               
ALTER PROCEDURE [dbo].[Status_Sp_Save]
(@Id smallint, @Name varchar(100), @Code varchar(50), @Description varchar(500), @StatusConfig_Id smallint, @Order smallint, @Icon varchar(255), @Active bit, @Color varchar(100))
as
IF (SELECT COUNT(*) from dbo.[Status] WHERE ( [Id] = @Id)  ) > 0
    BEGIN
        UPDATE dbo.[Status]
            SET  [Name] = @Name, [Code] = @Code, [Description] = @Description, [StatusConfig_Id] = @StatusConfig_Id, [Order] = @Order, [Icon] = @Icon, [Active] = @Active, [Color] = @Color 
            WHERE ( [Id] = @Id)
        SELECT @Id as Id
    END
ELSE
    BEGIN
        
        INSERT INTO dbo.[Status]   
              ([Name], [Code], [Description], [StatusConfig_Id], [Order], [Icon], [Active], [Color]) 
       VALUES (@Name , @Code , @Description , @StatusConfig_Id , @Order , @Icon , @Active , @Color  )
        SELECT SCOPE_IDENTITY() as Id
    END

Go
/****** Object:  StoredProcedure [dbo].[Status_Sp_GetByFilter]    Script Date: 07/05/2019 11:05:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
            

ALTER PROCEDURE [dbo].[Status_Sp_GetByFilter] 
(@Name varchar(100) = null, @Code varchar(50) = null, @Description varchar(500) = null, @StatusConfig_Id smallint = null, @StatusConfig_Ids varchar(MAX) = null, @Order smallint = null, @Icon varchar(255) = null, @Active bit = null, @Color varchar(100) = null, @Page int = null, @PageSize int = null) 
AS
SET NOCOUNT ON;
SELECT PAG.*, ROW_NUMBER() OVER (ORDER BY PAG.[Id] ASC) AS RowNumber 
INTO #RESUL
FROM dbo.[Status] AS PAG
WHERE (@Name is null or [Name] like @Name)
	 and (@Code is null or [Code] like @Code)
	 and (@Description is null or [Description] like @Description)
	 and (@StatusConfig_Id is null or [StatusConfig_Id] = @StatusConfig_Id)
	 and (@StatusConfig_Ids is null or [StatusConfig_Id] in (select * from dbo.SplitID(@StatusConfig_Ids)))
	 and (@Order is null or [Order] = @Order)
	 and (@Icon is null or [Icon] like @Icon)
	 and (@Color is null or [Color] like @Color)
	 and (@Active is null or [Active] = @Active)

DECLARE @RowsTotal int
SET @RowsTotal = @@ROWCOUNT

SELECT *, @RowsTotal AS RowsTotal
FROM #RESUL
WHERE @Page is null or (RowNumber > (@Page * @PageSize) - @PageSize and RowNumber <= (@Page * @PageSize))

DROP TABLE #RESUL