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
    [FileID] INT NULL, 
    CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED 
(
	[JobID] ASC
)
)