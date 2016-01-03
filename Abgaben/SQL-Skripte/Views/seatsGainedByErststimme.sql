USE [ElectionDB]
GO

/****** Object:  View [dbo].[SeatsGainedByErststimme]    Script Date: 03.01.2016 13:45:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER view 

[dbo].[SeatsGainedByErststimme] as
(  

select wks.ElectionId as Election_Id, 
	wk.Bundesland_Id as Bundesland_Id,
	wks.MemberOfParty as Party_Id,
	coalesce(count(wks.Person_Id),0) as numberOfVictories
from Wahlkreissieger wks, Wahlkreis wk
where wks.Wahlkreis_Id= wk.Id
group by wks.ElectionId, wk.Bundesland_Id, wks.MemberOfParty


)
GO


