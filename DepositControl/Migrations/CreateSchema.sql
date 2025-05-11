CREATE TABLE [dbo].[Access] (
	[Id] [int] IDENTITY(1, 1) NOT NULL,
	[Type_Id] [smallint] NOT NULL CONSTRAINT [DF__Access__Type__00551192]  DEFAULT ((1)),
	[Name] [varchar](255) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[Code] [varchar](100) NULL,
	[Data] [varchar](512) NULL,
	[Url] [varchar](512) NULL,
	[Icon] [varchar](50) NULL,
	[Posicion] [smallint] NOT NULL CONSTRAINT [DF__Access__Posicion__014935CB]  DEFAULT ((0)),
	[Parent_Access_Id] [int] NULL,
	CONSTRAINT [PK__Menu__787EE5A0] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Accion] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Order] [smallint] NOT NULL DEFAULT ((0)),
	[Category_Type_Id] [int] NULL,
	[Message] [varchar](255) NULL,
	CONSTRAINT [PK__Accion__3214EC0660FC61CA] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[BcpImport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Repository_Id] [int] NOT NULL,
	[ProcessConfig_Id] [smallint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[RepositoryConfig_Id] [smallint] NULL,
	[RepositoryCode] [varchar](100) NOT NULL,
	[Path] [varchar](255) NULL,
	[Delimiter] [varchar](5) NULL,
	[HasHeaderRow] [bit] CONSTRAINT [DF__BcpImport__HasHe__02D256E1] DEFAULT ((0)) NULL,
	[EnclosedInQuotes] [bit] CONSTRAINT [DF__BcpImport__Enclo__01DE32A8] DEFAULT ((0)) NULL,
	[Conexion_Id] [int] NULL,
	[SqlCommand] [varchar](max) NULL,
	[Deleted] [bit] CONSTRAINT [DF__BcpImport__Delet__149C0161] DEFAULT ((0)) NOT NULL,
	[OriginalId] [int] NULL,
	[Delimited] [bit] CONSTRAINT [DF__BcpImport__Delim__587CF5B9] DEFAULT ((0)) NOT NULL,	
	[SqlSource] [bit] CONSTRAINT [DF__BcpImport__SqlSo__597119F2] DEFAULT ((0)) NOT NULL,
	CONSTRAINT [PK_BcpImport] PRIMARY KEY CLUSTERED (
		[Id] ASC,
		[Repository_Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_BcpImportRepositoryId]
    ON [dbo].[BcpImport]([Repository_Id] ASC);
GO

CREATE TABLE [dbo].[Comment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Process_Id] [int] NOT NULL,
	[User_Id] [int] NOT NULL,
	[Message] [varchar](2100) NOT NULL,
	[Date] [datetime] NULL,
	CONSTRAINT [PK__Comment__3214EC0638A457AD] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Conexion](
	[Id] smallint IDENTITY(1, 1) NOT NULL,
	[Repository_Id] int NOT NULL,
	[Name] varchar(255) NOT NULL,
	[Code] varchar(50) NOT NULL,
	[Provider] varchar(512) DEFAULT ('') NOT NULL,
	[ConexionString] varchar(512) NULL,
	[Encrypted] bit CONSTRAINT [DF__Conexion__Encryp__2CF2ADDF] DEFAULT ((0)) NOT NULL,
	[Deleted] bit CONSTRAINT [DF__Conexion__Delete__503BEA1C] DEFAULT ((0)) NOT NULL,
	CONSTRAINT [PK__Conexion__3214EC0625518C17] PRIMARY KEY NONCLUSTERED (
		[Id] ASC,
		[Repository_Id] ASC
	)	
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_ConexionRepositoryId]
    ON [dbo].[Conexion]([Repository_Id] ASC);
GO

CREATE TABLE [dbo].[DataType](
	[Id] [smallint] NOT NULL,
	[Behavior] [tinyint] NOT NULL,
	[SQL] [varchar](50) NOT NULL,
	[C] [varchar](50) NOT NULL,
	[Parameter] [varchar](50) NOT NULL,
	[Specified_Length] [bit] NOT NULL,
	CONSTRAINT [PK_DataType] PRIMARY KEY CLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[DimensionDate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateKey] [int] NOT NULL,
	[Date] [datetime] NULL,
	[FullDateUK] [char](10) NULL,
	[FullDateUSA] [char](10) NULL,
	[DayOfMonth] [varchar](2) NULL,
	[DaySuffix] [varchar](4) NULL,
	[DayName] [varchar](9) NULL,
	[DayOfWeekUSA] [char](1) NULL,
	[DayOfWeekUK] [char](1) NULL,
	[DayOfWeekInMonth] [varchar](2) NULL,
	[DayOfWeekInYear] [varchar](2) NULL,
	[DayOfQuarter] [varchar](3) NULL,
	[DayOfYear] [varchar](3) NULL,
	[WeekOfMonth] [varchar](1) NULL,
	[WeekOfQuarter] [varchar](2) NULL,
	[WeekOfYear] [varchar](2) NULL,
	[Month] [varchar](2) NULL,
	[MonthName] [varchar](15) NULL,
	[MonthOfQuarter] [varchar](2) NOT NULL,
	[Quarter] [char](1) NULL,
	[QuarterName] [varchar](9) NULL,
	[Year] [char](4) NULL,
	[YearName] [char](7) NULL,
	[MonthYear] [char](10) NULL,
	[MMYYYY] [char](6) NULL,
	[FirstDayOfMonth] [date] NULL,
	[LastDayOfMonth] [date] NULL,
	[FirstDayOfQuarter] [date] NULL,
	[LastDayOfQuarter] [date] NULL,
	[FirstDayOfYear] [date] NULL,
	[LastDayOfYear] [date] NULL,
	[IsHolidayUSA] [bit] NULL,
	[IsWeekday] [bit] NULL,
	[HolidayUSA] [varchar](50) NULL,
	[IsHolidayUK] [bit] NULL,
	[HolidayUK] [varchar](50) NULL,
	CONSTRAINT [PK__Dimensio__3214EC0633CA93F7] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DimensionDate]
    ON [dbo].[DimensionDate]([Date] ASC) WITH (IGNORE_DUP_KEY = ON);
GO

CREATE TABLE [dbo].[DynamicDataSet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Type_Id] [bigint] NOT NULL,
	[Entity_Id] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
 CONSTRAINT [PK__ChangeSe__3214EC0665F62111] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[DynamicDataSetData](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DynamicDataSetRow_Id] [bigint] NOT NULL,
	[Struct_Id] [smallint] NOT NULL,
	[OldValue] [varchar](max) NULL,
	[Value] [varchar](max) NULL,
	CONSTRAINT [PK__ChangeSe__3214EC0669C6B1F5] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[DynamicDataSetRow] (
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DynamicDataSet_Id] [bigint] NOT NULL,
	[Accion_Type_Id] [bigint] NULL,
	[UniqueId] [bigint] NOT NULL,
	CONSTRAINT [PK__ChangeSe__3214EC066F7F8B4B] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Entity] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[NameClass] [varchar](255) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[Class] [varchar](255) NULL,
	[Assembly] [varchar](255) NULL,
	[LargeData] [bit] NOT NULL,
	[IsLoggeable] [bit] CONSTRAINT [DF__Entity__IsLoggea__0C519F7A] DEFAULT ((0)) NULL,
	CONSTRAINT [PK__Entity__3214EC0679FD19BE] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Entity] ADD  DEFAULT ((0)) FOR [LargeData]

