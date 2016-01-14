USE [ElectionDB]
GO

/****** Object:  View [dbo].[Parties5]    Script Date: 03.01.2016 13:41:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





ALTER view [dbo].[Parties5]  as

--parties above 5% threshold u parties with 3 or more Direktmandate
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
)


union
 ( 
	select wks.ElectionID, wks.MemberOfParty
	from Wahlkreissieger wks
	group by wks.ElectionID, wks.MemberOfParty
	having count(*) > 2 
	and wks.MemberOfParty!= 36 --36:= dummy party_Id for electable firstvote candidates who are not party affiliates.
)

GO


