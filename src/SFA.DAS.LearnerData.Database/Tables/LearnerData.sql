CREATE TABLE [dbo].[LearnerData](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [ULN] [bigint] NOT NULL,
    [UKPRN] [bigint] NOT NULL,
    [Firstname] [nvarchar](100) NOT NULL,
    [Lastname] [nvarchar](100) NOT NULL,
    [Email] [nvarchar](200) NULL,
    [Dob] [date] NOT NULL,
    [AcademicYear] [int] NOT NULL,
    [StartDate] [date] NOT NULL,
    [PlannedEndDate] [date] NOT NULL,
    [PercentageLearningToBeDelivered] [int] NULL,
    [EpaoPrice] [int] NOT NULL,
    [TrainingPrice] [int] NOT NULL,
    [AgreementId] [nvarchar](20) NULL,
    [TrainingCode] [nvarchar](20) NULL,
    [TrainingName] [nvarchar](126) NULL,
    [LearningType] [tinyint] NULL,
    [IsFlexiJob] [bit] NOT NULL,
    [PlannedOTJTrainingHours] [int] NOT NULL,
    [CreatedDate] [datetime] NOT NULL,
    [UpdatedDate] [datetime] NULL,
    [ReceivedDate] DATETIME NOT NULL,
    [CorrelationId] UNIQUEIDENTIFIER NOT NULL,
    [ConsumerReference] [nvarchar](100) NULL,
    [ApprenticeshipId] BIGINT NULL, 
    CONSTRAINT [PK_LearnerData] PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    GO

CREATE NONCLUSTERED INDEX [idx_learnerData] ON [dbo].[LearnerData]
(
	[ULN] ASC,
	[UKPRN] ASC,
	[Firstname] ASC,
	[Lastname] ASC,
	[StartDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [idx_learnerData_0] ON [dbo].[LearnerData]
(
	[UKPRN] ASC,
	[ReceivedDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
       
CREATE NONCLUSTERED INDEX [IX_LearnerData_StartDate] ON [dbo].[LearnerData]
(
	[StartDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
GO

CREATE NONCLUSTERED INDEX [IX_LearnerData_NaturalKey_Simplified] ON [dbo].[LearnerData]
(
	[UKPRN] ASC,
	[ULN] ASC,
	[ApprenticeshipId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_LearnerData_ApprenticeshipId] ON [dbo].[LearnerData] 
(
    ApprenticeshipId
)
WHERE ApprenticeshipId IS NOT NULL;
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date the record was received in AS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LearnerData', @level2type=N'COLUMN',@level2name=N'ReceivedDate'
GO