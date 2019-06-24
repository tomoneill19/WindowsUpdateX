CREATE TABLE [dbo].[Application](
	[UID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Version] [varchar](100) NULL,
	[Description] [varchar](500) NULL,
	[InstallDate] [datetime] NULL,
 CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Config]    Script Date: 10/03/2018 15:28:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Config](
	[SmtpHost] [nvarchar](100) NULL,
	[SmtpPort] [int] NULL,
	[FromEmailAddress] [nvarchar](500) NULL,
	[FromEmailDisplayName] [nvarchar](100) NULL,
	[FromPassword] [nvarchar](100) NULL,
	[DefaultSendEmail] [nvarchar](500) NULL,
	[IsValidated] [bit] NULL,
	[AlertsForErrors] [bit] NULL,
	[AlertsForWarnings] [bit] NULL,
	[DaysEitherSideUpdateToDisplay] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Hardware]    Script Date: 10/03/2018 15:28:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hardware](
	[UID] [bigint] IDENTITY(1,1) NOT NULL,
	[TotalRam] [real] NULL,
	[AvailableRam] [real] NULL,
	[UsedCPU] [real] NULL,
	[AvailableDisk] [real] NULL,
	[MaxDisk] [real] NULL,
	[DiskRead] [float] NULL,
	[DiskWrite] [float] NULL,
	[LogDate] [datetime] NULL,
 CONSTRAINT [PK_Hardware] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Insight]    Script Date: 10/03/2018 15:28:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Insight](
	[UID] [bigint] IDENTITY(1,1) NOT NULL,
	[Insight] [nvarchar](500) NULL,
	[DateLogged] [datetime] NULL,
 CONSTRAINT [PK_Insight] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MessageLog]    Script Date: 10/03/2018 15:28:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageLog](
	[UID] [bigint] IDENTITY(1,1) NOT NULL,
	[DateLogged] [datetime] NULL,
	[ErrorNumber] [int] NULL,
	[ErrorSeverity] [int] NULL,
	[ErrorState] [int] NULL,
	[ErrorProcedure] [nvarchar](128) NULL,
	[ErrorLine] [int] NULL,
	[ErrorMessage] [varchar](8000) NULL,
	[OurMessage] [varchar](8000) NULL,
	[Severity] [varchar](50) NULL,
	[Caller] [varchar](100) NULL,
	[CallerFilePath] [varchar](1000) NULL,
	[CallerLineNumber] [varchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SummaryAnalysis]    Script Date: 10/03/2018 15:28:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SummaryAnalysis](
	[LogDate] [date] NULL,
	[UsedCPU] [float] NULL,
	[UsedRam] [float] NULL,
	[CPU_Run_Avg] [float] NULL,
	[RAM_Run_Avg] [float] NULL,
	[PossibleIssue] [bit] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Update]    Script Date: 10/03/2018 15:28:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Update](
	[UID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) NOT NULL,
	[KBID] [varchar](500) NULL,
	[Description] [varchar](4000) NULL,
	[isDownloaded] [bit] NULL,
	[isInstalled] [bit] NULL,
	[url] [varchar](500) NULL,
	[neededDiskSpace] [varchar](50) NULL,
	[DateTimeCreated] [datetime] NULL,
	[DateTimeDownloaded] [datetime] NULL,
	[DateTimeInstalled] [datetime] NULL,
	[DateTimeRemoved] [datetime] NULL,
	[IssueFlag] [bit] NULL,
	[Identity] [varchar](500) NULL
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[Config] ADD  CONSTRAINT [DF_Config_IsValidated]  DEFAULT ((0)) FOR [IsValidated]
GO
ALTER TABLE [dbo].[Config] ADD  CONSTRAINT [DF_Config_AlertsForErrors]  DEFAULT ((1)) FOR [AlertsForErrors]
GO
ALTER TABLE [dbo].[Config] ADD  CONSTRAINT [DF_Config_AlertsForWarnings]  DEFAULT ((1)) FOR [AlertsForWarnings]
GO
ALTER TABLE [dbo].[Config] ADD  CONSTRAINT [DF_Config_DaysEitherSideUpdateToDisplay]  DEFAULT ((7)) FOR [DaysEitherSideUpdateToDisplay]
GO
ALTER TABLE [dbo].[Hardware] ADD  CONSTRAINT [DF_Hardware_LogDate]  DEFAULT (getdate()) FOR [LogDate]
GO
ALTER TABLE [dbo].[Insight] ADD  CONSTRAINT [DF_Insight_DateLogged]  DEFAULT (getdate()) FOR [DateLogged]
GO
ALTER TABLE [dbo].[MessageLog] ADD  CONSTRAINT [DF_REF_MESSAGE_LOG_DateLogged]  DEFAULT (getdate()) FOR [DateLogged]
GO
ALTER TABLE [dbo].[SummaryAnalysis] ADD  CONSTRAINT [DF_SummaryAnalysis_PossibleIssue]  DEFAULT ((0)) FOR [PossibleIssue]
GO
ALTER TABLE [dbo].[Update] ADD  CONSTRAINT [DF_Update_DateTimeCreated]  DEFAULT (getdate()) FOR [DateTimeCreated]
GO
/****** Object:  StoredProcedure [dbo].[DoAnalysis]    Script Date: 10/03/2018 15:28:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DoAnalysis]
AS
BEGIN

	SET NOCOUNT ON;

	Truncate Table SummaryAnalysis;

	Declare @DaysAveragePeriod as int;
	Declare @DaysBloack as int;
	Declare @IssueTolerence as int;


	Set @DaysAveragePeriod = 30;   -- use a month to average
	Set @DaysBloack = 5;
	Set @IssueTolerence = 2;   -- 2 bad days on the run

	-- Create a daily summary for a month
	insert into SummaryAnalysis (LogDate, UsedCPU, UsedRam)
	Select Cast([LogDate] as Date) As LogDate, Avg(isnull(UsedCPU,0)) As UsedCPU, Avg((isnull(TotalRam,0)-isnull(AvailableRam,0))) As UsedRam 
	from [dbo].[Hardware] H
	Where UsedCPU > 0 And (TotalRam-AvailableRam) > 0
	and datediff(dd, H.LogDate, GetDate()) <= (@DaysAveragePeriod + @DaysBloack)    -- we need another 5 days of summary so the first record has some data
	Group By Cast([LogDate] as Date)

	-- Create a running 5 day smoothed out average
	update samain
	set [CPU_Run_Avg] = AVGDATA.[CPU_Run_Avg], [RAM_Run_Avg] = AVGDATA.[RAM_Run_Avg]
	from [dbo].[SummaryAnalysis] samain
	inner join
	(
	Select sa.LogDate, avg(isnull(sad.UsedCPU,0)) As [CPU_Run_Avg], avg(isnull(sad.UsedRam,0)) As [RAM_Run_Avg]
	from SummaryAnalysis sa
	cross join SummaryAnalysis sad   -- (sa detail)
	where sad.LogDate Between sa.LogDate And dateadd(dd, -@DaysBloack, sa.LogDate)
	Group by sa.LogDate) AVGDATA
	On samain.LogDate = AVGDATA.LogDate

	-- What is the std for the whole period
	Declare @SIGMACPU as float;
	Set @SIGMACPU = (select stdev(isnull([UsedCPU],0)) from [dbo].[SummaryAnalysis])

	Declare @SIGMARAM as float;
	Set @SIGMARAM = (select stdev(isnull([UsedRAM],0)) from [dbo].[SummaryAnalysis])

	Declare @AVGCPU as float;
	Set @AVGCPU = (select avg(isnull([UsedCPU],0)) from [dbo].[SummaryAnalysis])

	Declare @AVGRAM as float;
	Set @AVGRAM = (select avg(isnull([UsedRAM],0)) from [dbo].[SummaryAnalysis])

	-- Look at the last block of data to see if it is above the average for the whole period and flag them
	Update samain
	Set [PossibleIssue] = 'true'
	from [dbo].[SummaryAnalysis] samain
	Inner Join
	(
	Select [LogDate], [CPU_Run_Avg], [RAM_Run_Avg]
	from [dbo].[SummaryAnalysis] sa
	Where datediff(dd, LogDate, GetDate()) <=  @DaysBloack
	and (abs([RAM_Run_Avg]-@AVGRAM) > @SIGMARAM Or abs([CPU_Run_Avg]-@AVGCPU) > @SIGMACPU)) AVGDATA
	On samain.LogDate = AVGDATA.LogDate

	Declare @NumIssues as int;
	Set @NumIssues = (Select count(*) from [dbo].[SummaryAnalysis] where [PossibleIssue] = 'true')


	if (@NumIssues >= @IssueTolerence)
	Begin
		-- Flag the latest update as having possible issue
		Update up
		Set [IssueFlag] = 'true'
		from [dbo].[Update] up
		Inner join
		(Select max(UID) As MAXUID from [dbo].[Update] where [isInstalled] = 'true') MAXUP
		On up.UID = MAXUP.MAXUID

		Insert into Insight([Insight]) values('Possible issue identified analysis and flagged against the latesy Update')

		Exec [dbo].[LogMessage] 'Possible issue identified analysis and flagged against the latesy Update', 'WARNING', '', '', 0
	End

	
END

GO
/****** Object:  StoredProcedure [dbo].[LogMessage]    Script Date: 10/03/2018 15:28:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LogMessage]
(@OurMessage As varchar(8000), @Severity As varchar(50), @Caller As varchar(100), @CallerFilePath As varchar(1000), @CallerLineNumbe As varchar(50))
AS
BEGIN
   SET NOCOUNT ON;
   Declare @ENABLE_WARNING As varchar(1)
   Declare @ENABLE_DEBUG As varchar(1)
   Set @ENABLE_WARNING = 'Y'
   Set @ENABLE_DEBUG = 'Y'
   If @Severity = 'ERROR'
   Begin
       Insert into dbo.MessageLog (DateLogged, ErrorNumber, ErrorSeverity, ErrorState, ErrorProcedure, ErrorLine, ErrorMessage, OurMessage, Severity, [Caller], CallerFilePath, CallerLineNumber)
       SELECT
       GetDate() As DateLogged,
       ERROR_NUMBER() AS ErrorNumber,
       ERROR_SEVERITY() AS ErrorSeverity,
       ERROR_STATE() AS ErrorState,
       ERROR_PROCEDURE() AS ErrorProcedure,
       ERROR_LINE() AS ErrorLine,
       ERROR_MESSAGE() AS ErrorMessage,
       @OurMessage + ' ...' As OurMessage,
       @Severity As Severity,
	   @Caller As Caller,
	   @CallerFilePath As CallerFilePath,
	   @CallerLineNumbe As CallerLineNumber
   End
   If @Severity = 'WARNING' And @ENABLE_WARNING = 'Y'
   Begin
       Insert into dbo.MessageLog (DateLogged, ErrorNumber, ErrorSeverity, ErrorState, ErrorProcedure, ErrorLine, ErrorMessage, OurMessage, Severity, [Caller], CallerFilePath, CallerLineNumber)
       SELECT
       GetDate() As DateLogged,
       0 AS ErrorNumber,
       0 AS ErrorSeverity,
       0 AS ErrorState,
       '' AS ErrorProcedure,
       0 AS ErrorLine,
       '' AS ErrorMessage,
       @OurMessage + ' ...' As OurMessage,
       @Severity As Severity,
	   @Caller As Caller,
	   @CallerFilePath As CallerFilePath,
	   @CallerLineNumbe As CallerLineNumber
   End
   If @Severity = 'DEBUG' And @ENABLE_DEBUG = 'Y'
   Begin
       Insert into dbo.MessageLog (DateLogged, ErrorNumber, ErrorSeverity, ErrorState, ErrorProcedure, ErrorLine, ErrorMessage, OurMessage, Severity, [Caller], CallerFilePath, CallerLineNumber)
       SELECT
       GetDate() As DateLogged,
       0 AS ErrorNumber,
       0 AS ErrorSeverity,
       0 AS ErrorState,
       '' AS ErrorProcedure,
       0 AS ErrorLine,
       '' AS ErrorMessage,
       @OurMessage + ' ...' As OurMessage,
       @Severity As Severity,
	   @Caller As Caller,
	   @CallerFilePath As CallerFilePath,
	   @CallerLineNumbe As CallerLineNumber
   End
END

INSERT INTO Config (SmtpHost, SmtpPort, FromEmailAddress, FromEmailDisplayName, FromPassword, DefaultSendEmail, IsValidated, AlertsForErrors, AlertsForWarnings, DaysEitherSideUpdateToDisplay)
VALUES ('<NOT SET>',0,'<NOT SET>','<NOT SET>','<NOT SET>','<NOT SET>',0,0,0,7)

-- Now the permissions
EXEC sp_grantlogin N'NT AUTHORITY\NETWORK SERVICE'
GO
exec sp_grantdbaccess N'NT AUTHORITY\NETWORK SERVICE'
GO





