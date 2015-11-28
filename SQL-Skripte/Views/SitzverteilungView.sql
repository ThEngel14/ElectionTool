USE [ElectionDB]
GO

/****** Object:  View [dbo].[Sitzverteilung]    Script Date: 28.11.2015 20:20:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER view [dbo].[Sitzverteilung](Election_Id, OverallSeats, Party_Id, PartyName, SeatsParty, PercentParty) as 


with Seats as (select * from Election as E cross apply dbo.IncreasingSeats_Ausgleichsmandate(E.Id))

select  s.Id as Election_Id, (Select sum(temp_Seats) from Seats s1 where s.Id=s1.Id) as OverallSeats, s.Party_Id as Party_Id, p.Name as Party_Name, s.temp_Seats as SeatsParty,
	(Select round(100.0* s.temp_Seats	/ 
				(select sum(x.temp_Seats) from Seats x
				where s.Id = x.Id
				)
				,2)) as PercentParty
from Seats s, Party p
where p.Id = s.Party_Id



GO


