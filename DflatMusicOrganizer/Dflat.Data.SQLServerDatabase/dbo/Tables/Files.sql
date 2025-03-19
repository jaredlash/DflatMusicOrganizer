CREATE TABLE [dbo].[Files]
(
	[FileID] UNIQUEIDENTIFIER NOT NULL ,
	[Filename] NVARCHAR(300) NOT NULL,
	[Extension] NVARCHAR(30) NOT NULL,
	[Directory] NVARCHAR(300) NOT NULL, 
    [Size] INT NOT NULL,
	[LastModifiedTime] DATETIME2 NOT NULL,
	[MarkedAsRemoved] BIT NOT NULL DEFAULT(0),
	[MD5Sum] NVARCHAR(32) NOT NULL, 
    CONSTRAINT [PK_Files] PRIMARY KEY NONCLUSTERED ([FileID])
)

GO

CREATE INDEX [IX_Files_MD5Sum] ON [dbo].[Files] ([MD5Sum])

GO

CREATE INDEX [IX_Files_Directory] ON [dbo].[Files] ([Directory])
