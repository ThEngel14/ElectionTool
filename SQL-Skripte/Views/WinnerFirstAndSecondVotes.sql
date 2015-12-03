create view WinnerFirstAndSecondVotes as

select e.Election_Id,
	   b.Id as Bundesland_Id, b.Name as Bundesland_Name,
	   e.Wahlkreis_Id, e.Wahlkreis_Name,
	   e.Person_Id, e.Title, e.Firstname, e.Lastname,
	   e.Party_Id as FirstVote_Party_Id, e.Party_Name as FirstVote_Party_Name,
	   z.Party_Id as SecondVote_Party_Id, z.Party_Name as SecondVote_Party_Name
from WinnerErststimme e join WinnerZweitstimme z on e.Election_Id = z.Election_Id and e.Wahlkreis_Id = z.Wahlkreis_Id
						join Wahlkreis k on e.Wahlkreis_Id = k.Id
						join Bundesland b on k.Bundesland_Id = b.Id