-- =============================================================================
-- openPDC Data Structures for SQL Server
--
-- Tennessee Valley Authority, 2009
-- No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
--
-- James Ritchie Carroll
-- 09/01/2009
-- =============================================================================

USE [master]
GO
CREATE DATABASE [openPDC];
GO
ALTER DATABASE [openPDC] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [openPDC] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [openPDC] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [openPDC] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [openPDC] SET ARITHABORT OFF 
GO
ALTER DATABASE [openPDC] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [openPDC] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [openPDC] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [openPDC] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [openPDC] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [openPDC] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [openPDC] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [openPDC] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [openPDC] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [openPDC] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [openPDC] SET  ENABLE_BROKER 
GO
ALTER DATABASE [openPDC] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [openPDC] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [openPDC] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [openPDC] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [openPDC] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [openPDC] SET  READ_WRITE 
GO
ALTER DATABASE [openPDC] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [openPDC] SET  MULTI_USER 
GO
ALTER DATABASE [openPDC] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [openPDC] SET DB_CHAINING OFF 
GO
-- The next three commented statements are used to create a user with access to the openPDC database.
-- Be sure to change the username and password.
-- Replace-all from NewUser to the desired username is the preferred method of changing the username.

--IF  NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'NewUser')
--CREATE LOGIN [NewUser] WITH PASSWORD=N'MyPassword', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
--GO
USE [openPDC]
GO
--CREATE USER [NewUser] FOR LOGIN [NewUser]
--GO
CREATE ROLE [openPDCManagerRole] AUTHORIZATION [dbo]
GO
--EXEC sp_addrolemember N'openPDCManagerRole', N'NewUser'
--GO
EXEC sp_addrolemember N'db_datareader', N'openPDCManagerRole'
GO
EXEC sp_addrolemember N'db_datawriter', N'openPDCManagerRole'
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ErrorLog](
      [ID] [int] IDENTITY(1,1) NOT NULL,
      [Source] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
      [Message] [varchar](1024) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
      [Detail] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
      [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ErrorLog_CreatedOn]  DEFAULT (getdate()),
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
      [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [Runtime](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceID] [int] NOT NULL,
	[SourceTable] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Runtime] PRIMARY KEY CLUSTERED 
(
	[SourceID] ASC,
	[SourceTable] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Runtime] ON [Runtime] 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Company](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](50) NOT NULL,
	[MapAcronym] [nchar](3) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[URL] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_Company_LoadOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ConfigurationEntity](
	[SourceName] [nvarchar](100) NOT NULL,
	[RuntimeName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_ConfigurationEntity_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_ConfigurationEntity_Enabled]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Vendor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](3) NULL,
	[Name] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](100) NULL,
	[ContactEmail] [nvarchar](100) NULL,
	[URL] [nvarchar](max) NULL,
 CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Protocol](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_Protocol_LoadOrder] DEFAULT ((0)),
 CONSTRAINT [PK_Protocol] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SignalType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Acronym] [nvarchar](4) NOT NULL,
	[Suffix] [nvarchar](2) NOT NULL,
	[Abbreviation] [nvarchar](2) NOT NULL,
	[Source] [nvarchar](10) NOT NULL,
	[EngineeringUnits] [nvarchar](10) NULL,
 CONSTRAINT [PK_SignalType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Interconnection](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[LoadOrder] [int] NULL CONSTRAINT [DF_Interconnection_LoadOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_Interconnection] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OtherDevice](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](16) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[IsConcentrator] [bit] NOT NULL CONSTRAINT [DF_OtherDevices_IsConcentrator]  DEFAULT ((0)),
	[CompanyID] [int] NULL,
	[VendorDeviceID] [int] NULL,
	[Longitude] [decimal](9, 6) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[InterconnectionID] [int] NULL,
	[Planned] [bit] NOT NULL CONSTRAINT [DF_OtherDevices_Planned]  DEFAULT ((0)),
	[Desired] [bit] NOT NULL CONSTRAINT [DF_OtherDevices_Desired]  DEFAULT ((0)),
	[InProgress] [bit] NOT NULL CONSTRAINT [DF_OtherDevices_InProgress]  DEFAULT ((0)),
 CONSTRAINT [PK_OtherDevice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Node](
	[ID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Node_ID]  DEFAULT (newid()),
	[Name] [nvarchar](100) NOT NULL,
	[CompanyID] [int] NULL,
	[Longitude] [decimal](9, 6) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Description] [nvarchar](max) NULL,
	[ImagePath] [nvarchar](max) NULL,
	[Master] [bit] NOT NULL CONSTRAINT [DF_Node_Master]  DEFAULT ((0)),
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_Node_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_Node_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_Node] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Device](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParentID] [int] NULL,
	[Acronym] [nvarchar](16) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[IsConcentrator] [bit] NOT NULL CONSTRAINT [DF_Device_IsConcentrator]  DEFAULT ((0)),
	[CompanyID] [int] NULL,
	[HistorianID] [int] NULL,
	[AccessID] [int] NOT NULL CONSTRAINT [DF_Device_AccessID]  DEFAULT ((0)),
	[VendorDeviceID] [int] NULL,
	[ProtocolID] [int] NULL,
	[Longitude] [decimal](9, 6) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[InterconnectionID] [int] NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[TimeZone] [nvarchar](128) NULL,
	[FramesPerSecond] [int] NULL DEFAULT ((30)),
	[TimeAdjustmentTicks] [bigint] NOT NULL CONSTRAINT [DF_Device_TimeAdjustmentTicks]  DEFAULT ((0)),
	[DataLossInterval] [float] NOT NULL CONSTRAINT [DF_Device_DataLossInterval]  DEFAULT ((35)),
	[ContactList] [nvarchar](max) NULL,
	[MeasuredLines] [int] NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_Device_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_Device_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Device_Acronym] ON [Device]
