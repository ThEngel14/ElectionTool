USE [ElectionDB]
GO

/****** Object:  View [dbo].[DistributionWithinStates]    Script Date: 21.12.2015 16:02:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO









ALTER View [dbo].[DistributionWithinStates] as
--stored as "MinSeatsForPartyPerState.sql


with Both as (
--output max(seats gained by zweitstimme, seats gained by erststimme
select distinct
	z.Election_Id as Election_Id, 
	z.Bundesland_Id as Bundesland_Id,
	iif(e.NumberOfVictories > z.Seats, e.NumberOfVictories, z.seats ) as Seats ,
	z.Party_Id as Party_Id
 from seatsGainedByZweitstimme z, SeatsGainedByErststimme e
 where e.Election_Id=z.Election_Id 
 and e.Bundesland_Id=z.Bundesland_Id
 and e.Party_Id = z.Party_Id
 ),

 --same as above, but for parties that did not gain any second votes and are therefore not handle in Both-Table
 OnlyErststimme as (
 select 
	e.Election_Id as Election_Id, 
	e.Bundesland_Id as Bundesland_Id,
	e.numberOfVictories as Seats,
	e.Party_Id as Party_Id 
 from seatsGainedByErststimme e, Both b
 where e.Election_Id = b.Election_Id
 and e.Bundesland_Id = b.Bundesland_Id
 and e.Party_Id not in (select b_.Party_Id from Both b_ 
					 where e.Election_Id = b_.Election_Id
					 and e.Bundesland_Id = b_.Bundesland_Id)
 ),

 OnlyZweitstimme as (
 select 
	z.Election_Id as Election_Id, 
	z.Bundesland_Id as Bundesland_Id,
	z.seats as Seats,
	z.Party_Id as Party_Id 
 from seatsGainedByZweitstimme z, Both b
 where z.Election_Id = b.Election_Id
 and z.Bundesland_Id = b.Bundesland_Id
 and z.Party_Id not in (select b_.Party_Id from Both b_ 
					 where z.Election_Id = b_.Election_Id
					 and z.Bundesland_Id = b_.Bundesland_Id)
 
 )
 

 select * from Both b

 union select * from OnlyErststimme oe

 union select * from OnlyZweitstimme oz











GO