GO

CREATE TABLE [dbo].[File] (
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Content] [varbinary](max) NOT NULL,
	[ContentType] [varchar](100) NOT NULL,
	[Company_Id] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[FrequencyHolyDay] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Date] [date] NOT NULL,
	[Deleted] [bit] NOT NULL, PRIMARY KEY NONCLUSTERED  (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[FrequencyHolyDay] ADD  DEFAULT ((0)) FOR [Deleted]

GO

CREATE TABLE [dbo].[FrequencySimpleInput] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Frequency_Type_Id] [smallint] NULL,
	[FrequencyIncrease] [smallint] NOT NULL,
	[FrequencyAll] [bit] NOT NULL,
	CONSTRAINT [PK__SimpleIn__3214EC0667A92F6F] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Grid] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NULL,
	[Code] [varchar](100) NOT NULL,
	CONSTRAINT [PK__Grid__3214EC06457F2FDE] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[GridCustomView](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Grid_Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[State] [varchar](max) NULL,
	[Owner_User_Id] [int] NULL,
	CONSTRAINT [PK_GridCustomView] PRIMARY KEY CLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[GridCustomViewDefault] (
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Grid_Id] [bigint] NOT NULL,
	[GridCustomView_Id] [bigint] NOT NULL,
	[User_Id] [bigint] NULL,
	[Profile_Id] [bigint] NULL,
	[Date] [datetime] NOT NULL,
	CONSTRAINT [PK_GridCustomViewDefault] PRIMARY KEY CLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[GridCustomViewShared] (
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GridCustomView_Id] [bigint] NOT NULL,
	[Profile_Id] [bigint] NULL,
	[Date] [datetime] NULL,
	CONSTRAINT [PK_GridCustomViewShared] PRIMARY KEY CLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[LogAccion] (
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[User_Id] [int] NOT NULL,
	[Accion_Id] [int] NOT NULL,
	[AccionTo] [varchar](255) NULL CONSTRAINT [DF__LogAccion__SubAc__108B795B]  DEFAULT (''),
	[AccionToId] [bigint] NULL,
	[Access_Id] [int] NULL,
	[Data] [varchar](4000) NULL,
	[DynamicDataSet_Id] [bigint] NULL,
	CONSTRAINT [PK__LogAccion__3EDC53F0] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[LogError] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Thread] [varchar](255) NOT NULL,
	[Level] [varchar](50) NOT NULL,
	[Logger] [varchar](255) NOT NULL,
	[Message] [varchar](4000) NOT NULL,
	[Exception] [varchar](max) NULL,
	[Detail] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[PivotCube] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Entity_Id] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Code] [char](30) NOT NULL,
	CONSTRAINT [PK_CubePivot] PRIMARY KEY CLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[PivotQuery] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Code] [char](30) NULL,
	[User_Id] [int] NOT NULL,
	[RepositoryConfig_Id] [int] NULL,
	[Report_Id] [int] NULL,
	CONSTRAINT [PK_QueryPivot] PRIMARY KEY CLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[PivotQueryStruct] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PivotQuery_Id] [int] NOT NULL,
	[HiddenAttributes] [varchar](max) NULL,
	[MenuLimit] [varchar](max) NULL,
	[Cols] [varchar](max) NULL,
	[Rows] [varchar](max) NULL,
	[Vals] [varchar](max) NULL,
	[Exclusions] [varchar](max) NULL,
	[Inclusions] [varchar](max) NULL,
	[UnusedAttrsVertical] [varchar](max) NULL,
	[AutoSortUnusedAttrs] [varchar](max) NULL,
	[InclusionsInfo] [varchar](max) NULL,
	[AggregatorName] [varchar](max) NULL,
	[RendererName] [varchar](max) NULL,
	CONSTRAINT [PK_StructQueryPivot] PRIMARY KEY CLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Process] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProcessConfig_Id] [smallint] NOT NULL,
	[Status_Id] [smallint] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Period] [date] NOT NULL,
	[Version] [int] NOT NULL,
	[IsOpen] [bit] NOT NULL CONSTRAINT [DF__Process__Open__7172C0B5]  DEFAULT ((0)),
	[LastRefresh] [datetime] NULL,
	[Data] [varbinary](max) NULL,
	[Error] [varchar](max) NULL,
	CONSTRAINT [PK__Process__3214EC065F7E2DAC] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[ProcessConfig] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Code] [varchar](50) NOT NULL,
	[Type_Id] [tinyint] NULL,
	[NewPage] [varchar](255) NULL,
	[Class] [varchar](255) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[AutoExecute] [bit] NOT NULL,
	[RealTime] [bit] NOT NULL,
	[RealTimeRefreshMinutes] [int] NOT NULL,
	[ExecuteOnHour] [smallint] NOT NULL,
	[ExecuteOnMinute] [smallint] NOT NULL,
	[ExecuteDelayDays] [smallint] NOT NULL,
	[AutoApprove] [bit] NOT NULL,
	[Assembly] [varchar](255) NULL,
	[HolyDays_Type_Id] [int] NULL,
	[Frequency_Type_Id] [smallint] NOT NULL,
	[Regenerate] BIT CONSTRAINT [DF_ProcessConfig_Regenerate] DEFAULT ((0)) NOT NULL,
	CONSTRAINT [PK__ProcessC__3214EC06625A9A57] PRIMARY KEY NONCLUSTERED (
	[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProcessConfig] ADD  CONSTRAINT [DF__ProcessCo__NewPa__339FAB6E]  DEFAULT ('') FOR [NewPage]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  CONSTRAINT [DF__ProcessCo__Class__32AB8735]  DEFAULT ('') FOR [Class]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  CONSTRAINT [DF__ProcessCo__AutoE__7152C524]  DEFAULT ((0)) FOR [AutoExecute]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  DEFAULT ((0)) FOR [RealTime]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  DEFAULT ((0)) FOR [RealTimeRefreshMinutes]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  CONSTRAINT [DF__ProcessCo__Execu__4CE05A84]  DEFAULT ((0)) FOR [ExecuteOnHour]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  CONSTRAINT [DF__ProcessCo__Execu__4AF81212]  DEFAULT ((0)) FOR [ExecuteOnMinute]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  CONSTRAINT [DF__ProcessCo__Execu__4BEC364B]  DEFAULT ((1)) FOR [ExecuteDelayDays]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  CONSTRAINT [DF__ProcessCo__AutoA__7246E95D]  DEFAULT ((0)) FOR [AutoApprove]
GO
ALTER TABLE [dbo].[ProcessConfig] ADD  CONSTRAINT [DF__ProcessCo__Frequ__6F95653B]  DEFAULT ((13)) FOR [Frequency_Type_Id]

GO

CREATE TABLE [dbo].[ProcessInput] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Process_Id] [int] NOT NULL,
	[ProcessInputConfig_Id] [smallint] NOT NULL,
	[Repository_Id] [int] NOT NULL,
	PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_ProcessInputRepositoryId]
    ON [dbo].[ProcessInput]([Repository_Id] ASC);