(
	[Acronym] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
	This trigger after INSERT and DELETE on [dbo].[Device_RuntimeSync] performs INSERT and DELETE on [dbo].[Runtime] table.
	Primary Developer: Mehul Thakkar, 9/02/2009
	Email: mpthakka@tva.gov
*/

CREATE TRIGGER [Device_RuntimeSync] 
   ON  [Device]
   AFTER INSERT, DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
		BEGIN
			INSERT INTO Runtime (SourceID, SourceTable)
			SELECT ID, 'Device' FROM INSERTED
		END

	-- If a record has been deleted
	IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'Device'
		END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [VendorDevice](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[VendorID] [int] NOT NULL CONSTRAINT [DF_VendorDevice_VendorID]  DEFAULT ((10)),
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[URL] [nvarchar](max) NULL,
 CONSTRAINT [PK_VendorDevice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OutputStreamDeviceDigital](
	[NodeID] [uniqueidentifier] NOT NULL,
	[OutputStreamDeviceID] [int] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](256) NOT NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_OutputStreamDeviceDigital_LoadOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_OutputStreamDeviceDigital] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OutputStreamDevicePhasor](
	[NodeID] [uniqueidentifier] NOT NULL,
	[OutputStreamDeviceID] [int] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](12) NOT NULL,
	[Type] [nchar](1) NOT NULL CONSTRAINT [DF_OutputStreamDevicePhasor_Type]  DEFAULT (N'V'),
	[Phase] [nchar](1) NOT NULL CONSTRAINT [DF_OutputStreamDevicePhasor_Phase]  DEFAULT (N'+'),
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_OutputStreamDevicePhasor_LoadOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_OutputStreamDevicePhasor] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OutputStreamDeviceAnalog](
	[NodeID] [uniqueidentifier] NOT NULL,
	[OutputStreamDeviceID] [int] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](16) NOT NULL,
	[Type] [int] NOT NULL CONSTRAINT [DF_OutputStreamDeviceAnalog_Type]  DEFAULT ((0)),
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_OutputStreamDeviceAnalog_LoadOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_OutputStreamDeviceAnalog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Measurement](
	[SignalID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Measurement_SignalID]  DEFAULT (newid()),
	[HistorianID] [int] NULL,
	[PointID] [int] IDENTITY(1,1) NOT NULL,
	[DeviceID] [int] NULL,
	[PointTag] [nvarchar](50) NOT NULL,
	[AlternateTag] [nvarchar](50) NULL,
	[SignalTypeID] [int] NOT NULL,
	[PhasorSourceIndex] [int] NULL,
	[SignalReference] [nvarchar](24) NOT NULL,
	[Adder] [float] NOT NULL CONSTRAINT [DF_Measurement_Adder]  DEFAULT ((0.0)),
	[Multiplier] [float] NOT NULL CONSTRAINT [DF_Measurement_Multiplier]  DEFAULT ((1.0)),
	[Description] [nvarchar](max) NULL,
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_Measurement_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_Measurement] PRIMARY KEY CLUSTERED 
(
	[SignalID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Measurement_PointID] ON [Measurement] 
(
	[PointID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Measurement_PointTag] ON [Measurement] 
(
	[PointTag] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Measurement_SignalReference] ON [Measurement] 
(
	[SignalReference] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OutputStreamMeasurement](
	[NodeID] [uniqueidentifier] NOT NULL,
	[AdapterID] [int] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[HistorianID] [int] NULL,
	[PointID] [int] NOT NULL,
	[SignalReference] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_OutputStreamMeasurement] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OutputStreamDevice](
	[NodeID] [uniqueidentifier] NOT NULL,
	[AdapterID] [int] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](16) NOT NULL,
	[BpaAcronym] [nvarchar](4) NULL,
	[Name] [nvarchar](100) NOT NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_OutputStreamDevices_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_OutputStreamDevices_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_OutputStreamDevice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Phasor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DeviceID] [int] NOT NULL,
	[Label] [nvarchar](50) NOT NULL,
	[Type] [nchar](1) NOT NULL CONSTRAINT [DF_Phasor_Type]  DEFAULT (N'V'),
	[Phase] [nchar](1) NOT NULL CONSTRAINT [DF_Phasor_Phase]  DEFAULT (N'+'),
	[DestinationPhasorID] [int] NULL,
	[SourceIndex] [int] NOT NULL CONSTRAINT [DF_Phasor_SourceIndex]  DEFAULT ((0)),
 CONSTRAINT [PK_Phasor] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CalculatedMeasurement](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[AssemblyName] [nvarchar](max) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[ConfigSection] [nvarchar](100) NULL,
	[InputMeasurements] [nvarchar](max) NULL,
	[OutputMeasurements] [nvarchar](max) NULL,
	[MinimumMeasurementsToUse] [int] NOT NULL CONSTRAINT [DF_CalculatedMeasurement_MinimumMeasurementsToUse]  DEFAULT ((-1)),
	[FramesPerSecond] [int] NOT NULL CONSTRAINT [DF_CalculatedMeasurement_FramesPerSecond]  DEFAULT ((30)),
	[LagTime] [float] NOT NULL CONSTRAINT [DF_CalculatedMeasurement_LagTime]  DEFAULT ((3.0)),
	[LeadTime] [float] NOT NULL CONSTRAINT [DF_CalculatedMeasurement_LeadTime]  DEFAULT ((1.0)),
	[UseLocalClockAsRealTime] [bit] NOT NULL CONSTRAINT [DF_CalculatedMeasurement_UseLocalClockAsRealTime]  DEFAULT ((0)),
	[AllowSortsByArrival] [bit] NOT NULL CONSTRAINT [DF_CalculatedMeasurement_AllowSortsByArrival]  DEFAULT ((0)),
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_CalculatedMeasurement_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_CalculatedMeasurement_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_CalculatedMeasurement] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
	This trigger after INSERT and DELETE on [dbo].[CalculatedMeasurement_RuntimeSync] performs INSERT and DELETE on [dbo].[Runtime] table.
	Primary Developer: Mehul Thakkar, 9/02/2009
	Email: mpthakka@tva.gov
*/

CREATE TRIGGER [CalculatedMeasurement_RuntimeSync] 
   ON  [CalculatedMeasurement]
   AFTER INSERT, DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
		BEGIN
			INSERT INTO Runtime (SourceID, SourceTable)
			SELECT ID, 'CalculatedMeasurement' FROM INSERTED
		END

	-- If a record has been deleted
	IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'CalculatedMeasurement'
		END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CustomActionAdapter](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AdapterName] [nvarchar](50) NOT NULL,
	[AssemblyName] [nvarchar](max) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomActionAdapter_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomActionAdapter_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_CustomActionAdapter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
	This trigger after INSERT and DELETE on [dbo].[CustomActionAdapter_RuntimeSync] performs INSERT and DELETE on [dbo].[Runtime] table.
	Primary Developer: Mehul Thakkar, 9/02/2009
	Email: mpthakka@tva.gov
*/

CREATE TRIGGER [CustomActionAdapter_RuntimeSync] 
   ON  [CustomActionAdapter]
   AFTER INSERT, DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
		BEGIN
			INSERT INTO Runtime (SourceID, SourceTable)
			SELECT ID, 'CustomActionAdapter' FROM INSERTED
		END

	-- If a record has been deleted
	IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'CustomActionAdapter'
		END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Historian](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[AssemblyName] [nvarchar](max) NULL,
	[TypeName] [nvarchar](max) NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[IsLocal] [bit] NOT NULL CONSTRAINT [DF_Historian_IsLocal]  DEFAULT ((0)),
	[Description] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_Historian_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_Historian_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_Historian] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
	This trigger after INSERT and DELETE on [dbo].[Historian_RuntimeSync] performs INSERT and DELETE on [dbo].[Runtime] table.
	Primary Developer: Mehul Thakkar, 9/02/2009
	Email: mpthakka@tva.gov
*/

CREATE TRIGGER [Historian_RuntimeSync] 
   ON  [Historian]
   AFTER INSERT, DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
		BEGIN
			INSERT INTO Runtime (SourceID, SourceTable)
			SELECT ID, 'Historian' FROM INSERTED
		END

	-- If a record has been deleted
	IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'Historian'
		END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CustomInputAdapter](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AdapterName] [nvarchar](50) NOT NULL,
	[AssemblyName] [nvarchar](max) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomInputAdapter_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomInputAdapter_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_CustomInputAdapter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
	This trigger after INSERT and DELETE on [dbo].[CustomInputAdapter_RuntimeSync] performs INSERT and DELETE on [dbo].[Runtime] table.
	Primary Developer: Mehul Thakkar, 9/02/2009
	Email: mpthakka@tva.gov
