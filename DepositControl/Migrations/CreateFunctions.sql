CREATE FUNCTION [dbo].[SplitChar]
(@RowData nvarchar(MAX),
 @SplitOn nvarchar(5) = ',')
RETURNS @RtnValue table
(
[Data] nvarchar(100) COLLATE Modern_Spanish_CI_AS NULL
)
WITH EXEC AS CALLER
AS
BEGIN

if(@RowData is null)
begin
 return 
end 
	Declare @Cnt int
	Set @Cnt = 1

	While (Charindex(@SplitOn,@RowData)>0)
	Begin
		Insert Into @RtnValue (data)
		Select 
			Data = ltrim(rtrim(Substring(@RowData,1,Charindex(@SplitOn,@RowData)-1)))

		Set @RowData = Substring(@RowData,Charindex(@SplitOn,@RowData)+1,len(@RowData))
		Set @Cnt = @Cnt + 1
	End
	
	Insert Into @RtnValue (data)
	Select Data = ltrim(rtrim(@RowData))

	Return
END

GO

CREATE FUNCTION [dbo].[SplitID]
(@RowData nvarchar(MAX))
RETURNS @RtnValue table
(
[Data] nvarchar(100) COLLATE Modern_Spanish_CI_AS NULL
)
WITH EXEC AS CALLER
AS
BEGIN
if(@RowData is null)
begin
 return 
end 
	Declare @Cnt int
	declare @SplitOn nvarchar(5)
  set @SplitOn= N','
	Set @Cnt = 1

	While (Charindex(@SplitOn,@RowData)>0)
	Begin
		Insert Into @RtnValue (data)
		Select 
			Data = ltrim(rtrim(Substring(@RowData,1,Charindex(@SplitOn,@RowData)-1)))

		Set @RowData = Substring(@RowData,Charindex(@SplitOn,@RowData)+1,len(@RowData))
		Set @Cnt = @Cnt + 1
	End
	
	Insert Into @RtnValue (data)
	Select Data = ltrim(rtrim(@RowData))

	Return
END

GO