GO

CREATE TABLE [dbo].[ProcessInputConfig] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[ProcessConfig_Id] [smallint] NOT NULL,
	[RepositoryConfig_Id] [smallint] NOT NULL,
	[Frequency_Type_Id] [smallint] NULL,
	[FrequencyIncrease] [smallint] NOT NULL,
	[Visibility_Type_Id] [smallint] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[FrequencySimpleInput_Id] [int] NULL,
	[FrequencyAll] [bit] NOT NULL,
	CONSTRAINT [PK__ProcessC__3214EC0619DFD96B] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ProcessInputConfig] ADD  CONSTRAINT [DF__ProcessCo__Frequ__1BC821DD]  DEFAULT ((0)) FOR [FrequencyIncrease]
GO
ALTER TABLE [dbo].[ProcessInputConfig] ADD  CONSTRAINT [DF__ProcessCo__Delet__5224328E]  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[ProcessInputConfig] ADD  CONSTRAINT [DF__ProcessIn__Frequ__7B0717E7]  DEFAULT ((0)) FOR [FrequencyAll]

GO


CREATE TABLE [dbo].[ProcessOutput] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Process_Id] [int] NOT NULL,
	[ProcessOutputConfig_Id] [smallint] NOT NULL,
	[Repository_Id] [int] NOT NULL,
	PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_ProcessOutputRepositoryId]
    ON [dbo].[ProcessOutput]([Repository_Id] ASC);
