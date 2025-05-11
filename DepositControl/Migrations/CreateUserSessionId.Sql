
alter table [User] add [SessionId] varchar(50) NULL
GO

/****** Object:  StoredProcedure [dbo].[User_Sp_Save]    Script Date: 09/04/2018 11:24:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
            
ALTER PROCEDURE [dbo].[User_Sp_Save]
(@Id int, @Name varchar(255), @FullName varchar(255), @LastLogin datetime, @Email varchar(255), @SessionOpen smallint, @State_Type_Id bigint, @Password varchar(MAX), @SecretQuestion_Type_Id bigint, @SecretAnswer varchar(MAX), @ActivationKey varchar(100), @Language_Type_Id bigint, @FailLoginCount smallint, @Company_Id int, @SessionId varchar(50) = null)
as
IF (SELECT COUNT(*) from dbo.[User] WHERE ( [Id] = @Id)  ) > 0
    BEGIN
        UPDATE dbo.[User]
            SET  [Name] = @Name, [FullName] = @FullName, [LastLogin] = @LastLogin, [Email] = @Email, [SessionOpen] = @SessionOpen, [State_Type_Id] = @State_Type_Id, [Password] = @Password, [SecretQuestion_Type_Id] = @SecretQuestion_Type_Id, [SecretAnswer] = @SecretAnswer, [ActivationKey] = @ActivationKey, [Language_Type_Id] = @Language_Type_Id, [FailLoginCount] = @FailLoginCount, [Company_Id] = @Company_Id, [SessionId] = @SessionId
            WHERE ( [Id] = @Id)
        SELECT @Id as Id
    END
ELSE
    BEGIN
        
        INSERT INTO dbo.[User]   
              ([Name], [FullName], [LastLogin], [Email], [SessionOpen], [State_Type_Id], [Password], [SecretQuestion_Type_Id], [SecretAnswer], [ActivationKey], [Language_Type_Id], [FailLoginCount], [Company_Id], [SessionId]) 
       VALUES (@Name , @FullName , @LastLogin , @Email , @SessionOpen , @State_Type_Id , @Password , @SecretQuestion_Type_Id , @SecretAnswer , @ActivationKey , @Language_Type_Id , @FailLoginCount , @Company_Id , @SessionId )
        SELECT SCOPE_IDENTITY() as Id
    END

GO


ALTER PROCEDURE [dbo].[User_Sp_GetByFilter] 
(@Name varchar(255) = null, @FullName varchar(255) = null, @LastLogin datetime = null, @LastLoginStart datetime = null, @LastLoginEnd datetime = null, @Email varchar(255) = null, @SessionOpen smallint = null, @State_Type_Id bigint = null, @State_Type_Ids varchar(MAX) = null, @Password varchar(MAX) = null, @SecretQuestion_Type_Id bigint = null, @SecretQuestion_Type_Ids varchar(MAX) = null, @SecretAnswer varchar(MAX) = null, @ActivationKey varchar(100) = null, @Language_Type_Id bigint = null, @Language_Type_Ids varchar(MAX) = null, @FailLoginCount smallint = null, @Company_Id int = null, @SessionId varchar(50) = null, @Page int = null, @PageSize int = null) 
AS
SET NOCOUNT ON;
SELECT PAG.*, ROW_NUMBER() OVER (ORDER BY PAG.[Id] ASC) AS RowNumber 
INTO #RESUL
FROM dbo.[User] AS PAG
WHERE (@Name is null or [Name] like @Name)
	 and (@FullName is null or [FullName] like @FullName)
	 and (@LastLogin is null or [LastLogin] = @LastLogin)
	 and (@LastLoginStart is null or [LastLogin] between @LastLoginStart and @LastLoginEnd)
	 and (@Email is null or [Email] like @Email)
	 and (@SessionOpen is null or [SessionOpen] = @SessionOpen)
	 and (@State_Type_Id is null or [State_Type_Id] = @State_Type_Id)
	 and (@State_Type_Ids is null or [State_Type_Id] in (select * from dbo.SplitID(@State_Type_Ids)))
	 and (@Password is null or [Password] like @Password)
	 and (@SecretQuestion_Type_Id is null or [SecretQuestion_Type_Id] = @SecretQuestion_Type_Id)
	 and (@SecretQuestion_Type_Ids is null or [SecretQuestion_Type_Id] in (select * from dbo.SplitID(@SecretQuestion_Type_Ids)))
	 and (@SecretAnswer is null or [SecretAnswer] like @SecretAnswer)
	 and (@ActivationKey is null or [ActivationKey] like @ActivationKey)
	 and (@Language_Type_Id is null or [Language_Type_Id] = @Language_Type_Id)
	 and (@Language_Type_Ids is null or [Language_Type_Id] in (select * from dbo.SplitID(@Language_Type_Ids)))
	 and (@FailLoginCount is null or [FailLoginCount] = @FailLoginCount)
	 and (@Company_Id is null or [Company_Id] = @Company_Id)
	 and (@SessionId is null or [SessionId] like @SessionId)

DECLARE @RowsTotal int
SET @RowsTotal = @@ROWCOUNT

SELECT *, @RowsTotal AS RowsTotal
FROM #RESUL
WHERE @Page is null or (RowNumber > (@Page * @PageSize) - @PageSize and RowNumber <= (@Page * @PageSize))

DROP TABLE #RESUL

GO



CREATE PROCEDURE User_Sp_ClearAllSessions
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	update [User] 
	set SessionId = '',[SessionOpen] = 0

		

END
GO