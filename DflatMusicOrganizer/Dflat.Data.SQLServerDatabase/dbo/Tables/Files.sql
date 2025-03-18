CREATE TABLE [dbo].[Files]
(
	[FileID] INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
	[Filename] NVARCHAR(300) NOT NULL,
	[Extension] NVARCHAR(30) NOT NULL,
	[Directory] NVARCHAR(300) NOT NULL, 
    [Size] INT NOT NULL,
	[LastModifiedTime] DATETIME2 NOT NULL,
	[MarkedAsRemoved] BIT NOT NULL DEFAULT(0),
	[MD5Sum] NVARCHAR(32) NOT NULL
)

GO

CREATE INDEX [IX_Files_MD5Sum] ON [dbo].[Files] ([MD5Sum])

GO

CREATE INDEX [IX_Files_Directory] ON [dbo].[Files] ([Directory])