*/

CREATE TRIGGER [CustomInputAdapter_RuntimeSync] 
   ON  [CustomInputAdapter]
   AFTER INSERT, DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
		BEGIN
			INSERT INTO Runtime (SourceID, SourceTable)
			SELECT ID, 'CustomInputAdapter' FROM INSERTED
		END

	-- If a record has been deleted
	IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'CustomInputAdapter'
		END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OutputStream](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Type] [int] NOT NULL CONSTRAINT [DF_OutputStream_Type]  DEFAULT ((0)),
	[ConnectionString] [nvarchar](max) NULL,
	[DataChannel] [nvarchar](max) NULL,
	[CommandChannel] [nvarchar](max) NULL,
	[IDCode] [int] NOT NULL CONSTRAINT [DF_OutputStream_IDCode]  DEFAULT ((0)),
	[AutoPublishConfigFrame] [bit] NOT NULL CONSTRAINT [DF_OutputStream_AutoPublishConfigFrame]  DEFAULT ((0)),
	[AutoStartDataChannel] [bit] NOT NULL CONSTRAINT [DF_OutputStream_AutoStartDataChannel]  DEFAULT ((1)),
	[NominalFrequency] [int] NOT NULL CONSTRAINT [DF_OutputStream_NominalFrequency]  DEFAULT ((60)),
	[FramesPerSecond] [int] NOT NULL CONSTRAINT [DF_OutputStream_FramesPerSecond]  DEFAULT ((30)),
	[LagTime] [float] NOT NULL CONSTRAINT [DF_OutputStream_LagTime]  DEFAULT ((3.0)),
	[LeadTime] [float] NOT NULL CONSTRAINT [DF_OutputStream_LeadTime]  DEFAULT ((1.0)),
	[UseLocalClockAsRealTime] [bit] NOT NULL CONSTRAINT [DF_OutputStream_UseLocalClockAsRealTime]  DEFAULT ((0)),
	[AllowSortsByArrival] [bit] NOT NULL CONSTRAINT [DF_OutputStream_AllowSortsByArrival]  DEFAULT ((0)),
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_OutputStream_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_OutputStream_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_OutputStream] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
	This trigger after INSERT and DELETE on [dbo].[OutputStream_RuntimeSync] performs INSERT and DELETE on [dbo].[Runtime] table.
	Primary Developer: Mehul Thakkar, 9/02/2009
	Email: mpthakka@tva.gov
*/

CREATE TRIGGER [OutputStream_RuntimeSync] 
   ON  [OutputStream]
   AFTER INSERT, DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
		BEGIN
			INSERT INTO Runtime (SourceID, SourceTable)
			SELECT ID, 'OutputStream' FROM INSERTED
		END

	-- If a record has been deleted
	IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'OutputStream'
		END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CustomOutputAdapter](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AdapterName] [nvarchar](50) NOT NULL,
	[AssemblyName] [nvarchar](max) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomOutputAdapter_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomOutputAdapter_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_CustomOutputAdapter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*
	This trigger after INSERT and DELETE on [dbo].[CustomOutputAdapter_RuntimeSync] performs INSERT and DELETE on [dbo].[Runtime] table.
	Primary Developer: Mehul Thakkar, 9/02/2009
	Email: mpthakka@tva.gov
*/

CREATE TRIGGER [CustomOutputAdapter_RuntimeSync] 
   ON  [CustomOutputAdapter]
   AFTER INSERT, DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
		BEGIN
			INSERT INTO Runtime (SourceID, SourceTable)
			SELECT ID, 'CustomOutputAdapter' FROM INSERTED
		END

	-- If a record has been deleted
	IF EXISTS(SELECT * FROM DELETED)
		BEGIN
			DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'CustomOutputAdapter'
		END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeOutputStreamMeasurement]
AS
SELECT     TOP (100) PERCENT dbo.OutputStreamMeasurement.NodeID, dbo.Runtime.ID AS AdapterID, dbo.Historian.Acronym AS Historian, 
                      dbo.OutputStreamMeasurement.PointID, dbo.OutputStreamMeasurement.SignalReference
FROM         dbo.OutputStreamMeasurement LEFT OUTER JOIN
                      dbo.Historian ON dbo.OutputStreamMeasurement.HistorianID = dbo.Historian.ID LEFT OUTER JOIN
                      dbo.Runtime ON dbo.OutputStreamMeasurement.AdapterID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'OutputStream'
ORDER BY dbo.OutputStreamMeasurement.HistorianID, dbo.OutputStreamMeasurement.PointID

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeHistorian]
AS
SELECT     TOP (100) PERCENT dbo.Historian.NodeID, dbo.Runtime.ID, dbo.Historian.Acronym AS AdapterName, COALESCE (LTRIM(RTRIM(dbo.Historian.AssemblyName)), 
                      N'HistorianAdapters.dll') AS AssemblyName, COALESCE (LTRIM(RTRIM(dbo.Historian.TypeName)), 
                      CASE dbo.Historian.IsLocal WHEN 1 THEN N'HistorianAdapters.LocalOutputAdapter' ELSE N'HistorianAdapters.RemoteOutputAdapter' END) 
                      AS TypeName, CASE WHEN Historian.ConnectionString IS NULL THEN N'' ELSE Historian.ConnectionString + N'; ' END + 
                      N'instanceName=' + dbo.Historian.Acronym + N'; sourceids=' + dbo.Historian.Acronym AS ConnectionString
FROM         dbo.Historian LEFT OUTER JOIN
                      dbo.Runtime ON dbo.Historian.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'Historian'
WHERE     (dbo.Historian.Enabled <> 0)
ORDER BY dbo.Historian.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeDevice]
AS
SELECT     TOP (100) PERCENT dbo.Device.NodeID, dbo.Runtime.ID, dbo.Device.Acronym AS AdapterName, N'TVA.PhasorProtocols.dll' AS AssemblyName, 
                      N'TVA.PhasorProtocols.PhasorMeasurementMapper' AS TypeName, CASE WHEN dbo.Device.ConnectionString IS NULL 
                      THEN N'' ELSE dbo.Device.ConnectionString END + N'; isConcentrator=' + CONVERT(NVARCHAR(10), dbo.Device.IsConcentrator) 
                      + N'; accessID=' + CONVERT(NVARCHAR(10), dbo.Device.AccessID) + CASE WHEN dbo.Device.TimeZone IS NULL 
                      THEN N'' ELSE N'; timeZone=' + dbo.Device.TimeZone END + N'; timeAdjustmentTicks=' + CONVERT(NVARCHAR(30), dbo.Device.TimeAdjustmentTicks) 
                      + CASE WHEN dbo.Protocol.Acronym IS NULL THEN N'' ELSE N'; phasorProtocol=' + dbo.Protocol.Acronym END + N'; dataLossInterval=' + CONVERT(NVARCHAR(10), 
                      dbo.Device.DataLossInterval) AS ConnectionString