GO

CREATE TABLE [dbo].[ProcessOutputConfig] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[ProcessConfig_Id] [smallint] NOT NULL,
	[Name] [varchar](255) NULL,
	[Code] [varchar](50) NULL,
	[RepositoryConfig_Id] [smallint] NOT NULL,
	[Visibility_Type_Id] [smallint] NULL,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF__ProcessCo__Delet__531856C7]  DEFAULT ((0)),
	CONSTRAINT [PK__ProcessC__3214EC062180FB33] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Profile] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[State_Type_Id] [bigint] NOT NULL,
	CONSTRAINT [PK__Profile__5AEE82B9] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ProfileAccess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Profile_Id] [int] NOT NULL,
	[Access_Id] [int] NOT NULL,
	CONSTRAINT [PK__ProfileAccess__5DCAEF64] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY],
	CONSTRAINT [UQ__ProfileAccess__5CD6CB2B] UNIQUE NONCLUSTERED (
		[Profile_Id] ASC,
		[Access_Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Report] (
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Code] [varchar](50) NULL,
	[Description] [varchar](max) NULL,
	[Entity_Id] [int] NULL,
	[PivotEnabled] [bit] NOT NULL CONSTRAINT [DF_Report_Pivot]  DEFAULT ((0)),
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Report_Deleted]  DEFAULT ((0)),
	CONSTRAINT [PK_Report] PRIMARY KEY CLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Repository] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RepositoryConfig_Id] [smallint] NOT NULL,
	[ProcessConfig_Id] [smallint] NULL,
	[Period] [date] NOT NULL,
	[Version] [smallint] NOT NULL CONSTRAINT [DF__Repositor__Versi__133DC8D4]  DEFAULT ((1)),
	[Valid] [bit] NOT NULL CONSTRAINT [DF__Repositor__Valid__11558062]  DEFAULT ((0)),
	[Open] [bit] NOT NULL DEFAULT ((0)),
	CONSTRAINT [PK__Reposito__3214EC0610566F31] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[RepositoryConfig] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Code] [varchar](100) NOT NULL,
	[Type_Id] [smallint] NOT NULL,
	[ViewPage] [varchar](255) NULL,
	[EditPage] [varchar](255) NULL,
	[Entity_Id] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[Regenerate] [bit] CONSTRAINT [DF_RepositoryConfig_Regenerate] DEFAULT ((0)) NOT NULL,
	CONSTRAINT [PK__Reposito__3214EC060C85DE4D] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RepositoryConfig] ADD  DEFAULT ('') FOR [Code]
