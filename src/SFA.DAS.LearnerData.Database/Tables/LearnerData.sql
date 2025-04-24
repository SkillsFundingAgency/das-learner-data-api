CREATE TABLE [dbo].[LearnerData](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [ULN] [bigint] NOT NULL,
    [UKPRN] [bigint] NOT NULL,
    [Firstname] [nvarchar](200) NOT NULL,
    [Lastname] [nvarchar](200) NOT NULL,
    [Email] [nvarchar](max) NULL,
    [Dob] [date] NOT NULL,
    [AcademicYear] [int] NOT NULL,
    [StartDate] [date] NOT NULL,
    [PlannedEndDate] [date] NULL,
    [PercentageLearningToBeDelivered] [int] NOT NULL,
    [EpaoPrice] [int] NOT NULL,
    [TrainingPrice] [int] NOT NULL,
    [AgreementId] [nvarchar](max) NOT NULL,
    [StandardCode] [int] NOT NULL,
    [IsFlexiJob] [bit] NOT NULL,
    [PlannedOTJTrainingHours] [int] NOT NULL,
    [CreatedDate] [datetime] NOT NULL,
    [UpdatedDate] [datetime] NULL,
    [ReceivedDate] [date] NOT NULL,
    [CorrelationId] [nvarchar](max) NOT NULL,
    [ConsumerReference] [nvarchar](max) NULL,
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
	[AcademicYear] ASC
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
       
CREATE NONCLUSTERED INDEX [IX_LearnerData_AcademicYear] ON [dbo].[LearnerData]
(
	[AcademicYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date the record was received in AS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LearnerData', @level2type=N'COLUMN',@level2name=N'ReceivedDate'
GO