USE [ElectionDB]
GO

/****** Object:  View [dbo].[ElectionParticipation]    Script Date: 03.01.2016 13:32:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[ElectionParticipation] as

Select p.Election_Id, p.Wahlkreis_Id, 1.0*p.Amount / a.Amount as Participation
from ParticipationAmount p 
	join AllowedToElectAmount a 
		on p.Election_Id = a.Election_Id 
		and p.Wahlkreis_Id = a.Wahlkreis_Id
GO


