USE [ElectionDB]
GO

/****** Object:  View [dbo].[Wahlkreissieger]    Script Date: 17.11.2015 16:06:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER view [dbo].[Wahlkreissieger] as
--actually computed from Erststimme, But a table ERststimmeAmount is created seperatly for performance reasons (no looping here to count votes) 


	--output: Election, Winnerperson, district, partyAffiliation in this election <-- can be null!
select votes.Election_Id as ElectionID, votes.Person_Id as Person_Id, votes.Wahlkreis_Id as Wahlkreis_Id, (select party.Party_Id 
																						from PartyAffiliation party 
																						where party.Person_Id=votes.Person_Id 
																						and party.Election_Id=votes.Election_Id)
																						as MemberOfParty
from ErststimmeAmount votes 
 where votes.Amount = (select max(alle.Amount)
						from ErststimmeAmount alle
						where votes.Election_Id = alle.Election_Id and 
						votes.Wahlkreis_Id = alle.Wahlkreis_Id)
 


 

GO


