CREATE TABLE [dbo].[ExcludePaths](
	[ExcludePathID] [int] IDENTITY(1,1) NOT NULL,
	[Path] [nvarchar](max) NOT NULL,
	[FileSourceFolderID] [int] NOT NULL,
 CONSTRAINT [PK_ExcludePaths] PRIMARY KEY CLUSTERED 
(
	[ExcludePathID] ASC
), 
    CONSTRAINT [FK_ExcludePaths_FileSourceFolder] FOREIGN KEY ([FileSourceFolderID]) REFERENCES [FileSourceFolders]([FileSourceFolderID])
	ON DELETE CASCADE
)