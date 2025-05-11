SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Repository_SP_GetByPeriods]
(
    @Periods varchar(max),
    @RepositoryConfig_Id int = null,
    @Valid bit = 1 
)
AS

SELECT
    *
FROM Repository
WHERE
    [Period] IN (
        select A.[Data] from SplitChar(@Periods, ',') as A
    )
AND ( @RepositoryConfig_Id IS NULL OR  RepositoryConfig_Id = @RepositoryConfig_Id )
AND Valid = @Valid
GO