FROM         dbo.Device LEFT OUTER JOIN
                      dbo.Protocol ON dbo.Device.ProtocolID = dbo.Protocol.ID LEFT OUTER JOIN
                      dbo.Runtime ON dbo.Device.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'Device'
WHERE     (dbo.Device.Enabled <> 0 AND Device.ParentID IS NULL)
ORDER BY dbo.Device.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeCustomOutputAdapter]
AS
SELECT     TOP (100) PERCENT dbo.CustomOutputAdapter.NodeID, dbo.Runtime.ID, dbo.CustomOutputAdapter.AdapterName, 
                      LTRIM(RTRIM(dbo.CustomOutputAdapter.AssemblyName)) AS AssemblyName, LTRIM(RTRIM(dbo.CustomOutputAdapter.TypeName)) AS TypeName, dbo.CustomOutputAdapter.ConnectionString
FROM         dbo.CustomOutputAdapter LEFT OUTER JOIN
                      dbo.Runtime ON dbo.CustomOutputAdapter.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'CustomOutputAdapter'
WHERE     (dbo.CustomOutputAdapter.Enabled <> 0)
ORDER BY dbo.CustomOutputAdapter.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeInputStreamDevice]
AS
SELECT     TOP (100) PERCENT dbo.Device.NodeID, Runtime_P.ID AS ParentID, dbo.Runtime.ID, dbo.Device.Acronym, dbo.Device.AccessID
FROM         dbo.Device LEFT OUTER JOIN
                      dbo.Runtime ON dbo.Device.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'Device' LEFT OUTER JOIN
                      dbo.Runtime AS Runtime_P ON dbo.Device.ParentID = Runtime_P.SourceID AND Runtime_P.SourceTable = N'Device'
WHERE     (dbo.Device.IsConcentrator = 0) AND (dbo.Device.Enabled <> 0) AND (dbo.Device.ParentID IS NOT NULL)
ORDER BY dbo.Device.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeCustomInputAdapter]
AS
SELECT     TOP (100) PERCENT dbo.CustomInputAdapter.NodeID, dbo.Runtime.ID, dbo.CustomInputAdapter.AdapterName, 
                      LTRIM(RTRIM(dbo.CustomInputAdapter.AssemblyName)) AS AssemblyName, LTRIM(RTRIM(dbo.CustomInputAdapter.TypeName)) AS TypeName, dbo.CustomInputAdapter.ConnectionString
FROM         dbo.CustomInputAdapter LEFT OUTER JOIN
                      dbo.Runtime ON dbo.CustomInputAdapter.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'CustomInputAdapter'
WHERE     (dbo.CustomInputAdapter.Enabled <> 0)
ORDER BY dbo.CustomInputAdapter.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeOutputStreamDevice]
AS
SELECT     TOP (100) PERCENT dbo.OutputStreamDevice.NodeID, dbo.Runtime.ID AS ParentID, dbo.OutputStreamDevice.ID, dbo.OutputStreamDevice.Acronym, 
                      dbo.OutputStreamDevice.BpaAcronym, dbo.OutputStreamDevice.Name, dbo.OutputStreamDevice.LoadOrder
FROM         dbo.OutputStreamDevice LEFT OUTER JOIN
                      dbo.Runtime ON dbo.OutputStreamDevice.AdapterID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'OutputStream'
WHERE     (dbo.OutputStreamDevice.Enabled <> 0)
ORDER BY dbo.OutputStreamDevice.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeOutputStream]
AS
SELECT     TOP (100) PERCENT dbo.OutputStream.NodeID, dbo.Runtime.ID, dbo.OutputStream.Acronym AS AdapterName, 
                      N'TVA.PhasorProtocols.dll' AS AssemblyName, 
                      CASE Type WHEN 1 THEN N'TVA.PhasorProtocols.BpaPdcStream.Concentrator' ELSE N'TVA.PhasorProtocols.IeeeC37_118.Concentrator' END AS TypeName,
                      CASE WHEN dbo.OutputStream.ConnectionString IS NULL THEN N'' ELSE dbo.OutputStream.ConnectionString + N'; ' END
                      + CASE WHEN dbo.OutputStream.DataChannel IS NULL THEN N'' ELSE N'dataChannel={' + dbo.OutputStream.DataChannel + N'}' END
                      + CASE WHEN dbo.OutputStream.CommandChannel IS NULL THEN N'' ELSE N'; commandChannel={' + dbo.OutputStream.CommandChannel + N'}' END
                      + N'; idCode=' + CONVERT(NVARCHAR(10), dbo.OutputStream.IDCode)
                      + N'; autoPublishConfigFrame=' + CONVERT(NVARCHAR(10), dbo.OutputStream.AutoPublishConfigFrame)
                      + N'; autoStartDataChannel=' + CONVERT(NVARCHAR(10), dbo.OutputStream.AutoStartDataChannel)
                      + N'; nominalFrequency=' + CONVERT(NVARCHAR(10), dbo.OutputStream.NominalFrequency)
                      + N'; lagTime=' + CONVERT(NVARCHAR(10), dbo.OutputStream.LagTime)
                      + N'; leadTime=' + CONVERT(NVARCHAR(10), dbo.OutputStream.LeadTime) 
                      + N'; framesPerSecond=' + CONVERT(NVARCHAR(10), dbo.OutputStream.FramesPerSecond)
                      + N'; useLocalClockAsRealTime=' + CONVERT(NVARCHAR(10), dbo.OutputStream.UseLocalClockAsRealTime) 
                      + N'; allowSortsByArrival=' + CONVERT(NVARCHAR(10), dbo.OutputStream.AllowSortsByArrival) AS ConnectionString
FROM         dbo.OutputStream LEFT OUTER JOIN
                      dbo.Runtime ON dbo.OutputStream.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'OutputStream'
WHERE     (dbo.OutputStream.Enabled <> 0)
ORDER BY dbo.OutputStream.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeCustomActionAdapter]
AS
SELECT     TOP (100) PERCENT dbo.CustomActionAdapter.NodeID, dbo.Runtime.ID, dbo.CustomActionAdapter.AdapterName, 
                      LTRIM(RTRIM(dbo.CustomActionAdapter.AssemblyName)) AS AssemblyName, LTRIM(RTRIM(dbo.CustomActionAdapter.TypeName)) AS TypeName, dbo.CustomActionAdapter.ConnectionString
FROM         dbo.CustomActionAdapter LEFT OUTER JOIN
                      dbo.Runtime ON dbo.CustomActionAdapter.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'CustomActionAdapter'
