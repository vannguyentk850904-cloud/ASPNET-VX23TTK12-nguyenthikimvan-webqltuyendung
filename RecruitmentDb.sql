
USE [master];
GO

IF DB_ID('RecruitmentDb') IS NULL
BEGIN
    CREATE DATABASE RecruitmentDb;
END
GO

USE RecruitmentDb;
GO

-- XÓA BẢNG CŨ (NẾU ĐANG TỒN TẠI)
IF OBJECT_ID('[dbo].[Applications]') IS NOT NULL DROP TABLE [dbo].[Applications];
IF OBJECT_ID('[dbo].[JobPosts]') IS NOT NULL DROP TABLE [dbo].[JobPosts];
IF OBJECT_ID('[dbo].[Users]') IS NOT NULL DROP TABLE [dbo].[Users];
GO

----------------------------------------------------
-- TẠO BẢNG USERS
----------------------------------------------------
CREATE TABLE [dbo].[Users](
    [UserId] [int] IDENTITY(1,1) NOT NULL,
    [FullName] [nvarchar](100) NOT NULL,
    [Email] [nvarchar](100) NOT NULL,
    [PasswordHash] [nvarchar](200) NOT NULL,
    [Role] [nvarchar](20) NOT NULL,
    [CreatedAt] [datetime] NULL DEFAULT GETDATE(),
    [IsApproved] [bit] NOT NULL DEFAULT 0,
PRIMARY KEY CLUSTERED ([UserId] ASC),
UNIQUE ([Email])
);
GO

ALTER TABLE [dbo].[Users] WITH CHECK 
ADD CHECK ([Role] IN ('Admin','Employer','Candidate'));
GO

----------------------------------------------------
-- TẠO BẢNG JOBPOSTS
----------------------------------------------------
CREATE TABLE [dbo].[JobPosts](
    [JobId] [int] IDENTITY(1,1) NOT NULL,
    [EmployerId] [int] NOT NULL,
    [Title] [nvarchar](200) NOT NULL,
    [Description] [nvarchar](max) NULL,
    [Location] [nvarchar](100) NULL,
    [Salary] [decimal](18, 2) NULL,
    [CreatedAt] [datetime] NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    [ExpiryDate] [datetime] NULL,
PRIMARY KEY CLUSTERED ([JobId] ASC)
);
GO

ALTER TABLE [dbo].[JobPosts] 
ADD FOREIGN KEY([EmployerId]) REFERENCES [dbo].[Users]([UserId]);
GO

----------------------------------------------------
-- TẠO BẢNG APPLICATIONS
----------------------------------------------------
CREATE TABLE [dbo].[Applications](
    [ApplicationId] [int] IDENTITY(1,1) NOT NULL,
    [JobId] [int] NOT NULL,
    [CandidateId] [int] NOT NULL,
    [CoverLetter] [nvarchar](max),
    [AppliedAt] [datetime] NULL DEFAULT GETDATE(),
    [CvFilePath] [nvarchar](255) NULL,
    [Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED ([ApplicationId] ASC)
);
GO

ALTER TABLE [dbo].[Applications]
ADD CONSTRAINT FK_Applications_JobPosts FOREIGN KEY([JobId])
REFERENCES [dbo].[JobPosts] ([JobId]);

ALTER TABLE [dbo].[Applications]
ADD CONSTRAINT FK_Applications_Users FOREIGN KEY([CandidateId])
REFERENCES [dbo].[Users] ([UserId]);
GO

----------------------------------------------------
-- INSERT USERS
----------------------------------------------------
INSERT INTO Users (FullName, Email, PasswordHash, Role, IsApproved)
VALUES 
(N'Quản trị viên', 'admin@gmail.com', 'admin123', 'Admin', 1),
(N'Nhà tuyển dụng 1', 'nhatuyendung1@gmail.com', '123456', 'Employer', 1),
(N'Nhà tuyển dụng 2', 'nhatuyendung2@gmail.com', '123456', 'Employer', 1),
(N'Ứng viên 1', 'ungvien1@gmail.com', '123456', 'Candidate', 1),
(N'Ứng viên 2', 'ungvien2@gmail.com', '123456', 'Candidate', 1);
GO

----------------------------------------------------
-- INSERT JOB POSTS
----------------------------------------------------
INSERT INTO JobPosts (EmployerId, Title, Description, Location, Salary, ExpiryDate)
VALUES
(2, N'Lập trình viên .NET', N'Phát triển và bảo trì dự án ASP.NET MVC.', N'Hà Nội', 15000000, DATEADD(day, 30, GETDATE())),
(2, N'Chuyên viên Marketing', N'Lên kế hoạch và thực thi chiến dịch marketing.', N'Hồ Chí Minh', 12000000, DATEADD(day, 45, GETDATE())),
(3, N'Nhân viên Kinh Doanh', N'Tìm kiếm và chăm sóc khách hàng.', N'Hà Nội', 10000000, DATEADD(day, 20, GETDATE())),
(3, N'Tester phần mềm', N'Kiểm thử hệ thống web, mobile.', N'Đà Nẵng', 14000000, DATEADD(day, 25, GETDATE())),
(2, N'UI/UX Designer', N'Thiết kế giao diện website và ứng dụng.', N'Hồ Chí Minh', 16000000, DATEADD(day, 30, GETDATE())),
(3, N'Lập trình viên Frontend', N'ReactJS, HTML/CSS, UI performance.', N'Cần Thơ', 15000000, DATEADD(day, 28, GETDATE())),
(3, N'Kinh doanh phần mềm', N'Mở rộng thị trường sản phẩm công nghệ.', N'Hà Nội', 10000000, DATEADD(day, 20, GETDATE()));

GO

----------------------------------------------------
-- INSERT APPLICATIONS
----------------------------------------------------
INSERT INTO Applications (JobId, CandidateId, CoverLetter, Status)
VALUES
(1, 4, N'Em xin ứng tuyển vị trí này vì phù hợp kinh nghiệm!', N'Đang xem xét'),
(2, 5, N'Quan tâm vị trí Marketing và muốn thử sức.', N'Đang xem xét'),
(3, 4, N'Em có kinh nghiệm sale trên 1 năm.', N'Đang xem xét'),
(4, 5, N'Mong muốn cơ hội làm việc lâu dài.', N'Đang xem xét');
GO
