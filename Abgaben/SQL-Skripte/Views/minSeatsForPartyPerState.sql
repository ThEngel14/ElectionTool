USE [ElectionDB]
GO

/****** Object:  View [dbo].[DistributionWithinStates]    Script Date: 03.01.2016 13:26:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER View [dbo].[DistributionWithinStates] as
--stored as "MinSeatsForPartyPerState.sql
--output max(seats gained by zweitstimme, seats gained by erststimme

select distinct
	z.Election_Id as Election_Id, 
	z.Bundesland_Id as Bundesland_Id,
	iif(coalesce(e.NumberOfVictories,0) > coalesce(z.Seats,0), e.NumberOfVictories, z.seats ) as Seats,
	z.Party_Id as Party_Id
 from seatsGainedByZweitstimme z 
		full join  SeatsGainedByErststimme e
			on e.Election_Id=z.Election_Id 
			and e.Bundesland_Id=z.Bundesland_Id
			and e.Party_Id = z.Party_Id
 
GO