GO
ALTER TABLE [dbo].[RepositoryConfig] ADD  CONSTRAINT [DF__Repositor__Delet__55009F39]  DEFAULT ((0)) FOR [Deleted]

GO

CREATE TABLE [dbo].[ProcessUnitConfig] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [Name]                VARCHAR (100) NOT NULL,
    [Class]               VARCHAR (255) NOT NULL,
    [Assembly]            VARCHAR (255) NOT NULL,
    [RepositoryConfig_Id] INT           NULL,
    [Partial]             VARCHAR (100) NOT NULL,
    CONSTRAINT [PK__ProcessU__3214EC068EE5CB20] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (FILLFACTOR = 100)
);
GO

GO

CREATE TABLE [dbo].[SpProcess] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Repository_Id] [int] NOT NULL,
	[ProcessConfig_Id] [int] NULL,
	[Content] [varchar](max) NULL,
	PRIMARY KEY NONCLUSTERED (
		[Id] ASC,
		[Repository_Id] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Status] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Description] [varchar](500) NULL,
	[StatusConfig_Id] [smallint] NOT NULL,
	[Order] [smallint] NOT NULL CONSTRAINT [DF_Status_Order]  DEFAULT ((0)),
	[Icon] [varchar](255) NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_Status_Active]  DEFAULT ((1)),
	CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[StatusConfig] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Description] [varchar](500) NULL,
	[Order] [int] NOT NULL CONSTRAINT [DF_StatusConfig_Order]  DEFAULT ((0)),
	CONSTRAINT [PK_StatusConfig] PRIMARY KEY CLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[StatusWorkflow] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Start_Status_Id] [smallint] NOT NULL,
	[To_Status_Id] [smallint] NOT NULL,
	[All] [bit] NOT NULL,
	CONSTRAINT [PK_StatusWorkflow] PRIMARY KEY CLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[StatusWorkflow] ADD  CONSTRAINT [DF_StatusWorkflow_All]  DEFAULT ((0)) FOR [All]