WHERE     (dbo.CustomActionAdapter.Enabled <> 0)
ORDER BY dbo.CustomActionAdapter.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MeasurementDetail]
AS
SELECT     dbo.Device.CompanyID, dbo.Company.Acronym AS CompanyAcronym, dbo.Company.Name AS CompanyName, dbo.Measurement.SignalID, 
                      dbo.Measurement.HistorianID, dbo.Historian.Acronym AS HistorianAcronym, dbo.Historian.ConnectionString AS HistorianConnectionString, 
                      dbo.Measurement.PointID, dbo.Measurement.PointTag, dbo.Measurement.AlternateTag, dbo.Measurement.DeviceID, dbo.Device.NodeID, 
                      dbo.Device.Acronym AS DeviceAcronym, dbo.Device.Name AS DeviceName, COALESCE(dbo.Device.FramesPerSecond, 30) AS FramesPerSecond, dbo.Device.Enabled AS DeviceEnabled, dbo.Device.ContactList, 
                      dbo.Device.VendorDeviceID, dbo.VendorDevice.Name AS VendorDeviceName, dbo.VendorDevice.Description AS VendorDeviceDescription, 
                      dbo.Device.ProtocolID, dbo.Protocol.Acronym AS ProtocolAcronym, dbo.Protocol.Name AS ProtocolName, dbo.Measurement.SignalTypeID, 
                      dbo.Measurement.PhasorSourceIndex, dbo.Phasor.Label AS PhasorLabel, dbo.Phasor.Type AS PhasorType, dbo.Phasor.Phase, 
                      dbo.Measurement.SignalReference, dbo.Measurement.Adder, dbo.Measurement.Multiplier, dbo.Measurement.Description, dbo.Measurement.Enabled, 
                      COALESCE (dbo.SignalType.EngineeringUnits, N'') AS EngineeringUnits, dbo.SignalType.Source, dbo.SignalType.Acronym AS SignalAcronym, 
                      dbo.SignalType.Name AS SignalName, dbo.SignalType.Suffix AS SignalTypeSuffix, dbo.Device.Longitude, dbo.Device.Latitude
FROM         dbo.Company RIGHT OUTER JOIN
                      dbo.Device ON dbo.Company.ID = dbo.Device.CompanyID RIGHT OUTER JOIN
                      dbo.Measurement LEFT OUTER JOIN
                      dbo.SignalType ON dbo.Measurement.SignalTypeID = dbo.SignalType.ID ON dbo.Device.ID = dbo.Measurement.DeviceID LEFT OUTER JOIN
                      dbo.Phasor ON dbo.Measurement.DeviceID = dbo.Phasor.DeviceID AND 
                      dbo.Measurement.PhasorSourceIndex = dbo.Phasor.SourceIndex LEFT OUTER JOIN
                      dbo.VendorDevice ON dbo.Device.VendorDeviceID = dbo.VendorDevice.ID LEFT OUTER JOIN
                      dbo.Protocol ON dbo.Device.ProtocolID = dbo.Protocol.ID LEFT OUTER JOIN
                      dbo.Historian ON dbo.Measurement.HistorianID = dbo.Historian.ID


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [RuntimeCalculatedMeasurement]
AS
SELECT     TOP (100) PERCENT dbo.CalculatedMeasurement.NodeID, dbo.Runtime.ID, dbo.CalculatedMeasurement.Acronym AS AdapterName, 
                      LTRIM(RTRIM(dbo.CalculatedMeasurement.AssemblyName)) AS AssemblyName, LTRIM(RTRIM(dbo.CalculatedMeasurement.TypeName)) AS TypeName, CASE WHEN ConfigSection IS NULL 
                      THEN N'' ELSE N'configurationSection=' + ConfigSection + N'; ' END + N'minimumMeasurementsToUse=' + CONVERT(NVARCHAR(10), 
                      dbo.CalculatedMeasurement.MinimumMeasurementsToUse) + N'; framesPerSecond=' + CONVERT(NVARCHAR(10), 
                      dbo.CalculatedMeasurement.FramesPerSecond) + N'; lagTime=' + CONVERT(NVARCHAR(10), dbo.CalculatedMeasurement.LagTime) 
                      + N'; leadTime=' + CONVERT(NVARCHAR(10), dbo.CalculatedMeasurement.LeadTime) + CASE WHEN InputMeasurements IS NULL 
                      THEN N'' ELSE N'; inputMeasurementKeys={' + InputMeasurements + '}' END + CASE WHEN OutputMeasurements IS NULL
                      THEN N'' ELSE N'; outputMeasurements={' + OutputMeasurements + '}' END AS ConnectionString
FROM         dbo.CalculatedMeasurement LEFT OUTER JOIN
                      dbo.Runtime ON dbo.CalculatedMeasurement.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'CalculatedMeasurement'
WHERE     (dbo.CalculatedMeasurement.Enabled <> 0)
ORDER BY dbo.CalculatedMeasurement.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [ActiveMeasurement]
AS
SELECT     dbo.Device.NodeID, dbo.Historian.Acronym + ':' + CONVERT(NVARCHAR(10), dbo.Measurement.PointID) AS ID, dbo.Measurement.SignalID, 
                      dbo.Measurement.PointTag, dbo.Measurement.AlternateTag, dbo.Measurement.SignalReference, dbo.Device.Acronym AS Device, 
                      CASE WHEN dbo.Device.IsConcentrator = 0 AND dbo.Device.ParentID IS NOT NULL THEN RuntimeP.ID ELSE dbo.Runtime.ID END AS DeviceID,
                      COALESCE(dbo.Device.FramesPerSecond, 30) AS FramesPerSecond, dbo.Protocol.Acronym AS Protocol, dbo.SignalType.Acronym AS SignalType, dbo.Phasor.Type AS PhasorType, 
                      dbo.Phasor.Phase, dbo.Measurement.Adder, dbo.Measurement.Multiplier, dbo.Company.Acronym AS Company, dbo.Device.Longitude, 
                      dbo.Device.Latitude, dbo.Measurement.Description
FROM         dbo.Company RIGHT OUTER JOIN
                      dbo.Device ON dbo.Company.ID = dbo.Device.CompanyID RIGHT OUTER JOIN
                      dbo.Measurement LEFT OUTER JOIN
                      dbo.SignalType ON dbo.Measurement.SignalTypeID = dbo.SignalType.ID ON dbo.Device.ID = dbo.Measurement.DeviceID LEFT OUTER JOIN
                      dbo.Phasor ON dbo.Measurement.DeviceID = dbo.Phasor.DeviceID AND 
                      dbo.Measurement.PhasorSourceIndex = dbo.Phasor.SourceIndex LEFT OUTER JOIN
                      dbo.Protocol ON dbo.Device.ProtocolID = dbo.Protocol.ID LEFT OUTER JOIN
                      dbo.Historian ON dbo.Measurement.HistorianID = dbo.Historian.ID LEFT OUTER JOIN
                      dbo.Runtime ON dbo.Device.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'Device' LEFT OUTER JOIN
                      dbo.Runtime AS RuntimeP ON RuntimeP.SourceID = dbo.Device.ParentID AND RuntimeP.SourceTable = N'Device'
WHERE     (dbo.Device.Enabled <> 0 OR dbo.Device.Enabled IS NULL) AND (dbo.Measurement.Enabled <> 0)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [IaonOutputAdapter]
AS
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeHistorian
UNION
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeCustomOutputAdapter

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [IaonInputAdapter]
AS
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeDevice
UNION
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeCustomInputAdapter

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [IaonActionAdapter]
AS
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeOutputStream
UNION
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeCalculatedMeasurement
UNION
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeCustomActionAdapter

GO
CREATE VIEW [HistorianMetadata]
AS
SELECT 
    HistorianID             = PointID,
    DataType                = CASE SignalAcronym WHEN 'DIGI' THEN 1 ELSE 0 END,
    [Name]                  = PointTag,
    Synonym1                = CONVERT(VARCHAR(40), SignalReference),
    Synonym2                = SignalAcronym,
    Synonym3                = AlternateTag,
    Description             = CONVERT(VARCHAR(80), Description),
    HardwareInfo            = VendorDeviceDescription,    
    Remarks                 = '',
    PlantCode               = HistorianAcronym,
    UnitNumber              = 1,
    SystemName              = DeviceAcronym,
    SourceID                = ProtocolID,
    Enabled                 = Enabled,
    ScanRate                = 1.0 / FramesPerSecond,
    CompressionMinTime      = 0,
    CompressionMaxTime      = 0,
    EngineeringUnits        = EngineeringUnits,
    LowWarning              = CASE SignalAcronym WHEN 'FREQ' THEN 59.95 WHEN 'VPHM' THEN 475000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -181 WHEN 'IPHA' THEN -181 ELSE 0 END,
    HighWarning             = CASE SignalAcronym WHEN 'FREQ' THEN 60.05 WHEN 'VPHM' THEN 525000 WHEN 'IPHM' THEN 3150 WHEN 'VPHA' THEN 181 WHEN 'IPHA' THEN 181 ELSE 0 END,
    LowAlarm                = CASE SignalAcronym WHEN 'FREQ' THEN 59.90 WHEN 'VPHM' THEN 450000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -181 WHEN 'IPHA' THEN -181 ELSE 0 END,
    HighAlarm               = CASE SignalAcronym WHEN 'FREQ' THEN 60.10 WHEN 'VPHM' THEN 550000 WHEN 'IPHM' THEN 3300 WHEN 'VPHA' THEN 181 WHEN 'IPHA' THEN 181 ELSE 0 END,
    LowRange                = CASE SignalAcronym WHEN 'FREQ' THEN 59.95 WHEN 'VPHM' THEN 475000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -180 WHEN 'IPHA' THEN -180 ELSE 0 END,
    HighRange               = CASE SignalAcronym WHEN 'FREQ' THEN 60.05 WHEN 'VPHM' THEN 525000 WHEN 'IPHM' THEN 3000 WHEN 'VPHA' THEN 180 WHEN 'IPHA' THEN 180 ELSE 0 END,
    CompressionLimit        = 0.0,
    ExceptionLimit          = 0.0,
    DisplayDigits           = CASE SignalAcronym WHEN 'DIGI' THEN 0 ELSE 7 END,
    SetDescription          = '',
    ClearDescription        = '',
    AlarmState              = 0,
    ChangeSecurity          = 5,
    AccessSecurity          = 0,
    StepCheck               = 0,
    AlarmEnabled            = 0,
    AlarmFlags              = 0,
    AlarmDelay              = 0,
    AlarmToFile             = 0,
    AlarmByEmail            = 0,
    AlarmByPager            = 0,
    AlarmByPhone            = 0,
    AlarmEmails             = MeasurementDetail.ContactList,
    AlarmPagers             = '',
    AlarmPhones             = ''
FROM [dbo].[MeasurementDetail]

GO
CREATE VIEW [dbo].[CalculatedMeasurementDetail] AS
SELECT CM.NodeID, CM.ID, CM.Acronym, ISNULL(CM.Name, '') AS Name, CM.AssemblyName, CM.TypeName, ISNULL(CM.ConnectionString, '') AS ConnectionString,
		ISNULL(CM.ConfigSection, '') AS ConfigSection, ISNULL(CM.InputMeasurements, '') AS InputMeasurements, ISNULL(CM.OutputMeasurements, '') AS OutputMeasurements,
		CM.MinimumMeasurementsToUse, CM.FramesPerSecond, CM.LagTime, CM.LeadTime, CM.UseLocalClockAsRealTime, CM.AllowSortsByArrival, CM.LoadOrder, CM.Enabled,
		N.Name AS NodeName
FROM CalculatedMeasurement CM, Node N
WHERE CM.NodeID = N.ID

GO
CREATE VIEW [dbo].[VendorDeviceDetail]
AS
SELECT     VD.ID, VD.VendorID, VD.Name, ISNULL(VD.Description, '') AS Description, ISNULL(VD.URL, '') AS URL, V.Name AS VendorName, 
                      V.Acronym AS VendorAcronym
FROM         dbo.VendorDevice AS VD INNER JOIN
                      dbo.Vendor AS V ON VD.VendorID = V.ID
GO
CREATE VIEW [dbo].[DeviceDetail]
AS
SELECT     D.NodeID, D.ID, D.ParentID, D.Acronym, ISNULL(D.Name, '') AS Name, D.IsConcentrator, D.CompanyID, D.HistorianID, D.AccessID, D.VendorDeviceID, 
                      D.ProtocolID, D.Longitude, D.Latitude, D.InterconnectionID, ISNULL(D.ConnectionString, '') AS ConnectionString, ISNULL(D.TimeZone, '') AS TimeZone, 
                      D.TimeAdjustmentTicks, D.DataLossInterval, ISNULL(D.ContactList, '') AS ContactList, D.MeasuredLines, D.LoadOrder, D.Enabled, ISNULL(C.Name, '') 
                      AS CompanyName, ISNULL(C.Acronym, '') AS CompanyAcronym, ISNULL(C.MapAcronym, '') AS CompanyMapAcronym, ISNULL(H.Acronym, '') 
                      AS HistorianAcronym, ISNULL(VD.VendorAcronym, '') AS VendorAcronym, ISNULL(VD.Name, '') AS VendorDeviceName, ISNULL(P.Name, '') 
                      AS ProtocolName, ISNULL(I.Name, '') AS InterconnectionName, N.Name AS NodeName, ISNULL(PD.Acronym, '') AS ParentAcronym
FROM         dbo.Device AS D LEFT OUTER JOIN
                      dbo.Company AS C ON C.ID = D.CompanyID LEFT OUTER JOIN
                      dbo.Historian AS H ON H.ID = D.HistorianID LEFT OUTER JOIN
                      dbo.VendorDeviceDetail AS VD ON VD.ID = D.VendorDeviceID LEFT OUTER JOIN
                      dbo.Protocol AS P ON P.ID = D.ProtocolID LEFT OUTER JOIN
                      dbo.Interconnection AS I ON I.ID = D.InterconnectionID LEFT OUTER JOIN
                      dbo.Node AS N ON N.ID = D.NodeID LEFT OUTER JOIN
                      dbo.Device AS PD ON PD.ID = D.ParentID
GO
CREATE VIEW [dbo].[HistorianDetail] AS
SELECT     H.NodeID, H.ID, H.Acronym, ISNULL(H.Name, '') AS Name, ISNULL(H.AssemblyName, '') AS AssemblyName, ISNULL(H.TypeName, '') AS TypeName, 
                      ISNULL(H.ConnectionString, '') AS ConnectionString, H.IsLocal, ISNULL(H.Description, '') AS Description, H.LoadOrder, H.Enabled, 
                      N.Name AS NodeName
