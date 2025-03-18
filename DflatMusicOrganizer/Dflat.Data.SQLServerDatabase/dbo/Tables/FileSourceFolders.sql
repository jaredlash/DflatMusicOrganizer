CREATE TABLE [dbo].[FileSourceFolders]
(
	[FileSourceFolderID] INT IDENTITY(1, 1) NOT NULL, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Path] NVARCHAR(MAX) NOT NULL, 
    [IsTemporaryMedia] BIT NOT NULL, 
    [LastScanStart] DATETIME2 NULL,
    CONSTRAINT [PK_FileSourceFolders] PRIMARY KEY CLUSTERED 
(
	[FileSourceFolderID] ASC
))