GO

CREATE TABLE [dbo].[Struct] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Entity_Id] [int] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Description] [varchar](255) NULL,
	[DataType_Id] [smallint] NOT NULL,
	[Length] [int] NULL,
	[LengthDecimal] [int] NULL,
	[Nullable] [bit] NOT NULL,
	[PK] [bit] NOT NULL,
	[Identity] [bit] NOT NULL,
	[RefEntity] [varchar](255) NULL,
	[InView] [bit] NOT NULL,
	[InTable] [bit] NOT NULL,
	[InObject] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[InImport] [bit] NOT NULL,
	[ImportOrder] [int] NULL,
	[ImportLength] [int] NULL,
	[ImportFormat] [varchar](50) NULL,
	[ImportName]    VARCHAR (255) NULL,
	CONSTRAINT [PK__Struct__3214EC067FB5F314] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	)
	WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Struct] ADD  CONSTRAINT [DF__Struct__Active__1BFD2C07]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Struct] ADD  CONSTRAINT [DF__Struct__InFile__697C9932]  DEFAULT ((0)) FOR [InImport]

GO

CREATE TABLE [dbo].[TypeConfig] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Description] [varchar](500) NOT NULL CONSTRAINT [DF__TypeConfi__Descr__25869641]  DEFAULT (''),
	[Order] [int] NOT NULL CONSTRAINT [DF__TypeConfi__Order__267ABA7A]  DEFAULT ((0)),
	[HasIcon] [bit] NOT NULL CONSTRAINT [DF__TypeConfi__HasIc__276EDEB3]  DEFAULT ((0)),
	[HasUniqueCode] [bit] NOT NULL CONSTRAINT [DF__TypeConfi__HasUn__286302EC]  DEFAULT ((0)),
	CONSTRAINT [PK__TypeConf__3214EC063F9B6DFF] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Type] (
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Code] [varchar](50) NOT NULL CONSTRAINT [DF__Type__Code__30F848ED]  DEFAULT (''),
	[TypeConfig_Id] [smallint] NOT NULL,
	[Order] [smallint] NOT NULL CONSTRAINT [DF__Type__Order__31EC6D26]  DEFAULT ((0)),
	[Description] [varchar](500) NOT NULL CONSTRAINT [DF__Type__Descriptio__32E0915F]  DEFAULT (''),
	[Icon] [binary](50) NULL,
	[IsDefault] [bit] NOT NULL CONSTRAINT [DF__Type__IsDefault__33D4B598]  DEFAULT ((0)),
	CONSTRAINT [PK__Type__3214EC061FEDB87C] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Type]  WITH CHECK ADD  CONSTRAINT [FK_Type_TypeConfig] FOREIGN KEY([TypeConfig_Id])
REFERENCES [dbo].[TypeConfig] ([Id])
ON UPDATE CASCADE

GO

ALTER TABLE [dbo].[Type] CHECK CONSTRAINT [FK_Type_TypeConfig]

GO

CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[FullName] [varchar](255) NOT NULL,
	[LastLogin] [datetime] NULL,
	[Email] [varchar](255) NOT NULL,
	[SessionOpen] [smallint] NOT NULL CONSTRAINT [D_dbo_User_1]  DEFAULT ((0)),
	[State_Type_Id] [bigint] NULL,
	[Password] [varchar](max) NULL,
	[SecretQuestion_Type_Id] [bigint] NULL,
	[SecretAnswer] [varchar](max) NULL,
	[ActivationKey] [varchar](100) NULL,
	[Language_Type_Id] [bigint] NULL,
	[FailLoginCount] [smallint] NOT NULL DEFAULT ((0)),
	[Company_Id] [int] NULL CONSTRAINT [DF__User__Company_Id__2818EA29]  DEFAULT ((0)),
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[UserProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[User_Id] [int] NOT NULL,
	[Profile_Id] [int] NOT NULL,
	CONSTRAINT [PK__UserProfile__59063A47] PRIMARY KEY NONCLUSTERED (
		[Id] ASC
	) WITH (
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]

GO