FROM         dbo.Historian AS H INNER JOIN
                      dbo.Node AS N ON H.NodeID = N.ID
GO
CREATE VIEW [dbo].[NodeDetail] AS
SELECT N.ID, N.Name, ISNULL(N.CompanyID, 0) AS CompanyID, ISNULL(N.Longitude, 0) AS Longitude, ISNULL(N.Latitude, 0) AS Latitude, 
		ISNULL(N.Description, '') AS Description, ISNULL(N.ImagePath, '') AS ImagePath, N.Master, N.LoadOrder, N.Enabled, ISNULL(C.Name, '') AS CompanyName
FROM Node N, Company C 
WHERE N.CompanyID = C.ID
GO
CREATE VIEW [dbo].[VendorDetail] AS
Select ID, ISNULL(Acronym, '') AS Acronym, Name, ISNULL(PhoneNumber, '') AS PhoneNumber, ISNULL(ContactEmail, '') AS ContactEmail, ISNULL(URL, '') AS URL 
FROM Vendor
GO
CREATE VIEW [dbo].[CustomActionAdapterDetail] AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, ISNULL(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         dbo.CustomActionAdapter AS CA INNER JOIN
                      dbo.Node AS N ON CA.NodeID = N.ID
GO
CREATE VIEW [dbo].[CustomInputAdapterDetail] AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, ISNULL(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         dbo.CustomInputAdapter AS CA INNER JOIN
                      dbo.Node AS N ON CA.NodeID = N.ID
GO
CREATE VIEW [dbo].[CustomOutputAdapterDetail] AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, ISNULL(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         dbo.CustomOutputAdapter AS CA INNER JOIN
                      dbo.Node AS N ON CA.NodeID = N.ID
GO
CREATE VIEW [dbo].[IaonTreeView] AS
SELECT     'Action Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.IaonActionAdapter
UNION ALL
SELECT     'Input Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.IaonInputAdapter
UNION ALL
SELECT     'Output Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.IaonOutputAdapter
GO
CREATE VIEW [dbo].[OtherDeviceDetail] AS
SELECT     OD.ID, OD.Acronym, ISNULL(OD.Name, '') AS Name, OD.IsConcentrator, OD.CompanyID, OD.VendorDeviceID, OD.Longitude, OD.Latitude, 
                      OD.InterconnectionID, OD.Planned, OD.Desired, OD.InProgress, ISNULL(C.Name, '') AS CompanyName, ISNULL(C.Acronym, '') AS CompanyAcronym, 
                      ISNULL(C.MapAcronym, '') AS CompanyMapAcronym, ISNULL(VD.Name, '') AS VendorDeviceName, ISNULL(I.Name, '') AS InterconnectionName
FROM         dbo.OtherDevice AS OD LEFT OUTER JOIN
                      dbo.Company AS C ON OD.CompanyID = C.ID LEFT OUTER JOIN
                      dbo.VendorDevice AS VD ON OD.VendorDeviceID = VD.ID LEFT OUTER JOIN
                      dbo.Interconnection AS I ON OD.InterconnectionID = I.ID
GO
CREATE VIEW [dbo].[MapData] AS
SELECT     'Device' AS DeviceType, NodeID, ID, Acronym, ISNULL(Name, '') AS Name, CompanyMapAcronym, CompanyName, VendorDeviceName, Longitude, 
                      Latitude, CONVERT(BIT, '1') AS Reporting, CONVERT(BIT, '0') AS Inprogress, CONVERT(BIT, '0') AS Planned, CONVERT(BIT, '0') AS Desired
FROM         dbo.DeviceDetail AS D
UNION ALL
SELECT     'OtherDevice' AS DeviceType, NULL AS NodeID, ID, Acronym, ISNULL(Name, '') AS Name, CompanyMapAcronym, CompanyName, VendorDeviceName, 
                      Longitude, Latitude, CONVERT(BIT, '0') AS Reporting, CONVERT(BIT, '1') AS Inprogress, CONVERT(BIT, '1') AS Planned, CONVERT(BIT, '1') 
                      AS Desired
FROM         dbo.OtherDeviceDetail AS OD
GO
CREATE VIEW [dbo].[VendorDeviceDistribution] AS
SELECT dbo.Vendor.Name AS VendorName, COUNT(*) AS DeviceCount 
FROM dbo.Device 
      LEFT OUTER JOIN dbo.VendorDevice ON dbo.Device.VendorDeviceID = dbo.VendorDevice.ID
      INNER JOIN dbo.Vendor ON dbo.VendorDevice.VendorID = dbo.Vendor.ID
      GROUP BY dbo.Vendor.Name
GO
CREATE VIEW [dbo].[OutputStreamDetail] AS
SELECT     OS.NodeID, OS.ID, OS.Acronym, ISNULL(OS.Name, '') AS Name, OS.Type, ISNULL(OS.ConnectionString, '') AS ConnectionString, OS.IDCode, 
                      ISNULL(OS.CommandChannel, '') AS CommandChannel, ISNULL(OS.DataChannel, '') AS DataChannel, OS.AutoPublishConfigFrame, 
                      OS.AutoStartDataChannel, OS.NominalFrequency, OS.FramesPerSecond, OS.LagTime, OS.LeadTime, OS.UseLocalClockAsRealTime, 
                      OS.AllowSortsByArrival, OS.LoadOrder, OS.Enabled, N.Name AS NodeName
FROM         dbo.OutputStream AS OS INNER JOIN
                      dbo.Node AS N ON OS.NodeID = N.ID
GO
CREATE VIEW [dbo].[OutputStreamMeasurementDetail] AS
SELECT     OSM.NodeID, OSM.AdapterID, OSM.ID, OSM.HistorianID, OSM.PointID, OSM.SignalReference, M.PointTag AS SourcePointTag, ISNULL(H.Acronym, '') 
                      AS HistorianAcronym
FROM         dbo.OutputStreamMeasurement AS OSM INNER JOIN
                      dbo.Measurement AS M ON M.PointID = OSM.PointID LEFT OUTER JOIN
                      dbo.Historian AS H ON H.ID = OSM.HistorianID
GO
CREATE VIEW [dbo].[OutputStreamDeviceDetail] AS
SELECT OSD.NodeID, OSD.AdapterID, OSD.ID, OSD.Acronym, ISNULL(OSD.BpaAcronym, '') AS BpaAcronym, OSD.Name, OSD.LoadOrder, OSD.Enabled, 
                    CASE 
                        WHEN EXISTS (Select Acronym From Device Where Acronym = OSD.Acronym) THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1)
                    END AS Virtual
FROM dbo.OutputStreamDevice OSD
GO
CREATE VIEW [dbo].[PhasorDetail] AS
SELECT P.*, ISNULL(DP.Label, '') AS DestinationPhasorLabel, D.Acronym AS DeviceAcronym
FROM Phasor P LEFT OUTER JOIN Phasor DP ON P.DestinationPhasorID = DP.ID
      LEFT OUTER JOIN Device D ON P.DeviceID = D.ID
GO
ALTER TABLE [dbo].[OtherDevice]  WITH CHECK ADD  CONSTRAINT [FK_OtherDevice_Company] FOREIGN KEY([CompanyID])
REFERENCES [Company] ([ID])
GO
ALTER TABLE [dbo].[OtherDevice]  WITH CHECK ADD  CONSTRAINT [FK_OtherDevice_Interconnection] FOREIGN KEY([InterconnectionID])
REFERENCES [Interconnection] ([ID])
GO
ALTER TABLE [dbo].[OtherDevice]  WITH CHECK ADD  CONSTRAINT [FK_OtherDevice_VendorDevice] FOREIGN KEY([VendorDeviceID])
REFERENCES [VendorDevice] ([ID])
GO
ALTER TABLE [dbo].[Node]  WITH CHECK ADD  CONSTRAINT [FK_Node_Company] FOREIGN KEY([CompanyID])
REFERENCES [Company] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Company] FOREIGN KEY([CompanyID])
REFERENCES [Company] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Device] FOREIGN KEY([ParentID])
REFERENCES [Device] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Historian] FOREIGN KEY([HistorianID])
REFERENCES [Historian] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Interconnection] FOREIGN KEY([InterconnectionID])
REFERENCES [Interconnection] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Protocol] FOREIGN KEY([ProtocolID])
REFERENCES [Protocol] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_VendorDevice] FOREIGN KEY([VendorDeviceID])
REFERENCES [VendorDevice] ([ID])
GO
ALTER TABLE [dbo].[VendorDevice]  WITH CHECK ADD  CONSTRAINT [FK_VendorDevice_Vendor] FOREIGN KEY([VendorID])
REFERENCES [Vendor] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamDeviceDigital]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamDeviceDigital_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamDeviceDigital]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamDeviceDigital_OutputStreamDevice] FOREIGN KEY([OutputStreamDeviceID])
REFERENCES [OutputStreamDevice] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OutputStreamDevicePhasor]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamDevicePhasor_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamDevicePhasor]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamDevicePhasor_OutputStreamDevice] FOREIGN KEY([OutputStreamDeviceID])
REFERENCES [OutputStreamDevice] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OutputStreamDeviceAnalog]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamDeviceAnalog_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamDeviceAnalog]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamDeviceAnalog_OutputStreamDevice] FOREIGN KEY([OutputStreamDeviceID])
REFERENCES [OutputStreamDevice] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Measurement]  WITH CHECK ADD  CONSTRAINT [FK_Measurement_Device] FOREIGN KEY([DeviceID])
REFERENCES [Device] ([ID])
GO
ALTER TABLE [dbo].[Measurement]  WITH CHECK ADD  CONSTRAINT [FK_Measurement_Historian] FOREIGN KEY([HistorianID])
REFERENCES [Historian] ([ID])
GO
ALTER TABLE [dbo].[Measurement]  WITH CHECK ADD  CONSTRAINT [FK_Measurement_SignalType] FOREIGN KEY([SignalTypeID])
REFERENCES [SignalType] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamMeasurement]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamMeasurement_Historian] FOREIGN KEY([HistorianID])
REFERENCES [Historian] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamMeasurement]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamMeasurement_Measurement] FOREIGN KEY([PointID])
REFERENCES [Measurement] ([PointID])
GO
ALTER TABLE [dbo].[OutputStreamMeasurement]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamMeasurement_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamMeasurement]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamMeasurement_OutputStream] FOREIGN KEY([AdapterID])
REFERENCES [OutputStream] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamDevice]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamDevice_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[OutputStreamDevice]  WITH CHECK ADD  CONSTRAINT [FK_OutputStreamDevice_OutputStream] FOREIGN KEY([AdapterID])
REFERENCES [OutputStream] ([ID])
GO
ALTER TABLE [dbo].[Phasor]  WITH CHECK ADD  CONSTRAINT [FK_Phasor_Device] FOREIGN KEY([DeviceID])
REFERENCES [Device] ([ID])
GO
ALTER TABLE [dbo].[Phasor]  WITH CHECK ADD  CONSTRAINT [FK_Phasor_Phasor] FOREIGN KEY([DestinationPhasorID])
REFERENCES [Phasor] ([ID])
GO
ALTER TABLE [dbo].[CalculatedMeasurement]  WITH CHECK ADD  CONSTRAINT [FK_CalculatedMeasurement_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[CustomActionAdapter]  WITH CHECK ADD  CONSTRAINT [FK_CustomActionAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[Historian]  WITH CHECK ADD  CONSTRAINT [FK_Historian_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[CustomInputAdapter]  WITH CHECK ADD  CONSTRAINT [FK_CustomInputAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[OutputStream]  WITH CHECK ADD  CONSTRAINT [FK_OutputStream_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO
ALTER TABLE [dbo].[CustomOutputAdapter]  WITH CHECK ADD  CONSTRAINT [FK_CustomOutputAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])

/*
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [GetFormattedMeasurements]
	@measurementSql NVARCHAR(max),
	@includeAdjustments BIT,
	@measurements NVARCHAR(max) OUTPUT
AS
	-- Fill the table variable with the rows for your result set
	DECLARE @measurementID INT
	DECLARE @archiveSource NVARCHAR(50)
	DECLARE @adder FLOAT
	DECLARE @multiplier FLOAT

	SET @measurements = ''

	CREATE TABLE #temp
	(
		[MeasurementID] INT,
		[ArchiveSource] NVARCHAR(50),
		[Adder] FLOAT,
		[Multiplier] FLOAT
	)

	INSERT INTO #temp EXEC sp_executesql @measurementSql

	DECLARE SelectedMeasurements CURSOR LOCAL FAST_FORWARD FOR SELECT * FROM #temp
	OPEN SelectedMeasurements

	-- Get first row from measurements SQL
	FETCH NEXT FROM SelectedMeasurements INTO @measurementID, @archiveSource, @adder, @multiplier

	-- Step through selected measurements
	WHILE @@FETCH_STATUS = 0
	BEGIN		
		IF LEN(@measurements) > 0
			SET @measurements = @measurements + ';'

		IF @includeAdjustments <> 0 AND (@adder <> 0.0 OR @multiplier <> 1.0)
			SET @measurements = @measurements + @archiveSource + ':' + @measurementID + ',' + @adder + ',' + @multiplier
		ELSE
			SET @measurements = @measurements + @archiveSource + ':' + @measurementID
		
		-- Get next row from measurements SQL
		FETCH NEXT FROM SelectedMeasurements INTO @measurementID, @archiveSource, @adder, @multiplier
	END

	CLOSE SelectedMeasurements
	DEALLOCATE SelectedMeasurements

	DROP TABLE #temp

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [FormatMeasurements] (@measurementSql NVARCHAR(max), @includeAdjustments BIT)
RETURNS NVARCHAR(max) 
AS
BEGIN
    DECLARE @measurements NVARCHAR(max) 

	SET @measurements = ''

	EXEC GetFormattedMeasurements @measurementSql, @includeAdjustments, @measurements

	IF LEN(@measurements) > 0
		SET @measurements = '{' + @measurements + '}'
	ELSE
		SET @measurements = NULL
		
	RETURN @measurements
END

GO
*/
