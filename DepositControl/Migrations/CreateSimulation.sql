/****** Object: Table [Simulation]   Script Date: 25/01/2018 12:02:13 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [Simulation] (
[Id] int IDENTITY(1, 1) NOT NULL,
[SimulationConfig_Id] int NOT NULL,
[Name] varchar(100) NOT NULL,
[Owner_User_Id] int NOT NULL,
[CreationDate] date NOT NULL,
[Status_Id] int NOT NULL,
CONSTRAINT [PK__Simulati__3214EC062A0AFB9C] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (FILLFACTOR = 100)
)
ON [PRIMARY];
GO


/****** Object: Table [SimulationConfig]   Script Date: 25/01/2018 12:02:16 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [SimulationConfig] (
[Id] int IDENTITY(1, 1) NOT NULL,
[Name] varchar(100) NOT NULL,
[Code] varchar(100) NOT NULL,
[MaxSimulations] int NOT NULL,
CONSTRAINT [PK__Simulati__3214EC0663750A46] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (FILLFACTOR = 100)
)
ON [PRIMARY];
GO


/****** Object: Table [SimulationRepository]   Script Date: 25/01/2018 12:02:17 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [SimulationRepository] (
[Id] int IDENTITY(1, 1) NOT NULL,
[Simulation_Id] int NOT NULL,
[Repository_Id] int NOT NULL,
PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (FILLFACTOR = 100)
)
ON [PRIMARY];
GO


/****** Object: Table [SimulationRepositoryConfig]   Script Date: 25/01/2018 12:02:19 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [SimulationRepositoryConfig] (
[Id] int IDENTITY(1, 1) NOT NULL,
[SimulationConfig_Id] int NOT NULL,
[RepositoryConfig_Id] int NOT NULL,
CONSTRAINT [PK__Simulati__3214EC06C8DC561A] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (FILLFACTOR = 100)
)
ON [PRIMARY];
GO


/****** Object: Table [SimulationUser]   Script Date: 25/01/2018 12:02:20 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [SimulationUser] (
[Id] int IDENTITY(1, 1) NOT NULL,
[Simulation_Id] int NOT NULL,
[User_Id] int NOT NULL,
[JoinDate] datetime NOT NULL,
PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (FILLFACTOR = 100)
)
ON [PRIMARY];
GO



/****** Object: Procedure [Simulation_Sp_Delete]   Script Date: 25/01/2018 12:05:26 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [Simulation_Sp_Delete]
( 
@Id int = null, @lstId varchar(MAX) = null

)
AS
SET NOCOUNT ON;
DELETE FROM dbo.[Simulation] 
WHERE  
    
    ((not @Id is null 
        and [Id] = @Id)
   or (not @lstId is null
        and [Id] in(select * from dbo.SplitID(@lstId))))
    
GO

/****** Object: Procedure [Simulation_Sp_Get]   Script Date: 25/01/2018 12:05:26 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [Simulation_Sp_Get]  
( @Id int = null, @lstId varchar(MAX) = null 
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[Simulation] 
WHERE   
    ((not @Id is null and [Id] = @Id)
   or (not @lstId is null and [Id] in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [Simulation_Sp_GetAll]   Script Date: 25/01/2018 12:05:26 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [Simulation_Sp_GetAll] 
AS
SET NOCOUNT ON;
SELECT * 
FROM dbo.[Simulation]

GO

/****** Object: Procedure [Simulation_Sp_GetByFilter]   Script Date: 25/01/2018 12:05:26 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            

CREATE PROCEDURE [Simulation_Sp_GetByFilter] 
(@SimulationConfig_Id int = null, @SimulationConfig_Ids varchar(MAX) = null, @Name varchar(100) = null, @Owner_User_Id int = null, @Owner_User_Ids varchar(MAX) = null, @CreationDate date = null, @CreationDateStart datetime = null, @CreationDateEnd datetime = null, @Status_Id int = null, @Status_Ids varchar(MAX) = null, @Page int = null, @PageSize int = null) 
AS
SET NOCOUNT ON;
SELECT PAG.*, ROW_NUMBER() OVER (ORDER BY PAG.[Id] ASC) AS RowNumber 
INTO #RESUL
FROM dbo.[Simulation] AS PAG
WHERE (@SimulationConfig_Id is null or [SimulationConfig_Id] = @SimulationConfig_Id)
	 and (@SimulationConfig_Ids is null or [SimulationConfig_Id] in (select * from dbo.SplitID(@SimulationConfig_Ids)))
	 and (@Name is null or [Name] like @Name)
	 and (@Owner_User_Id is null or [Owner_User_Id] = @Owner_User_Id)
	 and (@Owner_User_Ids is null or [Owner_User_Id] in (select * from dbo.SplitID(@Owner_User_Ids)))
	 and (@CreationDate is null or [CreationDate] = @CreationDate)
	 and (@CreationDateStart is null or [CreationDate] between @CreationDateStart and @CreationDateEnd)
	 and (@Status_Id is null or [Status_Id] = @Status_Id)
	 and (@Status_Ids is null or [Status_Id] in (select * from dbo.SplitID(@Status_Ids)))

DECLARE @RowsTotal int
SET @RowsTotal = @@ROWCOUNT

SELECT *, @RowsTotal AS RowsTotal
FROM #RESUL
WHERE @Page is null or (RowNumber > (@Page * @PageSize) - @PageSize and RowNumber <= (@Page * @PageSize))

DROP TABLE #RESUL
GO

/****** Object: Procedure [Simulation_Sp_GetByOwner_User_Id]   Script Date: 25/01/2018 12:05:26 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [Simulation_Sp_GetByOwner_User_Id]  
( @Owner_User_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[Simulation] 
WHERE   
   ((not @Owner_User_Id is null and [Owner_User_Id] = @Owner_User_Id)
   or (not @lstId is null and [Owner_User_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [Simulation_Sp_GetBySimulationConfig_Id]   Script Date: 25/01/2018 12:05:27 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [Simulation_Sp_GetBySimulationConfig_Id]  
( @SimulationConfig_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[Simulation] 
WHERE   
   ((not @SimulationConfig_Id is null and [SimulationConfig_Id] = @SimulationConfig_Id)
   or (not @lstId is null and [SimulationConfig_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [Simulation_Sp_GetByStatus_Id]   Script Date: 25/01/2018 12:05:27 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [Simulation_Sp_GetByStatus_Id]  
( @Status_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[Simulation] 
WHERE   
   ((not @Status_Id is null and [Status_Id] = @Status_Id)
   or (not @lstId is null and [Status_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [Simulation_Sp_Save]   Script Date: 25/01/2018 12:05:27 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [Simulation_Sp_Save]
(@Id int, @SimulationConfig_Id int, @Name varchar(100), @Owner_User_Id int, @CreationDate date, @Status_Id int)
as
IF (SELECT COUNT(*) from dbo.[Simulation] WHERE ( [Id] = @Id)  ) > 0
    BEGIN
        UPDATE dbo.[Simulation]
            SET  [SimulationConfig_Id] = @SimulationConfig_Id, [Name] = @Name, [Owner_User_Id] = @Owner_User_Id, [CreationDate] = @CreationDate, [Status_Id] = @Status_Id
            WHERE ( [Id] = @Id)
        SELECT @Id as Id
    END
ELSE
    BEGIN
        
        INSERT INTO dbo.[Simulation]   
              ([SimulationConfig_Id], [Name], [Owner_User_Id], [CreationDate], [Status_Id]) 
       VALUES (@SimulationConfig_Id , @Name , @Owner_User_Id , @CreationDate , @Status_Id )
        SELECT SCOPE_IDENTITY() as Id
    END
GO

/****** Object: Procedure [SimulationConfig_Sp_Delete]   Script Date: 25/01/2018 12:05:27 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationConfig_Sp_Delete]
( 
@Id int = null, @lstId varchar(MAX) = null

)
AS
SET NOCOUNT ON;
DELETE FROM dbo.[SimulationConfig] 
WHERE  
    
    ((not @Id is null 
        and [Id] = @Id)
   or (not @lstId is null
        and [Id] in(select * from dbo.SplitID(@lstId))))
    
GO

/****** Object: Procedure [SimulationConfig_Sp_Get]   Script Date: 25/01/2018 12:05:28 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationConfig_Sp_Get]  
( @Id int = null, @lstId varchar(MAX) = null 
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationConfig] 
WHERE   
    ((not @Id is null and [Id] = @Id)
   or (not @lstId is null and [Id] in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationConfig_Sp_GetAll]   Script Date: 25/01/2018 12:05:28 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationConfig_Sp_GetAll] 
AS
SET NOCOUNT ON;
SELECT * 
FROM dbo.[SimulationConfig]

GO

/****** Object: Procedure [SimulationConfig_Sp_GetByFilter]   Script Date: 25/01/2018 12:05:28 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            

CREATE PROCEDURE [SimulationConfig_Sp_GetByFilter] 
(@Name varchar(100) = null, @Code varchar(100) = null, @MaxSimulations int = null, @Page int = null, @PageSize int = null) 
AS
SET NOCOUNT ON;
SELECT PAG.*, ROW_NUMBER() OVER (ORDER BY PAG.[Id] ASC) AS RowNumber 
INTO #RESUL
FROM dbo.[SimulationConfig] AS PAG
WHERE (@Name is null or [Name] like @Name)
	 and (@Code is null or [Code] like @Code)
	 and (@MaxSimulations is null or [MaxSimulations] = @MaxSimulations)

DECLARE @RowsTotal int
SET @RowsTotal = @@ROWCOUNT

SELECT *, @RowsTotal AS RowsTotal
FROM #RESUL
WHERE @Page is null or (RowNumber > (@Page * @PageSize) - @PageSize and RowNumber <= (@Page * @PageSize))

DROP TABLE #RESUL
GO

/****** Object: Procedure [SimulationConfig_Sp_Save]   Script Date: 25/01/2018 12:05:28 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationConfig_Sp_Save]
(@Id int, @Name varchar(100), @Code varchar(100), @MaxSimulations int)
as
IF (SELECT COUNT(*) from dbo.[SimulationConfig] WHERE ( [Id] = @Id)  ) > 0
    BEGIN
        UPDATE dbo.[SimulationConfig]
            SET  [Name] = @Name, [Code] = @Code, [MaxSimulations] = @MaxSimulations
            WHERE ( [Id] = @Id)
        SELECT @Id as Id
    END
ELSE
    BEGIN
        
        INSERT INTO dbo.[SimulationConfig]   
              ([Name], [Code], [MaxSimulations]) 
       VALUES (@Name , @Code , @MaxSimulations )
        SELECT SCOPE_IDENTITY() as Id
    END
GO

/****** Object: Procedure [SimulationRepository_Sp_CopyRepositoryData]   Script Date: 25/01/2018 12:05:28 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepository_Sp_CopyRepositoryData]
(@FromRepository_Id int = 0,@ToRepository_Id int, @ProcessConfig_Id smallint = null)
AS
SET NOCOUNT ON;
SET IDENTITY_INSERT [SimulationRepository] ON

insert into [SimulationRepository] ([Id], [Repository_Id],[Simulation_Id])
  Select [Id], @ToRepository_Id as [Repository_Id],[Simulation_Id]
  From [SimulationRepository]
  Where Repository_Id = @FromRepository_Id 

SET IDENTITY_INSERT [SimulationRepository] OFF
GO

/****** Object: Procedure [SimulationRepository_Sp_Delete]   Script Date: 25/01/2018 12:05:29 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepository_Sp_Delete]
( 
@Id int = null, @lstId varchar(MAX) = null

)
AS
SET NOCOUNT ON;
DELETE FROM dbo.[SimulationRepository] 
WHERE  
    
    ((not @Id is null 
        and [Id] = @Id)
   or (not @lstId is null
        and [Id] in(select * from dbo.SplitID(@lstId))))
    
GO

/****** Object: Procedure [SimulationRepository_Sp_Get]   Script Date: 25/01/2018 12:05:29 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepository_Sp_Get]  
( @Id int = null, @lstId varchar(MAX) = null 
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationRepository] 
WHERE   
    ((not @Id is null and [Id] = @Id)
   or (not @lstId is null and [Id] in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationRepository_Sp_GetAll]   Script Date: 25/01/2018 12:05:29 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepository_Sp_GetAll] 
AS
SET NOCOUNT ON;
SELECT * 
FROM dbo.[SimulationRepository]

GO

/****** Object: Procedure [SimulationRepository_Sp_GetByFilter]   Script Date: 25/01/2018 12:05:29 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            

CREATE PROCEDURE [SimulationRepository_Sp_GetByFilter] 
(@Simulation_Id int = null, @Simulation_Ids varchar(MAX) = null, @Repository_Id int = null, @Repository_Ids varchar(MAX) = null, @Page int = null, @PageSize int = null) 
AS
SET NOCOUNT ON;
SELECT PAG.*, ROW_NUMBER() OVER (ORDER BY PAG.[Id] ASC) AS RowNumber 
INTO #RESUL
FROM dbo.[SimulationRepository] AS PAG
WHERE (@Simulation_Id is null or [Simulation_Id] = @Simulation_Id)
	 and (@Simulation_Ids is null or [Simulation_Id] in (select * from dbo.SplitID(@Simulation_Ids)))
	 and (@Repository_Id is null or [Repository_Id] = @Repository_Id)
	 and (@Repository_Ids is null or [Repository_Id] in (select * from dbo.SplitID(@Repository_Ids)))

DECLARE @RowsTotal int
SET @RowsTotal = @@ROWCOUNT

SELECT *, @RowsTotal AS RowsTotal
FROM #RESUL
WHERE @Page is null or (RowNumber > (@Page * @PageSize) - @PageSize and RowNumber <= (@Page * @PageSize))

DROP TABLE #RESUL
GO

/****** Object: Procedure [SimulationRepository_Sp_GetByRepository_Id]   Script Date: 25/01/2018 12:05:29 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepository_Sp_GetByRepository_Id]  
( @Repository_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationRepository] 
WHERE   
   ((not @Repository_Id is null and [Repository_Id] = @Repository_Id)
   or (not @lstId is null and [Repository_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationRepository_Sp_GetBySimulation_Id]   Script Date: 25/01/2018 12:05:30 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepository_Sp_GetBySimulation_Id]  
( @Simulation_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationRepository] 
WHERE   
   ((not @Simulation_Id is null and [Simulation_Id] = @Simulation_Id)
   or (not @lstId is null and [Simulation_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationRepository_Sp_Save]   Script Date: 25/01/2018 12:05:30 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepository_Sp_Save]
(@Id int, @Simulation_Id int, @Repository_Id int)
as
IF (SELECT COUNT(*) from dbo.[SimulationRepository] WHERE ( [Id] = @Id)  ) > 0
    BEGIN
        UPDATE dbo.[SimulationRepository]
            SET  [Simulation_Id] = @Simulation_Id, [Repository_Id] = @Repository_Id
            WHERE ( [Id] = @Id)
        SELECT @Id as Id
    END
ELSE
    BEGIN
        
        INSERT INTO dbo.[SimulationRepository]   
              ([Simulation_Id], [Repository_Id]) 
       VALUES (@Simulation_Id , @Repository_Id )
        SELECT SCOPE_IDENTITY() as Id
    END
GO

/****** Object: Procedure [SimulationRepositoryConfig_Sp_Delete]   Script Date: 25/01/2018 12:05:30 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepositoryConfig_Sp_Delete]
( 
@Id int = null, @lstId varchar(MAX) = null

)
AS
SET NOCOUNT ON;
DELETE FROM dbo.[SimulationRepositoryConfig] 
WHERE  
    
    ((not @Id is null 
        and [Id] = @Id)
   or (not @lstId is null
        and [Id] in(select * from dbo.SplitID(@lstId))))
    
GO

/****** Object: Procedure [SimulationRepositoryConfig_Sp_Get]   Script Date: 25/01/2018 12:05:30 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepositoryConfig_Sp_Get]  
( @Id int = null, @lstId varchar(MAX) = null 
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationRepositoryConfig] 
WHERE   
    ((not @Id is null and [Id] = @Id)
   or (not @lstId is null and [Id] in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationRepositoryConfig_Sp_GetAll]   Script Date: 25/01/2018 12:05:30 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepositoryConfig_Sp_GetAll] 
AS
SET NOCOUNT ON;
SELECT * 
FROM dbo.[SimulationRepositoryConfig]

GO

/****** Object: Procedure [SimulationRepositoryConfig_Sp_GetByFilter]   Script Date: 25/01/2018 12:05:30 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            

CREATE PROCEDURE [SimulationRepositoryConfig_Sp_GetByFilter] 
(@SimulationConfig_Id int = null, @SimulationConfig_Ids varchar(MAX) = null, @RepositoryConfig_Id int = null, @RepositoryConfig_Ids varchar(MAX) = null, @Page int = null, @PageSize int = null) 
AS
SET NOCOUNT ON;
SELECT PAG.*, ROW_NUMBER() OVER (ORDER BY PAG.[Id] ASC) AS RowNumber 
INTO #RESUL
FROM dbo.[SimulationRepositoryConfig] AS PAG
WHERE (@SimulationConfig_Id is null or [SimulationConfig_Id] = @SimulationConfig_Id)
	 and (@SimulationConfig_Ids is null or [SimulationConfig_Id] in (select * from dbo.SplitID(@SimulationConfig_Ids)))
	 and (@RepositoryConfig_Id is null or [RepositoryConfig_Id] = @RepositoryConfig_Id)
	 and (@RepositoryConfig_Ids is null or [RepositoryConfig_Id] in (select * from dbo.SplitID(@RepositoryConfig_Ids)))

DECLARE @RowsTotal int
SET @RowsTotal = @@ROWCOUNT

SELECT *, @RowsTotal AS RowsTotal
FROM #RESUL
WHERE @Page is null or (RowNumber > (@Page * @PageSize) - @PageSize and RowNumber <= (@Page * @PageSize))

DROP TABLE #RESUL
GO

/****** Object: Procedure [SimulationRepositoryConfig_Sp_GetByRepositoryConfig_Id]   Script Date: 25/01/2018 12:05:31 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepositoryConfig_Sp_GetByRepositoryConfig_Id]  
( @RepositoryConfig_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationRepositoryConfig] 
WHERE   
   ((not @RepositoryConfig_Id is null and [RepositoryConfig_Id] = @RepositoryConfig_Id)
   or (not @lstId is null and [RepositoryConfig_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationRepositoryConfig_Sp_GetBySimulationConfig_Id]   Script Date: 25/01/2018 12:05:31 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepositoryConfig_Sp_GetBySimulationConfig_Id]  
( @SimulationConfig_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationRepositoryConfig] 
WHERE   
   ((not @SimulationConfig_Id is null and [SimulationConfig_Id] = @SimulationConfig_Id)
   or (not @lstId is null and [SimulationConfig_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationRepositoryConfig_Sp_Save]   Script Date: 25/01/2018 12:05:31 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationRepositoryConfig_Sp_Save]
(@Id int, @SimulationConfig_Id int, @RepositoryConfig_Id int)
as
IF (SELECT COUNT(*) from dbo.[SimulationRepositoryConfig] WHERE ( [Id] = @Id)  ) > 0
    BEGIN
        UPDATE dbo.[SimulationRepositoryConfig]
            SET  [SimulationConfig_Id] = @SimulationConfig_Id, [RepositoryConfig_Id] = @RepositoryConfig_Id
            WHERE ( [Id] = @Id)
        SELECT @Id as Id
    END
ELSE
    BEGIN
        
        INSERT INTO dbo.[SimulationRepositoryConfig]   
              ([SimulationConfig_Id], [RepositoryConfig_Id]) 
       VALUES (@SimulationConfig_Id , @RepositoryConfig_Id )
        SELECT SCOPE_IDENTITY() as Id
    END
GO

/****** Object: Procedure [SimulationUser_Sp_Delete]   Script Date: 25/01/2018 12:05:31 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationUser_Sp_Delete]
( 
@Id int = null, @lstId varchar(MAX) = null

)
AS
SET NOCOUNT ON;
DELETE FROM dbo.[SimulationUser] 
WHERE  
    
    ((not @Id is null 
        and [Id] = @Id)
   or (not @lstId is null
        and [Id] in(select * from dbo.SplitID(@lstId))))
    
GO

/****** Object: Procedure [SimulationUser_Sp_Get]   Script Date: 25/01/2018 12:05:31 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationUser_Sp_Get]  
( @Id int = null, @lstId varchar(MAX) = null 
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationUser] 
WHERE   
    ((not @Id is null and [Id] = @Id)
   or (not @lstId is null and [Id] in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationUser_Sp_GetAll]   Script Date: 25/01/2018 12:05:32 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationUser_Sp_GetAll] 
AS
SET NOCOUNT ON;
SELECT * 
FROM dbo.[SimulationUser]

GO

/****** Object: Procedure [SimulationUser_Sp_GetByFilter]   Script Date: 25/01/2018 12:05:32 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            

CREATE PROCEDURE [SimulationUser_Sp_GetByFilter] 
(@Simulation_Id int = null, @Simulation_Ids varchar(MAX) = null, @User_Id int = null, @User_Ids varchar(MAX) = null, @JoinDate datetime = null, @JoinDateStart datetime = null, @JoinDateEnd datetime = null, @Page int = null, @PageSize int = null) 
AS
SET NOCOUNT ON;
SELECT PAG.*, ROW_NUMBER() OVER (ORDER BY PAG.[Id] ASC) AS RowNumber 
INTO #RESUL
FROM dbo.[SimulationUser] AS PAG
WHERE (@Simulation_Id is null or [Simulation_Id] = @Simulation_Id)
	 and (@Simulation_Ids is null or [Simulation_Id] in (select * from dbo.SplitID(@Simulation_Ids)))
	 and (@User_Id is null or [User_Id] = @User_Id)
	 and (@User_Ids is null or [User_Id] in (select * from dbo.SplitID(@User_Ids)))
	 and (@JoinDate is null or [JoinDate] = @JoinDate)
	 and (@JoinDateStart is null or [JoinDate] between @JoinDateStart and @JoinDateEnd)

DECLARE @RowsTotal int
SET @RowsTotal = @@ROWCOUNT

SELECT *, @RowsTotal AS RowsTotal
FROM #RESUL
WHERE @Page is null or (RowNumber > (@Page * @PageSize) - @PageSize and RowNumber <= (@Page * @PageSize))

DROP TABLE #RESUL
GO

/****** Object: Procedure [SimulationUser_Sp_GetBySimulation_Id]   Script Date: 25/01/2018 12:05:32 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationUser_Sp_GetBySimulation_Id]  
( @Simulation_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationUser] 
WHERE   
   ((not @Simulation_Id is null and [Simulation_Id] = @Simulation_Id)
   or (not @lstId is null and [Simulation_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationUser_Sp_GetByUser_Id]   Script Date: 25/01/2018 12:05:32 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationUser_Sp_GetByUser_Id]  
( @User_Id int = null, @lstId varchar(MAX) = null
)
AS
SET NOCOUNT ON;

SELECT * FROM dbo.[SimulationUser] 
WHERE   
   ((not @User_Id is null and [User_Id] = @User_Id)
   or (not @lstId is null and [User_Id]  in(select * from dbo.SplitID(@lstId))))
   
GO

/****** Object: Procedure [SimulationUser_Sp_Save]   Script Date: 25/01/2018 12:05:32 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
            
CREATE PROCEDURE [SimulationUser_Sp_Save]
(@Id int, @Simulation_Id int, @User_Id int, @JoinDate datetime)
as
IF (SELECT COUNT(*) from dbo.[SimulationUser] WHERE ( [Id] = @Id)  ) > 0
    BEGIN
        UPDATE dbo.[SimulationUser]
            SET  [Simulation_Id] = @Simulation_Id, [User_Id] = @User_Id, [JoinDate] = @JoinDate
            WHERE ( [Id] = @Id)
        SELECT @Id as Id
    END
ELSE
    BEGIN
        
        INSERT INTO dbo.[SimulationUser]   
              ([Simulation_Id], [User_Id], [JoinDate]) 
       VALUES (@Simulation_Id , @User_Id , @JoinDate )
        SELECT SCOPE_IDENTITY() as Id
    END
GO

/****** Object: Procedure [User_Sp_GetBySimulation_Id]   Script Date: 25/01/2018 12:05:33 PM ******/

GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
create PROCEDURE [User_Sp_GetBySimulation_Id]
(@Simulation_Id bigint = null, @lstId varchar(MAX) = null)
AS
SET NOCOUNT ON;

SELECT [User].* FROM dbo.[User] 
inner join SimulationUser 
  on dbo.[User].Id =  dbo.SimulationUser.[User_Id]
WHERE ((not @Simulation_Id is null 
        and dbo.SimulationUser.Simulation_Id = @Simulation_Id)
   or (not @lstId is null
        and dbo.SimulationUser.Simulation_Id in(select * from dbo.SplitID(@lstId))))
GO

insert into Resources (Culture, Name, IsEntity, EntityName, ColumName, RowCodeId, Value, Type) VALUES
('en', 'simulation', '0', NULL, NULL, NULL, 'Simulation', 'String')
insert into Resources (Culture, Name, IsEntity, EntityName, ColumName, RowCodeId, Value, Type) VALUES
('en', 'simulationOut', '0', NULL, NULL, NULL, 'Left Simulation', 'String')
insert into Resources (Culture, Name, IsEntity, EntityName, ColumName, RowCodeId, Value, Type) VALUES
('en', 'left', '0', NULL, NULL, NULL, 'Left', 'String')
insert into Resources (Culture, Name, IsEntity, EntityName, ColumName, RowCodeId, Value, Type) VALUES
('en', 'join', '0', NULL, NULL, NULL, 'Join', 'String')
insert into Resources (Culture, Name, IsEntity, EntityName, ColumName, RowCodeId, Value, Type) VALUES
('en', 'creationDate', '0', NULL, NULL, NULL, 'Creation Date', 'String')
insert into Resources (Culture, Name, IsEntity, EntityName, ColumName, RowCodeId, Value, Type) VALUES
('en', 'simulationConfirmDelete', '0', NULL, NULL, NULL, 'Are you sure you want to delete the simulation', 'String')
insert into Resources (Culture, Name, IsEntity, EntityName, ColumName, RowCodeId, Value, Type) VALUES
('en', 'simulationMaxError', '0', NULL, NULL, NULL, 'the maximum number of simulations has been reached', 'String')

GO
SET IDENTITY_INSERT StatusConfig ON 
insert into StatusConfig (id,[Name], Code, Description, [Order]) 
VALUES                  (3, 'Simulation', 'Simulation', NULL, '0')
SET IDENTITY_INSERT StatusConfig OFF

insert into Status ([Name], Code, Description, StatusConfig_Id, [Order], Icon, Active) VALUES
('Creating', 'Creating', '', '3', '0', '', '0')
insert into Status ([Name], Code, Description, StatusConfig_Id, [Order], Icon, Active) VALUES
('Ready', 'Ready', '', '3', '1', '', '1')
insert into Status ([Name], Code, Description, StatusConfig_Id, [Order], Icon, Active) VALUES
('Error', 'Error', '', '3', '2', '', '0')

