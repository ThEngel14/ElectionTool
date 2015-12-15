USE [ElectionDB]
GO

/****** Object:  View [dbo].[Parties5]    Script Date: 13.11.2015 11:11:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER view [dbo].[Parties5]  as

(
select votes.Election_Id, p.Id  
from Party p, ZweitstimmeAmount votes
where 1.00*(select sum(votes1.Amount) as PartyVotes
		from  ZweitstimmeAmount votes1
		where votes1.Party_Id=p.Id 
		and votes1.Election_Id=votes.Election_Id
		group by votes1.Election_Id, votes1.Party_Id) 
	/
		(select sum(votes2.Amount) as OverallVotes
		from ZweitstimmeAmount votes2
		where votes2.Election_Id = votes.Election_Id
		group by votes2.Election_Id)

	>=0.05
		--end of computation of all parties above 5% threshold
)


union
 (-- parteien mit mehr als 2 kreissieger aus Wahlkreissieger view mit Party=party
	select wks.ElectionID, wks.MemberOfParty
	from Wahlkreissieger wks
	--where wks.ElectionID=fZS.Election_Id
	group by wks.ElectionID, wks.MemberOfParty
	having count(*) > 2 
	and wks.MemberOfParty!= 36 --36:= party_Id der parteilosen. Die soll nicht in die "Parteien der über 5% gezählt werden)
)

				 





/* read from Zweitstimme votes -> bad runtime
(select votes.Election_Id, p.Id, p.Name  
from dbo.Party p, Zweitstimme votes
where (select count(*) as PartyVotes
		from  Zweitstimme votes1
		where votes1.Party_Id=p.Id
		group by votes1.Party_Id) 
		/
		(select COUNT(*) as OverallVotes
		from Zweitstimme)
		>=0.05
		--end of computation of all parties above 5% threshold

--		union all
-- parteien mit mehr als 2 kreissieger aus Wahlkreissieger view mit Party=party

);
*/


GO


