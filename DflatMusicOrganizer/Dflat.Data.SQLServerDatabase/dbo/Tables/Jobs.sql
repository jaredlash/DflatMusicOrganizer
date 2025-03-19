CREATE TABLE [dbo].[Jobs](
	[JobID] [int] IDENTITY(1,1) NOT NULL,
	[CreationTime] [datetime2](7) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[IgnoreCache] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[Output] [nvarchar](max) NOT NULL,
	[Errors] [nvarchar](max) NOT NULL,
	[JobType] [int] NOT NULL,
	[FileSourceFolderID] [int] NULL,
    [FileID] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED 
(
	[JobID] ASC
)
)
GO

CREATE INDEX [IX_Jobs_JobType] ON [dbo].[Jobs] ([JobType])

GO

CREATE INDEX [IX_Jobs_Status] ON [dbo].[Jobs] ([Status])

GO


CREATE INDEX [IX_Jobs_CreationTime] ON [dbo].[Jobs] ([CreationTime])
