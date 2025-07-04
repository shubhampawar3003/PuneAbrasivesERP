USE [master]
GO

/****** Object:  Database [DB_WLSPLCRM]    Script Date: 6/26/2025 11:43:11 AM ******/
CREATE DATABASE [DB_WLSPLCRM]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DB_WLSPLCRM', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\DB_WLSPLCRM_5731edba2be243c98df3281a482db653.mdf' , SIZE = 25664KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DB_WLSPLCRM_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\DB_WLSPLCRM_862831e1ad86474196a493d8703af1dc.ldf' , SIZE = 7616KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DB_WLSPLCRM].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [DB_WLSPLCRM] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET ARITHABORT OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET AUTO_CLOSE ON 
GO

ALTER DATABASE [DB_WLSPLCRM] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [DB_WLSPLCRM] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [DB_WLSPLCRM] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET  DISABLE_BROKER 
GO

ALTER DATABASE [DB_WLSPLCRM] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [DB_WLSPLCRM] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [DB_WLSPLCRM] SET  MULTI_USER 
GO

ALTER DATABASE [DB_WLSPLCRM] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [DB_WLSPLCRM] SET DB_CHAINING OFF 
GO

ALTER DATABASE [DB_WLSPLCRM] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [DB_WLSPLCRM] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [DB_WLSPLCRM] SET  READ_WRITE 
GO


