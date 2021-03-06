USE [bishe]
GO
/****** Object:  Table [dbo].[users]    Script Date: 05/27/2014 21:28:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[userID] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[password] [nvarchar](50) NOT NULL,
	[isdel] [int] NOT NULL,
	[ismanager] [nvarchar](50) NULL,
	[sex] [nvarchar](50) NULL,
	[telphone] [nvarchar](50) NULL,
	[email] [nvarchar](50) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[userID] ASC,
	[username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[info]    Script Date: 05/27/2014 21:28:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[info](
	[infoID] [int] IDENTITY(1,1) NOT NULL,
	[voltage] [nvarchar](50) NULL,
	[temperature] [nvarchar](50) NULL,
	[light] [nvarchar](50) NULL,
	[time] [datetime] NULL,
	[IP] [nvarchar](50) NULL,
	[username] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_info] PRIMARY KEY CLUSTERED 
(
	[infoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Default [DF_users_password]    Script Date: 05/27/2014 21:28:59 ******/
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_password]  DEFAULT ((123456)) FOR [password]
GO
/****** Object:  Default [DF_user_isdel]    Script Date: 05/27/2014 21:28:59 ******/
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_user_isdel]  DEFAULT ((0)) FOR [isdel]
GO
/****** Object:  ForeignKey [FK_users_user]    Script Date: 05/27/2014 21:28:59 ******/
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_user] FOREIGN KEY([userID], [username])
REFERENCES [dbo].[users] ([userID], [username])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_user]
GO
