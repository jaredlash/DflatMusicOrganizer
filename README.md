# DflatMusicOrganizer

## About
Initially intended to help organize my music collection, this project has become an area for my own personal learning and
experimentation.  This is not intended for use by others.

## Structure
- [`DflatMusicOrganizer`](DflatMusicOrganizer/) - .NET / C# project being migrated from a WPF / Desktop monolith to a microservice-based application.  This directory will eventually be renamed to something to indicate it is a microservice monorepo with the intention being that client applications will be in other top-level directories.
- [`DflatMusicOrganizer/FileService`](DflatMusicOrganizer/FileService/) - Microservice to handle performing operations to files and folders on the host system.
- [`DflatMusicOrganizer/Dflat.Application/Services/JobServices`](DflatMusicOrganizer/Dflat.Application/Services/JobServices/) - Home-grown implementation of a job queueing system.  This is slated to be replaced with RabbitMQ.

## Work in progress
- ~~Convert model IDs to GUIDs~~ ([PR 1](https://github.com/jaredlash/DflatMusicOrganizer/pull/1))
- ~~Remove use of Automapper~~ ([PR 2](https://github.com/jaredlash/DflatMusicOrganizer/pull/2))
- ~~Clean up FileCollectionCompare~~ ([PR 3](https://github.com/jaredlash/DflatMusicOrganizer/pull/3))
- ~~Update WPF UI to use Community MVVM Toolkit~~ ([PR 4](https://github.com/jaredlash/DflatMusicOrganizer/pull/4))
- ~~Miscellaneous cleanups (nullable refs, file-scoped namespaces, unneeded usings)~~ ([PR 5](https://github.com/jaredlash/DflatMusicOrganizer/pull/5))
- Create FileService ([Branch](https://github.com/jaredlash/DflatMusicOrganizer/tree/File-Service-API))
- Create WPF File/folder hierarchy browser
- Use File/folder browser to configure FileSourceFolders in WPF client by accessing FileService
- Split WPF client into its own top-level project
- Replace custom JobServices with RabbitMQ
- Split former JobServices into respective microservices